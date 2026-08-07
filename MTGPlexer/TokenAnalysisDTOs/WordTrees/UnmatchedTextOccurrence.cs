namespace MTGPlexer.TokenAnalysisDTOs.WordTrees;

/// <summary>
/// Represents a single, specific occurrence of an unmatched span of text from one line of a card.
/// This record is the "ground truth" and holds the complete context of the line it appeared in.
/// </summary>
public record UnmatchedTextOccurrence
{
    public string CardName { get; }
    public int LineIndex { get; }

    /// <summary>
    /// The complete array of tokens from the line where this unmatched span occurred.
    /// </summary>
    public CaptureUnit[] LineTokenUnits { get; }

    /// <summary>
    /// The index of the specific token of interest within the LineTokens list.
    /// </summary>
    public int UnmatchedTokenIndex { get; }

    /// <summary>
    /// The full, original unmatched span token.
    /// </summary>
    public UnmatchedString Anchor { get; }

    public string Text { get; }
    public string[] Words { get; }

    public UnmatchedTextOccurrence(string cardName, int lineIndex, IEnumerable<CaptureUnit> lineTokenUnits, int unmatchedTokenIndex)
    {
        LineIndex = lineIndex;
        LineTokenUnits = lineTokenUnits.ToArray();
        UnmatchedTokenIndex = unmatchedTokenIndex;
        Anchor = (UnmatchedString)LineTokenUnits[UnmatchedTokenIndex];
        Text = Anchor.CaptureValue;
        Words = Text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        CardName = cardName;
    }

    public override string ToString() => Text;
}