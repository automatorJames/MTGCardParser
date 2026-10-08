using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Glyphotype.Distiller.Workbench;

/// <summary>The definitions one piece of C# source declares, in source order.</summary>
public sealed record SourceDeclarations(IReadOnlyList<GlyphDefinition> Glyphs, IReadOnlyList<VocabularyDefinition> Vocabularies, IReadOnlyList<string> Markers)
{
    public bool IsEmpty => Glyphs.Count == 0 && Vocabularies.Count == 0 && Markers.Count == 0;

    public IEnumerable<string> Names => Markers.Concat(Vocabularies.Select(x => x.Name)).Concat(Glyphs.Select(x => x.Name));
}

/// <summary>C# that <see cref="GlyphSourceReader"/> couldn't read as definitions, with every problem found.</summary>
public sealed class GlyphSourceException(IReadOnlyList<string> errors)
    : FormatException(string.Join(Environment.NewLine, errors))
{
    public IReadOnlyList<string> Errors { get; } = errors;
}

/// <summary>
/// Reads definitions from C# source written the way hand-written glyph files are (and the way
/// <see cref="GlyphSourceWriter"/> writes them): glyph classes, vocabulary enums and marker interfaces, with the
/// attributes and nib helpers definitions express. The inverse of <see cref="GlyphSourceWriter"/>, working on
/// syntax alone - nothing is compiled, so the source may refer to definitions that exist only in a working grammar.
/// <para>
/// Refuses, rather than drops, anything a definition can't hold (methods, computed properties, unknown
/// attributes), reporting every such problem at once with its line number.
/// </para>
/// </summary>
public static class GlyphSourceReader
{
    static readonly HashSet<string> _genericPrimitives = ["OneOf", "CompoundOf", "ManyOf", "OptionalOf"];

    /// <summary>
    /// Reads every declaration in <paramref name="source"/>. A type name is a vocabulary when an enum of that name
    /// is declared in the source or in <paramref name="context"/>, and a glyph otherwise.
    /// </summary>
    /// <exception cref="GlyphSourceException">The source doesn't parse, or declares something a definition can't express.</exception>
    public static SourceDeclarations Read(string source, GrammarDefinition context = null)
    {
        var root = CSharpSyntaxTree.ParseText(source).GetCompilationUnitRoot();
        var errors = root.GetDiagnostics()
            .Where(x => x.Severity == DiagnosticSeverity.Error)
            .Select(x => $"line {x.Location.GetLineSpan().StartLinePosition.Line + 1}: {x.GetMessage(CultureInfo.InvariantCulture)}")
            .ToList();

        if (errors.Count > 0)
            throw new GlyphSourceException(errors);

        var declarations = root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>().ToList();

        var vocabularyNames = declarations.OfType<EnumDeclarationSyntax>().Select(x => x.Identifier.Text)
            .Concat(context?.Vocabularies.Select(x => x.Name) ?? [])
            .ToHashSet();

        var reader = new Reader(vocabularyNames, errors);
        List<GlyphDefinition> glyphs = [];
        List<VocabularyDefinition> vocabularies = [];
        List<string> markers = [];

        foreach (var declaration in declarations)
        {
            switch (declaration)
            {
                case ClassDeclarationSyntax @class:
                    if (reader.ReadGlyph(@class) is GlyphDefinition glyph)
                        glyphs.Add(glyph);
                    break;

                case EnumDeclarationSyntax @enum:
                    if (reader.ReadVocabulary(@enum) is VocabularyDefinition vocabulary)
                        vocabularies.Add(vocabulary);
                    break;

                case InterfaceDeclarationSyntax @interface:
                    if (@interface.Members.Count > 0 || @interface.BaseList is not null)
                        reader.Error(@interface, $"marker interface {@interface.Identifier.Text} must be empty, with no base interfaces");
                    else
                        markers.Add(@interface.Identifier.Text);
                    break;

                default:
                    reader.Error(declaration, $"{declaration.Identifier.Text}: only classes (glyphs), enums (vocabularies) and empty interfaces (markers) can be declared");
                    break;
            }
        }

        var repeated = glyphs.Select(x => x.Name).Concat(vocabularies.Select(x => x.Name)).Concat(markers)
            .GroupBy(x => x).Where(x => x.Count() > 1).Select(x => x.Key).ToList();

        if (repeated.Count > 0)
            errors.Add($"declared more than once: {string.Join(", ", repeated)}");

        if (errors.Count > 0)
            throw new GlyphSourceException(errors);

        return new(glyphs, vocabularies, markers);
    }

