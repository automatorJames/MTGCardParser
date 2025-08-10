namespace MTGPlexer.TokenAnalysis;

/// <summary>
/// Represents a node in an adjacency tree, now capable of holding consolidated segments.
/// </summary>
public record AdjacencyNode
{
    /// <summary>
    /// A list of sequential segments that make up this node's text.
    /// A non-consolidated node will have a single segment.
    /// </summary>
    [JsonPropertyName("segments")]
    public List<NodeSegment> Segments { get; init; }

    public List<CardSpanKey> SourceOccurrences { get; init; }

    public List<AdjacencyNode> Children { get; init; }

    // --- Properties for JS Visualization ---

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("sourceOccurrenceKeys")]
    public List<string> SourceOccurrenceKeys => SourceOccurrences.Select(k => k.Key).ToList();

    // This helper property joins the text for display logic in JS.
    [JsonPropertyName("text")]
    public string Text => string.Join(" ", Segments.Select(s => s.Text));

    public AdjacencyNode(List<NodeSegment> segments, List<CardSpanKey> sourceOccurrences, List<AdjacencyNode> children)
    {
        Segments = segments;
        SourceOccurrences = sourceOccurrences;
        Children = children;
    }
}