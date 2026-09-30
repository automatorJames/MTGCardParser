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

    /// <summary>The grammar built from <see cref="Definition"/> - null when it couldn't be built.</summary>
    public GlyphGrammar Grammar { get; init; }

    /// <summary>The corpus as <see cref="Grammar"/> tokenized it - null when it couldn't be built.</summary>
    public IReadOnlyList<ProcessedDocument> Documents { get; init; }
}

/// <summary>One edit of the working definition, as <see cref="GrammarWorkbench.Undo"/> takes it back.</summary>
public sealed record WorkbenchStep(int Number, string Description, GrammarDefinition Before, GrammarDefinition After, DateTimeOffset At);

/// <summary>A change set scored against the working definition without being made (see <see cref="GrammarWorkbench.EvaluateAsync"/>).</summary>
/// <param name="Before">The working definition's own score, which the change set was applied on top of.</param>
/// <param name="After">The score with the change set applied.</param>
public sealed record Evaluation(ChangeSet Changes, WorkingScore Before, WorkingScore After);

/// <summary>
/// A committed grammar, and a working definition to experiment on top of it: edits go to the working definition
/// only - saved as JSON, and re-scored against the same corpus in the background after each one - until
/// <see cref="Commit"/> writes them into the committed grammar's C# sources.
/// <para>
/// Thread-safe: edits are serialized, and a re-score that an edit makes stale is cancelled and its result
/// discarded - so several editors (a person in a UI, an agent through tools) can work on one workbench, each
/// seeing the other's edits. <see cref="Changed"/> fires, from whatever thread, whenever the working definition,
/// its score or the scoring state changes. Every edit is a <see cref="WorkbenchStep"/> in <see cref="History"/>,
/// which <see cref="Undo"/> walks back.
/// </para>
/// </summary>
public sealed class GrammarWorkbench
{
    readonly IReadOnlyList<IDocument> _documents;
    readonly WorkbenchOptions _options;
    readonly object _gate = new();
    readonly GlyphGrammar _committedGrammar;
    readonly IReadOnlyList<ProcessedDocument> _committedDocuments;
    Task<WorkingScore> _committedTrial;

    /// <summary>The latest scores of definitions, newest last, keyed by their JSON: so applying a change set that was just evaluated doesn't score it again.</summary>
    readonly List<(string Json, WorkingScore Score)> _recentScores = [];
    const int _recentScoreCapacity = 2;

    readonly List<WorkbenchStep> _history = [];
    const int _historyCapacity = 500;
    int _stepNumber;

    CancellationTokenSource _scoring;
    Task _rescore = Task.CompletedTask;
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

    /// <summary>The documents every definition is scored against.</summary>
    public IReadOnlyList<IDocument> Documents => _documents;

    /// <summary>What the working definition changes relative to the committed one.</summary>
    /// <remarks>Recomputed only when either definition changes (see <see cref="SetDefinitions"/>) - diffing compares every definition, and callers read this freely.</remarks>
    public IReadOnlyList<DefinitionChange> Changes { get; private set; } = [];

    public bool HasChanges => Changes.Count > 0;

    /// <summary>The edits made since the workbench started or last committed, oldest first.</summary>
    public IReadOnlyList<WorkbenchStep> History
    {
        get
        {
            lock (_gate)
                return _history.ToList();
        }
    }

    void SetDefinitions(GrammarDefinition committed, GrammarDefinition working)
    {
        CommittedDefinition = committed;
        WorkingDefinition = working;
        Changes = DefinitionDiff.Compare(committed, working);
    }

    /// <summary>The committed grammar's own score, computed on first request.</summary>
    public async Task<MdlScore> GetCommittedScoreAsync() => (await GetCommittedTrialAsync()).Score;

