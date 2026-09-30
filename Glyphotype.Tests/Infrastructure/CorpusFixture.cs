namespace Glyphotype.Tests.Infrastructure;

/// <summary>
/// Runs the whole <see cref="TestCorpus"/> through the real pipeline - <see cref="CorpusAnalyzer"/>, the same
/// entry point the app uses - exactly once, shared by every test in <see cref="CorpusCollection"/>.
/// </summary>
public sealed class CorpusFixture
{
    readonly Dictionary<string, ProcessedDocument> _processedByText;

    /// <summary>The test grammar: every Glyph type in this assembly, under the whole-segment rule the app runs with.</summary>
    public GlyphGrammar Grammar { get; } = GlyphGrammar.FromAssemblies([typeof(CorpusFixture).Assembly], allowPartialSegmentMatches: false);

    public IReadOnlyList<ProcessedDocument> ProcessedDocuments { get; }

    public CorpusFixture()
    {
        var duplicateTexts = TestCorpus.Documents.GroupBy(x => x.Text).Where(x => x.Count() > 1).Select(x => x.Key).ToList();

        if (duplicateTexts.Count > 0)
            throw new InvalidOperationException($"Corpus document texts must be unique (they identify test cases), but these repeat: {string.Join(" | ", duplicateTexts)}");

        ProcessedDocuments = Process(Grammar);
        _processedByText = ProcessedDocuments.ToDictionary(x => x.Document.Text);
    }

    /// <summary>The whole corpus, run through <paramref name="grammar"/> by the real pipeline.</summary>
    public static IReadOnlyList<ProcessedDocument> Process(GlyphGrammar grammar)
    {
        var analyzer = new CorpusAnalyzer(new Repository(), grammar);
        analyzer.EnsureInitializedAsync().GetAwaiter().GetResult();

        return analyzer.ProcessedDocuments;
    }

    /// <summary>What <paramref name="text"/>'s document actually tokenized into, one signature per line.</summary>
    public string[] ActualLines(string text) =>
        _processedByText[text].Lines.Select(GlyphSignature.Of).ToArray();

    sealed class Repository : IDocumentRepository
    {
        public Task<List<IDocument>> GetDocumentsAsync() =>
            Task.FromResult(TestCorpus.Documents.Cast<IDocument>().ToList());
    }
}

[CollectionDefinition(Name)]
public sealed class CorpusCollection : ICollectionFixture<CorpusFixture>
{
    public const string Name = "Corpus";
}
