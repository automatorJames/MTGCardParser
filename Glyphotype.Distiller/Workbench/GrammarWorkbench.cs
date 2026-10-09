using Glyphotype.Distiller.Scoring;

namespace Glyphotype.Distiller.Workbench;

/// <summary>Where a <see cref="GrammarWorkbench"/> keeps its working definition, and where and how it commits.</summary>
/// <param name="WorkingDefinitionPath">The JSON file the working definition is saved to after every edit, and restored from on startup.</param>
/// <param name="SourceDirectory">
/// The directory holding the committed grammar's C# sources - declarations are found anywhere beneath it, and new ones
/// written into it. Null for a workbench whose baseline is kept as JSON instead (see <paramref name="BaselinePath"/>).
/// </param>
/// <param name="SourceNamespace">The namespace new source files declare.</param>
/// <param name="AllowPartialClauseMatches">The setting working grammars are tokenized under - the committed grammar's own.</param>
/// <param name="BaselinePath">For a workbench with no C# sources: the JSON file its baseline (see <see cref="GrammarWorkbench.Checkpoint"/>) is kept in.</param>
/// <param name="HistoryPath">The JSON file <see cref="GrammarWorkbench.History"/> is saved to after every step, so undo survives a restart - or null to keep it in memory only.</param>
public sealed record WorkbenchOptions(
    string WorkingDefinitionPath,
    string SourceDirectory,
    string SourceNamespace,
    bool AllowPartialClauseMatches,
    string BaselinePath = null,
    string HistoryPath = null);

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
/// <param name="Round">For a step an agent applied: which of its rounds of steps - one working session, from start to check-in - it was applied in, counted from 1. Null for a person's edit.</param>
/// <param name="Restores">Whether the step returned the working definition to an earlier state (see <see cref="GrammarWorkbench.Restore"/>), so begins nothing of its own.</param>
public sealed record WorkbenchStep(int Number, string Description, GrammarDefinition Before, GrammarDefinition After, DateTimeOffset At, int? Round = null, bool Restores = false);

/// <summary>A change set scored against the working definition without being made (see <see cref="GrammarWorkbench.EvaluateAsync"/>).</summary>
/// <param name="Before">The working definition's own score, which the change set was applied on top of.</param>
/// <param name="After">The score with the change set applied.</param>
public sealed record Evaluation(ChangeSet Changes, WorkingScore Before, WorkingScore After);

/// <summary>
/// A committed grammar, and a working definition to experiment on top of it: edits go to the working definition
/// only - saved as JSON, and re-scored against the same corpus in the background after each one - until
/// <see cref="Commit"/> writes them into the committed grammar's C# sources. A workbench with no sources keeps its
/// committed grammar (its baseline) as JSON, and <see cref="Checkpoint"/>s into that instead.
/// <para>
/// Thread-safe: edits are serialized, and a re-score that an edit makes stale is cancelled and its result
/// discarded - so several editors (a person in a UI, an agent through tools) can work on one workbench, each
/// seeing the other's edits. <see cref="Changed"/> fires, from whatever thread, whenever the working definition,
/// its score or the scoring state changes. Every edit is a <see cref="WorkbenchStep"/> in <see cref="History"/>,
/// which <see cref="Undo"/> walks back.
/// </para>
/// </summary>
public sealed class GrammarWorkbench : IDisposable
{
    readonly IReadOnlyList<IDocument> _documents;
    readonly WorkbenchOptions _options;
    readonly object _gate = new();
    readonly GlyphGrammar _committedGrammar;
    readonly IReadOnlyList<ProcessedDocument> _committedDocuments;
    Task<WorkingScore> _committedTrial;

    /// <summary>Whether a commit has made the compiled grammar out of date (it stays so until the app restarts).</summary>
    bool _compiledGrammarSuperseded;

    /// <summary>The latest scores of definitions, newest last, keyed by their JSON: so applying a change set that was just evaluated doesn't score it again.</summary>
    readonly List<(string Json, WorkingScore Score)> _recentScores = [];
    const int _recentScoreCapacity = 2;

