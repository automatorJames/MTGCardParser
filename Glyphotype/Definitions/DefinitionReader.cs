namespace Glyphotype.Definitions;

/// <summary>
/// Reads definitions from CLR types (see <see cref="GrammarDefinition.FromTypes"/>). Refuses, rather than
/// silently drops, anything the engine reads that a definition can't express - so a definition read from a
/// working grammar compiles back into one that behaves identically.
/// </summary>
static class DefinitionReader
{
    static readonly Assembly _glyphotype = typeof(Glyph).Assembly;

    public static GrammarDefinition ReadGrammar(IEnumerable<Type> glyphTypes)
    {
        var definedTypes = glyphTypes
            .Where(x => x.IsAssignableTo(typeof(Glyph)) && !x.IsGenericType && x.Assembly != _glyphotype)
            .Distinct()
            .OrderBy(x => x.Name, StringComparer.Ordinal)
            .ToList();

        var referencedTypes = definedTypes
            .SelectMany(x => GetNibBoundProps(x).Select(y => y.PropertyType).Append(x.BaseType))
            .SelectMany(GetConstituentTypes)
            .Distinct()
            .ToList();

        var vocabularyTypes = referencedTypes
            .Where(x => x.IsEnum && x.Assembly != _glyphotype)
            .OrderBy(x => x.Name, StringComparer.Ordinal)
            .ToList();

        var markerTypes = definedTypes
            .SelectMany(x => x.GetInterfaces())
            .Concat(definedTypes.SelectMany(GetNibBoundProps).Select(GetTypeFilter).Where(x => x is not null))
            .Where(IsMarkerCandidate)
            .Distinct()
            .OrderBy(x => x.Name, StringComparer.Ordinal)
            .ToList();

        ThrowIfNamesRepeat(definedTypes.Concat(vocabularyTypes).Concat(markerTypes));

        foreach (var marker in markerTypes)
            if (!marker.IsInterface || marker.GetMembers().Length > 0)
                throw new NotSupportedException($"{marker.Name} is used as a marker (implemented by a glyph, or named by a [TypeFilter]) but isn't a member-less interface, which is all a definition can express");

        return new()
        {
            Glyphs = definedTypes.Select(x => ReadGlyph(x, markerTypes)).ToList(),
            Vocabularies = vocabularyTypes.Select(ReadVocabulary).ToList(),
            Markers = markerTypes.Select(x => x.Name).ToList(),
        };
    }

    /// <summary>
    /// The definitions of Glyphotype's own glyphs and enums (see <see cref="GrammarDefinition.BuiltIns"/>) - which
    /// <see cref="ReadGrammar"/> leaves out, a grammar only referring to them. Only a glyph with a layout of its own: not
    /// <see cref="DynamicGlyph"/>, the wrapper a dynamic property's value comes in.
    /// </summary>
    public static GrammarDefinition ReadBuiltIns()
    {
        var types = _glyphotype.GetExportedTypes().Where(GrammarEmitter.IsBuiltIn).OrderBy(x => x.Name, StringComparer.Ordinal).ToList();

        return new()
        {
            Glyphs = types.Where(x => !x.IsEnum && IsOverridden(x, nameof(Glyph.Nibs))).Select(x => ReadGlyph(x, [])).ToList(),
            Vocabularies = types.Where(x => x.IsEnum).Select(ReadVocabulary).ToList(),
        };
    }

    static GlyphDefinition ReadGlyph(Type type, List<Type> markerTypes)
    {
        var (kind, aliasOf) = ReadBase(type);
        var referenceKind = BackReference.KindOf(type);
        var instance = (Glyph)Activator.CreateInstance(type);
        var props = GetNibBoundProps(type);

        return new()
        {
            Name = type.Name,
            Kind = kind,
            AliasOf = aliasOf,
            Nibs = IsOverridden(type, nameof(Glyph.Nibs)) ? instance.Nibs.Select(ReadNib).ToList() : [],
            Joiner = IsOverridden(type, nameof(Glyph.Joiner)) ? instance.Joiner : null,
            Properties = props.Select(ReadProperty).ToList(),
            Markers = type.GetInterfaces().Where(markerTypes.Contains).Select(x => x.Name).Order(StringComparer.Ordinal).ToList(),
            IsDependent = type.IsDefined(typeof(DependentAttribute), inherit: false),
            SpanRule =
                type.IsDefined(typeof(AllowPartialClauseMatchAttribute), inherit: false) ? SpanRule.PartialClause
                : SpanRule.Default,
            TokenizationOrder = type.GetCustomAttribute<TokenizationOrderAttribute>(inherit: false)?.Order,
            Patterns = type.GetCustomAttribute<RegexPatternAttribute>(inherit: false)?.Patterns ?? [],
            JoinedBy = type.GetCustomAttribute<JoinedByAttribute>(inherit: false)?.Joiner,
            IsReferent = type.IsDefined(typeof(ReferentAttribute), inherit: false),
            Number = type.GetCustomAttribute<ReferentAttribute>(inherit: false)?.Number ?? ReadNumber(type),
            ReferenceKind = referenceKind is null ? null : ReadTypeReference(referenceKind),
        };
    }

