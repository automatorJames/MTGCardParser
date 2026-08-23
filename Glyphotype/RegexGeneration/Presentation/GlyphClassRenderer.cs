namespace Glyphotype.RegexGeneration.Presentation;

/// <summary>
/// Renders one <see cref="Glyph"/> type's (<see cref="Render"/>) or enum's (<see cref="RenderEnum"/>) declared
/// C# source - reflected fresh via <see cref="Navigation.NodeType"/>/<see cref="Navigation.UnderlyingType"/>,
/// not stored anywhere - as a flat sequence of colored, data-pathed <see cref="ClassLine"/>s, for TypeRegexPage's
/// "C# Class" footer tray view. Colors every named-group-bearing span exactly the way the rest of the page
/// already colors that same node (via the same <c>palette</c> a caller builds once per
/// <see cref="RegexGraph"/>, e.g. <see cref="RegexGraph.GetNamedGroupPaletteSet(HexColor[])"/> the same way
/// <c>TypeTreeView</c> does), so hovering a property here highlights its type-tree box and matches-table
/// spans, and vice versa. Everything else - keywords, braces, punctuation, base-type names - renders in one
/// of three neutral grey shades (see <see cref="GlyphClassRenderRules"/>).
/// </summary>
/// <remarks>
/// <c>contextNode</c> is always a <see cref="NamedGroupNode"/> - a <see cref="GlyphNode"/>/<see cref="GlyphOneOfNode"/>
/// for <see cref="Render"/>, an <see cref="EnumNode"/> for <see cref="RenderEnum"/> (the caller picks which to
/// call based on <see cref="NamedGroupNode.NodeKind"/>) - reached one of two ways: the graph's own
/// <see cref="RegexGraph.RootNode"/> for a card's own top-level type, or navigating into one of its properties
/// (see <see cref="IsNavigable"/>), including into a resolved <see cref="Glyphotype.GlyphPrimitives.DynamicGlyph"/>
/// capture's own (separate) <see cref="RegexGraph"/> (see <see cref="ClassSpan.Resolutions"/>). Either way, an
/// optional <c>contextPath</c> lets the caller rebase every data-path this call emits onto wherever
/// <paramref name="contextNode"/> is conceptually positioned - its own real <see cref="RegexNode.FullyQualifiedName"/>
/// when navigation never left one graph, or a synthetic <c>Outer_Dynamic_ResolvedType</c> path (matching
/// <see cref="DynamicSectionBuilder"/>'s own rebasing) once it has. <c>palette</c> must be built with dynamic
/// resolutions already spliced in (e.g. <see cref="RegexGraph.GetNamedGroupPaletteSet(IEnumerable{RegexBrick}, HexColor[])"/>
/// against that card's own <see cref="RegexDisplayMode.MatchedOnly"/> bricks, the way <c>MatchContentRenderer</c>
/// builds its own) for a resolved type's nodes to have a color to look up at all - because
/// <see cref="DynamicSectionBuilder"/> recurses, this one upfront palette already covers every depth of
/// dynamic resolution reachable from that card, not just the first.
/// </remarks>
public static class GlyphClassRenderer
{
    static readonly Dictionary<Type, string> _builtInAliases = new()
    {
        [typeof(bool)] = "bool",
        [typeof(int)] = "int",
        [typeof(long)] = "long",
        [typeof(double)] = "double",
        [typeof(string)] = "string",
        [typeof(byte)] = "byte",
        [typeof(sbyte)] = "sbyte",
        [typeof(short)] = "short",
        [typeof(ushort)] = "ushort",
        [typeof(uint)] = "uint",
        [typeof(ulong)] = "ulong",
    };

    static readonly SpanStylePalette _keywordPalette = NeutralPalette(GlyphClassRenderRules.NeutralKeywordBrightness);
    static readonly SpanStylePalette _bracePalette = NeutralPalette(GlyphClassRenderRules.NeutralBraceBrightness);
    static readonly SpanStylePalette _punctuationPalette = NeutralPalette(GlyphClassRenderRules.NeutralPunctuationBrightness);

