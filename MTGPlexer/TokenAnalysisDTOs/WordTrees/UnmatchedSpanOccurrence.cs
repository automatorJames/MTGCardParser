namespace MTGPlexer.TokenAnalysisDTOs.WordTrees;

/// <summary>
/// Represents a single, specific occurrence of an unmatched span of text from one line of a card.
/// This record is the "ground truth" and holds the complete context of the line it appeared in.
/// </summary>
public record UnmatchedSpanOccurrence
{
    public CardSpanKey Key { get; }
    public int LineIndex { get; }

    /// <summary>
    /// The complete array of tokens from the line where this unmatched span occurred.
    /// </summary>
    public TokenUnit[] LineTokens { get; }

    /// <summary>
    /// The index of the specific token of interest within the LineTokens list.
    /// </summary>
    public int AnchorTokenIndex { get; }

    /// <summary>
    /// The full, original unmatched span token.
    /// </summary>
    public DefaultUnmatchedString AnchorSpan { get; }

    public string Text { get; }
    public string[] Words { get; }

    public UnmatchedSpanOccurrence(string cardName, int lineIndex, List<TokenUnit> lineTokens, int anchorTokenIndex)
    {
        LineIndex = lineIndex;
        LineTokens = lineTokens.ToArray();
        AnchorTokenIndex = anchorTokenIndex;
        AnchorSpan = (DefaultUnmatchedString)LineTokens[AnchorTokenIndex];
        Text =  AnchorSpan.Capture.Value;
        Words = Text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        Key = new(cardName, AnchorSpan);
    }

    public override string ToString() => Text;
}