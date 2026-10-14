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

    CorpusAnalyzer _current;
    IReadOnlyList<ProcessedDocument> _currentDocuments;
    Task<CorpusAnalyzer> _building;
    IReadOnlyList<ProcessedDocument> _buildingDocuments;
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
        _current = compiled;
        _currentDocuments = compiled.ProcessedDocuments;

        _workspaces.ActiveChanged += OnActiveChanged;
        Listen();
    }

    /// <summary>Fires, from whatever thread, when the active grammar has changed and a new analysis is due: await <see cref="EnsureInitializedAsync"/> to get it.</summary>
    public event Action Changed;

    public GlyphGrammar Grammar => _current.Grammar;
    public List<ProcessedDocument> ProcessedDocuments => _current.ProcessedDocuments;
    public int WordCount => _current.WordCount;
    public int CapturedWordCount => _current.CapturedWordCount;
    public DigestedText DigestedTextWithCaptureGlyphs => _current.DigestedTextWithCaptureGlyphs;
    public Dictionary<Type, GlyphOccurrenceSummary> GlyphOccurrenceSummaries => _current.GlyphOccurrenceSummaries;

    /// <summary>The volumes of the corpus read.</summary>
    public VolumeSelection Volumes => _compiled.Volumes;

    /// <summary>The volumes being read in place of <see cref="Volumes"/>, while <see cref="LoadVolumesAsync"/> is at it - else null.</summary>
    public VolumeSelection LoadingVolumes { get; private set; }

    /// <summary>Fires, from whatever thread, when <see cref="LoadingVolumes"/> or <see cref="Volumes"/> changes.</summary>
    public event Action VolumesChanged;

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

    /// <summary>Brings the analysis up to date with the active grammar - waiting for it to be scored, if it's being scored.</summary>
    public async Task EnsureInitializedAsync()
    {
        var corpus = await _workspaces.Active.GetCurrentCorpusAsync();

        // The active grammar doesn't build: keep showing the last one that did.
        if (corpus is not var (grammar, documents))
            return;

        Task<CorpusAnalyzer> building;

        lock (_gate)
        {
            if (documents == _currentDocuments)
                return;

            if (documents != _buildingDocuments)
            {
                _buildingDocuments = documents;
                _building = documents == _compiled.ProcessedDocuments
                    ? Task.FromResult(_compiled)
                    : Task.Run(() => CorpusAnalyzer.FromProcessedDocuments(documents, grammar));
            }

            building = _building;
        }

        var analyzer = await building;

        lock (_gate)
        {
            // Only the analysis of the latest grammar requested takes over.
            if (documents == _buildingDocuments)
            {
                _current = analyzer;
                _currentDocuments = documents;
            }
        }
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

        // Back to the committed grammar, or a newly scored working one that builds.
        if (!workbench.HasChanges || documents is not null && documents != _currentDocuments)
            Changed?.Invoke();
    }
}