    /// <summary>
    /// Whether <paramref name="node"/> names a type with its own declared source to navigate to directly: any
    /// enum (see <see cref="RenderEnum"/>), or a concrete (non-generic) <see cref="Glyph"/> subtype (see
    /// <see cref="Render"/>). Excludes primitives (<see cref="BoolNode"/>/<see cref="IntNode"/>), unbound
    /// generic wrappers like <c>OneOf&lt;,&gt;</c>/<c>ManyOf&lt;&gt;</c> (no source file of their own - only
    /// their closed-over type arguments might have one), and <see cref="DynamicGlyphNode"/> - a
    /// <see cref="Glyphotype.GlyphPrimitives.DynamicGlyph"/> capture has no *one* fixed type to jump straight
    /// to, so it's offered as a <see cref="ClassSpan.Resolutions"/> menu instead (see <see cref="GetDynamicResolutions"/>).
    /// </summary>
    public static bool IsNavigable(NamedGroupNode node) =>
        node.NodeKind switch
        {
            CaptureNodeKind.Enum => true,
            CaptureNodeKind.Token or CaptureNodeKind.OneOf => !node.Navigation.NodeType.IsGenericType,
            _ => false,
        };

    /// <summary>Renders <paramref name="contextNode"/>'s type as a whitespace-formatted C# class, colored per <paramref name="palette"/>.</summary>
    /// <param name="allSummaries">
    /// Every registered type's own corpus-wide <see cref="GlyphOccurrenceSummary"/>, keyed by type - used
    /// only to look up a <see cref="Glyphotype.GlyphPrimitives.DynamicGlyph"/> property's own resolutions
    /// (see <see cref="GetDynamicResolutions"/>). Pass null (or omit) to render without dynamic resolution
    /// menus - every <see cref="DynamicGlyphNode"/> property then renders as plain, non-interactive text.
    /// </param>
    public static List<ClassLine> Render(
        NamedGroupNode contextNode,
        IReadOnlyDictionary<NamedGroupNode, HexPalette> palette,
        IReadOnlyDictionary<Type, GlyphOccurrenceSummary> allSummaries = null,
        string contextPath = null)
    {
        var ctx = new RenderContext(contextNode, contextPath ?? contextNode.FullyQualifiedName, palette, allSummaries);
        var type = contextNode.Navigation.NodeType;
        List<ClassLine> lines = [];

        foreach (var attribute in GetDisplayableAttributes(type))
            lines.Add(BuildAttributeLine(attribute, indent: ""));

        List<ClassSpan> declarationSpans =
        [
            Keyword("public class "),
            new(FriendlyLeafName(type), ctx.Fqn(contextNode), HeaderPalette(contextNode, ctx)),
        ];

        if (type.BaseType is { } baseType && baseType != typeof(object))
        {
            declarationSpans.Add(Brace(" : "));
            declarationSpans.Add(Keyword(FormatTypeNamePlain(baseType)));
        }

        lines.Add(new ClassLine(declarationSpans));
        lines.Add(new ClassLine([Brace("{")]));

        var indent = string.Empty.PadLeft(GlyphClassRenderRules.BodyIndentSpaces);
        var nibsLine = BuildNibsLine(contextNode, ctx, indent);
        var joinerLine = BuildJoinerLine(contextNode, indent);

        if (nibsLine != null)
            lines.Add(nibsLine);

        if (joinerLine != null)
            lines.Add(joinerLine);

        if (nibsLine != null || joinerLine != null)
            lines.Add(new ClassLine([]));

        foreach (var property in type.GetOwnProps())
        {
            // Every own property of a valid Glyph type corresponds to exactly one nib-bound child node
            // (enforced by Glyph.ValidateStructure at registry startup) - skip defensively rather than
            // throw, since this is a display-only view.
            var propertyNode = contextNode.NamedGroupChildren.FirstOrDefault(x => x.Navigation.Prop == property);
            if (propertyNode == null)
                continue;

            foreach (var attribute in GetDisplayableAttributes(property))
                lines.Add(BuildAttributeLine(attribute, indent));

            lines.Add(BuildPropertyLine(propertyNode, ctx, indent));
        }

        lines.Add(new ClassLine([Brace("}")]));

        return lines;
    }