    sealed class Reader(HashSet<string> vocabularyNames, List<string> errors)
    {
        public void Error(SyntaxNode node, string message) =>
            errors.Add($"line {node.GetLocation().GetLineSpan().StartLinePosition.Line + 1}: {message}");

        // ---- Glyphs ----

        public GlyphDefinition ReadGlyph(ClassDeclarationSyntax @class)
        {
            var name = @class.Identifier.Text;
            var errorCount = errors.Count;

            if (@class.TypeParameterList is not null)
                Error(@class, $"{name}: a glyph can't be generic");

            var baseTypes = @class.BaseList?.Types.Select(x => x.Type).ToList() ?? [];

            if (baseTypes.Count == 0)
            {
                Error(@class, $"{name} needs a base: Glyph, GlyphOneOf, or a generic primitive (OneOf, CompoundOf, ManyOf, OptionalOf)");
                return null;
            }

            var (kind, aliasOf, referenceKind) = ReadBase(name, baseTypes[0]);
            var glyph = new GlyphDefinition
            {
                Name = name,
                Documentation = GlyphDocComment.Read(@class).Documentation,
                Kind = kind,
                AliasOf = aliasOf,
                ReferenceKind = referenceKind,
                Markers = baseTypes.Skip(1).Select(x => x.ToString()).Order(StringComparer.Ordinal).ToList(),
            };

            glyph = ReadClassAttributes(glyph, @class);

            List<PropertyDefinition> properties = [];

            foreach (var member in @class.Members)
            {
                if (member is not PropertyDeclarationSyntax property)
                {
                    Error(member, $"{name}: only nib-bound properties and the Nibs/Joiner overrides can be declared - {member.Kind()} isn't expressible");
                    continue;
                }

                switch (property.Identifier.Text)
                {
                    case nameof(Glyph.Nibs) when IsOverride(property):
                        glyph = glyph with { Nibs = ReadNibs(name, property) };
                        break;

                    case nameof(Glyph.Joiner) when IsOverride(property):
                        glyph = glyph with { Joiner = ReadEnumMember<Joiner>(GetExpressionBody(name, property)) };
                        break;

                    default:
                        if (ReadProperty(name, property) is PropertyDefinition read)
                            properties.Add(read);
                        break;
                }
            }

            glyph = glyph with { Properties = properties };

            foreach (var nib in glyph.Nibs.SelectMany(Flatten).OfType<NibDefinition.Property>())
                if (!properties.Any(x => x.Name == nib.Name))
                    Error(@class, $"{name}: Prop({nib.Name}) refers to no property of {name}");

            return errors.Count == errorCount ? glyph : null;
        }

        (GlyphKind Kind, TypeReference AliasOf, TypeReference ReferenceKind) ReadBase(string name, TypeSyntax baseType)
        {
            switch (baseType)
            {
                case IdentifierNameSyntax { Identifier.Text: nameof(Glyph) }:
                    return (GlyphKind.Glyph, null, null);
                case IdentifierNameSyntax { Identifier.Text: nameof(GlyphOneOf) }:
                    return (GlyphKind.GlyphOneOf, null, null);
                case IdentifierNameSyntax { Identifier.Text: nameof(BackReference) }:
                    return (GlyphKind.BackReference, null, null);
                case GenericNameSyntax { Identifier.Text: nameof(BackReference), TypeArgumentList.Arguments: [var kind] }:
                    return (GlyphKind.BackReference, null, ReadTypeReference(kind));
                case GenericNameSyntax generic when _genericPrimitives.Contains(generic.Identifier.Text):
                    return (GlyphKind.Alias, ReadTypeReference(generic), null);
                default:
                    Error(baseType, $"{name} derives from {baseType}, but a glyph's base must be Glyph, GlyphOneOf, BackReference, BackReference<T> or a generic primitive (OneOf, CompoundOf, ManyOf, OptionalOf) - markers come after it");
                    return (GlyphKind.Glyph, null, null);
            }
        }

