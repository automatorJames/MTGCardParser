using System.IO.Compression;
using Glyphotype.Distiller.Workbench;

namespace Glyphotype.Distiller.Workspaces;

/// <summary>Where a workspace's committed grammar lives.</summary>
public enum WorkspaceKind
{
    /// <summary>C# sources, compiled into the running app: committing writes them.</summary>
    Source,

    /// <summary>A JSON baseline in the workspace's own folder: checkpointing writes it, and exporting turns it into C#.</summary>
    Scratch,
}

/// <summary>What a new workspace starts from (see <see cref="WorkspaceManager.Create"/>).</summary>
public enum WorkspaceSeed
{
    /// <summary>No glyphs, vocabularies or markers.</summary>
    Empty,

    /// <summary>Another workspace's vocabularies, and nothing else: a clean slate that keeps its word sets.</summary>
    Vocabularies,

    /// <summary>Everything another workspace has, as it stands.</summary>
    Copy,
}

/// <summary>One workspace: its name, its kind, and the folder (under the manager's root) it keeps its files in.</summary>
/// <param name="Parent">The folder of the workspace it started from, if any: where it's merged into by default.</param>
public sealed record WorkspaceInfo(string Name, WorkspaceKind Kind, string Folder, DateTimeOffset Created, string Parent = null);

/// <summary>The grammar compiled from C# sources that a <see cref="WorkspaceManager"/> always offers as its first workspace.</summary>
/// <param name="Documents">The corpus as <paramref name="Grammar"/> already tokenized it - every workspace is scored against these documents.</param>
public sealed record SourceWorkspace(string Name, GlyphGrammar Grammar, IReadOnlyList<ProcessedDocument> Documents, string SourceDirectory, string SourceNamespace);

/// <summary>
/// Named grammars to work on against one corpus, one of them active at a time: the grammar compiled from C# sources,
/// plus any number of scratch grammars kept as JSON - so an experiment never has to be a mass edit of the real grammar.
/// Each workspace has its own <see cref="GrammarWorkbench"/> (a baseline, a working definition and a step history),
/// all kept under one root folder and restored on restart, along with which one was active.
/// <para>
/// The active workspace is shared by everything using the manager: a person in a UI and an agent through tools look
/// at the same grammar, and <see cref="ActiveChanged"/> tells each when the other switches.
/// </para>
/// </summary>
public sealed class WorkspaceManager
{
    const string _indexFileName = "workspaces.json";
    const string _workingFileName = "working.json";
    const string _baselineFileName = "baseline.json";
    const string _historyFileName = "history.json";
    const string _guidanceFileName = "guidance.md";
    const string _mergeBaseFileName = "merge-base.json";

    readonly SourceWorkspace _source;
    readonly string _root;
    readonly bool _allowPartialClauseMatches;
    readonly IReadOnlyList<IDocument> _documents;
    readonly object _gate = new();
    readonly List<WorkspaceInfo> _workspaces = [];
    readonly Dictionary<string, string> _configuredGuidance;

    /// <summary>Each workspace's journal, by folder, once asked for.</summary>
    readonly Dictionary<string, WorkspaceJournal> _journals = [];

    /// <summary>The source workspace's workbench, kept for the whole session: after a commit it knows more than the compiled grammar does until the app restarts.</summary>
    GrammarWorkbench _sourceWorkbench;