    /// <summary>
    /// Renders <paramref name="contextNode"/>'s enum type as a whitespace-formatted C# enum, colored per
    /// <paramref name="palette"/> - the enum-view analog of <see cref="Render"/>, much simpler since an enum
    /// has no nested navigable structure of its own: just its own attributes, its name (colored the same way
    /// a class name is - <see cref="RegexSpanKind.CommentGroupOpenHeaderText"/>'s treatment applied to its own
    /// positional hue), and its members (each an attribute line, if any, plus a bare name colored via
    /// <see cref="RegexSpanKind.RegexEnumMember"/> applied to the *enum's own* hue - members aren't named
    /// groups of their own in the graph, the same reason a Nibs-array literal borrows its class's hue via
    /// <see cref="LiteralPalette"/>). A member isn't click-navigable (there's nowhere to navigate to - an enum
    /// value isn't its own type), but it does carry a data-path of <c><paramref name="contextNode"/>'s
    /// FullyQualifiedName + "_" + the member's name</c> - the same <c>Enum_Member</c> shape
    /// <see cref="EnumMemberNode.FullyQualifiedName"/> already gives that member's own row(s) in the formatted
    /// regex column (see <see cref="EnumNode.AddReflectedChildren"/>), so hovering a member here cross-highlights
    /// its regex-column row, and vice versa - the same connection every other span on the page gets.
    /// </summary>
    public static List<ClassLine> RenderEnum(NamedGroupNode contextNode, IReadOnlyDictionary<NamedGroupNode, HexPalette> palette, string contextPath = null)
    {
        var ctx = new RenderContext(contextNode, contextPath ?? contextNode.FullyQualifiedName, palette, null);
        var type = contextNode.Navigation.UnderlyingType;
        List<ClassLine> lines = [];

        foreach (var attribute in GetDisplayableAttributes(type))
            lines.Add(BuildAttributeLine(attribute, indent: ""));

        List<ClassSpan> declarationSpans =
        [
            Keyword("public enum "),
            new(FriendlyLeafName(type), ctx.Fqn(contextNode), HeaderPalette(contextNode, ctx)),
        ];

        var underlyingIntegralType = Enum.GetUnderlyingType(type);
        if (underlyingIntegralType != typeof(int))
        {
            declarationSpans.Add(Brace(" : "));
            declarationSpans.Add(Keyword(FriendlyLeafName(underlyingIntegralType)));
        }

        lines.Add(new ClassLine(declarationSpans));
        lines.Add(new ClassLine([Brace("{")]));

        var indent = string.Empty.PadLeft(GlyphClassRenderRules.BodyIndentSpaces);
        var memberPalette = ResolveRolePalette(contextNode, ctx, RegexSpanKind.RegexEnumMember);
        var members = type.GetFields(BindingFlags.Public | BindingFlags.Static);

        for (int i = 0; i < members.Length; i++)
        {
            var member = members[i];

            foreach (var attribute in GetDisplayableAttributes(member))
                lines.Add(BuildAttributeLine(attribute, indent));

            var suffix = i < members.Length - 1 ? "," : "";
            var memberPath = $"{ctx.RootPath}_{member.Name}";
            lines.Add(new ClassLine([new ClassSpan($"{indent}{member.Name}{suffix}", memberPath, memberPalette)]));
        }

        lines.Add(new ClassLine([Brace("}")]));

        return lines;
    }

