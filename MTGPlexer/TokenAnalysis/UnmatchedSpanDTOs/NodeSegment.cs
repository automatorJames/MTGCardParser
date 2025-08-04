namespace MTGPlexer.TokenAnalysis.UnmatchedSpanDTOs;

/// <summary>
/// Represents a single, non-divisible segment within a consolidated AdjacencyNode.
/// This preserves the original text and token type for future inline coloring.
/// </summary>
public record NodeSegment(string Text, Type TokenType);