namespace MTGPlexer.TokenAnalysis;

/// <summary>
/// A single word plus how many times it appeared immediately
/// before or after a given span in the corpus.
/// </summary>
public record SpanAdjacentWord(string Word, int Frequency);
