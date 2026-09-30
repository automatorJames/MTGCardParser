using Glyphotype.Distiller.Scoring;

namespace Glyphotype.Distiller.Workbench;

/// <summary>Where a <see cref="GrammarWorkbench"/> keeps its working definition, and where and how it commits.</summary>
/// <param name="WorkingDefinitionPath">The JSON file the working definition is saved to after every edit, and restored from on startup.</param>
/// <param name="SourceDirectory">The directory holding the committed grammar's C# sources - declarations are found anywhere beneath it, and new ones written into it.</param>
/// <param name="SourceNamespace">The namespace new source files declare.</param>
/// <param name="AllowPartialSegmentMatches">The setting working grammars are tokenized under - the committed grammar's own.</param>
public sealed record WorkbenchOptions(string WorkingDefinitionPath, string SourceDirectory, string SourceNamespace, bool AllowPartialSegmentMatches);

/// <summary>The outcome of scoring one working definition: a score, or the reasons it couldn't be built.</summary>
public sealed record WorkingScore(GrammarDefinition Definition, MdlScore Score, IReadOnlyList<string> Errors, TimeSpan Elapsed)
{
    public bool Succeeded => Score is not null;
}

/// <summary>
/// A committed grammar, and a working definition to experiment on top of it: edits go to the working definition
/// only - saved as JSON, and re-scored against the same corpus in the background after each one - until
/// <see cref="Commit"/> writes them into the committed grammar's C# sources.
/// <para>
/// Thread-safe for one editor at a time: edits are serialized, and a re-score that an edit makes stale is
/// cancelled and its result discarded. <see cref="Changed"/> fires, from whatever thread, whenever the working
/// definition, its score or the scoring state changes.
/// </para>
/// </summary>
public sealed class GrammarWorkbench
{
    readonly IReadOnlyList<IDocument> _documents;
    readonly WorkbenchOptions _options;
    readonly object _gate = new();
    readonly GlyphGrammar _committedGrammar;
    readonly IReadOnlyList<ProcessedDocument> _committedDocuments;
    Task<MdlScore> _committedScore;

    CancellationTokenSource _scoring;
    int _version;

    /// <param name="committedGrammar">The grammar the sources under <see cref="WorkbenchOptions.SourceDirectory"/> compile to.</param>
    /// <param name="committedDocuments">The corpus as <paramref name="committedGrammar"/> already tokenized it.</param>
    public GrammarWorkbench(GlyphGrammar committedGrammar, IReadOnlyList<ProcessedDocument> committedDocuments, WorkbenchOptions options)
    {
        _documents = committedDocuments.Select(x => x.Document).ToList();
        _options = options;

        _committedGrammar = committedGrammar;
        _committedDocuments = committedDocuments;
        var committed = committedGrammar.ToDefinition();
        SetDefinitions(committed, LoadWorkingDefinition() ?? committed);

        if (HasChanges)
            _ = RescoreAsync();
    }

    public event Action Changed;

    /// <summary>The grammar as the C# sources declare it.</summary>
    public GrammarDefinition CommittedDefinition { get; private set; }

    /// <summary>The grammar being experimented on.</summary>
    public GrammarDefinition WorkingDefinition { get; private set; }

    /// <summary>The latest completed score of a working definition, or null if none has been scored (it matched the committed one throughout).</summary>
    public WorkingScore LatestWorkingScore { get; private set; }

    /// <summary>Whether a working definition is being scored right now.</summary>
    public bool IsScoring { get; private set; }

    public string WorkingDefinitionPath => _options.WorkingDefinitionPath;

    public string SourceDirectory => _options.SourceDirectory;

    /// <summary>What the working definition changes relative to the committed one.</summary>
    /// <remarks>Recomputed only when either definition changes (see <see cref="SetDefinitions"/>) - diffing compares every definition, and callers read this freely.</remarks>
    public IReadOnlyList<DefinitionChange> Changes { get; private set; } = [];

    public bool HasChanges => Changes.Count > 0;

    void SetDefinitions(GrammarDefinition committed, GrammarDefinition working)
    {
        CommittedDefinition = committed;
        WorkingDefinition = working;
        Changes = DefinitionDiff.Compare(committed, working);
    }

