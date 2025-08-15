namespace MTGPlexer.TokenAnalysis.SpanDTOs;

/// <summary>
/// Represents a single, non-divisible segment within a consolidated AdjacencyNode.
/// This preserves the original text and token type for future inline coloring.
/// The Palettes dictionary maps a character start index within the Text to a color,
/// which should apply from that point forward until the next defined start index.
/// </summary>
public record NodeSegment(string Text, Dictionary<int, DeterministicPalette> Palettes);