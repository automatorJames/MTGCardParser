using MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

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

    /// <summary>
    /// Per-type occurrence summaries for every registered top-level TokenUnit type, keyed by type.
    /// Types with no matches across the corpus are still represented, with OccurrenceCount == 0.
    /// </summary>
    public Dictionary<Type, TokenOccurrenceSummary> TokenOccurrenceSummaries { get; private set; } = [];

    public DocumentCorpusAnalyzer(IDocumentRepository repository)
    {
        _repository = repository;
    }

    public async Task EnsureInitializedAsync()
    {
        if (_isInitialized) return;

        // set ProcessedDocuments
        // Each document tokenizes independently, so this is CPU-bound and embarrassingly
        // parallel; AsOrdered keeps the result in the same order as GetDocumentsAsync returned it.
        var documents = await _repository.GetDocumentsAsync();
        ProcessedDocuments = documents
            .AsParallel()
            .AsOrdered()
            .Select(x => new ProcessedDocument(x))
            .ToList();
        DigestedTextWithCaptureTokens = new DigestedText(ProcessedDocuments);

        // set TokenOccurrenceSummaries
        var rootTokenUnitsByType = ProcessedDocuments
            .SelectMany(x => x.Lines)
            .SelectMany(x => x.TokenUnits)
            .OfType<TokenUnit>()
            .GroupBy(x => x.Type);

        TokenOccurrenceSummaries = rootTokenUnitsByType.ToDictionary(x => x.Key, x => new TokenOccurrenceSummary(x));

        // Registered types with zero matches are still represented, so "hide zero-capture" filtering
        // has actual zero-occurrence entries to hide rather than the type disappearing outright.
        var unmatchedTypes = TokenTypeRegistry.GetAllTopLevelTokenTypes()
            .Where(x => typeof(TokenUnit).IsAssignableFrom(x) && !TokenOccurrenceSummaries.ContainsKey(x));

        foreach (var type in unmatchedTypes)
            TokenOccurrenceSummaries[type] = new TokenOccurrenceSummary(type);

        _isInitialized = true;
    }

    public ProcessedDocument ReprocessDocument(IDocument document)
    {
        var oldProcessedDocument = ProcessedDocuments.FirstOrDefault(x => x.Document == document);
        var index = ProcessedDocuments.IndexOf(oldProcessedDocument);
        ProcessedDocument reprocessedDocument = new(document);
        ProcessedDocuments[index] = reprocessedDocument;

        return reprocessedDocument;
    }
}