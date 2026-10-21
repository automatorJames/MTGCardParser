using Glyphotype.Distiller.Workbench;
using Glyphotype.Distiller.Workspaces;
using Glyphotype.GlyphAnalysisDTOs;
using Glyphotype.GlyphAnalysisDTOs.TypeExpressions;
using Glyphotype.GlyphAnalysisDTOs.WordTrees;
using Glyphotype.Interfaces;
using System.Text.Json;

namespace DocumentAnalysisInterface;

/// <summary>
/// The corpus analysis every tab shows, for the active workspace's grammar as it stands - so switching workspaces (or
/// editing the active one) changes what every tab inspects, without touching any source files. Answers with the same
/// members as the <see cref="CorpusAnalyzer"/> it stands in for.
/// <para>
/// Built lazily from the corpus the workspace's workbench already tokenized: nothing is tokenized again. While the
/// source workspace is unchanged, that's the analysis the app started with. <see cref="Changed"/> fires when a new
/// grammar is ready to show - a switch, or a re-score after an edit - and pages refresh by awaiting
/// <see cref="EnsureInitializedAsync"/> again. While the active grammar doesn't build, the last one that did stays shown.
/// </para>
/// <para>
/// Which of the corpus's volumes are read is shared too, by every page and the agent: <see cref="LoadVolumesAsync"/> reads a
/// different selection, has every workspace score against it, and keeps it for the next time the app starts.
/// </para>
/// </summary>
public sealed class ActiveCorpus
{
    readonly WorkspaceManager _workspaces;
    readonly IDocumentRepository _repository;
    readonly string _volumesPath;
    readonly object _gate = new();
    readonly SemaphoreSlim _loading = new(1, 1);

    CorpusAnalyzer _compiled;

    /// <summary>
    /// The analysis shown, built on first use (see <see cref="EnsureInitializedAsync"/>) - and replaced, unbuilt, the moment
    /// a newer grammar is scored, so the corpus an earlier grammar tokenized (and with it every type that grammar
    /// rebuilt, with their compiled regexes) isn't held here for as long as no page asks. Lazy, so nothing is built for
    /// a step no page looks at.
    /// </summary>
    Lazy<CorpusAnalyzer> _current;
    IReadOnlyList<ProcessedDocument> _currentDocuments;
    GrammarWorkbench _listeningTo;

    /// <param name="compiled">The analysis of the corpus under the compiled grammar, already initialized.</param>
    /// <param name="repository">Where the corpus is read from, when a different selection of its volumes is loaded.</param>
    /// <param name="volumesPath">Where the selection of volumes is kept between runs (see <see cref="ReadVolumes"/>).</param>
    public ActiveCorpus(WorkspaceManager workspaces, CorpusAnalyzer compiled, IDocumentRepository repository, string volumesPath)
    {
        _workspaces = workspaces;
        _repository = repository;
        _volumesPath = volumesPath;
        _compiled = compiled;
        _current = new(() => compiled);
        _currentDocuments = compiled.ProcessedDocuments;

        _workspaces.ActiveChanged += OnActiveChanged;
        Listen();
    }

    /// <summary>Fires, from whatever thread, when the active grammar has changed and a new analysis is due: await <see cref="EnsureInitializedAsync"/> to get it.</summary>
    public event Action Changed;

    public GlyphGrammar Grammar => _current.Value.Grammar;
    public List<ProcessedDocument> ProcessedDocuments => _current.Value.ProcessedDocuments;
    public int WordCount => _current.Value.WordCount;
    public int CapturedWordCount => _current.Value.CapturedWordCount;
    public DigestedText DigestedTextWithCaptureGlyphs => _current.Value.DigestedTextWithCaptureGlyphs;
    public Dictionary<Type, GlyphOccurrenceSummary> GlyphOccurrenceSummaries => _current.Value.GlyphOccurrenceSummaries;

    /// <summary>Whether the analysis of the grammar as it stands has been built - reading any other member builds it, on the calling thread, if not.</summary>
    public bool IsReady => _current.IsValueCreated;

    /// <summary>The volumes of the corpus read.</summary>
    public VolumeSelection Volumes => _compiled.Volumes;

    /// <summary>The volumes being read in place of <see cref="Volumes"/>, while <see cref="LoadVolumesAsync"/> is at it - else null.</summary>
    public VolumeSelection LoadingVolumes { get; private set; }

    /// <summary>Fires, from whatever thread, when <see cref="LoadingVolumes"/> or <see cref="Volumes"/> changes.</summary>
    public event Action VolumesChanged;

    /// <summary>The name of the volume <paramref name="document"/> is in - null where the corpus doesn't name its volumes.</summary>
    public string GetVolumeName(IDocument document) =>
        document.Volume is int volume && _repository.GetVolumeNames().TryGetValue(volume, out var name) ? name : null;

