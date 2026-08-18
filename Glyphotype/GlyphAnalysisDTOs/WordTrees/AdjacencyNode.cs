namespace Glyphotype.GlyphAnalysisDTOs.WordTrees;

/// <summary>
/// Represents a node in an adjacency tree. Each node corresponds to a single logical segment,
/// which may be a combination of several collapsed raw tokens.
/// </summary>
public record AdjacencyNode
{
    /// <summary>
    /// The segment of text this node represents. For collapsed unmatched-text nodes (i.e. not backed
    /// by a <see cref="Glyph"/>), this contains the combined text, and its Palettes dictionary will be null.
    /// </summary>
    public NodeSegment Segment { get; init; }

    public List<string> SourceOccurrenceDocumentNames { get; init; }

    public List<AdjacencyNode> Children { get; init; }

    // --- Properties for JS Visualization ---

    public string Id { get; set; }

    /// <summary>
    /// The text for this node, derived directly from its segment.
    /// </summary>
    public string Text { get; init; }

    /// <summary>
    /// The map of palettes for this node, derived directly from its segment.
    /// The keys are character start indices within the Text property.
    /// </summary>
    public Dictionary<int, HexPalette> SpanPalettes => Segment.Palettes;

    /// <summary>
    /// The simplified constructor that was a primary goal of this refactoring.
    /// </summary>
    public AdjacencyNode(NodeSegment segment, List<string> sourceOccurrenceDocumentNames, List<AdjacencyNode> children)
    {
        Segment = segment;
        SourceOccurrenceDocumentNames = sourceOccurrenceDocumentNames;
        Children = children;
        Text = Segment.Text;
    }

    public override string ToString() => Text;
}