namespace MTGPlexer.RegexGeneration.Graph;

/// <summary>
/// The compiled regex-matching representation of one root <see cref="TokenUnit"/> type: the
/// <see cref="Graph.Nodes.RegexNode"/> tree walked from that type, the <see cref="BuiltRegex"/> compiled
/// from it, and lookup tables (<see cref="NamedGroupFlatGraph"/>, <see cref="SimpleUniqueNames"/>) used
/// to resolve captures and simplified display names.
/// </summary>
public class RegexGraph
{
    static readonly char[] _boundaryChars = [' ', '.'];

    /// <summary>The root <see cref="TokenUnit"/> type this graph was built from.</summary>
    public Type RootTokenUnitType { get; }

    /// <summary>The root of the walked <see cref="Graph.Nodes.RegexNode"/> tree.</summary>
    public TokenUnitNode RootNode { get; }

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

    public RegexGraph(Type rootTokenUnitType, TokenUnitNode rootNode)
    {
        RootTokenUnitType = rootTokenUnitType;
        RootNode = rootNode;
        RegexCollector collector = new();
        RootNode.AppendRegexBricks(collector);
        BuiltRegex = collector.GetBuiltRegex();
        PopulateFlatGraphRecursive();
        PopulateSimpleUniqueNames();
    }

    /// <summary>Builds the root <see cref="Graph.Nodes.RegexNode"/> for <paramref name="rootTokenUnitType"/> and compiles a full <see cref="RegexGraph"/> from it.</summary>
    public static RegexGraph Create(Type rootTokenUnitType)
    {
        Navigation navigation = new(rootTokenUnitType);

        TokenUnitNode root = rootTokenUnitType switch
        {
            { } t when t.IsAssignableTo(typeof(DefaultUnmatchedString)) => new UnmatchedTokenUnitNode(null, navigation),
            { } t when typeof(TokenUnitOneOf).IsAssignableFrom(t) => new TokenUnitOneOfNode(null, navigation),
            { } t when typeof(TokenUnit).IsAssignableFrom(t) => new TokenUnitNode(null, navigation),
            _ => throw new Exception($"'{rootTokenUnitType}' is not an enum or a {nameof(TokenUnit)} type, which are the only types that are valid named groups")
        };

        return new(rootTokenUnitType, root);
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
    public bool TryMatch(string sourceText, out TokenUnit tokenUnit) =>
        TryMatch(sourceText, 0, sourceText.Length, out tokenUnit);

    /// <summary>
    /// Evaluates if the source text at the current index satisfies the regex and MTG boundary rules.
    /// </summary>
    public bool TryMatch(string sourceText, int currentIndex, int endIndex, out TokenUnit tokenUnit)
    {
        tokenUnit = null;
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
                var success = RootNode.TryHydrate(captureContext, out tokenUnit);

                if (!success)
                    return false;

                captureContext.RootCaptureTrace.ClrValue = tokenUnit;
                return true;
            }
        }

        return false;
    }
}
