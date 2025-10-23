namespace MTGPlexer.CommonDTOs;

/// <summary>
/// Represents a single, fully processed line from a card, containing both the
/// hierarchical analysis of matched tokens (TokenCaptureSummaries) and a list of any
/// unmatched occurrences.
/// </summary>
public class ProcessedLine
{
    public Card Card { get; init; }
    public int LineIndex { get; init; }
    public string EvaluatedText { get; init; }

    /// <summary>
    /// The hierarchical representation of matched tokens on this line.
    /// </summary>
    public List<SpanRoot> SpanRoots { get; init; }

    /// <summary>
    /// A list of all full spans found on this specific line.
    /// </summary>
    public List<UnmatchedTextOccurrence> UnmatchedTextOccurrences { get; init; }

    public ProcessedLine(Card card, int lineIndex, string evaluatedText, List<SpanRoot> spanRoots, List<UnmatchedTextOccurrence> unmatchedTextOccurrences)
    {
        Card = card;
        LineIndex = lineIndex;
        EvaluatedText = evaluatedText;
        SpanRoots = spanRoots;
        UnmatchedTextOccurrences = unmatchedTextOccurrences;
    }

    public static List<ProcessedLine> GetAll(Card card)
    {
        List<ProcessedLine> lines = [];

        for (int i = 0; i < card.FormattedLinesLower.Length; i++)
        {
            var lineText = card.FormattedLinesLower[i];
            var originalLineText = card.FormattedLines[i];

            if (string.IsNullOrWhiteSpace(lineText))
                continue;

            List<SpanRoot> spanRoots =
                TokenTypeRegistry.Tokenize(lineText)
                .Select(x => TokenCaptureBuilder.CreateFrom(x, originalLineText, card.Name, i))
                .ToList();

            List<UnmatchedTextOccurrence> unmatchedStringOccurrences = GetUnmatchedStringOccurrences(card, spanRoots, i, originalLineText);
            lines.Add(new ProcessedLine(card, i, lineText, spanRoots, unmatchedStringOccurrences));
        }


        return lines;
    }

    static List<UnmatchedTextOccurrence> GetUnmatchedStringOccurrences(Card card, List<SpanRoot> lineSpanRoots, int lineIndex, string originalLineText)
    {
        var occurrences = new List<UnmatchedTextOccurrence>();

        for (int i = 0; i < lineSpanRoots.Count; i++)
        {
            var spanRoot = lineSpanRoots[i];

            // Check for and record unmatched tokens
            // Create a new occurrence, giving it the context of the entire line's tokens.
            if (spanRoot.RootToken.Type == typeof(DefaultUnmatchedString))
                occurrences.Add(new UnmatchedTextOccurrence(card.Name, lineIndex, lineSpanRoots, i));
        }

        return occurrences;
    }
}