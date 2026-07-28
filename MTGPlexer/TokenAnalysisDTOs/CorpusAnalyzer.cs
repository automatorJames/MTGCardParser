namespace MTGPlexer.TokenAnalysisDTOs;

/// <summary>
/// A consolidated processor that tokenizes a corpus of cards and produces a complete
/// analysis of both matched tokens (as TokenCaptureSummary) and word span trees in a single workflow.
/// </summary>
public class CorpusAnalyzer
{
    /// <summary>
    /// Structured list of all processed cards, containing the hierarchical
    /// TokenCaptureSummary analysis for each line. This is the output for your matched-token logic.
    /// </summary>
    public List<ProcessedDocument> ProcessedCards { get; }


    /// <summary>
    /// Word trees build around all maximal repeated spans across the corpus
    /// including TokenUnit class captures. Useful for analyzing which spans
    /// of text have not yet been captured by any TokenUnit.
    /// </summary>
    public DigestedTextCorpus DigestedCorpusWithCaptureTokens { get; }

    public CorpusAnalyzer(List<IDocument> documents)
    {
        ProcessedCards = documents.Select(x => new ProcessedDocument(x)).ToList();
        DigestedCorpusWithCaptureTokens = new DigestedTextCorpus(ProcessedCards);
    }

    public async Task<ProcessedDocument> ReprocessDocument(IDocument document)
    {
        ProcessedDocument reprocessedDocument = new(document);

        // There should be only one, but let's be tolerant of duplicates
        ProcessedCards
            .Where(x => x.Document.Name == document.Name)
            .ToList()
            .ForEach(x => x = reprocessedDocument);

        return reprocessedDocument;
    }

}