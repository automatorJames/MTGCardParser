namespace Glyphotype.RegexGeneration.Graph;

/// <summary>
/// The compiled regex-matching representation of one root <see cref="Glyph"/> type: the
/// <see cref="Graph.Nodes.RegexNode"/> tree walked from that type, the <see cref="BuiltRegex"/> compiled
/// from it, and lookup tables (<see cref="NamedGroupFlatGraph"/>, <see cref="SimpleUniqueNames"/>) used
/// to resolve captures and simplified display names.
/// </summary>
public class RegexGraph
{
    static readonly char[] _boundaryChars = [' ', '.'];

    /// <summary>The root <see cref="Glyph"/> type this graph was built from.</summary>
    public Type RootGlyphType { get; }

    /// <summary>The root of the walked <see cref="Graph.Nodes.RegexNode"/> tree.</summary>
    public GlyphNode RootNode { get; }

    /// <summary>The compiled matching regex produced by walking <see cref="RootNode"/>.</summary>
    public BuiltRegex BuiltRegex { get; }

    /// <summary>
    /// Whether <see cref="RootGlyphType"/> carries <see cref="MustMatchWholeLineAttribute"/> - if so,
    /// <see cref="TryMatch(string, int, int, out Glyph)"/> only accepts a match that consumes the entire
    /// requested scope, rather than the usual "ends at a boundary char" allowance for a partial match.
    /// </summary>
    public bool MustMatchWholeLine { get; }

    /// <summary>
    /// Maps NamedGroupNode FullyQualifiedName -> RegexNode.
    /// </summary>
    public Dictionary<string, NamedGroupNode> NamedGroupFlatGraph { get; } = [];

    /// <summary>
    /// Maps each NamedGroupNode FullyQualifiedName to a minimum unique simplified
    /// name. The simplified name is typically the name of the node itself
    /// disambiguation is required. For example, "My_Path_To_Node" -> "Node".
    /// </summary>
    public Dictionary<string, string> SimpleUniqueNames { get; } = [];

    /// <summary>
    /// The positional rainbow palette for every named group in this graph, in <see cref="NamedGroupFlatGraph"/>'s
    /// declaration order (transparent root first) - with the leading <paramref name="positionalOverrideColors"/>
    /// positions pinned to a fixed color instead of a rainbow hue (e.g. a neutral color for the never-boxed
    /// root). The one place every "one fixed color per named group" consumer (the formatted regex's own
    /// coloring, TypeTreeView's boxes) gets its base ordering from, so they only ever differ by choice of
    /// override color, never by a re-derived named-group ordering.
    /// </summary>
    public Dictionary<NamedGroupNode, HexPalette> GetNamedGroupPaletteSet(params HexColor[] positionalOverrideColors) =>
        DeterministicPalette.GetPositionalPaletteSet(NamedGroupFlatGraph.Values, positionalOverrideColors);

    /// <summary>
    /// Same as <see cref="GetNamedGroupPaletteSet(HexColor[])"/>, but appended with any further named
    /// groups that only show up via <paramref name="extraBricks"/> - typically a dynamic capture's
    /// resolved sub-type, spliced in by <see cref="Presentation.DynamicSectionBuilder"/> once actual
    /// occurrence data is available to expand it, which this static graph never declared on its own. Each
    /// gets its own further rainbow slot, in first-appearance order, joining the same rainbow as the base
    /// set rather than starting a new one - the exact named-group ordering <see cref="Presentation.SmartLineRenderer"/>
    /// colors a formatted regex with.
    /// </summary>
    public Dictionary<NamedGroupNode, HexPalette> GetNamedGroupPaletteSet(IEnumerable<RegexBrick> extraBricks, params HexColor[] positionalOverrideColors)
    {
        var namedGroupsInDisplayOrder = NamedGroupFlatGraph.Values
            .Concat(extraBricks.Select(x => x.NamedGroupParent).Where(x => x is not null))
            .Distinct();

        return DeterministicPalette.GetPositionalPaletteSet(namedGroupsInDisplayOrder, positionalOverrideColors);
    }

