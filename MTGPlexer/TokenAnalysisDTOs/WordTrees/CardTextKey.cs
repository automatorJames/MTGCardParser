namespace MTGPlexer.TokenAnalysisDTOs.WordTrees;

/// <summary>
/// Uniquely identifies a span of text within a corpus of card texts.
/// </summary>
public record CardTextKey
{
    public string Key { get; }
    public string CardName { get; }
    public int SpanStartIndex { get; }
    public int SpanEndIndex { get; }

    public CardTextKey(string cardName, TokenUnit anchorToken)
        : this(cardName, anchorToken.Match.RegexMatch.Index, anchorToken.Match.RegexMatch.Length + anchorToken.Match.RegexMatch.Index)
    {
    }

    public CardTextKey(string cardName, int spanStartIndex, int spanEndIndex)
    {
        CardName = cardName;
        SpanStartIndex = spanStartIndex;
        SpanEndIndex = spanEndIndex;
        Key = cardName; // The Key is now simplified to just the CardName.
    }
}