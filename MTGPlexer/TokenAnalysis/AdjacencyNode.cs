namespace MTGPlexer.TokenAnalysis;

/// <summary>
/// Represents a single node in an adjacency tree, now enriched with layout metadata for rendering.
/// </summary>
public record AdjacencyNode
{
    public string Text { get; init; }
    public Type TokenType { get; init; }
    public int Frequency { get; init; }
    public List<AdjacencyNode> Children { get; init; }

    // --- New Layout Properties ---

    /// <summary>
    /// The vertical "lane" or "track" this node occupies. The root is 0.
    /// </summary>
    public int VerticalLane { get; set; }

    /// <summary>
    /// The total number of vertical lanes this node and all its descendants occupy.
    /// </summary>
    public int TotalDescendantLanes { get; set; }


    public AdjacencyNode(string text, Type tokenType, int frequency, List<AdjacencyNode> children)
    {
        Text = text;
        TokenType = tokenType;
        Frequency = frequency;
        Children = children;
    }
}