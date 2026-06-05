namespace MTGPlexer.TokenAnalysisDTOs.SpanAnalysis;

/// <summary>
/// Represents a single, fully processed line from a document, containing both the
/// hierarchical analysis of matched tokens (TokenCaptureSummaries) and a list of any
/// unmatched occurrences.
/// </summary>
public class ProcessedLine
{
    public SourceTextDTO SourceText { get; set; }

    /// <summary>
    /// The hierarchical representation of matched tokens on this line.
    /// </summary>
    public List<TokenUnit> TokenUnits { get; init; }

    /// <summary>
    /// A list of all full spans found on this specific line.
    /// </summary>
    public List<UnmatchedTextOccurrence> UnmatchedTextOccurrences { get; init; }


    public string DataPath { get; init; }

    public ProcessedLine(SourceTextDTO sourceText, List<TokenUnit> tokenUnits, List<UnmatchedTextOccurrence> unmatchedTextOccurrences, string dataPath)
    {
        SourceText = sourceText;
        TokenUnits = tokenUnits;
        UnmatchedTextOccurrences = unmatchedTextOccurrences;
        DataPath = dataPath;
    }

    public static List<ProcessedLine> GetAll(IDocument document)
    {
        var formattedLines = document.GetFormattedLines();
        List<ProcessedLine> lines = [];
        
        for (int i = 0; i < formattedLines.Length; i++)
        {
            var formattedText = formattedLines[i];

            if (string.IsNullOrWhiteSpace(formattedText))
                continue;

            SourceTextDTO sourceText = new(formattedText, document.Name, i);

            var lineTokenUnits = TokenTypeRegistry.Tokenize(sourceText.FormattedText);
            var unmatchedTextOccurrences = GetUnmatchedStringOccurrences(document, lineTokenUnits, i);
            var dataPath = document.Name.Replace(' ', '_') + $"-line[{i}]";

            lines.Add(new ProcessedLine(sourceText, lineTokenUnits, unmatchedTextOccurrences, dataPath));
        }


        return lines;
    }

    static List<UnmatchedTextOccurrence> GetUnmatchedStringOccurrences(IDocument document, List<TokenUnit> lineTokenUnits, int lineIndex)
    {
        var occurrences = new List<UnmatchedTextOccurrence>();

        for (int i = 0; i < lineTokenUnits.Count; i++)
        {
            var tokenUnit = lineTokenUnits[i];

            // Check for and record unmatched tokens
            // Create a new occurrence, giving it the context of the entire line's tokens.
            if (tokenUnit.Type == typeof(DefaultUnmatchedString))
                occurrences.Add(new UnmatchedTextOccurrence(document.Name, lineIndex, lineTokenUnits, i));
        }

        return occurrences;
    }

    //public int GetDeepestChildDepth() => TokenUnits.Max(x => x.NodeGraph.GetRecursiveDepth());
}