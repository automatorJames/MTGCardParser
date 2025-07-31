namespace MTGPlexer.TokenAnalysis;

/// <summary>
/// Represents a single, concrete occurrence of a full unmatched span on a specific card.
/// This record captures the full context at the point of tokenization.
/// It replaces the old SpanContext.
/// </summary>
public record UnmatchedSpanOccurrence
{
    public string Key { get; init; }
    public string CardName { get; init; }
    public int LineIndex { get; init; }

    /// <summary>
    /// The full, original unmatched span token.
    /// </summary>
    public Token<Type> UnmatchedToken { get; init; }

    /// <summary>
    /// The token immediately preceding the UnmatchedToken on its line. Can be null.
    /// </summary>
    public Token<Type>? PrecedingToken { get; init; }

    /// <summary>
    /// The token immediately following the UnmatchedToken on its line. Can be null.
    /// </summary>
    public Token<Type>? FollowingToken { get; init; }

    public string PrecedingWord { get; init; }
    public string FollowingWord { get; init; }

    // Properties are calculated once and stored, per Axiom #2.
    public string SpanText { get; }
    public string[] SpanWords { get; }
    public int SpanWordCount { get; }

    public UnmatchedSpanOccurrence(string cardName, int lineIndex, Token<Type>? preceding, Token<Type> unmatched, Token<Type>? following)
    {
        CardName = cardName;
        LineIndex = lineIndex;
        PrecedingToken = preceding;
        UnmatchedToken = unmatched;
        FollowingToken = following;
        PrecedingWord = preceding?.ToStringValue().Split(' ').LastOrDefault();
        FollowingWord = following?.ToStringValue().Split(' ').FirstOrDefault();

        // Calculate text-based properties immediately.
        SpanText = UnmatchedToken.ToStringValue();
        SpanWords = SpanText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        SpanWordCount = SpanWords.Length;

        Key = cardName.Dot(unmatched.Span.IndexString());
    }
}