    /// <param name="root">The folder every workspace's files are kept under.</param>
    /// <param name="legacyWorkingDefinitionPath">Where the source grammar's working definition was kept before workspaces, if anywhere: moved into its workspace on first run.</param>
    /// <param name="configuredGuidance">
    /// Guidance by workspace name, from configuration that travels with the code (unlike the root folder): where a workspace has some,
    /// it overrides any kept in the workspace's folder, and can't be set here.
    /// </param>
    public WorkspaceManager(SourceWorkspace source, string root, bool allowPartialClauseMatches, string legacyWorkingDefinitionPath = null,
        IReadOnlyDictionary<string, string> configuredGuidance = null)
    {
        _source = source;
        _root = root;
        _allowPartialClauseMatches = allowPartialClauseMatches;
        _configuredGuidance = (configuredGuidance ?? new Dictionary<string, string>())
            .Where(x => !string.IsNullOrWhiteSpace(x.Value))
            .ToDictionary(x => x.Key, x => x.Value.Trim(), StringComparer.OrdinalIgnoreCase);
        _documents = source.Documents.Select(x => x.Document).ToList();

        var active = LoadIndex();

        if (!_workspaces.Any(x => x.Kind == WorkspaceKind.Source))
            _workspaces.Insert(0, new(source.Name, WorkspaceKind.Source, "source", DateTimeOffset.Now));

        var sourceWorking = Path.Combine(FolderOf(_workspaces.First(x => x.Kind == WorkspaceKind.Source)), _workingFileName);

        if (legacyWorkingDefinitionPath is not null && File.Exists(legacyWorkingDefinitionPath) && !File.Exists(sourceWorking))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(sourceWorking));
            File.Move(legacyWorkingDefinitionPath, sourceWorking);
        }

        ActiveWorkspace = _workspaces.FirstOrDefault(x => x.Name == active) ?? _workspaces.First(x => x.Kind == WorkspaceKind.Source);
        Active = Open(ActiveWorkspace);
        SaveIndex();
    }

    /// <summary>Fires, from whatever thread, when a different workspace becomes active - or the list of workspaces changes.</summary>
    public event Action ActiveChanged;

    public IReadOnlyList<WorkspaceInfo> Workspaces
    {
        get
        {
            lock (_gate)
                return _workspaces.ToList();
        }
    }

    public WorkspaceInfo ActiveWorkspace { get; private set; }

    /// <summary>The active workspace's workbench.</summary>
    public GrammarWorkbench Active { get; private set; }

    /// <summary>
    /// Notes for an AI on what the named workspace's grammar is and how to approach iterating it (the active workspace by default) - empty
    /// where there are none. Kept apart from the grammar itself, so no step, revert or checkpoint touches it.
    /// </summary>
    public string GetGuidance(string name = null)
    {
        lock (_gate)
            return ReadGuidance(name is null ? ActiveWorkspace : Find(name));
    }

    /// <summary>Whether the named workspace's guidance (the active workspace's by default) comes from configuration, and so can't be set here.</summary>
    public bool IsGuidanceConfigured(string name = null)
    {
        lock (_gate)
            return _configuredGuidance.ContainsKey((name is null ? ActiveWorkspace : Find(name)).Name);
    }

    /// <summary>Replaces the named workspace's guidance (the active workspace's by default); blank removes it.</summary>
    /// <exception cref="InvalidOperationException">The workspace's guidance comes from configuration.</exception>
    public void SetGuidance(string guidance, string name = null)
    {
        lock (_gate)
        {
            var workspace = name is null ? ActiveWorkspace : Find(name);

            if (_configuredGuidance.ContainsKey(workspace.Name))
                throw new InvalidOperationException($"{workspace.Name}'s guidance is set in the app's settings - change it there");

            var folder = FolderOf(workspace);
            var path = Path.Combine(folder, _guidanceFileName);

            if (string.IsNullOrWhiteSpace(guidance))
                File.Delete(path);
            else
            {
                Directory.CreateDirectory(folder);
                File.WriteAllText(path, guidance.Trim());
            }
        }

        ActiveChanged?.Invoke();
    }

    string ReadGuidance(WorkspaceInfo workspace)
    {
        if (_configuredGuidance.TryGetValue(workspace.Name, out var configured))
            return configured;

        var path = Path.Combine(FolderOf(workspace), _guidanceFileName);
        return File.Exists(path) ? File.ReadAllText(path).Trim() : "";
    }

    /// <summary>
    /// The named workspace's journal (the active workspace's by default): what agents have learned working on its grammar.
    /// Kept apart from the grammar, as guidance is, and copied with each commit or checkpoint (see <see cref="WorkspaceJournal.Snapshot"/>).
    /// </summary>
    public WorkspaceJournal GetJournal(string name = null)
    {
        lock (_gate)
            return JournalOf(name is null ? ActiveWorkspace : Find(name));
    }

    /// <summary>Fires, from whatever thread, when any workspace's journal changes.</summary>
    public event Action JournalChanged;

    WorkspaceJournal JournalOf(WorkspaceInfo workspace)
    {
        var folder = FolderOf(workspace);

        if (!_journals.TryGetValue(folder, out var journal))
        {
            _journals[folder] = journal = new WorkspaceJournal(folder);
            journal.Changed += () => JournalChanged?.Invoke();
        }

        return journal;
    }

    /// <summary>Makes the named workspace the active one.</summary>
    public void Switch(string name)
    {
        lock (_gate)
        {
            var workspace = Find(name);

            if (workspace == ActiveWorkspace)
                return;

            Activate(workspace);
        }

        ActiveChanged?.Invoke();
    }

    /// <summary>
    /// Creates a scratch workspace named <paramref name="name"/>, starting from <paramref name="seed"/> of the workspace named <paramref name="from"/>
    /// (the active one by default), and makes it active. With <paramref name="copyGuidance"/>, it starts with that workspace's guidance too,
    /// and with <paramref name="copyJournal"/>, its journal.
    /// </summary>
    /// <exception cref="InvalidOperationException">The name is taken or unusable, or <paramref name="from"/> names no workspace.</exception>
    public WorkspaceInfo Create(string name, WorkspaceSeed seed = WorkspaceSeed.Empty, string from = null, bool copyGuidance = false, bool copyJournal = false)
    {
        WorkspaceInfo workspace;

        lock (_gate)
        {
            name = ValidateNewName(name);

            var originWorkspace = seed == WorkspaceSeed.Empty && !copyGuidance && !copyJournal ? null : from is null ? ActiveWorkspace : Find(from);
            var origin = seed == WorkspaceSeed.Empty ? null : GetWorkingDefinition(originWorkspace);
            var baseline = seed switch
            {
                WorkspaceSeed.Vocabularies => new GrammarDefinition { Vocabularies = origin.Vocabularies },
                WorkspaceSeed.Copy => origin,
                _ => new GrammarDefinition(),
            };

            workspace = new(name, WorkspaceKind.Scratch, NewFolder(name), DateTimeOffset.Now, seed == WorkspaceSeed.Empty ? null : originWorkspace.Folder);
            var folder = FolderOf(workspace);
            Directory.CreateDirectory(folder);
            File.WriteAllText(Path.Combine(folder, _baselineFileName), baseline.ToJson());
            File.WriteAllText(Path.Combine(folder, _mergeBaseFileName), baseline.ToJson());

            if (copyGuidance && ReadGuidance(originWorkspace) is { Length: > 0 } guidance)
                File.WriteAllText(Path.Combine(folder, _guidanceFileName), guidance);

            if (copyJournal)
                JournalOf(originWorkspace).CopyTo(folder);

            _workspaces.Add(workspace);
            Activate(workspace);
        }

        ActiveChanged?.Invoke();
        return workspace;
    }

    public void Rename(string name, string newName)
    {
        lock (_gate)
        {
            var workspace = Find(name);

            if (workspace.Kind == WorkspaceKind.Source)
                throw new InvalidOperationException($"{name} is named for its C# sources, and can't be renamed");

            newName = ValidateNewName(newName, except: workspace);
            var renamed = workspace with { Name = newName };
            _workspaces[_workspaces.IndexOf(workspace)] = renamed;

            if (ActiveWorkspace == workspace)
                ActiveWorkspace = renamed;

            SaveIndex();
        }

        ActiveChanged?.Invoke();
    }

    /// <summary>Deletes a scratch workspace and its files. If it's the active one, the source workspace becomes active.</summary>
    public void Delete(string name)
    {
        lock (_gate)
        {
            var workspace = Find(name);

            if (workspace.Kind == WorkspaceKind.Source)
                throw new InvalidOperationException($"{name} is the grammar compiled from C# sources, and can't be deleted");

            if (ActiveWorkspace == workspace)
                Activate(_workspaces.First(x => x.Kind == WorkspaceKind.Source));

            _workspaces.Remove(workspace);
            _journals.Remove(FolderOf(workspace));
            SaveIndex();

            if (Directory.Exists(FolderOf(workspace)))
                Directory.Delete(FolderOf(workspace), recursive: true);
        }

        ActiveChanged?.Invoke();
    }

    /// <summary>The workspace <paramref name="name"/> is merged into by default: the one it started from, while there is one, else the source workspace.</summary>
    public string DefaultMergeTarget(string name)
    {
        lock (_gate)
        {
            var workspace = Find(name);
            return (_workspaces.FirstOrDefault(x => x.Folder == workspace.Parent && x != workspace) ?? _workspaces.First(x => x.Kind == WorkspaceKind.Source)).Name;
        }
    }

    /// <summary>
    /// What merging the scratch workspace <paramref name="from"/> into <paramref name="into"/> would do, for review: what
    /// <paramref name="from"/> added, changed or removed since it started or last merged - working changes included - with
    /// where <paramref name="into"/> changed the same definition its own way marked as a conflict.
    /// </summary>
    /// <exception cref="InvalidOperationException">Either names no workspace, they're the same one, or <paramref name="from"/> is the source workspace.</exception>
    public WorkspaceMerge PlanMerge(string from, string into)
    {
        lock (_gate)
        {
            var source = Find(from);
            var target = Find(into);

            if (source == target)
                throw new InvalidOperationException($"{source.Name} can't be merged into itself");

            if (source.Kind == WorkspaceKind.Source)
                throw new InvalidOperationException($"{source.Name} is the grammar compiled from C# sources - commit it rather than merging it");

            var recordedBase = ReadMergeBase(source);
            var targetCommitted = GetCommittedDefinition(target);
            var targetWorking = GetWorkingDefinition(target);

            return WorkspaceMerge.Plan(source.Name, target.Name, recordedBase ?? targetCommitted, targetWorking, GetWorkingDefinition(source), recordedBase is not null)
                with { TargetWorkingChanges = DefinitionDiff.Compare(targetCommitted, targetWorking).Count };
        }
    }

    /// <summary>
    /// Merges <paramref name="taken"/> (items of <paramref name="merge"/>, from <see cref="PlanMerge"/>) into its target, as one
    /// step on the target's working definition, and makes the target active - so the merge is there to score, review, undo
    /// and commit (or checkpoint) as any working change is. The merged workspace is left with no working changes: what it
    /// has is its baseline now, and its merge base too, but for what wasn't taken - which the next merge offers again.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Either workspace changed since the merge was planned, the merge would remove something that's still referred to, or
    /// what it would leave doesn't build.
    /// </exception>
    public void Merge(WorkspaceMerge merge, IEnumerable<MergeItem> taken)
    {
        var takenItems = taken.ToList();

        lock (_gate)
        {
            var source = Find(merge.From);
            var target = Find(merge.Into);

            if (GetWorkingDefinition(source).ToJson() != merge.Incoming.ToJson() || GetWorkingDefinition(target).ToJson() != merge.Target.ToJson())
                throw new InvalidOperationException($"{source.Name} or {target.Name} changed since this merge was planned - look it over again");

            var merged = merge.Apply(takenItems);

            try
            {
                GlyphGrammar.FromDefinition(merged.WithoutUnreferencedTerminals(), _allowPartialClauseMatches);
            }
            catch (AggregateException exception)
            {
                throw new InvalidOperationException($"What the merge would leave in {target.Name} doesn't build: {exception.Message}");
            }

            if (ActiveWorkspace != target)
                Activate(target);

            Active.Replace(merged, $"merge {source.Name}");

            // The merged workspace's working definition becomes its baseline, as a checkpoint makes it.
            var folder = FolderOf(source);
            File.WriteAllText(Path.Combine(folder, _baselineFileName), merge.Incoming.ToJson());
            File.WriteAllText(Path.Combine(folder, _mergeBaseFileName), merge.NextBase(takenItems).ToJson());
            File.Delete(Path.Combine(folder, _workingFileName));
            File.Delete(Path.Combine(folder, _historyFileName));
            JournalOf(source).Snapshot($"Merged into {target.Name}");
        }

        ActiveChanged?.Invoke();
    }

    /// <summary>
    /// What the workspace had when it started or last merged - or, for one started before that was kept, its baseline if
    /// it was never checkpointed (it's still what the workspace started from). Null where neither is known.
    /// </summary>
    GrammarDefinition ReadMergeBase(WorkspaceInfo workspace)
    {
        var folder = FolderOf(workspace);
        var path = Path.Combine(folder, _mergeBaseFileName);

        if (File.Exists(path))
            return GrammarDefinition.FromJson(File.ReadAllText(path));

        var baseline = Path.Combine(folder, _baselineFileName);
        var neverCheckpointed = File.Exists(baseline) && (File.GetLastWriteTimeUtc(baseline) - workspace.Created.UtcDateTime).Duration() < TimeSpan.FromMinutes(1);

        return neverCheckpointed ? ReadBaseline(workspace) : null;
    }

    /// <summary>
    /// The named workspace's grammar as it stands (its working definition) as a zip: one C# file per glyph, vocabulary
    /// and marker, declared in <paramref name="namespace"/>, plus the whole definition as <c>grammar.json</c>.
    /// </summary>
    public byte[] Export(string name, string @namespace)
    {
        GrammarDefinition definition;

        lock (_gate)
            definition = GetWorkingDefinition(Find(name));

        if (string.IsNullOrWhiteSpace(@namespace) || !@namespace.Split('.').All(IsIdentifier))
            throw new InvalidOperationException($"'{@namespace}' isn't a C# namespace");

        using var stream = new MemoryStream();

        using (var zip = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
        {
            void Add(string fileName, string text)
            {
                using var writer = new StreamWriter(zip.CreateEntry(fileName).Open(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
                writer.Write(text);
            }

            foreach (var marker in definition.Markers)
                Add($"{marker}.cs", GlyphSourceWriter.Write(new GrammarDefinition { Markers = [marker] }, @namespace));

            foreach (var vocabulary in definition.Vocabularies)
                Add($"{vocabulary.Name}.cs", GlyphSourceWriter.Write(new GrammarDefinition { Vocabularies = [vocabulary] }, @namespace));

            foreach (var glyph in definition.Glyphs)
                Add($"{glyph.Name}.cs", GlyphSourceWriter.Write(new GrammarDefinition { Glyphs = [glyph] }, @namespace));

            Add("grammar.json", definition.ToJson());
        }

        return stream.ToArray();
    }

    /// <summary>A namespace for exporting <paramref name="name"/>: its letters and digits, as one identifier.</summary>
    public static string SuggestNamespace(string name)
    {
        var identifier = string.Concat(name.Split([' ', '-', '_', '.'], StringSplitOptions.RemoveEmptyEntries).Select(x => char.ToUpperInvariant(x[0]) + x[1..]))
            .Where(char.IsLetterOrDigit)
            .Aggregate("", (text, c) => text + c);

        return identifier.Length == 0 ? "Grammar" : char.IsDigit(identifier[0]) ? "Grammar" + identifier : identifier;
    }

    // ---- Workbenches ----

    void Activate(WorkspaceInfo workspace)
    {
        var previous = Active;

        ActiveWorkspace = workspace;
        Active = Open(workspace);
        SaveIndex();

        // Only scratch workbenches are let go of; the source one lives for the session.
        if (previous is not null && previous != _sourceWorkbench && previous != Active)
            previous.Dispose();
    }

    GrammarWorkbench Open(WorkspaceInfo workspace)
    {
        var folder = FolderOf(workspace);
        Directory.CreateDirectory(folder);

        if (workspace.Kind == WorkspaceKind.Source)
        {
            if (_sourceWorkbench is null)
            {
                _sourceWorkbench = new GrammarWorkbench(_source.Grammar, _source.Documents, new(
                    Path.Combine(folder, _workingFileName), _source.SourceDirectory, _source.SourceNamespace, _allowPartialClauseMatches,
                    HistoryPath: Path.Combine(folder, _historyFileName)));

                KeepJournalVersions(_sourceWorkbench, workspace);
            }

            return _sourceWorkbench;
        }

        var workbench = new GrammarWorkbench(ReadBaseline(workspace), _documents, new(
            Path.Combine(folder, _workingFileName), SourceDirectory: null, SourceNamespace: SuggestNamespace(workspace.Name), _allowPartialClauseMatches,
            BaselinePath: Path.Combine(folder, _baselineFileName),
            HistoryPath: Path.Combine(folder, _historyFileName)));

        KeepJournalVersions(workbench, workspace);
        return workbench;
    }

    /// <summary>Has the journal keep a version of itself each time <paramref name="workbench"/> is committed or checkpointed.</summary>
    void KeepJournalVersions(GrammarWorkbench workbench, WorkspaceInfo workspace)
    {
        var journal = JournalOf(workspace);
        workbench.Committed += journal.Snapshot;
    }

    /// <summary>The workspace's grammar as it stands: the active workbench's working definition, or what's saved for a dormant one.</summary>
    GrammarDefinition GetWorkingDefinition(WorkspaceInfo workspace)
    {
        if (workspace == ActiveWorkspace)
            return Active.WorkingDefinition;

        if (workspace.Kind == WorkspaceKind.Source && _sourceWorkbench is not null)
            return _sourceWorkbench.WorkingDefinition;

        var working = Path.Combine(FolderOf(workspace), _workingFileName);

        if (File.Exists(working))
            return GrammarDefinition.FromJson(File.ReadAllText(working));

        return workspace.Kind == WorkspaceKind.Source ? _source.Grammar.ToDefinition() : ReadBaseline(workspace);
    }

    /// <summary>The workspace's committed grammar: its C# sources' or its baseline.</summary>
    GrammarDefinition GetCommittedDefinition(WorkspaceInfo workspace) =>
        workspace == ActiveWorkspace ? Active.CommittedDefinition
        : workspace.Kind == WorkspaceKind.Source ? Open(workspace).CommittedDefinition
        : ReadBaseline(workspace);

    GrammarDefinition ReadBaseline(WorkspaceInfo workspace)
    {
        var path = Path.Combine(FolderOf(workspace), _baselineFileName);
        return File.Exists(path) ? GrammarDefinition.FromJson(File.ReadAllText(path)) : new GrammarDefinition();
    }

    // ---- Index ----

    sealed record Index(string Active, List<WorkspaceInfo> Workspaces);

    string IndexPath => Path.Combine(_root, _indexFileName);

    /// <summary>Reads the saved workspaces into <see cref="_workspaces"/>, returning the name of the one that was active.</summary>
    string LoadIndex()
    {
        if (!File.Exists(IndexPath))
            return null;

        var index = DefinitionJson.Deserialize<Index>(File.ReadAllText(IndexPath));

        // A source workspace saved under another name (the sources moved) is this one now.
        foreach (var workspace in index.Workspaces)
            _workspaces.Add(workspace.Kind == WorkspaceKind.Source ? workspace with { Name = _source.Name } : workspace);

        return index.Workspaces.FirstOrDefault(x => x.Name == index.Active)?.Kind == WorkspaceKind.Source ? _source.Name : index.Active;
    }

    void SaveIndex()
    {
        Directory.CreateDirectory(_root);
        File.WriteAllText(IndexPath, DefinitionJson.Serialize(new Index(ActiveWorkspace?.Name, _workspaces)));
    }

    WorkspaceInfo Find(string name) =>
        _workspaces.FirstOrDefault(x => string.Equals(x.Name, name?.Trim(), StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"There's no workspace named '{name}' - there are {string.Join(", ", _workspaces.Select(x => x.Name))}");

    string FolderOf(WorkspaceInfo workspace) => Path.Combine(_root, workspace.Folder);

    string ValidateNewName(string name, WorkspaceInfo except = null)
    {
        name = name?.Trim();

        if (string.IsNullOrEmpty(name) || name.Length > 60)
            throw new InvalidOperationException("A workspace needs a name of 1 to 60 characters");

        if (_workspaces.Any(x => x != except && string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"There's already a workspace named '{name}'");

        return name;
    }

    /// <summary>A folder name for a new workspace: its name, reduced to what any file system takes, made unique.</summary>
    string NewFolder(string name)
    {
        var slug = new string(name.ToLowerInvariant().Select(x => char.IsLetterOrDigit(x) ? x : '-').ToArray()).Trim('-');
        slug = slug.Length == 0 ? "workspace" : slug[..Math.Min(slug.Length, 40)];

        var folder = slug;

        for (int i = 2; _workspaces.Any(x => x.Folder == folder) || Directory.Exists(Path.Combine(_root, folder)); i++)
            folder = $"{slug}-{i}";

        return folder;
    }

    static bool IsIdentifier(string text) =>
        text.Length > 0 && (char.IsLetter(text[0]) || text[0] == '_') && text.All(x => char.IsLetterOrDigit(x) || x == '_');
}
