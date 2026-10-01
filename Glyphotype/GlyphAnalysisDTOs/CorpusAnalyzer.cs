using Glyphotype.GlyphAnalysisDTOs.TypeExpressions;

namespace Glyphotype.GlyphAnalysisDTOs;

/// <summary>
/// A consolidated processor that tokenizes a corpus of documents and produces a complete
/// analysis of both matched tokens (as GlyphCaptureSummary) and word span trees in a single workflow.
/// </summary>
public class CorpusAnalyzer
{
    IDocumentRepository _repository;
    readonly GlyphGrammar _grammar;
    bool _isInitialized;

    /// <summary>
    /// Structured list of all processed documents, containing the hierarchical
    /// GlyphCaptureSummary analysis for each line. This is the output for your matched-token logic.
    /// </summary>
    public List<ProcessedDocument> ProcessedDocuments { get; private set; }

    /// <summary>
    /// Total count of all words across every document in the corpus.
    /// </summary>
    public int WordCount { get; private set; }

    /// <summary>
    /// Count of all words captured by a matched RootCaptureTrace across every document in the corpus.
    /// </summary>
    public int CapturedWordCount { get; private set; }

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

    /// <summary>Analyzes <paramref name="repository"/>'s documents as tokenized by <paramref name="grammar"/>.</summary>
    public CorpusAnalyzer(IDocumentRepository repository, GlyphGrammar grammar)
    {
        _repository = repository;
        _grammar = grammar;
    }

    /// <summary>The grammar the corpus is tokenized by.</summary>
    public GlyphGrammar Grammar => _grammar;

    /// <summary>An analysis of a corpus <paramref name="grammar"/> has already tokenized - nothing is tokenized again.</summary>
    public static CorpusAnalyzer FromProcessedDocuments(IReadOnlyList<ProcessedDocument> documents, GlyphGrammar grammar)
    {
        var analyzer = new CorpusAnalyzer(repository: null, grammar);
        analyzer.Analyze(documents.ToList());
        return analyzer;
    }

    public async Task EnsureInitializedAsync()
    {
        if (_isInitialized) return;

        // set ProcessedDocuments
        // Each document tokenizes independently, so this is CPU-bound and parallelizable.
        // AsOrdered keeps the result in the same order as GetDocumentsAsync returned it.
        var documents = await _repository.GetDocumentsAsync();

        Analyze(documents
            .AsParallel()
            .AsOrdered()
            .Select(x => new ProcessedDocument(x, _grammar))
            .ToList());
    }

    void Analyze(List<ProcessedDocument> processedDocuments)
    {
        ProcessedDocuments = processedDocuments;

        WordCount = ProcessedDocuments.Sum(x => x.WordCount);
        CapturedWordCount = ProcessedDocuments.Sum(x => x.CapturedWordCount);
        DigestedTextWithCaptureGlyphs = new DigestedText(ProcessedDocuments);

        // set GlyphOccurrenceSummaries
        var rootMatchOccurrencesByType = ProcessedDocuments
            .SelectMany(document => document.Lines
                .SelectMany(line => line.Glyphs)
                .OfType<Glyph>()
                .Select(glyph => new MatchOccurrence(document.Document.Name, glyph)))
            .GroupBy(x => x.Glyph.Type);

        GlyphOccurrenceSummaries = rootMatchOccurrencesByType.ToDictionary(x => x.Key, x => new GlyphOccurrenceSummary(x.Key, x));

        // Registered types with zero matches are still represented, so "hide zero-capture" filtering
        // has actual zero-occurrence entries to hide rather than the type disappearing outright.
        var unmatchedTypes = _grammar.TopLevelTypes
            .Where(x => typeof(Glyph).IsAssignableFrom(x) && !GlyphOccurrenceSummaries.ContainsKey(x));

        foreach (var type in unmatchedTypes)
            GlyphOccurrenceSummaries[type] = new GlyphOccurrenceSummary(type);

        _isInitialized = true;
    }
}