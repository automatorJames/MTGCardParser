using Glyphotype.Distiller.Workbench;
using Glyphotype.Distiller.Workspaces;
using Glyphotype.GlyphAnalysisDTOs;
using Glyphotype.GlyphAnalysisDTOs.TypeExpressions;
using Glyphotype.GlyphAnalysisDTOs.WordTrees;

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
/// </summary>
public sealed class ActiveCorpus
{
    readonly WorkspaceManager _workspaces;
    readonly CorpusAnalyzer _compiled;
    readonly object _gate = new();

    CorpusAnalyzer _current;
    IReadOnlyList<ProcessedDocument> _currentDocuments;
    Task<CorpusAnalyzer> _building;
    IReadOnlyList<ProcessedDocument> _buildingDocuments;
    GrammarWorkbench _listeningTo;

    /// <param name="compiled">The analysis of the corpus under the compiled grammar, already initialized.</param>
    public ActiveCorpus(WorkspaceManager workspaces, CorpusAnalyzer compiled)
    {
        _workspaces = workspaces;
        _compiled = compiled;
        _current = compiled;
        _currentDocuments = compiled.ProcessedDocuments;

        _workspaces.ActiveChanged += OnActiveChanged;
        Listen();
    }

    /// <summary>Fires, from whatever thread, when the active grammar has changed and a new analysis is due: await <see cref="EnsureInitializedAsync"/> to get it.</summary>
    public event Action Changed;

    /// <summary>The workspace whose grammar this analyzes.</summary>
    public string WorkspaceName => _workspaces.ActiveWorkspace.Name;

    public GlyphGrammar Grammar => _current.Grammar;
    public List<ProcessedDocument> ProcessedDocuments => _current.ProcessedDocuments;
    public int WordCount => _current.WordCount;
    public int CapturedWordCount => _current.CapturedWordCount;
    public DigestedText DigestedTextWithCaptureGlyphs => _current.DigestedTextWithCaptureGlyphs;
    public Dictionary<Type, GlyphOccurrenceSummary> GlyphOccurrenceSummaries => _current.GlyphOccurrenceSummaries;

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