        GlyphDefinition ReadClassAttributes(GlyphDefinition glyph, ClassDeclarationSyntax @class)
        {
            foreach (var attribute in @class.AttributeLists.SelectMany(x => x.Attributes))
            {
                var arguments = GetArguments(attribute);

                glyph = AttributeName(attribute) switch
                {
                    "Dependent" => glyph with { IsDependent = true },
                    "AllowPartialClauseMatch" => glyph with { SpanRule = SpanRule.PartialClause },
                    "TokenizationOrder" when arguments.Count == 1 => glyph with { TokenizationOrder = (int)ReadInteger(arguments[0]) },
                    "RegexPattern" => glyph with { Patterns = arguments.Select(ReadString).ToList() },
                    "JoinedBy" when arguments.Count == 1 => glyph with { JoinedBy = ReadEnumMember<Joiner>(arguments[0]) },
                    "Singular" or "Plural" when glyph.Number != GrammaticalNumber.Unspecified
                        => Unsupported(glyph, attribute, $"{glyph.Name} is both [Singular] and [Plural] - it can only be one"),
                    "Referent" when arguments.Count == 0 => glyph with { IsReferent = true },
                    "Referent" => Unsupported(glyph, attribute, $"{glyph.Name}: [Referent] takes no number - give it [Singular] or [Plural] beside it"),
                    "Singular" when arguments.Count == 0 => glyph with { Number = GrammaticalNumber.Singular },
                    "Plural" when arguments.Count == 0 => glyph with { Number = GrammaticalNumber.Plural },
                    _ => Unsupported(glyph, attribute, $"{glyph.Name}: [{attribute}] isn't an attribute a glyph definition can hold (Dependent, AllowPartialClauseMatch, TokenizationOrder, RegexPattern, JoinedBy, Referent, Singular, Plural)"),
                };
            }

            return glyph;
        }

        PropertyDefinition ReadProperty(string glyphName, PropertyDeclarationSyntax property)
        {
            var name = property.Identifier.Text;
            var accessors = property.AccessorList?.Accessors.Select(x => x.Keyword.Text).ToList();

            if (!property.Modifiers.Any(SyntaxKind.PublicKeyword) || property.Modifiers.Any(SyntaxKind.StaticKeyword) || property.Modifiers.Any(SyntaxKind.OverrideKeyword)
                || accessors is null || !accessors.SequenceEqual(["get", "set"]) || property.AccessorList.Accessors.Any(x => x.Body is not null || x.ExpressionBody is not null || x.Modifiers.Count > 0))
            {
                Error(property, $"{glyphName}.{name}: a nib-bound property is written 'public T {name} {{ get; set; }}' - computed or restricted properties aren't expressible");
                return null;
            }

            var definition = new PropertyDefinition { Name = name, Type = ReadTypeReference(property.Type) };

            foreach (var attribute in property.AttributeLists.SelectMany(x => x.Attributes))
            {
                var arguments = GetArguments(attribute);

                definition = AttributeName(attribute) switch
                {
                    "Optional" => definition with { IsOptional = true },
                    "AllowUnmatched" => definition with { AllowsUnmatched = true },
                    "RegexPattern" => definition with { Patterns = arguments.Select(ReadString).ToList() },
                    "JoinedBy" when arguments.Count == 1 => definition with { JoinedBy = ReadEnumMember<Joiner>(arguments[0]) },
                    "TypeFilter" when arguments is [TypeOfExpressionSyntax typeOf] => definition with { TypeFilter = typeOf.Type.ToString() },
                    "Referent" when arguments.Count == 0 => definition with { IsReferent = true },
                    "Referent" => Unsupported(definition, attribute, $"{glyphName}.{name}: [Referent] takes no number - give it [Singular] or [Plural] beside it"),
                    "Singular" or "Plural" when definition.Number != GrammaticalNumber.Unspecified
                        => Unsupported(definition, attribute, $"{glyphName}.{name} is both [Singular] and [Plural] - it can only be one"),
                    "Singular" when arguments.Count == 0 => definition with { Number = GrammaticalNumber.Singular },
                    "Plural" when arguments.Count == 0 => definition with { Number = GrammaticalNumber.Plural },
                    "RefersTo" when arguments.Count == 1 => definition with { RefersTo = ReadPropertyName(arguments[0]) },
                    _ => Unsupported(definition, attribute, $"{glyphName}.{name}: [{attribute}] isn't an attribute a property definition can hold (Optional, AllowUnmatched, RegexPattern, JoinedBy, TypeFilter(typeof(Marker)), Referent, Singular, Plural, RefersTo(nameof(Property)))"),
                };
            }

            return definition;
        }

