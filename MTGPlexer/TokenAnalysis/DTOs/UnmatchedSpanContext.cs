namespace MTGPlexer.TokenAnalysis.DTOs;

/// <summary>
/// An unmatched span, its global count, and the
/// frequency‐ordered lists of preceding & following words.
/// </summary>
public record UnmatchedSpanContext
(
    UnmatchedSpanCount UnmatchedSpanCount,
    List<AdjacencyNode> Preceding,
    List<AdjacencyNode> Following,
    List<SpanContext> Contexts
)
{
    public string Text => UnmatchedSpanCount.Text;
    public int OccurrenceCount => UnmatchedSpanCount.OccurrenceCount;
    public int Length => UnmatchedSpanCount.Length;
    public int WordCount => UnmatchedSpanCount.WordCount;
}