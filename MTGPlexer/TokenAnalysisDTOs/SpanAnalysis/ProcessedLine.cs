namespace MTGPlexer.TokenAnalysisDTOs.SpanAnalysis;

/// <summary>
/// Represents a single, fully processed line from a card, containing both the
/// hierarchical analysis of matched tokens (TokenCaptureSummaries) and a list of any
/// unmatched occurrences.
/// </summary>
public class ProcessedLine
{
    public SourceTextDTO SourceText { get; set; }

    /// <summary>
    /// The hierarchical representation of matched tokens on this line.
    /// </summary>
    public List<SpanRoot> SpanRoots { get; init; }

    /// <summary>
    /// A list of all full spans found on this specific line.
    /// </summary>
    public List<UnmatchedTextOccurrence> UnmatchedTextOccurrences { get; init; }

    public string DataPath { get; init; }

    public ProcessedLine(SourceTextDTO sourceText, List<SpanRoot> spanRoots, List<UnmatchedTextOccurrence> unmatchedTextOccurrences, string dataPath)
    {
        SourceText = sourceText;
        SpanRoots = spanRoots;
        UnmatchedTextOccurrences = unmatchedTextOccurrences;
        DataPath = dataPath;
    }

    public static List<ProcessedLine> GetAll(Card card)
    {
        List<ProcessedLine> lines = [];

        for (int i = 0; i < card.FormattedLinesLower.Length; i++)
        {
            var formattedText = card.FormattedLinesLower[i];
            var originalText = card.FormattedLines[i];
            SourceTextDTO sourceText = new(formattedText, originalText, card.Name, i);

            if (string.IsNullOrWhiteSpace(formattedText))
                continue;

            List<SpanRoot> spanRoots =
                TokenTypeRegistry.Tokenize(sourceText)
                .Select(x => SpanBuilder.Create(x, originalText, card.Name, i))
                .ToList();

            List<UnmatchedTextOccurrence> unmatchedStringOccurrences = GetUnmatchedStringOccurrences(card, spanRoots, i, originalText);
            var dataPath = card.Name.Replace(' ', '_') + $"-line[{i}]";
            lines.Add(new ProcessedLine(sourceText, spanRoots, unmatchedStringOccurrences, dataPath));
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

    public int GetDeepestChildDepth() => SpanRoots.Max(x => x.GetRecursiveDepth());
}