using Glyphotype.GlyphAnalysisDTOs.TypeExpressions;

namespace Glyphotype.GlyphAnalysisDTOs;

/// <summary>
/// A consolidated processor that tokenizes a corpus of documents and produces a complete
/// analysis of both matched tokens (as GlyphCaptureSummary) and word span trees in a single workflow.
/// </summary>
public class DocumentCorpusAnalyzer
{
    IDocumentRepository _repository;
    bool _isInitialized;

    /// <summary>
    /// Structured list of all processed documents, containing the hierarchical
    /// GlyphCaptureSummary analysis for each line. This is the output for your matched-token logic.
    /// </summary>
    public List<ProcessedDocument> ProcessedDocuments { get; private set; }


    /// <summary>
    /// Word trees build around all maximal repeated spans across the corpus
    /// including Glyph class captures. Useful for analyzing which spans
    /// of text have not yet been captured by any Glyph.
    /// </summary>
    public DigestedText DigestedTextWithCaptureGlyphs { get; private set; }

    /// <summary>
    /// Per-type occurrence summaries for every registered top-level Glyph type, keyed by type.
    /// Types with no matches across the corpus are still represented, with OccurrenceCount == 0.
    /// </summary>
    public Dictionary<Type, GlyphOccurrenceSummary> GlyphOccurrenceSummaries { get; private set; } = [];

    public DocumentCorpusAnalyzer(IDocumentRepository repository)
    {
        _repository = repository;
    }

    public async Task EnsureInitializedAsync()
    {
        if (_isInitialized) return;

        // set ProcessedDocuments
        // Each document tokenizes independently, so this is CPU-bound and parallelizable.
        // AsOrdered keeps the result in the same order as GetDocumentsAsync returned it.
        var documents = await _repository.GetDocumentsAsync();

        ProcessedDocuments = documents
            .AsParallel()
            .AsOrdered()
            .Select(x => new ProcessedDocument(x))
            .ToList();

        DigestedTextWithCaptureGlyphs = new DigestedText(ProcessedDocuments);

        // set GlyphOccurrenceSummaries
        var rootGlyphsByType = ProcessedDocuments
            .SelectMany(x => x.Lines)
            .SelectMany(x => x.Glyphs)
            .OfType<Glyph>()
            .GroupBy(x => x.Type);

        GlyphOccurrenceSummaries = rootGlyphsByType.ToDictionary(x => x.Key, x => new GlyphOccurrenceSummary(x.Key, x));

        // Registered types with zero matches are still represented, so "hide zero-capture" filtering
        // has actual zero-occurrence entries to hide rather than the type disappearing outright.
        var unmatchedTypes = GlyphTypeRegistry.GetAllTopLevelGlyphTypes()
            .Where(x => typeof(Glyph).IsAssignableFrom(x) && !GlyphOccurrenceSummaries.ContainsKey(x));

        foreach (var type in unmatchedTypes)
            GlyphOccurrenceSummaries[type] = new GlyphOccurrenceSummary(type);

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