        List<NibDefinition> ReadNibs(string glyphName, PropertyDeclarationSyntax property)
        {
            var elements = GetExpressionBody(glyphName, property) switch
            {
                CollectionExpressionSyntax collection => collection.Elements.Select(x => x is ExpressionElementSyntax element ? element.Expression : null).ToList(),
                ArrayCreationExpressionSyntax { Initializer: not null } array => array.Initializer.Expressions.ToList(),
                ImplicitArrayCreationExpressionSyntax array => array.Initializer.Expressions.ToList(),
                null => null,
                var other => Unsupported<List<ExpressionSyntax>>(null, other, $"{glyphName}.Nibs must be an array of nibs, e.g. [\"literal\", Prop(Name)]"),
            };

            return elements?.Select(x => x is null ? Unsupported<NibDefinition>(null, property, $"{glyphName}.Nibs: spreads aren't expressible") : ReadNib(glyphName, x))
                .Where(x => x is not null)
                .ToList() ?? [];
        }

        NibDefinition ReadNib(string glyphName, ExpressionSyntax expression)
        {
            if (expression is LiteralExpressionSyntax { RawKind: (int)SyntaxKind.StringLiteralExpression } literal)
                return new NibDefinition.Literal(literal.Token.ValueText);

            if (expression is MemberAccessExpressionSyntax { Expression: IdentifierNameSyntax { Identifier.Text: "Nib" }, Name.Identifier.Text: "This" })
                return new NibDefinition.This();

            if (expression is InvocationExpressionSyntax { Expression: IdentifierNameSyntax method } invocation)
            {
                var arguments = invocation.ArgumentList.Arguments.Select(x => x.Expression).ToList();

                switch (method.Identifier.Text)
                {
                    case "Pattern" when arguments.Count == 1:
                        return new NibDefinition.Pattern(ReadString(arguments[0]));
                    case "Alt" when arguments.Count > 0:
                        return new NibDefinition.Alternatives(arguments.Select(ReadString).ToList());
                    case "Opt" when arguments.Count == 1:
                        return ReadNib(glyphName, arguments[0]) is NibDefinition inner ? new NibDefinition.Optional(inner) : null;
                    case "Plural" when arguments.Count == 1:
                        return ReadNib(glyphName, arguments[0]) is NibDefinition plural ? new NibDefinition.Plural(plural) : null;
                    case "Plural":
                        Error(expression, $"{glyphName}: Plural takes the nib it makes plural, e.g. Plural(\"card\") or Plural(Prop(CardType))");
                        return null;
                    case "Prop" when arguments.Count == 1:
                        var propName = arguments[0] switch
                        {
                            IdentifierNameSyntax identifier => identifier.Identifier.Text,
                            InvocationExpressionSyntax { Expression: IdentifierNameSyntax { Identifier.Text: "nameof" } } nameOf => nameOf.ArgumentList.Arguments[0].Expression.ToString(),
                            var other => Unsupported<string>(null, other, $"{glyphName}: Prop takes the property itself, e.g. Prop(Quantity)"),
                        };
                        return propName is null ? null : new NibDefinition.Property(propName);
                }
            }

            Error(expression, $"{glyphName}: '{expression}' isn't a nib - use a string literal, Pattern(\"regex\"), Alt(\"a\", \"b\"), Opt(nib), Plural(nib), Prop(Property) or Nib.This");
            return null;
        }

