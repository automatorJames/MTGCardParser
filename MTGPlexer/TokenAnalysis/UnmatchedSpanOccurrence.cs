namespace MTGPlexer.TokenAnalysis;

/// <summary>
/// Represents a single, specific occurrence of an unmatched span of text from one line of a card.
/// This record is the "ground truth" and holds the complete context of the line it appeared in.
/// </summary>
public record UnmatchedSpanOccurrence
{
    public string CardName { get; }
    public int LineIndex { get; }

    /// <summary>
    /// The complete list of tokens from the line where this unmatched span occurred.
    /// </summary>
    public List<Token<Type>> LineTokens { get; }

    /// <summary>
    /// The index of this specific UnmatchedToken within the LineTokens list.
    /// </summary>
    public int UnmatchedTokenIndex { get; }

    // --- Derived Properties for Convenience ---

    /// <summary>
    /// The full, original unmatched span token.
    /// </summary>
    public Token<Type> UnmatchedToken => LineTokens[UnmatchedTokenIndex];

    /// <summary>
    /// The token immediately preceding the UnmatchedToken on its line. Can be null.
    /// </summary>
    public Token<Type>? PrecedingToken => UnmatchedTokenIndex > 0 ? LineTokens[UnmatchedTokenIndex - 1] : null;

    /// <summary>
    /// The token immediately following the UnmatchedToken on its line. Can be null.
    /// </summary>
    public Token<Type>? FollowingToken => UnmatchedTokenIndex < LineTokens.Count - 1 ? LineTokens[UnmatchedTokenIndex + 1] : null;

    public string SpanText { get; }
    public string[] SpanWords { get; }
    public int SpanWordCount => SpanWords.Length;

    public UnmatchedSpanOccurrence(string cardName, int lineIndex, List<Token<Type>> lineTokens, int unmatchedTokenIndex)
    {
        CardName = cardName;
        LineIndex = lineIndex;
        LineTokens = lineTokens;
        UnmatchedTokenIndex = unmatchedTokenIndex;

        SpanText = UnmatchedToken.ToStringValue();
        SpanWords = SpanText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    }
}