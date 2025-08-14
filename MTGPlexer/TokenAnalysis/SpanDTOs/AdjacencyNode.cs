using MTGPlexer.TokenAnalysis.SpanDTOs;

/// <summary>
/// Represents a node in an adjacency tree. Each node corresponds to a single logical segment,
/// which may be a combination of several collapsed raw tokens.
/// </summary>
public record AdjacencyNode
{
    /// <summary>
    /// The segment of text this node represents. For collapsed "DefaultUnmatchedString" nodes,
    /// this contains the combined text, and its Palette will be null.
    /// </summary>
    public NodeSegment Segment { get; init; }

    public List<CardSpanKey> SourceOccurrences { get; init; }

    public List<AdjacencyNode> Children { get; init; }

    // --- Properties for JS Visualization ---

    public string Id { get; set; }
    public List<string> SourceOccurrenceKeys => SourceOccurrences.Select(k => k.Key).ToList();

    /// <summary>
    /// The text for this node, derived directly from its segment.
    /// </summary>
    public string Text { get; init; }

    /// <summary>
    /// The palette for this node, derived directly from its segment.
    /// </summary>
    public DeterministicPalette SpanPalette => Segment.Palette;

    /// <summary>
    /// The simplified constructor that was a primary goal of this refactoring.
    /// </summary>
    public AdjacencyNode(NodeSegment segment, List<CardSpanKey> sourceOccurrences, List<AdjacencyNode> children)
    {
        Segment = segment;
        SourceOccurrences = sourceOccurrences;
        Children = children;
        Text = Segment.Text;
    }

    public override string ToString() => Text;
}