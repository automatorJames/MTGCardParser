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

    /// <summary>
    /// Total count of all words across every line in this document.
    /// </summary>
    public int WordCount { get; init; }

    /// <summary>
    /// Count of all words captured by a matched RootCaptureTrace across every line in this document.
    /// </summary>
    public int CapturedWordCount { get; init; }

    /// <summary>Count of every <see cref="BackReference"/> across every line in this document.</summary>
    public int BackReferenceCount { get; init; }

    /// <summary>
    /// Count of the back-references no referent was found for (see <see cref="BackReferenceResolver"/>) - modeled text whose
    /// meaning the grammar still can't pin down, the counterpart for reference of unmatched text for matching.
    /// </summary>
    public int UnresolvedBackReferenceCount { get; init; }

    public ProcessedDocument(IDocument document, GlyphGrammar grammar)
        : this(document, ProcessedLine.GetAll(document, grammar))
    {
    }

    ProcessedDocument(IDocument document, List<ProcessedLine> lines)
    {
        Document = document;
        Lines = lines;

        // "Fully matched" means no unmatched text occurrences (except isolated periods) exist
        IsFullyMatched = Lines
            .SelectMany(x => x.UnmatchedTextOccurrences.Where(y => !_irrelevantUnmatchedStrings.Contains(y.Text)))
            .Count() == 0;

        WordCount = Lines.Sum(x => x.WordCount);
        CapturedWordCount = Lines.Sum(x => x.CapturedWordCount);
        BackReferenceCount = Lines.Sum(x => x.BackReferences.Count);
        UnresolvedBackReferenceCount = Lines.Sum(x => x.BackReferences.Count(y => !y.IsResolved));
    }

    /// <summary>
    /// This document as <paramref name="grammar"/> tokenizes it, where this is how another grammar did and
    /// <paramref name="change"/> is how <paramref name="grammar"/> differs from that one: only the lines that could
    /// tokenize differently are tokenized again, the rest kept as they are - and this document itself is kept, if none could.
    /// </summary>
    public ProcessedDocument Retokenize(GlyphGrammar grammar, TokenizationChange change) =>
        Retokenize(grammar, change.MayDiffer);

    /// <summary>
    /// This document with every line tokenized by <paramref name="grammar"/>'s own types: a line kept from an earlier
    /// grammar's tokenization that holds types <paramref name="grammar"/> superseded (see <see cref="TokenizationChange.Supersedes"/>)
    /// is tokenized again - to the same result, but with these types - and this document itself is kept, if none is.
    /// </summary>
    public ProcessedDocument Refresh(GlyphGrammar grammar) =>
        IsSupersededBy(grammar) ? Retokenize(grammar, x => TokenizationChange.Supersedes(grammar, x)) : this;

    /// <summary>Whether any line holds types other than <paramref name="grammar"/>'s (see <see cref="Refresh"/>).</summary>
    public bool IsSupersededBy(GlyphGrammar grammar) =>
        Lines.Any(x => TokenizationChange.Supersedes(grammar, x));

    ProcessedDocument Retokenize(GlyphGrammar grammar, Func<ProcessedLine, bool> mayDiffer)
    {
        var byIndex = Lines.ToDictionary(x => x.SourceText.LineIndex);
        var anyRetokenized = false;

        ProcessedLine Reuse(int lineIndex, string text)
        {
            if (byIndex.TryGetValue(lineIndex, out var line) && line.SourceText.FormattedText == text && !mayDiffer(line))
                return line;

            anyRetokenized = true;
            return null;
        }

        var lines = ProcessedLine.GetAll(Document, grammar, Reuse);

        return anyRetokenized || lines.Count != Lines.Count ? new(Document, lines) : this;
    }
}