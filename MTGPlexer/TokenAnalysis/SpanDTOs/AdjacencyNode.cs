namespace MTGPlexer.TokenAnalysis.SpanDTOs;

/// <summary>
/// Represents a node in an adjacency tree, now capable of holding consolidated segments.
/// </summary>
public record AdjacencyNode
{
    /// <summary>
    /// A list of sequential segments that make up this node's text.
    /// A non-consolidated node will have a single segment.
    /// </summary>
    public List<NodeSegment> Segments { get; init; }

    public List<CardSpanKey> SourceOccurrences { get; init; }

    public List<AdjacencyNode> Children { get; init; }

    // --- Properties for JS Visualization ---

    public string Id { get; set; }
    public List<string> SourceOccurrenceKeys => SourceOccurrences.Select(k => k.Key).ToList();
    public string Text { get; init; }
    public DeterministicPalette SpanPalette { get; init; }

    public AdjacencyNode(List<NodeSegment> segments, List<CardSpanKey> sourceOccurrences, List<AdjacencyNode> children)
    {
        Segments = segments;
        SourceOccurrences = sourceOccurrences;
        Children = children;
        Text = string.Join(" ", Segments.Select(s => s.Text));

    }
}