        static bool IsOverride(PropertyDeclarationSyntax property) =>
            property.Modifiers.Any(SyntaxKind.OverrideKeyword);

        ExpressionSyntax GetExpressionBody(string glyphName, PropertyDeclarationSyntax property)
        {
            var body = property.ExpressionBody?.Expression
                ?? (property.AccessorList?.Accessors is [{ Keyword.Text: "get" } getter] ? getter.ExpressionBody?.Expression ?? SingleReturn(getter.Body) : null);

            if (body is null)
                Error(property, $"{glyphName}.{property.Identifier.Text} must be an expression-bodied override, e.g. 'public override {property.Type} {property.Identifier.Text} => ...;'");

            return body;

            static ExpressionSyntax SingleReturn(BlockSyntax block) =>
                block?.Statements is [ReturnStatementSyntax { Expression: var expression }] ? expression : null;
        }

        // ---- Vocabularies ----

        public VocabularyDefinition ReadVocabulary(EnumDeclarationSyntax @enum)
        {
            var name = @enum.Identifier.Text;
            var errorCount = errors.Count;
            var vocabulary = new VocabularyDefinition { Name = name };

            if (@enum.BaseList is not null && @enum.BaseList.Types.Any(x => x.Type.ToString() != "int"))
                Error(@enum, $"{name}: vocabularies are int-backed");

            foreach (var attribute in @enum.AttributeLists.SelectMany(x => x.Attributes))
                vocabulary = AttributeName(attribute) == "OptionalPlural"
                    ? vocabulary with { IsOptionalPlural = true }
                    : Unsupported(vocabulary, attribute, $"{name}: [{attribute}] isn't an attribute a vocabulary can hold (OptionalPlural)");

            List<VocabularyMemberDefinition> members = [];
            long implicitValue = 0;

            foreach (var field in @enum.Members)
            {
                var member = new VocabularyMemberDefinition { Name = field.Identifier.Text };
                var value = field.EqualsValue is null ? implicitValue : ReadInteger(field.EqualsValue.Value);

                if (value != implicitValue)
                    member = member with { Value = value };

                foreach (var attribute in field.AttributeLists.SelectMany(x => x.Attributes))
                {
                    var arguments = GetArguments(attribute);

                    member = AttributeName(attribute) switch
                    {
                        "RegexPattern" => member with { Patterns = arguments.Select(ReadString).ToList() },
                        "Color" when arguments.Count == 1 => member with { Color = ReadString(arguments[0]) },
                        _ => Unsupported(member, attribute, $"{name}.{member.Name}: [{attribute}] isn't an attribute a vocabulary member can hold (RegexPattern, Color)"),
                    };
                }

                members.Add(member);
                implicitValue = value + 1;
            }

            return errors.Count == errorCount ? vocabulary with { Members = members } : null;
        }

        // ---- Types ----

