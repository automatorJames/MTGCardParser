namespace MTGPlexer.DTOs;

/// <summary>
/// Represents a single, fully processed line from a card, containing both the
/// hierarchical analysis of matched tokens (SpanRoots) and a list of any
/// unmatched occurrences.
/// </summary>
public record ProcessedLine
{
    public Card Card { get; init; }
    public int LineIndex { get; init; }
    public string SourceText { get; init; }
    public List<Token<Type>> SourceTokens { get; init; }

    /// <summary>
    /// The hierarchical representation of matched tokens on this line.
    /// This is the property you need for your other downstream code.
    /// </summary>
    public List<SpanRoot> SpanRoots { get; init; }

    /// <summary>
    /// A list of all full spans found on this specific line.
    /// </summary>
    public List<SpanOccurrence> SpanOccurrences { get; init; }
}

