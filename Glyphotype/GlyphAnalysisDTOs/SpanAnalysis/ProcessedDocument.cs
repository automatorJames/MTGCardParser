namespace Glyphotype.GlyphAnalysisDTOs.SpanAnalysis;
/// <summary>
/// Represents a single document and all its processed lines of text.
/// This is a lightweight container for the results of the CorpusAnalyzer.
/// </summary>
public class ProcessedDocument
{
    static HashSet<string> _irrelevantUnmatchedStrings = [".", ". ", " "];

    public IDocument Document { get; init; }
    public List<ProcessedLine> Lines { get; init; }
    public bool IsFullyMatched { get; init; }

    public ProcessedDocument(IDocument document)
    {
        Document = document;
        Lines = ProcessedLine.GetAll(document);

        // "Fully matched" means no unmatched text occurrences (except isolated periods) exist
        IsFullyMatched = Lines
            .SelectMany(x => x.UnmatchedTextOccurrences.Where(y => !_irrelevantUnmatchedStrings.Contains(y.Text)))
            .Count() == 0;
    }
}