    public RegexGraph(Type rootGlyphType, GlyphNode rootNode)
    {
        RootGlyphType = rootGlyphType;
        RootNode = rootNode;
        MustMatchWholeLine = rootGlyphType.IsDefined(typeof(MustMatchWholeLineAttribute));
        RegexCollector collector = new();
        RootNode.AppendRegexBricks(collector);
        BuiltRegex = collector.GetBuiltRegex();
        PopulateFlatGraphRecursive();
        PopulateSimpleUniqueNames();
    }

    /// <summary>Builds the root <see cref="Graph.Nodes.RegexNode"/> for <paramref name="rootGlyphType"/> and compiles a full <see cref="RegexGraph"/> from it.</summary>
    public static RegexGraph Create(Type rootGlyphType)
    {
        Navigation navigation = new(rootGlyphType);

        var root = GlyphNode.GetNodeForNavigaton(null, navigation);

        if (root is not GlyphNode glyphNodeRoot)
            throw new Exception($"Expected a {nameof(GlyphNode)}, but got a {root.GetType().Name}");

        return new(rootGlyphType, glyphNodeRoot);
    }

    /// <summary>Depth-first walk populating <see cref="NamedGroupFlatGraph"/> from every named group node in the tree.</summary>
    void PopulateFlatGraphRecursive(NamedGroupNode node = null)
    {
        node ??= RootNode;
        NamedGroupFlatGraph[node.FullyQualifiedName] = node;

        foreach (var child in node.Children.OfType<NamedGroupNode>())
            PopulateFlatGraphRecursive(child);
    }

    /// <summary>Computes <see cref="SimpleUniqueNames"/> by growing each name's suffix (shortest first) until it uniquely identifies that node among all others.</summary>
    void PopulateSimpleUniqueNames()
    {
        foreach (var fullyQualifiedName in NamedGroupFlatGraph.Keys)
        {
            var parts = fullyQualifiedName.Split('_');

            SimpleUniqueNames[fullyQualifiedName] = Enumerable
                .Range(1, parts.Length)
                .Select(partCount => string.Join('_', parts[^partCount..]))
                .First(candidate => NamedGroupFlatGraph.Keys.Count(
                    key => key == candidate || key.EndsWith($"_{candidate}")) == 1);
        }
    }

    /// <summary>Attempts to match and hydrate <paramref name="sourceText"/> in full, from its start to its end.</summary>
    public bool TryMatch(string sourceText, out Glyph glyph) =>
        TryMatch(sourceText, 0, sourceText.Length, out glyph);

    /// <summary>
    /// Evaluates if the source text at the current index satisfies the regex and MTG boundary rules.
    /// </summary>
    public bool TryMatch(string sourceText, int currentIndex, int endIndex, out Glyph glyph)
    {
        glyph = null;
        var match = BuiltRegex.Regex.Match(sourceText, currentIndex);
        int matchEndIndex = match.Index + match.Length;

        // A MustMatchWholeLine type is a special case: nothing else may share its tokenization pass, so
        // it must consume the entire requested scope - ending at a boundary char partway through isn't
        // good enough. Every other type keeps the normal "end of scope, or followed by a boundary char"
        // partial-match allowance.
        bool endsAtBoundary = MustMatchWholeLine
            ? matchEndIndex == endIndex
            : matchEndIndex == endIndex || (matchEndIndex < endIndex && _boundaryChars.Contains(sourceText[matchEndIndex]));

        bool matchIsValid =
            match.Success                   // 1. Regex:        Match must be successful
            && match.Index == currentIndex  // 2. Anchoring:    Must start exactly at currentIndex
            && match.Length > 0             // 3. Length:       Must be non-empty
            && matchEndIndex <= endIndex    // 4. Scope:        Must not exceed endIndex
            && endsAtBoundary;              // 5. Boundary:     Must end at end index, or a boundary char, or if applicable match whole line

        if (!matchIsValid)
            return false;

        CaptureContext captureContext = new(RootNode, match, sourceText);
        var success = RootNode.TryHydrate(captureContext.RootCaptureTrace, out glyph);

        if (!success)
            return false;

        captureContext.RootCaptureTrace.ClrValue = glyph;
        return true;
    }
}