    /// <summary>The numbers of the corpus's volumes - empty if it isn't divided into volumes.</summary>
    public int[] GetAvailableVolumes() => _repository.GetVolumes();

    /// <summary>
    /// Reads <paramref name="volumes"/> of the corpus in place of those read now, and has every workspace score against them -
    /// after which <see cref="Changed"/> fires. One load at a time: a second waits for the first.
    /// </summary>
    public async Task LoadVolumesAsync(VolumeSelection volumes)
    {
        await _loading.WaitAsync();

        try
        {
            if (volumes == Volumes)
                return;

            LoadingVolumes = volumes;
            VolumesChanged?.Invoke();

            var compiled = new CorpusAnalyzer(_repository, _compiled.Grammar, volumes);
            await Task.Run(compiled.EnsureInitializedAsync);

            _workspaces.SetCorpus(compiled.ProcessedDocuments);

            lock (_gate)
                _compiled = compiled;

            WriteVolumes(_volumesPath, volumes);
        }
        finally
        {
            LoadingVolumes = null;
            _loading.Release();
            VolumesChanged?.Invoke();
        }

        Changed?.Invoke();
    }

    /// <summary>The selection of volumes kept at <paramref name="path"/> - every volume, where none is kept or it can't be read.</summary>
    public static VolumeSelection ReadVolumes(string path)
    {
        try
        {
            return File.Exists(path) ? JsonSerializer.Deserialize<VolumeSelection>(File.ReadAllText(path)) ?? VolumeSelection.All : VolumeSelection.All;
        }
        catch (JsonException)
        {
            return VolumeSelection.All;
        }
    }

    static void WriteVolumes(string path, VolumeSelection volumes)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllText(path, JsonSerializer.Serialize(volumes));
    }

    /// <summary>Brings the analysis up to date with the active grammar - waiting for it to be scored, if it's being scored - and builds it, off the calling thread, if it isn't built.</summary>
    public async Task EnsureInitializedAsync()
    {
        var corpus = await _workspaces.Active.GetCurrentCorpusAsync();

        // The active grammar doesn't build: keep showing the last one that did.
        if (corpus is not var (grammar, documents))
            return;

        Lazy<CorpusAnalyzer> current;

        lock (_gate)
        {
            if (documents != _currentDocuments)
                Replace(grammar, documents);

            current = _current;
        }

        if (!current.IsValueCreated)
            await Task.Run(() => current.Value);
    }

    /// <summary>Makes the analysis of <paramref name="documents"/> (as <paramref name="grammar"/> tokenized them) the one shown, to be built when first read. Call under <see cref="_gate"/>.</summary>
    void Replace(GlyphGrammar grammar, IReadOnlyList<ProcessedDocument> documents)
    {
        // Its digested text is built only where the analysis before it was asked for its own: the echo views ask, and
        // nothing else does, so an agent's run of steps with no echo view open doesn't build a digest of the corpus
        // after every one.
        var digest = _current is { IsValueCreated: true } previous && previous.Value.WasDigestRequested;

        _current = documents == _compiled.ProcessedDocuments
            ? new(() => _compiled)
            : new(() =>
            {
                var analyzer = CorpusAnalyzer.FromProcessedDocuments(documents, grammar);

                if (digest)
                    analyzer.StartDigestingText();

                return analyzer;
            });

        _currentDocuments = documents;
    }

    void OnActiveChanged()
    {
        Listen();
        Changed?.Invoke();
    }

    void Listen()
    {
        lock (_gate)
        {
            if (_listeningTo == _workspaces.Active)
                return;

            if (_listeningTo is not null)
                _listeningTo.Changed -= OnWorkbenchChanged;

            _listeningTo = _workspaces.Active;
            _listeningTo.Changed += OnWorkbenchChanged;
        }
    }

    /// <summary>A workbench fires for every change of state; only a newly scored grammar means something new to show.</summary>
    void OnWorkbenchChanged()
    {
        var workbench = _workspaces.Active;

        if (workbench.IsScoring)
            return;

        var latest = workbench.LatestWorkingScore;
        var documents = !workbench.HasChanges ? null : latest?.Definition == workbench.WorkingDefinition ? latest.Documents : null;

        // A newly scored working grammar that builds: it's what's shown from now on, so the one before it goes now
        // rather than when a page next asks (see _current).
        if (documents is not null)
            lock (_gate)
            {
                if (documents == _currentDocuments)
                    return;

                Replace(latest.Grammar, documents);
            }

        // Back to the committed grammar, or that newly scored working one.
        if (!workbench.HasChanges || documents is not null)
            Changed?.Invoke();
    }
}