    /// <summary>
    /// The <c>public override Nib[] Nibs => [...]</c> line, or null if <paramref name="contextNode"/>'s type
    /// doesn't declare its own <see cref="Glyph.Nibs"/> override (e.g. <see cref="OneOfBase"/> subclasses
    /// like a concrete <see cref="GlyphOneOf"/>, which rely entirely on reflected property nibs and so have
    /// no such line in their real source). Walks the raw declared <see cref="Nib"/>[] itself - not
    /// <paramref name="contextNode"/>'s <see cref="NamedGroupNode.Children"/>, which flattens every non-property
    /// nib down to plain literal text and so can't tell an <see cref="OptionalNib"/>/<see cref="NibAlternatives"/>/
    /// <see cref="OptionalPluralNib"/> apart from an ordinary string - so each nib's real authored call syntax
    /// (<c>Opt(...)</c>, <c>Alt(...)</c>, <c>Plural()</c>) can be reconstructed exactly.
    /// </summary>
    static ClassLine BuildNibsLine(NamedGroupNode contextNode, RenderContext ctx, string indent)
    {
        var type = contextNode.Navigation.NodeType;

        if (!type.GetProps().Any(x => x.Name == nameof(Glyph.Nibs)))
            return null;

        List<ClassSpan> spans = [Keyword($"{indent}public override Nib[] Nibs => "), Brace("[")];
        var nibs = contextNode.Navigation.GlyphTypeConfiguration.Nibs;
        var propertyNodes = contextNode.NamedGroupChildren;
        var propertyIndex = 0;

        for (int i = 0; i < nibs.Length; i++)
        {
            if (i > 0)
                spans.Add(Brace(", "));

            switch (nibs[i])
            {
                case PropertyNib:
                    var propertyNode = propertyNodes[propertyIndex++];
                    spans.Add(Keyword("Prop("));
                    spans.Add(BuildReferenceSpan(propertyNode.Navigation.Prop.Name, propertyNode, ctx, PropertyPalette(propertyNode, ctx)));
                    spans.Add(Keyword(")"));
                    break;

                case NibAlternatives alternatives:
                    spans.Add(Keyword("Alt("));
                    for (int a = 0; a < alternatives.Alternatives.Length; a++)
                    {
                        if (a > 0)
                            spans.Add(Brace(", "));
                        spans.AddRange(LiteralSpans(alternatives.Alternatives[a], contextNode, ctx));
                    }
                    spans.Add(Keyword(")"));
                    break;

                case OptionalPluralNib:
                    spans.Add(Keyword("Plural()"));
                    break;

                case OptionalNib optional:
                    spans.Add(Keyword("Opt("));
                    spans.AddRange(LiteralSpans(optional.Text, contextNode, ctx));
                    spans.Add(Keyword(")"));
                    break;

                case var plain:
                    spans.AddRange(LiteralSpans(plain.Text, contextNode, ctx));
                    break;
            }
        }

        spans.Add(Brace("];"));
        return new ClassLine(spans);
    }

    /// <summary>The <c>public override Joiner Joiner => Joiner.X;</c> line, or null if <paramref name="contextNode"/>'s type doesn't declare its own <see cref="Glyph.Joiner"/> override.</summary>
    static ClassLine BuildJoinerLine(NamedGroupNode contextNode, string indent)
    {
        var type = contextNode.Navigation.NodeType;

        if (!type.GetProps().Any(x => x.Name == nameof(Glyph.Joiner)))
            return null;

        return new ClassLine(
        [
            Keyword($"{indent}public override Joiner Joiner => "),
            Keyword($"{nameof(Joiner)}.{contextNode.Navigation.GlyphTypeConfiguration.ChildJoiner}"),
            Punctuation(";"),
        ]);
    }

    /// <summary>A quoted literal's spans: neutral quote marks (verbatim <c>@"</c> when <paramref name="text"/> contains a backslash - see <see cref="FormatStringLiteral"/>) around content colored via <see cref="LiteralPalette"/>.</summary>
    static List<ClassSpan> LiteralSpans(string text, NamedGroupNode contextNode, RenderContext ctx)
    {
        var isVerbatim = text.Contains('\\');
        var content = isVerbatim ? text.Replace("\"", "\"\"") : text.Replace("\"", "\\\"");

        return
        [
            Brace(isVerbatim ? "@\"" : "\""),
            new ClassSpan(content, null, LiteralPalette(contextNode, ctx)),
            Brace("\""),
        ];
    }

    static ClassLine BuildPropertyLine(NamedGroupNode node, RenderContext ctx, string indent)
    {
        List<ClassSpan> spans = [Keyword($"{indent}public ")];
        spans.AddRange(BuildTypeSpans(node.Navigation.Type, node, ctx));
        spans.Add(Keyword(" "));
        spans.Add(BuildReferenceSpan(node.Navigation.Prop.Name, node, ctx, PropertyPalette(node, ctx)));

        // { get; set; } - three neutral shades (brace / accessor keyword / punctuation), per GlyphClassRenderRules.
        spans.Add(Brace(" {"));
        spans.Add(Keyword(" get"));
        spans.Add(Punctuation(";"));
        spans.Add(Keyword(" set"));
        spans.Add(Punctuation(";"));
        spans.Add(Brace(" }"));

        return new ClassLine(spans);
    }

    static ClassLine BuildAttributeLine(CustomAttributeData attribute, string indent) =>
        new([Brace($"{indent}["), Keyword(FormatAttribute(attribute)), Brace("]")]);