    static (GlyphKind Kind, TypeReference AliasOf) ReadBase(Type type)
    {
        var baseType = type.BaseType;

        if (baseType == typeof(Glyph))
            return (GlyphKind.Glyph, null);

        if (baseType == typeof(GlyphOneOf))
            return (GlyphKind.GlyphOneOf, null);

        if (baseType == typeof(BackReference) || baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(BackReference<>))
            return (GlyphKind.BackReference, null);

        if (baseType.IsGenericType && ReadTypeReference(baseType) is { Kind: not TypeReferenceKind.Glyph } primitive)
            return (GlyphKind.Alias, primitive);

        throw new NotSupportedException($"{type.Name} derives from {baseType.Name}, but a definition's base must be {nameof(Glyph)}, {nameof(GlyphOneOf)}, {nameof(BackReference)} (or BackReference<T>) or a generic primitive (OneOf, CompoundOf, ManyOf, OptionalOf)");
    }

    /// <summary>The number a back-reference type itself declares with <see cref="SingularAttribute"/> or <see cref="PluralAttribute"/>.</summary>
    static GrammaticalNumber ReadNumber(MemberInfo member) =>
        member.GetCustomAttributes<GrammaticalNumberAttribute>(inherit: false).FirstOrDefault()?.Number ?? GrammaticalNumber.Unspecified;

    static PropertyDefinition ReadProperty(PropertyInfo prop) =>
        new()
        {
            Name = prop.Name,
            Type = ReadTypeReference(prop.PropertyType),
            IsOptional = prop.IsDefined(typeof(OptionalAttribute)),
            AllowsUnmatched = prop.IsDefined(typeof(AllowUnmatchedAttribute)),
            Patterns = prop.GetCustomAttribute<RegexPatternAttribute>()?.Patterns ?? [],
            JoinedBy = prop.GetCustomAttribute<JoinedByAttribute>()?.Joiner,
            TypeFilter = GetTypeFilter(prop)?.Name,
            IsReferent = prop.IsDefined(typeof(ReferentAttribute)),
            Number = prop.GetCustomAttribute<ReferentAttribute>()?.Number ?? GrammaticalNumber.Unspecified,
            RefersTo = prop.GetCustomAttribute<RefersToAttribute>()?.PropertyName,
        };

    static NibDefinition ReadNib(Nib nib) =>
        nib switch
        {
            PropertyNib property => new NibDefinition.Property(property.Name),
            NibAlternatives alternatives => new NibDefinition.Alternatives(alternatives.Alternatives),
            OptionalNib optional => new NibDefinition.Optional(ReadNib(optional.Inner)),
            PluralNib plural => new NibDefinition.Plural(ReadNib(plural.Inner)),
            PatternNib pattern => new NibDefinition.Pattern(pattern.Text),
            EmbeddedGlyphNib { GlyphType: var type } when type == typeof(This) => new NibDefinition.This(),
            _ when nib.GetType() == typeof(Nib) => new NibDefinition.Literal(nib.Text),
            _ => throw new NotSupportedException($"Nib type {nib.GetType().Name} has no definition counterpart"),
        };

