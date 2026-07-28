namespace MTGPlexer.TokenAnalysisDTOs;

/// <summary>
/// A consolidated processor that tokenizes a corpus of documents and produces a complete
/// analysis of both matched tokens (as TokenCaptureSummary) and word span trees in a single workflow.
/// </summary>
public class DocumentCorpusAnalyzer
{
    IDocumentRepository _repository;
    bool _isInitialized;

    /// <summary>
    /// Structured list of all processed documents, containing the hierarchical
    /// TokenCaptureSummary analysis for each line. This is the output for your matched-token logic.
    /// </summary>
    public List<ProcessedDocument> ProcessedDocuments { get; private set; }


    /// <summary>
    /// Word trees build around all maximal repeated spans across the corpus
    /// including TokenUnit class captures. Useful for analyzing which spans
    /// of text have not yet been captured by any TokenUnit.
    /// </summary>
    public DigestedText DigestedTextWithCaptureTokens { get; private set; }

    public DocumentCorpusAnalyzer(IDocumentRepository repository)
    {
        _repository = repository;
    }

    public async Task EnsureInitializedAsync()
    {
        if (_isInitialized) return;

        var documents = await _repository.GetDocumentsAsync();
        ProcessedDocuments = documents.Select(x => new ProcessedDocument(x)).ToList();
        DigestedTextWithCaptureTokens = new DigestedText(ProcessedDocuments);

        _isInitialized = true;
    }
}