    /// <summary>
    /// <paramref name="declaredType"/>'s spans, recursively unwrapping <c>List&lt;&gt;</c>/nullable wrappers
    /// and, for a generic Glyph wrapper like <c>OneOf&lt;T1,T2&gt;</c>, coloring the wrapper name via
    /// <paramref name="node"/>'s own hue and each type argument via its own matching child node's - see
    /// <see cref="GlyphNode.GetDescriptiveChildName"/>, which is what gives each such argument its own
    /// distinct child node in the first place. Every type-name occurrence built here (the wrapper name and
    /// each leaf type name) uses <see cref="TypePalette"/> rather than <see cref="PropertyPalette"/>, so a
    /// property's type reads as visually distinct from its name even though both share the same node/hue.
    /// </summary>
    static List<ClassSpan> BuildTypeSpans(Type declaredType, NamedGroupNode node, RenderContext ctx)
    {
        if (Navigation.IsListType(declaredType))
        {
            List<ClassSpan> listSpans = [Brace("List<")];
            listSpans.AddRange(BuildTypeSpans(declaredType.GetGenericArguments()[0], node, ctx));
            listSpans.Add(Brace(">"));
            return listSpans;
        }

        var isNullable = Nullable.GetUnderlyingType(declaredType) != null;
        var underlyingType = declaredType.GetUnderlyingType();
        List<ClassSpan> spans;

        if (underlyingType.IsGenericType)
        {
            var wrapperName = underlyingType.Name[..underlyingType.Name.IndexOf('`')];
            spans = [new ClassSpan(wrapperName, ctx.Fqn(node), TypePalette(node, ctx)), Brace("<")];

            var typeArguments = underlyingType.GetGenericArguments();
            for (int i = 0; i < typeArguments.Length; i++)
            {
                if (i > 0)
                    spans.Add(Brace(", "));

                var argumentUnderlyingType = typeArguments[i].GetUnderlyingType();
                var argumentNode = node.NamedGroupChildren.FirstOrDefault(x => x.Navigation.UnderlyingType == argumentUnderlyingType);

                spans.AddRange(argumentNode != null
                    ? BuildTypeSpans(typeArguments[i], argumentNode, ctx)
                    : [Keyword(FormatTypeNamePlain(argumentUnderlyingType))]);
            }

            spans.Add(Brace(">"));
        }
        else
        {
            spans = [BuildReferenceSpan(FriendlyLeafName(underlyingType), node, ctx, TypePalette(node, ctx))];
        }

        if (isNullable)
            spans.Add(Brace("?"));

        return spans;
    }

    /// <summary>
    /// The shared span shape for anything that names <paramref name="node"/> and might be interactive - a
    /// property's type-name occurrence, its property-name occurrence, or its <c>Prop(...)</c> reference in a
    /// Nibs array - all three read the same node the same way: data-pathed via <see cref="RenderContext.Fqn"/>,
    /// and either directly click-navigable (<see cref="IsNavigable"/>) or offering a <see cref="ClassSpan.Resolutions"/>
    /// menu instead (see <see cref="GetDynamicResolutions"/>) - never both. <paramref name="palette"/> is left
    /// to the caller so a type-name occurrence (<see cref="TypePalette"/>) and a name occurrence
    /// (<see cref="PropertyPalette"/>) of the very same node can still read as visually distinct.
    /// </summary>
    static ClassSpan BuildReferenceSpan(string content, NamedGroupNode node, RenderContext ctx, SpanStylePalette palette) =>
        new(content, ctx.Fqn(node), palette, IsNavigable(node) ? node : null, GetDynamicResolutions(node, ctx));

