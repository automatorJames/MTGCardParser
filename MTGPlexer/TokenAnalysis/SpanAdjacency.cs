namespace MTGPlexer.TokenAnalysis;

/// <summary>
/// Represents an item (either a matched token or an unmatched word) that appeared
/// immediately before or after a given span in the corpus.
/// </summary>
/// <param name="Text">The text of the adjacent item (e.g., "faces" or "with glee").</param>
/// <param name="TokenType">The Type of the adjacent token if it was a matched token; otherwise, null.</param>
/// <param name="Frequency">How many times this exact adjacency occurred.</param>
public record SpanAdjacency(string Text, Type TokenType, int Frequency);
