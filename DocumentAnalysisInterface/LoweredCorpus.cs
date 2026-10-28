using Glyphotype.Lowering;

namespace DocumentAnalysisInterface;

/// <summary>
/// The corpus as the app's <see cref="ILoweringEngine"/> lowers it - when there is one: an app composed without an
/// engine has no lowered view. Summaries are made for every document, once per corpus (they're cheap, and the page's
/// headline figures and blockers come from them); full trees are made for the documents a page shows, as it shows them.
/// Follows <see cref="ActiveCorpus"/>: a new tokenization of the corpus brings new summaries.
/// </summary>
public sealed class LoweredCorpus
{
    readonly ILoweringEngine _engine;
    readonly ActiveCorpus _corpus;
    readonly object _gate = new();
    List<ProcessedDocument> _summarizedFrom;
    IReadOnlyList<LoweredSummary> _summaries;
    IReadOnlyList<LoweringBlocker> _blockers;

    public LoweredCorpus(ActiveCorpus corpus, ILoweringEngine engine = null)
    {
        _corpus = corpus;
        _engine = engine;
    }

    /// <summary>Whether the app has an engine to lower with.</summary>
    public bool IsAvailable => _engine is not null;

    public ILoweringEngine Engine => _engine;

    /// <summary>Raised when the summaries have been remade for a new tokenization of the corpus.</summary>
    public event Action Changed;

    /// <summary>Every document's summary, under the corpus's current tokenization - made on first use, and again after the corpus changes.</summary>
    public IReadOnlyList<LoweredSummary> Summaries
    {
        get
        {
            EnsureSummarized();
            return _summaries;
        }
    }

    /// <summary>The kinds that stayed opaque, most lines blocked first (see <see cref="LoweringBlocker"/>).</summary>
    public IReadOnlyList<LoweringBlocker> Blockers
    {
        get
        {
            EnsureSummarized();
            return _blockers;
        }
    }

    public int DocumentCount => Summaries.Count;
    public int FullyLoweredDocumentCount => Summaries.Count(x => x.IsFullyLowered);
    public int LineCount => Summaries.Sum(x => x.Lines.Count);
    public int FullyLoweredLineCount => Summaries.Sum(x => x.Lines.Count(y => y.IsFullyLowered));

    /// <summary>The summary of <paramref name="document"/>, by its place in the corpus.</summary>
    public LoweredSummary SummaryOf(ProcessedDocument document)
    {
        EnsureSummarized();
        var index = _summarizedFrom.IndexOf(document);
        return index >= 0 ? _summaries[index] : _engine.Summarize(document);
    }

    /// <summary>The full lowered tree of <paramref name="document"/>, made now.</summary>
    public LoweredDocument Lower(ProcessedDocument document) => _engine.Lower(document);

    /// <summary>Waits for the corpus, then makes the summaries off the UI thread.</summary>
    public async Task EnsureInitializedAsync()
    {
        if (!IsAvailable)
            return;

        await _corpus.EnsureInitializedAsync();
        await Task.Run(EnsureSummarized);
    }

    void EnsureSummarized()
    {
        if (!IsAvailable)
            throw new InvalidOperationException("The app has no lowering engine");

        var documents = _corpus.ProcessedDocuments;

        lock (_gate)
        {
            if (ReferenceEquals(documents, _summarizedFrom))
                return;

            var summaries = documents.AsParallel().AsOrdered().Select(_engine.Summarize).ToList();
            _summaries = summaries;
            _blockers = LoweringBlocker.Rank(summaries);
            _summarizedFrom = documents;
        }

        Changed?.Invoke();
    }
}

/// <summary>One kind that stayed opaque somewhere in the corpus: how many lines it blocks, how often it occurs, and a few of its texts.</summary>
public sealed record LoweringBlocker(string Kind, int Lines, int Occurrences, IReadOnlyList<string> Examples)
{
    public static IReadOnlyList<LoweringBlocker> Rank(IEnumerable<LoweredSummary> summaries)
    {
        var lines = new Dictionary<string, int>();
        var occurrences = new Dictionary<string, int>();
        var examples = new Dictionary<string, List<string>>();

        foreach (var summary in summaries)
            foreach (var line in summary.Lines)
            {
                foreach (var kind in line.OpaqueKinds.Distinct())
                    lines[kind] = lines.GetValueOrDefault(kind) + 1;

                for (int i = 0; i < line.OpaqueKinds.Count; i++)
                {
                    var kind = line.OpaqueKinds[i];
                    occurrences[kind] = occurrences.GetValueOrDefault(kind) + 1;

                    if (!examples.TryGetValue(kind, out var texts))
                        examples[kind] = texts = [];

                    if (texts.Count < 3 && i < line.OpaqueTexts.Count && !texts.Contains(line.OpaqueTexts[i]))
                        texts.Add(line.OpaqueTexts[i]);
                }
            }

        return lines
            .Select(x => new LoweringBlocker(x.Key, x.Value, occurrences[x.Key], examples[x.Key]))
            .OrderByDescending(x => x.Lines)
            .ThenByDescending(x => x.Occurrences)
            .ThenBy(x => x.Kind, StringComparer.Ordinal)
            .ToList();
    }
}
