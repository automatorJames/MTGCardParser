namespace MTGPlexer.TokenAnalysis.SpanDTOs;

/// <summary>
/// A record to hold structured information about a single token (a word or a recognized token type).
/// </summary>
public record TokenInfo(string Text, Type TokenType);