    /// <summary>The committed grammar's own score, computed on first request.</summary>
    public Task<MdlScore> GetCommittedScoreAsync()
    {
        lock (_gate)
            return _committedScore ??= Task.Run(() => MdlScorer.Score(_committedGrammar, _committedDocuments));
    }

    /// <summary>
    /// The score of the working definition as it stands: the committed score while they match, the latest working
    /// score when it's current, else null (a re-score is under way, or the last one failed).
    /// </summary>
    public async Task<MdlScore> GetCurrentWorkingScoreAsync()
    {
        if (!HasChanges)
            return await GetCommittedScoreAsync();

        var latest = LatestWorkingScore;
        return latest is not null && latest.Definition == WorkingDefinition ? latest.Score : null;
    }

    // ---- Edits ----

    /// <summary>Adds <paramref name="glyph"/>, or replaces the glyph named <paramref name="replacing"/> (by default, its own name) with it.</summary>
    public void SetGlyph(GlyphDefinition glyph, string replacing = null) =>
        Edit(x => x.WithGlyph(glyph, replacing));

    /// <summary>Adds <paramref name="vocabulary"/>, or replaces the vocabulary named <paramref name="replacing"/> (by default, its own name) with it.</summary>
    public void SetVocabulary(VocabularyDefinition vocabulary, string replacing = null) =>
        Edit(x => x.WithVocabulary(vocabulary, replacing));

    /// <summary>Removes a glyph - refused while another glyph refers to it.</summary>
    public void RemoveGlyph(string name) =>
        Edit(x => ThrowIfReferenced(x, name).WithoutGlyph(name));

    /// <summary>Removes a vocabulary - refused while a glyph refers to it.</summary>
    public void RemoveVocabulary(string name) =>
        Edit(x => ThrowIfReferenced(x, name).WithoutVocabulary(name));

    /// <summary>
    /// Returns the named definition to its committed state: restoring it if removed (along with any committed
    /// glyphs and vocabularies it refers to that are missing), reverting it if modified, removing it if added.
    /// </summary>
    public void Revert(DefinitionKind kind, string name) =>
        Edit(working => kind switch
        {
            DefinitionKind.Glyph => CommittedDefinition.Glyphs.FirstOrDefault(x => x.Name == name) is GlyphDefinition glyph
                ? RestoreReferences(working.WithGlyph(glyph), glyph)
                : ThrowIfReferenced(working, name).WithoutGlyph(name),
            DefinitionKind.Vocabulary => CommittedDefinition.Vocabularies.FirstOrDefault(x => x.Name == name) is VocabularyDefinition vocabulary
                ? working.WithVocabulary(vocabulary)
                : ThrowIfReferenced(working, name).WithoutVocabulary(name),
            _ => CommittedDefinition.Markers.Contains(name)
                ? working with { Markers = working.Markers.Union([name]).ToList() }
                : working with { Markers = working.Markers.Where(x => x != name).ToList() },
        });

    /// <summary>Discards every working change.</summary>
    public void RevertAll() => Edit(_ => CommittedDefinition);

    void Edit(Func<GrammarDefinition, GrammarDefinition> edit)
    {
        lock (_gate)
        {
            SetDefinitions(CommittedDefinition, edit(WorkingDefinition));
            SaveWorkingDefinition();
        }

        Changed?.Invoke();
        _ = RescoreAsync();
    }

    GrammarDefinition RestoreReferences(GrammarDefinition working, GlyphDefinition glyph)
    {
        foreach (var name in GrammarDefinitionEdits.GetReferencedNames(glyph))
        {
            if (working.Glyphs.Any(x => x.Name == name) || working.Vocabularies.Any(x => x.Name == name) || working.Markers.Contains(name))
                continue;

            if (CommittedDefinition.Glyphs.FirstOrDefault(x => x.Name == name) is GlyphDefinition referenced)
                working = RestoreReferences(working.WithGlyph(referenced), referenced);
            else if (CommittedDefinition.Vocabularies.FirstOrDefault(x => x.Name == name) is VocabularyDefinition vocabulary)
                working = working.WithVocabulary(vocabulary);
            else if (CommittedDefinition.Markers.Contains(name))
                working = working with { Markers = [.. working.Markers, name] };
        }

        return working;
    }

    static GrammarDefinition ThrowIfReferenced(GrammarDefinition grammar, string name)
    {
        var referrers = grammar.GetReferrers(name);

        if (referrers.Count > 0)
            throw new InvalidOperationException($"{name} can't be removed while {string.Join(", ", referrers)} refer{(referrers.Count == 1 ? "s" : "")} to it");

        return grammar;
    }

