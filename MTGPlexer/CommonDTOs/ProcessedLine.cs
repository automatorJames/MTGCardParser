namespace MTGPlexer.CommonDTOs;

/// <summary>
/// Represents a single, fully processed line from a card, containing both the
/// hierarchical analysis of matched tokens (SpanRoots) and a list of any
/// unmatched occurrences.
/// </summary>
public record ProcessedLine
{
    public Card Card { get; init; }
    public int LineIndex { get; init; }
    public string EvaluatedText { get; init; }
    public List<TokenUnit> SourceTokens { get; init; }

    /// <summary>
    /// The hierarchical representation of matched tokens on this line.
    /// </summary>
    public List<SpanRoot> SpanRoots { get; init; }

    /// <summary>
    /// A list of all full spans found on this specific line.
    /// </summary>
    public List<SpanOccurrence> SpanOccurrences { get; init; }
}