        TypeReference ReadTypeReference(TypeSyntax type)
        {
            switch (type)
            {
                case NullableTypeSyntax nullable:
                    return ReadTypeReference(nullable.ElementType)?.AsNullable();

                case PredefinedTypeSyntax { Keyword.Text: "bool" }:
                    return TypeReference.Bool;

                case PredefinedTypeSyntax predefined when PrimitiveTerminal.SupportedDisplayNames.Contains(predefined.Keyword.Text):
                    return TypeReference.Primitive(predefined.Keyword.Text);

                case IdentifierNameSyntax { Identifier.Text: nameof(DynamicGlyph) }:
                    return TypeReference.Dynamic;

                case IdentifierNameSyntax identifier:
                    var name = identifier.Identifier.Text;
                    return vocabularyNames.Contains(name) ? TypeReference.Vocabulary(name) : TypeReference.Glyph(name);

                case GenericNameSyntax generic when _genericPrimitives.Contains(generic.Identifier.Text):
                    var arguments = generic.TypeArgumentList.Arguments.Select(ReadTypeReference).ToArray();

                    if (arguments.Any(x => x is null))
                        return null;

                    return (generic.Identifier.Text, arguments.Length) switch
                    {
                        ("OneOf", 2 or 3) => TypeReference.OneOf(arguments),
                        ("CompoundOf", 1) => TypeReference.CompoundOf(arguments[0]),
                        ("ManyOf", 1) => TypeReference.ManyOf(arguments[0]),
                        ("OptionalOf", 1) => TypeReference.OptionalOf(arguments[0]),
                        _ => Unsupported<TypeReference>(null, type, $"{type} has the wrong number of type arguments"),
                    };

                default:
                    Error(type, $"'{type}' isn't a type a glyph property can be: a glyph, a vocabulary (enum), bool, {string.Join(", ", PrimitiveTerminal.SupportedDisplayNames)}, DynamicGlyph, or OneOf/CompoundOf/ManyOf/OptionalOf of those");
                    return null;
            }
        }

        // ---- Values ----

        static string AttributeName(AttributeSyntax attribute)
        {
            var name = attribute.Name is QualifiedNameSyntax qualified ? qualified.Right.Identifier.Text : attribute.Name.ToString();
            return name.EndsWith("Attribute") ? name[..^"Attribute".Length] : name;
        }

        static List<ExpressionSyntax> GetArguments(AttributeSyntax attribute) =>
            attribute.ArgumentList?.Arguments.Select(x => x.Expression).ToList() ?? [];

        string ReadString(ExpressionSyntax expression)
        {
            if (expression is LiteralExpressionSyntax { RawKind: (int)SyntaxKind.StringLiteralExpression } literal)
                return literal.Token.ValueText;

            Error(expression, $"'{expression}' must be a string literal");
            return "";
        }

        long ReadInteger(ExpressionSyntax expression)
        {
            switch (expression)
            {
                case LiteralExpressionSyntax { RawKind: (int)SyntaxKind.NumericLiteralExpression } literal:
                    return Convert.ToInt64(literal.Token.Value, CultureInfo.InvariantCulture);
                case PrefixUnaryExpressionSyntax { RawKind: (int)SyntaxKind.UnaryMinusExpression } negative:
                    return -ReadInteger(negative.Operand);
                default:
                    Error(expression, $"'{expression}' must be an integer literal");
                    return 0;
            }
        }

        T ReadEnumMember<T>(ExpressionSyntax expression) where T : struct, Enum
        {
            if (expression is MemberAccessExpressionSyntax { Expression: IdentifierNameSyntax type } access
                && type.Identifier.Text == typeof(T).Name
                && Enum.TryParse<T>(access.Name.Identifier.Text, out var value))
                return value;

            if (expression is not null)
                Error(expression, $"'{expression}' must be one of {string.Join(", ", Enum.GetNames<T>().Select(x => $"{typeof(T).Name}.{x}"))}");

            return default;
        }

        /// <summary>A property named by <c>nameof(Property)</c>, or by its name as a string literal.</summary>
        string ReadPropertyName(ExpressionSyntax expression) =>
            expression is InvocationExpressionSyntax { Expression: IdentifierNameSyntax { Identifier.Text: "nameof" }, ArgumentList.Arguments: [{ Expression: IdentifierNameSyntax property }] }
                ? property.Identifier.Text
                : ReadString(expression);

        T Unsupported<T>(T fallback, SyntaxNode node, string message)
        {
            Error(node, message);
            return fallback;
        }
    }

    static IEnumerable<NibDefinition> Flatten(NibDefinition nib) =>
        nib switch
        {
            NibDefinition.Optional optional => Flatten(optional.Inner).Prepend(nib),
            NibDefinition.Plural plural => Flatten(plural.Inner).Prepend(nib),
            _ => [nib],
        };
}