    readonly List<WorkbenchStep> _history = [];
    const int _historyCapacity = 100;
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
        Initialize(SourceCommitter.WithDocumentation(committedGrammar.ToDefinition(), options.SourceDirectory));
    }

    /// <summary>
    /// A workbench with no C# sources behind it: its committed grammar is <paramref name="baseline"/> (kept at
    /// <see cref="WorkbenchOptions.BaselinePath"/>), built and scored against <paramref name="documents"/> on first request.
    /// </summary>
    public GrammarWorkbench(GrammarDefinition baseline, IReadOnlyList<IDocument> documents, WorkbenchOptions options)
    {
        if (options.SourceDirectory is not null || options.BaselinePath is null)
            throw new ArgumentException("A workbench without a compiled grammar keeps its baseline as JSON: give it a BaselinePath, and no SourceDirectory");

        _documents = documents;
        _options = options;
        Initialize(baseline);
    }

    void Initialize(GrammarDefinition committed)
    {
        SetDefinitions(committed, LoadWorkingDefinition() ?? committed);
        LoadHistory();
        (ChangeRounds, ChangeSteps) = FindChangeOrigins();

        if (HasChanges)
            _ = RescoreAsync();
    }

    /// <summary>Whether the committed grammar is C# sources, which <see cref="Commit"/> writes to - else a JSON baseline, which <see cref="Checkpoint"/> writes to.</summary>
    public bool IsSourceBacked => _options.SourceDirectory is not null;

    /// <summary>Stops any re-score under way.</summary>
    public void Dispose()
    {
        lock (_gate)
            _scoring?.Cancel();
    }

    public event Action Changed;

    /// <summary>Fires once the working definition has been committed to C# or checkpointed, with a few words saying which.</summary>
    public event Action<string> Committed;

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

    /// <summary>The agent round (see <see cref="WorkbenchStep.Round"/>) each of <see cref="Changes"/> began in. A change a person began isn't here, nor one whose first step <see cref="History"/> no longer holds.</summary>
    public IReadOnlyDictionary<(DefinitionKind Kind, string Name), int> ChangeRounds { get; private set; } = new Dictionary<(DefinitionKind, string), int>();

    /// <summary>The number of the agent step each of <see cref="ChangeRounds"/>' changes began in.</summary>
    public IReadOnlyDictionary<(DefinitionKind Kind, string Name), int> ChangeSteps { get; private set; } = new Dictionary<(DefinitionKind, string), int>();

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
        (ChangeRounds, ChangeSteps) = FindChangeOrigins();
    }

    /// <summary>
    /// The round and number of the step each change began in, where an agent's round did: the latest step to find the
    /// definition as committed, and leave it otherwise - other than a restoring step, which brings back a change an
    /// earlier step began.
    /// </summary>
    (Dictionary<(DefinitionKind, string), int> Rounds, Dictionary<(DefinitionKind, string), int> Steps) FindChangeOrigins()
    {
        var rounds = new Dictionary<(DefinitionKind, string), int>();
        var steps = new Dictionary<(DefinitionKind, string), int>();

        if (!_history.Any(x => x.Round is not null))
            return (rounds, steps);

        foreach (var change in Changes)
        {
            var committed = change.Before is null ? null : DefinitionJson.Serialize(change.Before);

            bool IsCommitted(GrammarDefinition grammar)
            {
                object definition = change.Kind switch
                {
                    DefinitionKind.Glyph => grammar.Glyphs.FirstOrDefault(x => x.Name == change.Name),
                    DefinitionKind.Vocabulary => grammar.Vocabularies.FirstOrDefault(x => x.Name == change.Name),
                    _ => grammar.Markers.Contains(change.Name) ? change.Name : null,
                };

                return ReferenceEquals(definition, change.Before)
                    || (definition is not null && committed is not null && DefinitionJson.Serialize(definition) == committed);
            }

            // The latest step to begin the change: to find the definition as committed, and leave it otherwise.
            for (int i = _history.Count - 1; i >= 0; i--)
            {
                var step = _history[i];

                if (step.Restores || !IsCommitted(step.Before) || IsCommitted(step.After))
                    continue;

                if (step.Round is int round)
                {
                    rounds[(change.Kind, change.Name)] = round;
                    steps[(change.Kind, change.Name)] = step.Number;
                }

                break;
            }
        }

        return (rounds, steps);
    }

    /// <summary>The committed grammar's own score, computed on first request.</summary>
    public async Task<MdlScore> GetCommittedScoreAsync() => (await GetCommittedTrialAsync()).Score;

    /// <summary>The committed grammar, the corpus as it tokenized it, and its score, computed on first request.</summary>
    public Task<WorkingScore> GetCommittedTrialAsync()
    {
        lock (_gate)
        {
            if (_committedGrammar is null)
            {
                var baseline = CommittedDefinition;
                return _committedTrial ??= Task.Run(() => Score(baseline, CancellationToken.None));
            }

            return _committedTrial ??= Task.Run(() =>
            {
                var started = DateTime.UtcNow;
                var score = MdlScorer.Score(_committedGrammar, _committedDocuments);

                return new WorkingScore(CommittedDefinition, score, [], DateTime.UtcNow - started) { Grammar = _committedGrammar, Documents = _committedDocuments };
            });
        }
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

    /// <summary>
    /// The grammar as it stands and the corpus as it tokenized it - straight away while that's still the compiled grammar
    /// (nothing needs scoring), else once the working definition is scored. Null if the working definition doesn't build.
    /// </summary>
    public async Task<(GlyphGrammar Grammar, IReadOnlyList<ProcessedDocument> Documents)?> GetCurrentCorpusAsync(CancellationToken cancellation = default)
    {
        if (!HasChanges && _committedGrammar is not null && !_compiledGrammarSuperseded)
            return (_committedGrammar, _committedDocuments);

        var trial = await GetCurrentTrialAsync(cancellation);
        return trial.Succeeded ? (trial.Grammar, trial.Documents) : null;
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
        Edit(working => RevertIn(working, kind, name), $"revert {kind.ToString().ToLowerInvariant()} {name}");

    /// <summary>Reverts (as <see cref="Revert"/> does) every change an agent began in <paramref name="round"/> (see <see cref="ChangeRounds"/>), as one step.</summary>
    public void RevertRound(int round) =>
        RevertEach(ChangeRounds.Where(x => x.Value == round).Select(x => x.Key).ToList(), $"No working change began in AI round {round}", $"revert AI round {round}");

    /// <summary>Reverts (as <see cref="Revert"/> does) every change no agent round began - those not in <see cref="ChangeRounds"/> - as one step.</summary>
    public void RevertOtherChanges() =>
        RevertEach(Changes.Select(x => (x.Kind, x.Name)).Where(x => !ChangeRounds.ContainsKey(x)).ToList(), "Every working change began in an AI round", "revert other changes");

    void RevertEach(List<(DefinitionKind Kind, string Name)> pending, string noneMessage, string description) =>
        Edit(working =>
        {
            if (pending.Count == 0)
                throw new InvalidOperationException(noneMessage);

            // A change the round added may be referred to by another it added: keep going round until each
            // revert goes through, or none of those left can.
            while (pending.Count > 0)
            {
                var left = pending.Count;
                InvalidOperationException refusal = null;

                foreach (var (kind, name) in pending.ToList())
                {
                    try
                    {
                        working = RevertIn(working, kind, name);
                        pending.Remove((kind, name));
                    }
                    catch (InvalidOperationException exception)
                    {
                        refusal = exception;
                    }
                }

                // Nothing went through this time round, so nothing would next time either.
                if (pending.Count == left)
                    throw refusal;
            }

            return working;
        }, description);

    GrammarDefinition RevertIn(GrammarDefinition working, DefinitionKind kind, string name) =>
        kind switch
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
        };

    /// <summary>Makes <paramref name="definition"/> the working definition, as one step - how a caller takes back, or redoes, steps of its own.</summary>
    public void Restore(GrammarDefinition definition, string description) =>
        Edit(_ => definition, description, restores: true);

    /// <summary>Discards every working change.</summary>
    public void RevertAll() => Edit(_ => CommittedDefinition, "revert all");

    /// <summary>Makes <paramref name="changes"/> as one step, described as <paramref name="description"/> (by default, the change set's own summary).</summary>
    /// <param name="round">For an agent's step: the round it's applied in (see <see cref="WorkbenchStep.Round"/>).</param>
    /// <returns>The step, or null if the change set changed nothing.</returns>
    /// <exception cref="InvalidOperationException">The change set can't be applied (see <see cref="ChangeSet.ApplyTo"/>).</exception>
    public WorkbenchStep Apply(ChangeSet changes, string description = null, int? round = null) =>
        Edit(changes.ApplyTo, string.IsNullOrWhiteSpace(description) ? changes.Describe() : description, round);

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
            SaveHistory();
        }

        Changed?.Invoke();
        _ = RescoreAsync();
        return step;
    }

    WorkbenchStep Edit(Func<GrammarDefinition, GrammarDefinition> edit, string description, int? round = null, bool restores = false)
    {
        WorkbenchStep step;

        lock (_gate)
        {
            var before = WorkingDefinition;
            var after = edit(before);

            if (DefinitionDiff.Compare(before, after).Count == 0)
                return null;

            step = new WorkbenchStep(++_stepNumber, description, before, after, DateTimeOffset.Now, round, restores);
            _history.Add(step);

            if (_history.Count > _historyCapacity)
                _history.RemoveAt(0);

            SetDefinitions(CommittedDefinition, after);
            SaveWorkingDefinition();
            SaveHistory();
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
        GlyphGrammar.FromDefinition(definition.WithoutUnreferencedTerminals(), _options.AllowPartialClauseMatches);

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
    /// <param name="resolution">What to do about names the commit adds that types in the project already have - null to leave it to the person (see <see cref="SourceCommitPlan.Conflicts"/>).</param>
    public SourceCommitPlan PlanCommit(NameConflictResolution? resolution = null) =>
        IsSourceBacked
            ? SourceCommitter.Plan(CommittedDefinition, WorkingDefinition, _options.SourceDirectory, _options.SourceNamespace, resolution)
            : throw new InvalidOperationException("This grammar has no C# sources to commit to - checkpoint it instead, or export it");

    /// <summary>
    /// Writes <paramref name="plan"/> (from <see cref="PlanCommit"/>) into the sources, after which the definition they
    /// declare - the working one, unless the plan renamed some of it - is both the committed and the working one, and
    /// <see cref="History"/> starts over. The running process's compiled grammar is unchanged until it's rebuilt;
    /// until then the committed trial is the working one's.
    /// </summary>
    public void Commit(SourceCommitPlan plan)
    {
        SourceCommitter.Apply(plan);

        lock (_gate)
        {
            // The compiled grammar still predates the commit, so the committed trial is now the working one's.
            _compiledGrammarSuperseded = true;
            var definition = plan.Definition;
            var workingTrial = LatestWorkingScore is { Succeeded: true } latest && latest.Definition == definition ? latest : null;
            _committedTrial = workingTrial is not null ? Task.FromResult(workingTrial) : Task.Run(() => Score(definition, CancellationToken.None));

            // Steps from before the commit would take back committed work.
            _history.Clear();

            SetDefinitions(definition, definition);
            SaveWorkingDefinition();
            SaveHistory();
        }

        Changed?.Invoke();
        Committed?.Invoke("Committed to C#");
    }

    /// <summary>
    /// For a workbench without C# sources: makes the working definition the committed one, saved as its baseline -
    /// what <see cref="Commit"/> does for sources. Refused until the working definition has been scored and builds,
    /// so a baseline always does. <see cref="History"/> starts over.
    /// </summary>
    public void Checkpoint()
    {
        if (IsSourceBacked)
            throw new InvalidOperationException("This grammar's committed form is its C# sources - commit to them instead");

        lock (_gate)
        {
            if (HasChanges && (LatestWorkingScore is not { Succeeded: true } latest || latest.Definition != WorkingDefinition))
                throw new InvalidOperationException("Only a working grammar that's been scored and builds can be checkpointed - wait for scoring to finish, or fix what doesn't build");

            if (HasChanges)
                _committedTrial = Task.FromResult(LatestWorkingScore);

            _history.Clear();

            SetDefinitions(WorkingDefinition, WorkingDefinition);
            WriteFile(_options.BaselinePath, WorkingDefinition.ToJson());
            SaveWorkingDefinition();
            SaveHistory();
        }

        Changed?.Invoke();
        Committed?.Invoke("Checkpoint");
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

        WriteFile(_options.WorkingDefinitionPath, WorkingDefinition.ToJson());
    }

    /// <summary>One step as saved: what came after it is the next step's <see cref="Before"/>, or the working definition.</summary>
    sealed record SavedStep(int Number, string Description, DateTimeOffset At, GrammarDefinition Before, int? Round = null, bool Restores = false);

    void SaveHistory()
    {
        if (_options.HistoryPath is null)
            return;

        if (_history.Count == 0)
        {
            File.Delete(_options.HistoryPath);
            return;
        }

        WriteFile(_options.HistoryPath, DefinitionJson.Serialize(_history.Select(x => new SavedStep(x.Number, x.Description, x.At, x.Before, x.Round, x.Restores)).ToList()));
    }

    void LoadHistory()
    {
        if (_options.HistoryPath is null || !File.Exists(_options.HistoryPath))
            return;

        try
        {
            var saved = DefinitionJson.Deserialize<List<SavedStep>>(File.ReadAllText(_options.HistoryPath));

            for (int i = 0; i < saved.Count; i++)
                _history.Add(new(saved[i].Number, saved[i].Description, saved[i].Before, i + 1 < saved.Count ? saved[i + 1].Before : WorkingDefinition, saved[i].At, saved[i].Round, saved[i].Restores));

            _stepNumber = _history.Count > 0 ? _history.Max(x => x.Number) : 0;
        }
        catch (Exception)
        {
            // History is a convenience: an unreadable file is set aside, and the working definition stands as it is.
            _history.Clear();
            File.Move(_options.HistoryPath, _options.HistoryPath + $".unreadable-{DateTime.Now:yyyyMMddHHmmss}");
        }
    }

    static void WriteFile(string path, string text)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllText(path, text);
    }
}