    // ---- Scoring ----

    /// <summary>
    /// Scores the working definition in the background: builds a grammar from it, tokenizes the corpus with it and
    /// scores the result. Cancels any re-score still running for an older definition.
    /// </summary>
    public async Task RescoreAsync()
    {
        CancellationTokenSource scoring;
        GrammarDefinition definition;
        int version;

        lock (_gate)
        {
            _scoring?.Cancel();
            _scoring = scoring = new CancellationTokenSource();
            definition = WorkingDefinition;
            version = ++_version;
            IsScoring = HasChanges;
        }

        Changed?.Invoke();

        if (!HasChanges)
            return;

        var result = await Task.Run(() => Score(definition, scoring.Token));

        lock (_gate)
        {
            // A newer edit's re-score owns the state now.
            if (version != _version || result is null)
                return;

            LatestWorkingScore = result;
            IsScoring = false;
        }

        Changed?.Invoke();
    }

    WorkingScore Score(GrammarDefinition definition, CancellationToken cancellation)
    {
        var started = DateTime.UtcNow;

        try
        {
            var grammar = GlyphGrammar.FromDefinition(definition.WithoutUnreferencedTerminals(), _options.AllowPartialSegmentMatches);

            var documents = _documents
                .AsParallel()
                .AsOrdered()
                .WithCancellation(cancellation)
                .Select(x => new ProcessedDocument(x, grammar))
                .ToList();

            return new(definition, MdlScorer.Score(grammar, documents), [], DateTime.UtcNow - started);
        }
        catch (OperationCanceledException)
        {
            return null;
        }
        catch (AggregateException exception) when (exception.InnerExceptions.All(x => x is not OperationCanceledException))
        {
            // Grammar validation reports every problem at once: a summary line, then one line per problem.
            var errors = exception.Message.Split('\n').Select(x => x.Trim()).Where(x => x.Length > 0).ToList();

            return new(definition, null, errors, DateTime.UtcNow - started);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            return new(definition, null, [exception.Message], DateTime.UtcNow - started);
        }
    }

    // ---- Committing ----

    /// <summary>The source edits committing the working definition would make, for review.</summary>
    public SourceCommitPlan PlanCommit() =>
        SourceCommitter.Plan(CommittedDefinition, WorkingDefinition, _options.SourceDirectory, _options.SourceNamespace);

    /// <summary>
    /// Writes <paramref name="plan"/> (from <see cref="PlanCommit"/>) into the sources, after which the working
    /// definition is the committed one. The running process's compiled grammar is unchanged until it's rebuilt;
    /// until then the committed score is the working one's.
    /// </summary>
    public void Commit(SourceCommitPlan plan)
    {
        SourceCommitter.Apply(plan);

        lock (_gate)
        {
            // The compiled grammar still predates the commit, so the committed score is now the working one's.
            var workingScore = LatestWorkingScore is { Succeeded: true } latest && latest.Definition == WorkingDefinition ? latest.Score : null;
            var definition = WorkingDefinition;
            _committedScore = workingScore is not null ? Task.FromResult(workingScore) : Task.Run(() => Score(definition, CancellationToken.None)?.Score);

            SetDefinitions(WorkingDefinition, WorkingDefinition);
            SaveWorkingDefinition();
        }

        Changed?.Invoke();
    }

    // ---- Persistence ----

    GrammarDefinition LoadWorkingDefinition()
    {
        try
        {
            return File.Exists(_options.WorkingDefinitionPath)
                ? GrammarDefinition.FromJson(File.ReadAllText(_options.WorkingDefinitionPath))
                : null;
        }
        catch (Exception)
        {
            // An unreadable working file is set aside rather than lost, and work starts over from the committed grammar.
            File.Move(_options.WorkingDefinitionPath, _options.WorkingDefinitionPath + $".unreadable-{DateTime.Now:yyyyMMddHHmmss}");
            return null;
        }
    }

    void SaveWorkingDefinition()
    {
        if (!HasChanges)
        {
            File.Delete(_options.WorkingDefinitionPath);
            return;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(_options.WorkingDefinitionPath));
        File.WriteAllText(_options.WorkingDefinitionPath, WorkingDefinition.ToJson());
    }
}