    static VocabularyDefinition ReadVocabulary(Type enumType)
    {
        if (Enum.GetUnderlyingType(enumType) != typeof(int))
            throw new NotSupportedException($"Enum {enumType.Name} has underlying type {Enum.GetUnderlyingType(enumType).Name}; vocabularies are int-backed");

        List<VocabularyMemberDefinition> members = [];
        long implicitValue = 0;

        foreach (var field in enumType.GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var value = Convert.ToInt64(field.GetRawConstantValue());

            members.Add(new()
            {
                Name = field.Name,
                Value = value == implicitValue ? null : value,
                Patterns = field.GetCustomAttribute<RegexPatternAttribute>()?.Patterns ?? [],
                Color = field.GetCustomAttribute<ColorAttribute>()?.Color.Value,
            });

            implicitValue = value + 1;
        }

        return new()
        {
            Name = enumType.Name,
            IsOptionalPlural = enumType.IsDefined(typeof(OptionalPluralAttribute)),
            Members = members,
        };
    }

    /// <summary>The definition-side reference to <paramref name="type"/>, which must be something a glyph property can be.</summary>
    public static TypeReference ReadTypeReference(Type type)
    {
        if (Nullable.GetUnderlyingType(type) is Type underlying)
            return ReadTypeReference(underlying).AsNullable();

        if (type == typeof(bool))
            return TypeReference.Bool;

        if (PrimitiveTerminal.TryGet(type, out var primitive))
            return TypeReference.Primitive(primitive.DisplayName);

        if (type.IsEnum)
            return TypeReference.Vocabulary(type.Name);

        if (type == typeof(DynamicGlyph))
            return TypeReference.Dynamic;

        if (type.IsGenericType)
        {
            var definition = type.GetGenericTypeDefinition();
            var arguments = type.GetGenericArguments().Select(ReadTypeReference).ToArray();

            if (definition == typeof(OneOf<,>) || definition == typeof(OneOf<,,>))
                return TypeReference.OneOf(arguments);

            if (definition == typeof(CompoundOf<>))
                return TypeReference.CompoundOf(arguments[0]);

            if (definition == typeof(ManyOf<>))
                return TypeReference.ManyOf(arguments[0]);

            if (definition == typeof(OptionalOf<>))
                return TypeReference.OptionalOf(arguments[0]);
        }
        // Glyphotype's own concrete glyphs (e.g. the standard pronouns) are referred to by name, like its enums - neither
        // is defined by the grammar, and both resolve when it's emitted.
        else if (type.IsAssignableTo(typeof(Glyph)) && !type.IsAbstract)
        {
            return TypeReference.Glyph(type.Name);
        }

        throw new NotSupportedException($"{type} has no definition counterpart: a glyph property must be a glyph, an enum, a bool, a supported primitive, a {nameof(DynamicGlyph)}, or a OneOf/CompoundOf/ManyOf/OptionalOf of those");
    }

    /// <summary>A glyph type's own nib-bound properties, in declaration order: publicly settable, and not an override of a base member.</summary>
    static PropertyInfo[] GetNibBoundProps(Type type) =>
        type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Where(x => x.SetMethod is { IsPublic: true } && x.GetMethod.GetBaseDefinition().DeclaringType == x.DeclaringType)
            .ToArray();

    /// <summary>
    /// Whether <paramref name="type"/>, or a type between it and Glyphotype's own bases (so not a primitive's built-in
    /// layout), overrides <paramref name="propertyName"/>. The type itself always counts, so a built-in's own layout does.
    /// </summary>
    static bool IsOverridden(Type type, string propertyName)
    {
        for (var current = type; current == type || current.Assembly != _glyphotype; current = current.BaseType)
            if (current.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly) is not null)
                return true;

        return false;
    }

    static Type GetTypeFilter(PropertyInfo prop) =>
        prop.GetCustomAttribute<TypeFilterAttribute>()?.Type;

    static bool IsMarkerCandidate(Type type) =>
        type.Assembly != _glyphotype && type.Namespace?.StartsWith("System") != true;

    /// <summary><paramref name="type"/> and, recursively, its nullable-unwrapped generic arguments.</summary>
    static IEnumerable<Type> GetConstituentTypes(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;

        return type.IsGenericType
            ? type.GetGenericArguments().SelectMany(GetConstituentTypes).Prepend(type)
            : [type];
    }

    static void ThrowIfNamesRepeat(IEnumerable<Type> types)
    {
        var repeated = types.GroupBy(x => x.Name).Where(x => x.Count() > 1).Select(x => x.Key).ToList();

        if (repeated.Count > 0)
            throw new NotSupportedException($"Definitions refer to each other by name, but these names are shared by more than one type: {string.Join(", ", repeated)}");
    }
}
