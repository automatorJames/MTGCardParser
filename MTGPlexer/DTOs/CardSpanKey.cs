namespace MTGPlexer.DTOs;

/// <summary>
/// Uniquely identifies a span of text within a corpus of card texts.
/// </summary>
public record CardSpanKey
{
    public string Key { get; }
    public string CardName { get; }
    public int SpanStartIndex { get; }
    public int SpanEndIndex { get; }

    public CardSpanKey(string cardName, TextSpan textSpan)
        : this(cardName, textSpan.Position.Absolute, textSpan.Length - textSpan.Position.Absolute)
    {
    }

    public CardSpanKey(string cardName, int spanStartIndex, int spanEndIndex)
    {
        CardName = cardName;
        SpanStartIndex = spanStartIndex;
        SpanEndIndex = spanEndIndex;
        Key = cardName; // The Key is now simplified to just the CardName.
    }
}