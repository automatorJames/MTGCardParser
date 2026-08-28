namespace Glyphotype.GlyphAnalysisDTOs.WordTrees;

/// <summary>
/// Represents a node in an adjacency tree. Each node corresponds to a single logical segment,
/// which may be a combination of several collapsed raw tokens.
/// </summary>
public record AdjacencyNode
{
    /// <summary>
    /// The segment of text this node represents. Server-side only: <see cref="Text"/> and
    /// <see cref="SpanGlyphTypes"/> are the projections the client actually consumes, so
    /// serializing the segment as well would just duplicate both over the wire.
    /// </summary>
    [JsonIgnore]
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
    /// Which top-level <see cref="Glyph"/> type captured each stretch of <see cref="Text"/>, keyed
    /// by character start index (see <see cref="NodeSegment.GlyphTypeNames"/>). The names index into
    /// <see cref="AnalyzedText.GlyphPalettes"/>, so the tree carries one short name per captured
    /// stretch rather than a repeated copy of the palette itself.
    /// </summary>
    public Dictionary<int, string> SpanGlyphTypes => Segment.GlyphTypeNames;

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