    /// <summary>
    /// Every concrete type <paramref name="node"/> (a <see cref="DynamicGlyphNode"/>) actually resolved to
    /// somewhere in the corpus, as click-to-pick <see cref="DynamicResolutionOption"/>s - null for any other
    /// node kind, when <see cref="RenderContext.AllSummaries"/> wasn't supplied, or when nothing was ever
    /// captured for it. Looked up via <paramref name="node"/>'s own true graph root
    /// (<see cref="RegexNode.Lineage"/>[0]) rather than <see cref="RenderContext.Root"/> itself, so this still
    /// finds the right <see cref="GlyphOccurrenceSummary"/> - the one keyed to whichever top-level type this
    /// dynamic node's own graph actually belongs to - even when <paramref name="node"/> sits deep inside a
    /// <see cref="DependentAttribute"/> sub-type nested within that graph, or inside a resolved dynamic
    /// capture's own separate graph reached by a previous jump through this same menu.
    /// </summary>
    static List<DynamicResolutionOption> GetDynamicResolutions(NamedGroupNode node, RenderContext ctx)
    {
        if (node.NodeKind != CaptureNodeKind.Dynamic || ctx.AllSummaries == null)
            return null;

        var owningType = ((GroupNode)node.Lineage[0]).Navigation.NodeType;

        if (!ctx.AllSummaries.TryGetValue(owningType, out var ownSummary))
            return null;

        if (!ownSummary.DynamicCaptureSummaries.TryGetValue(node.FullyQualifiedName, out var dynamicSummary) || dynamicSummary.ResolvedTypeGlyphs.Count == 0)
            return null;

        return dynamicSummary.ResolvedTypeGlyphs
            .OrderByDescending(x => x.Value.Count)
            .Select(x =>
            {
                var resolvedGraph = GlyphTypeRegistry.RegexGraphs[x.Key];
                // Must match DynamicSectionBuilder.BuildResolvedTypeContainerBricks's own containerFullyQualifiedName,
                // just rebased through ctx.Fqn instead of node's raw FullyQualifiedName - so navigating here via
                // this menu lands on the exact data-path that same resolved instance's own bricks use there.
                var resolvedPath = $"{ctx.Fqn(node)}_{resolvedGraph.RootNode.FullyQualifiedName}";
                return new DynamicResolutionOption(resolvedGraph.RootNode, resolvedPath, PropertyPalette(resolvedGraph.RootNode, ctx));
            })
            .ToList();
    }

    /// <summary>Every attribute actually declared on <paramref name="member"/> itself (never inherited), restricted to Glyphotype's own attribute types so an unrelated framework/compiler attribute never sneaks into the rendered class.</summary>
    static List<CustomAttributeData> GetDisplayableAttributes(MemberInfo member) =>
        member.GetCustomAttributesData()
            .Where(x => x.AttributeType.Namespace?.StartsWith("Glyphotype", StringComparison.Ordinal) == true)
            .ToList();

    static string FormatAttribute(CustomAttributeData attribute)
    {
        var name = attribute.AttributeType.Name;

        if (name.EndsWith("Attribute", StringComparison.Ordinal))
            name = name[..^"Attribute".Length];

        var arguments = attribute.ConstructorArguments.Select(FormatAttributeArgument).ToList();
        return arguments.Count > 0 ? $"{name}({string.Join(", ", arguments)})" : name;
    }

    static string FormatAttributeArgument(CustomAttributeTypedArgument argument)
    {
        if (argument.Value is IReadOnlyCollection<CustomAttributeTypedArgument> arrayValue)
            return string.Join(", ", arrayValue.Select(FormatAttributeArgument));

        return argument.Value switch
        {
            Type t => $"typeof({FriendlyLeafName(t)})",
            string s => FormatStringLiteral(s),
            null => "null",
            _ => argument.Value.ToString(),
        };
    }

