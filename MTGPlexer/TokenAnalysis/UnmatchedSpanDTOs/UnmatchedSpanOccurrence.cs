namespace MTGPlexer.TokenAnalysis.UnmatchedSpanDTOs;

/// <summary>
/// Represents a single, specific occurrence of an unmatched span of text from one line of a card.
/// This record is the "ground truth" and holds the complete context of the line it appeared in.
/// </summary>
public record UnmatchedSpanOccurrence
{
    public CardSpanKey Key { get; }
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

    /// <summary>
    /// Gets the sequence of tokens preceding the unmatched span on its original line,
    /// ordered from the one closest to the span to the one furthest away.
    /// </summary>
    public IEnumerable<Token<Type>> GetPrecedingTokensOnLine() =>
        LineTokens.Take(UnmatchedTokenIndex).Reverse();

    /// <summary>
    /// Gets the sequence of tokens following the unmatched span on its original line,
    /// ordered from the one closest to the span to the one furthest away.
    /// </summary>
    public IEnumerable<Token<Type>> GetFollowingTokensOnLine() =>
        LineTokens.Skip(UnmatchedTokenIndex + 1);

    public string Text { get; }
    public TextSpan Span { get; }
    public string[] Words { get; }
    public int SpanWordCount => Words.Length;

    public UnmatchedSpanOccurrence(string cardName, int lineIndex, List<Token<Type>> lineTokens, int unmatchedTokenIndex)
    {
        LineIndex = lineIndex;
        LineTokens = lineTokens;
        UnmatchedTokenIndex = unmatchedTokenIndex;
        Text = UnmatchedToken.ToStringValue();
        Span = UnmatchedToken.Span;
        Words = Text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        Key = new(cardName, Span);
    }

    public override string ToString() => Text;
}