    /// <summary>The committed grammar, the corpus as it tokenized it, and its score, computed on first request.</summary>
    public Task<WorkingScore> GetCommittedTrialAsync()
    {
        lock (_gate)
            return _committedTrial ??= Task.Run(() =>
            {
                var started = DateTime.UtcNow;
                var score = MdlScorer.Score(_committedGrammar, _committedDocuments);

                return new WorkingScore(CommittedDefinition, score, [], DateTime.UtcNow - started) { Grammar = _committedGrammar, Documents = _committedDocuments };
            });
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

    /// <summary>
    /// The working definition as it stands, scored: the committed trial while they match, else the working
    /// definition's own - waiting for it if a re-score is under way. Unlike <see cref="GetCurrentWorkingScoreAsync"/>,
    /// never null: a definition that couldn't be built comes back with its <see cref="WorkingScore.Errors"/>.
    /// </summary>
    public async Task<WorkingScore> GetCurrentTrialAsync(CancellationToken cancellation = default)
    {
        while (true)
        {
            GrammarDefinition definition = null;
            Task pending = null;

            lock (_gate)
            {
                if (HasChanges)
                {
                    definition = WorkingDefinition;
                    pending = _rescore;
                }
            }

            if (definition is null)
                return await GetCommittedTrialAsync().WaitAsync(cancellation);

            if (LatestWorkingScore is WorkingScore latest && latest.Definition == definition)
                return latest;

            // Either this definition's re-score is running, or the edit that made it hasn't started one yet.
            if (pending.IsCompleted)
                await Task.Delay(10, cancellation);
            else
                await pending.WaitAsync(cancellation);
        }
    }

    // ---- Edits ----

    /// <summary>Adds <paramref name="glyph"/>, or replaces the glyph named <paramref name="replacing"/> (by default, its own name) with it.</summary>
    public void SetGlyph(GlyphDefinition glyph, string replacing = null) =>
        Edit(x => x.WithGlyph(glyph, replacing), replacing is null || replacing == glyph.Name ? $"set glyph {glyph.Name}" : $"replace glyph {replacing} with {glyph.Name}");

    /// <summary>Adds <paramref name="vocabulary"/>, or replaces the vocabulary named <paramref name="replacing"/> (by default, its own name) with it.</summary>
    public void SetVocabulary(VocabularyDefinition vocabulary, string replacing = null) =>
        Edit(x => x.WithVocabulary(vocabulary, replacing), replacing is null || replacing == vocabulary.Name ? $"set vocabulary {vocabulary.Name}" : $"replace vocabulary {replacing} with {vocabulary.Name}");

    /// <summary>Removes a glyph - refused while another glyph refers to it.</summary>
    public void RemoveGlyph(string name) =>
        Edit(x => ThrowIfReferenced(x, name).WithoutGlyph(name), $"remove glyph {name}");

    /// <summary>Removes a vocabulary - refused while a glyph refers to it.</summary>
    public void RemoveVocabulary(string name) =>
        Edit(x => ThrowIfReferenced(x, name).WithoutVocabulary(name), $"remove vocabulary {name}");

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
        }, $"revert {kind.ToString().ToLowerInvariant()} {name}");

    /// <summary>Discards every working change.</summary>
    public void RevertAll() => Edit(_ => CommittedDefinition, "revert all");

    /// <summary>Makes <paramref name="changes"/> as one step, described as <paramref name="description"/> (by default, the change set's own summary).</summary>
    /// <returns>The step, or null if the change set changed nothing.</returns>
    /// <exception cref="InvalidOperationException">The change set can't be applied (see <see cref="ChangeSet.ApplyTo"/>).</exception>
    public WorkbenchStep Apply(ChangeSet changes, string description = null) =>
        Edit(changes.ApplyTo, string.IsNullOrWhiteSpace(description) ? changes.Describe() : description);

    /// <summary>Takes back the latest step in <see cref="History"/>, returning the working definition to what it was before it.</summary>
    /// <returns>The step taken back.</returns>
    public WorkbenchStep Undo()
    {
        WorkbenchStep step;

        lock (_gate)
        {
            if (_history.Count == 0)
                throw new InvalidOperationException("There's nothing to undo");

            step = _history[^1];
            _history.RemoveAt(_history.Count - 1);

            SetDefinitions(CommittedDefinition, step.Before);
            SaveWorkingDefinition();
        }

        Changed?.Invoke();
        _ = RescoreAsync();
        return step;
    }

    WorkbenchStep Edit(Func<GrammarDefinition, GrammarDefinition> edit, string description)
    {
        WorkbenchStep step;

        lock (_gate)
        {
            var before = WorkingDefinition;
            var after = edit(before);

            if (DefinitionDiff.Compare(before, after).Count == 0)
                return null;

            step = new WorkbenchStep(++_stepNumber, description, before, after, DateTimeOffset.Now);
            _history.Add(step);

            if (_history.Count > _historyCapacity)
                _history.RemoveAt(0);

            SetDefinitions(CommittedDefinition, after);
            SaveWorkingDefinition();
        }

        Changed?.Invoke();
        _ = RescoreAsync();
        return step;
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
    public Task RescoreAsync()
    {
        Task rescore;

        lock (_gate)
        {
            _scoring?.Cancel();
            _scoring = new CancellationTokenSource();
            IsScoring = HasChanges;
            var version = ++_version;
            _rescore = rescore = HasChanges ? RescoreInBackgroundAsync(WorkingDefinition, version, _scoring.Token) : Task.CompletedTask;
        }

        Changed?.Invoke();
        return rescore;
    }

    async Task RescoreInBackgroundAsync(GrammarDefinition definition, int version, CancellationToken cancellation)
    {
        var result = await Task.Run(() => Score(definition, cancellation));

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

    /// <summary>
    /// Scores the working definition with <paramref name="changes"/> applied, without making them: what
    /// <see cref="Apply"/> would do to the score. Runs alongside any re-score, and doesn't touch the workbench's state.
    /// </summary>
    /// <exception cref="InvalidOperationException">The change set can't be applied (see <see cref="ChangeSet.ApplyTo"/>).</exception>
    public async Task<Evaluation> EvaluateAsync(ChangeSet changes, CancellationToken cancellation = default)
    {
        var before = await GetCurrentTrialAsync(cancellation);
        var candidate = changes.ApplyTo(before.Definition);
        var after = await Task.Run(() => Score(candidate, cancellation), cancellation)
            ?? throw new OperationCanceledException(cancellation);

        return new(changes, before, after);
    }

    /// <summary>Builds the grammar <paramref name="definition"/> describes, as the workbench scores it - without tokenizing anything.</summary>
    /// <exception cref="AggregateException">The grammar's validation failed: one inner exception per problem.</exception>
    public GlyphGrammar BuildGrammar(GrammarDefinition definition) =>
        GlyphGrammar.FromDefinition(definition.WithoutUnreferencedTerminals(), _options.AllowPartialSegmentMatches);

    /// <summary>Scores <paramref name="definition"/> - or returns its recent score, if it was scored lately. Null if cancelled.</summary>
    WorkingScore Score(GrammarDefinition definition, CancellationToken cancellation)
    {
        var json = definition.ToJson();

        lock (_gate)
            if (_recentScores.FirstOrDefault(x => x.Json == json).Score is WorkingScore recent)
                return recent with { Definition = definition };

        var score = ScoreUncached(definition, cancellation);

        if (score is not null)
            lock (_gate)
            {
                _recentScores.Add((json, score));

                if (_recentScores.Count > _recentScoreCapacity)
                    _recentScores.RemoveAt(0);
            }

        return score;
    }

    WorkingScore ScoreUncached(GrammarDefinition definition, CancellationToken cancellation)
    {
        var started = DateTime.UtcNow;

        try
        {
            var grammar = BuildGrammar(definition);

            var documents = _documents
                .AsParallel()
                .AsOrdered()
                .WithCancellation(cancellation)
                .Select(x => new ProcessedDocument(x, grammar))
                .ToList();

            return new(definition, MdlScorer.Score(grammar, documents), [], DateTime.UtcNow - started) { Grammar = grammar, Documents = documents };
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
    /// definition is the committed one and <see cref="History"/> starts over. The running process's compiled grammar
    /// is unchanged until it's rebuilt; until then the committed trial is the working one's.
    /// </summary>
    public void Commit(SourceCommitPlan plan)
    {
        SourceCommitter.Apply(plan);

        lock (_gate)
        {
            // The compiled grammar still predates the commit, so the committed trial is now the working one's.
            var workingTrial = LatestWorkingScore is { Succeeded: true } latest && latest.Definition == WorkingDefinition ? latest : null;
            var definition = WorkingDefinition;
            _committedTrial = workingTrial is not null ? Task.FromResult(workingTrial) : Task.Run(() => Score(definition, CancellationToken.None));

            // Steps from before the commit would take back committed work.
            _history.Clear();

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