    /// <summary>
    /// A string as a C# literal: verbatim (<c>@"..."</c>, with embedded quotes doubled) when the value
    /// contains a backslash - regular escaped-string syntax would otherwise misrender an authored verbatim
    /// string like <c>@"\+1/\+1"</c> as the syntactically invalid <c>"\+1/\+1"</c> (<c>\+</c> isn't a real
    /// C# escape sequence) - or ordinary quoted syntax (with embedded quotes backslash-escaped) otherwise.
    /// </summary>
    static string FormatStringLiteral(string value) =>
        value.Contains('\\')
            ? $"@\"{value.Replace("\"", "\"\"")}\""
            : $"\"{value.Replace("\"", "\\\"")}\"";

    static string FriendlyLeafName(Type type) =>
        _builtInAliases.TryGetValue(type, out var alias) ? alias : type.Name;

    /// <summary>
    /// A type's full plain-text name, recursively expanding generic type arguments (e.g. <c>CompoundOf&lt;Keyword&gt;</c>
    /// instead of the raw <c>CompoundOf`1</c>) - used where a type name is rendered as plain neutral text with no
    /// per-argument coloring of its own (a base-type declaration, or a generic type argument with no matching
    /// child node to color it from).
    /// </summary>
    static string FormatTypeNamePlain(Type type)
    {
        if (!type.IsGenericType)
            return FriendlyLeafName(type);

        var name = type.Name[..type.Name.IndexOf('`')];
        var args = string.Join(", ", type.GetGenericArguments().Select(FormatTypeNamePlain));
        return $"{name}<{args}>";
    }

    /// <summary>The class-name header's color: <see cref="RegexSpanKind.CommentGroupOpenHeaderText"/>'s treatment applied to <paramref name="node"/>'s own positional hue (forced grayscale for the graph's transparent root).</summary>
    static SpanStylePalette HeaderPalette(NamedGroupNode node, RenderContext ctx) =>
        ResolveRolePalette(node, ctx, RegexSpanKind.CommentGroupOpenHeaderText);

    /// <summary>A Nibs-array literal's color: <see cref="RegexSpanKind.RegexLiteralMatch"/>'s treatment applied to the enclosing class's own hue (literals aren't named groups of their own, so they borrow their class's).</summary>
    static SpanStylePalette LiteralPalette(NamedGroupNode node, RenderContext ctx) =>
        ResolveRolePalette(node, ctx, RegexSpanKind.RegexLiteralMatch);

    /// <summary>A property's type-name occurrence's color: <see cref="RegexSpanKind.CommentGroupBorderWall"/>'s treatment applied to <paramref name="node"/>'s own hue - purely to give the type occurrence a visual treatment distinct from the property-name occurrence of that same node (see <see cref="PropertyPalette"/>), which stays its plain resting color.</summary>
    static SpanStylePalette TypePalette(NamedGroupNode node, RenderContext ctx) =>
        ResolveRolePalette(node, ctx, RegexSpanKind.CommentGroupBorderWall);

    static SpanStylePalette ResolveRolePalette(NamedGroupNode node, RenderContext ctx, RegexSpanKind kind)
    {
        var hex = ctx.Palette[node].Normal;
        var hueDegrees = DeterministicPalette.HexToHue(hex) * 360.0;
        return SmartSpanControlPanel.Resolve(kind, hueDegrees, forceGrayscale: HslMath.IsGrayscale(hex));
    }

    /// <summary>A node's own plain resting color - no role treatment - matching how TypeTreeView/MatchesTable already color this same node.</summary>
    static SpanStylePalette PropertyPalette(NamedGroupNode node, RenderContext ctx) =>
        SpanStylePalette.FromHexPalette(ctx.Palette[node]);

    static SpanStylePalette NeutralPalette(double brightness) =>
        SpanStylePalette.FromKnobs(new ColorKnobs(Saturation: 0, Brightness: brightness, SaturationRange: 0, BrightnessRange: 0));

    static ClassSpan Keyword(string text) => new(text, null, _keywordPalette);
    static ClassSpan Brace(string text) => new(text, null, _bracePalette);
    static ClassSpan Punctuation(string text) => new(text, null, _punctuationPalette);

    /// <summary>
    /// Everything one <see cref="Render"/>/<see cref="RenderEnum"/> call shares across its recursive helper
    /// calls: the palette (and, for dynamic-resolution lookups, every type's own corpus-wide summary), plus
    /// enough to rebase any reachable node's own <see cref="RegexNode.FullyQualifiedName"/> onto wherever
    /// <see cref="Root"/> is conceptually positioned for this call (see <see cref="Fqn"/>).
    /// </summary>
    readonly record struct RenderContext(
        NamedGroupNode Root,
        string RootPath,
        IReadOnlyDictionary<NamedGroupNode, HexPalette> Palette,
        IReadOnlyDictionary<Type, GlyphOccurrenceSummary> AllSummaries)
    {
        /// <summary>
        /// <paramref name="node"/>'s data-path, rebased from its own real <see cref="RegexNode.FullyQualifiedName"/>
        /// onto <see cref="RootPath"/> the same way <see cref="Root"/> itself was rebased - identity (returns
        /// <paramref name="node"/>'s own FullyQualifiedName unchanged) when <see cref="RootPath"/> is
        /// <see cref="Root"/>'s own real FullyQualifiedName, i.e. ordinary in-graph navigation.
        /// </summary>
        public string Fqn(NamedGroupNode node) =>
            RootPath + node.FullyQualifiedName[Root.FullyQualifiedName.Length..];
    }
}
