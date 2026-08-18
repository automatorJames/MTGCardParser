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
    /// Maps NamedGroupNode FullyQualifiedName -> RegexNode.
    /// </summary>
    public Dictionary<string, NamedGroupNode> NamedGroupFlatGraph { get; } = [];

    /// <summary>
    /// Maps each NamedGroupNode FullyQualifiedName to a minimum unique simplified 
    /// name. The simplified name is typically the name of the node itself
    /// disambiguation is required. For example, "My_Path_To_Node" -> "Node".
    /// </summary>
    public Dictionary<string, string> SimpleUniqueNames { get; } = [];

    public RegexGraph(Type rootGlyphType, GlyphNode rootNode)
    {
        RootGlyphType = rootGlyphType;
        RootNode = rootNode;
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
    public bool TryMatch(string sourceText, out CaptureUnit glyph) =>
        TryMatch(sourceText, 0, sourceText.Length, out glyph);

    /// <summary>
    /// Evaluates if the source text at the current index satisfies the regex and MTG boundary rules.
    /// </summary>
    public bool TryMatch(string sourceText, int currentIndex, int endIndex, out CaptureUnit glyph)
    {
        glyph = null;
        var match = BuiltRegex.Regex.Match(sourceText, currentIndex);
        
        // 1. Regex Success
        // 2. Anchoring: Match must start exactly at currentIndex
        // 3. Length: Match must be non-empty
        // 4. Scope: Match must not exceed endIndex
        if (match.Success && match.Index == currentIndex && match.Length > 0 && (match.Index + match.Length <= endIndex))
        {
            int matchEndIndex = match.Index + match.Length;
        
            // 5. Boundary Check: End of scope OR followed by a boundary char
            bool endsAtBoundary = matchEndIndex == endIndex || (matchEndIndex < endIndex && _boundaryChars.Contains(sourceText[matchEndIndex]));
        
            if (endsAtBoundary)
            {
                CaptureContext captureContext = new(RootNode, match, sourceText);
                var success = RootNode.TryHydrate(captureContext.RootCaptureTrace, out glyph);

                if (!success)
                    return false;

                captureContext.RootCaptureTrace.ClrValue = glyph;
                return true;
            }
        }

        return false;
    }
}
