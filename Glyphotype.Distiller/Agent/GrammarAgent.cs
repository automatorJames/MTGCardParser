using System.Text.RegularExpressions;
using Glyphotype.Distiller.Inspection;
using Glyphotype.Distiller.Scoring;
using Glyphotype.Distiller.Workbench;
using Glyphotype.Distiller.Workspaces;

namespace Glyphotype.Distiller.Agent;

/// <summary>A request an agent made that can't be carried out, with a message written for the agent to act on.</summary>
public sealed class AgentRequestException(string message, Exception inner = null) : Exception(message, inner);

/// <summary>
/// Everything an agent needs to compose a grammar over one <see cref="GrammarWorkbench"/>, as plain-text reports:
/// orientation (score, glyphs, definitions), inspection (lines, matches, residual text), probing (tokenize and
/// explain a draft on any text), and stepping (evaluate a change set without making it, apply it as a step,
/// undo) - and, given a <see cref="WorkspaceManager"/>, choosing which grammar to work on. The workbench is shared - a
/// person editing the same one sees every step, and the agent sees theirs.
/// <para>
/// Host-independent: a tool server (or anything else) exposes these methods as they are. Every method reads the
/// workbench's state afresh, and waits for a re-score under way. Requests that can't be carried out throw
/// <see cref="AgentRequestException"/>, with a message saying what to do instead.
/// </para>
/// </summary>
public sealed class GrammarAgent
{
    readonly Func<GrammarWorkbench> _workbench;
    readonly Func<WorkspaceJournal> _journal;
    readonly WorkspaceManager _workspaces;
    readonly string _corpusDescription;

    /// <summary>Steps applied, and evaluations made since the last applied step, since the session (re)started.</summary>
    int _stepsSinceCheckIn;
    int _attemptsSinceStep;

    /// <summary>The round the session's steps are applied in (see <see cref="WorkbenchStep.Round"/>), and the workbench that counts it - null until the session's first step.</summary>
    (GrammarWorkbench Workbench, int Number)? _round;

    /// <summary>How many <see cref="SuspendCheckIns"/> are under way - while any is, no check-in is due.</summary>
    int _checkInsSuspended;

    /// <summary>Whether the journal has been written to since the last applied step - so a stuck agent is reminded to record why only until it does.</summary>
    bool _journalEditedSinceStep;

    /// <summary>Whether the session is to hand off to a fresh one (see <see cref="RequestHandoff"/>), and whether the next session continues this one's round (see <see cref="ContinueRoundInNextSession"/>).</summary>
    volatile bool _handoffDue;
    volatile bool _continueRound;

    /// <summary>An agent working on whichever of <paramref name="workspaces"/> is active, and able to create and switch between them.</summary>
    public GrammarAgent(WorkspaceManager workspaces, string corpusDescription, AgentSessionSettings settings = null)
    {
        _workspaces = workspaces;
        _workbench = () => workspaces.Active;
        _journal = () => workspaces.GetJournal();
        _corpusDescription = corpusDescription;
        Settings = settings ?? new();
    }

    /// <summary>An agent working on <paramref name="workbench"/> alone - keeping <paramref name="journal"/>, if given.</summary>
    public GrammarAgent(GrammarWorkbench workbench, string corpusDescription, AgentSessionSettings settings = null, WorkspaceJournal journal = null)
    {
        _workbench = () => workbench;
        _journal = () => journal;
        _corpusDescription = corpusDescription;
        Settings = settings ?? new();
    }

    /// <summary>How sessions run. Settable while one runs: a new value holds from the next request on.</summary>
    public AgentSessionSettings Settings { get; set; }

    /// <summary>The workbench of the active workspace - read afresh by every request, so a switch takes effect at once.</summary>
    GrammarWorkbench Workbench => _workbench();

    /// <summary>The guide to the workbench, its scoring, and writing glyphs - see <c>AgentGuide.md</c>.</summary>
    public static string Guide { get; } = LoadGuide();

    /// <summary>A short description of the tools and where to start, for a tool server's instructions.</summary>
    public const string Instructions =
        "Tools for composing a grammar over a text corpus, glyph by glyph, on a working definition shared live with a person. " +
        "Start with `start_session` (passing the person's instructions): it returns the session's rules and where the grammar stands. Read `guide` once: it explains the loop (find recurring unmatched text, draft a glyph in C#, " +
        "`tokenize`/`explain_mismatch` it, `evaluate` it, `apply` it), how the score works, and how to write glyphs. " +
        "The active workspace (named in `overview`) is the grammar both you and the person see; create or switch workspaces only when asked. " +
        "Committing to C# source, or checkpointing or exporting a scratch workspace, is the person's decision, made in the app - never part of the loop.";

    static string LoadGuide()
    {
        using var stream = typeof(GrammarAgent).Assembly.GetManifestResourceStream("AgentGuide.md")
            ?? throw new InvalidOperationException("The agent guide isn't embedded in the assembly");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    // ---- Orientation ----

    public async Task<string> OverviewAsync(CancellationToken cancellation = default)
    {
        var trial = await Workbench.GetCurrentTrialAsync(cancellation);
        var definition = trial.Definition;
        var report = new StringBuilder();

        if (_workspaces is not null)
            report.AppendLine($"Workspace: {DescribeWorkspace(_workspaces.ActiveWorkspace)}.");

        report.AppendLine($"Corpus: {_corpusDescription} ({Workbench.Documents.Count:N0} documents).");
        report.AppendLine($"Working grammar: {definition.Glyphs.Count} glyphs, {definition.Vocabularies.Count} vocabularies. "
            + $"{Workbench.Changes.Count} uncommitted change{S(Workbench.Changes.Count)}, {Workbench.History.Count} step{S(Workbench.History.Count)} this session.");

        if (!trial.Succeeded)
        {
            report.AppendLine();
            report.AppendLine("The working definition doesn't build, so nothing can be scored or inspected until it's fixed (or the breaking step undone):");
            AppendErrors(report, trial.Errors);
            return report.ToString();
        }

        var score = trial.Score;
        report.AppendLine($"Score: {score.TotalBits:N0} bits = {score.CompressionRatio:P2} of the no-grammar baseline ({score.BaselineBits:N0}). Lower is better.");
        report.AppendLine($"  grammar {score.GrammarBits:N0} · {string.Join(" · ", score.ComponentBits.Select(x => $"{x.Key.ToString().ToLowerInvariant()} {x.Value:N0}"))}");
        report.AppendLine($"Coverage: {score.Coverage:P2} of words inside matches ({score.CapturedWords:N0} of {score.Words:N0}, {score.Lines:N0} lines).");

        // A glyph also matched nested is credited to the glyphs using it, so its own net undersells it.
        var losing = score.Glyphs.Where(x => x.IsTopLevel && !x.IsUsedNested && x.NetBits < 0).OrderBy(x => x.NetBits).ToList();

        if (losing.Count > 0)
            report.AppendLine($"Top-level glyphs costing more than they save: {string.Join(", ", losing.Take(8).Select(x => $"{x.Name} ({x.NetBits:N0})"))}{(losing.Count > 8 ? $", … {losing.Count - 8} more" : "")}. " +
                "Look for a cheaper or more general way to cover their text, rather than uncovering it - only one that matches nothing can simply go.");

        report.AppendLine();
        report.AppendLine("Costliest unmatched text:");
        AppendResiduals(report, score.Residuals.OrderByDescending(x => x.Bits).Take(10), score);

        if (Workbench.Changes.Count == 0 && Workbench.History.Count == 0)
        {
            report.AppendLine();
            report.AppendLine("New here? Read `guide` before your first change.");
        }

        return report.ToString();
    }

    /// <summary>Every glyph with its contribution, and every vocabulary with its cost and users.</summary>
    /// <param name="sort">"net" (default), "matches", "name" or "cost".</param>
    public async Task<string> ListGlyphsAsync(string sort = "net", CancellationToken cancellation = default)
    {
        var trial = await GetScoredTrialAsync(cancellation);
        var definition = trial.Definition;
        var contributions = trial.Score.Glyphs.ToDictionary(x => x.Name);
        var status = Workbench.Changes.ToDictionary(x => (x.Kind, x.Name), x => x.Change);

        var glyphs = definition.Glyphs.Select(x => (Glyph: x, Contribution: contributions.GetValueOrDefault(x.Name))).ToList();

        glyphs = (sort?.ToLowerInvariant()) switch
        {
            "name" => glyphs.OrderBy(x => x.Glyph.Name, StringComparer.Ordinal).ToList(),
            "matches" => glyphs.OrderByDescending(x => x.Contribution?.Occurrences ?? 0).ToList(),
            "cost" => glyphs.OrderByDescending(x => x.Contribution?.DefinitionBits ?? 0).ToList(),
            _ => glyphs.OrderByDescending(x => x.Contribution?.NetBits ?? 0).ToList(),
        };

        var report = new StringBuilder();
        report.AppendLine($"Glyphs ({glyphs.Count}; {glyphs.Count(x => x.Contribution?.IsTopLevel == true)} top-level). Matches and words count top-level matches; nested-only glyphs are credited to the glyphs using them.");
        report.AppendLine($"  {"net",9} {"matches",8} {"words",7} {"def",6}  glyph");

        foreach (var (glyph, contribution) in glyphs)
        {
            List<string> notes = [];

            if (glyph.Kind != GlyphKind.Glyph)
                notes.Add(glyph.Kind == GlyphKind.Alias ? $"alias of {glyph.AliasOf}" : "one-of");

            if (contribution?.IsTopLevel != true)
                notes.Add(glyph.IsDependent ? "dependent" : "nested only");
            else if (contribution.IsUsedNested)
                notes.Add($"also matched nested {contribution.NestedOccurrences:N0}×");

            if (status.TryGetValue((DefinitionKind.Glyph, glyph.Name), out var change))
                notes.Add(change.ToString().ToLowerInvariant() + " since commit");

            report.AppendLine($"  {contribution?.NetBits ?? 0,9:N0} {contribution?.Occurrences ?? 0,8:N0} {contribution?.Words ?? 0,7:N0} {contribution?.DefinitionBits ?? 0,6:N0}  {glyph.Name}{(notes.Count > 0 ? $"  ({string.Join(", ", notes)})" : "")}");
        }

        report.AppendLine();
        report.AppendLine($"Vocabularies ({definition.Vocabularies.Count}):");
        report.AppendLine($"  {"cost",7} {"members used",13}  vocabulary  (used by) - only used members cost anything");

        foreach (var vocabulary in definition.Vocabularies.OrderByDescending(x => trial.Score.Vocabularies.GetValueOrDefault(x.Name)))
        {
            var users = definition.GetReferrers(vocabulary.Name);
            var changed = status.TryGetValue((DefinitionKind.Vocabulary, vocabulary.Name), out var change) ? $", {change.ToString().ToLowerInvariant()} since commit" : "";

            var used = $"{trial.Score.VocabularyMembersUsed.GetValueOrDefault(vocabulary.Name)} of {vocabulary.Members.Count}";
            report.AppendLine($"  {trial.Score.Vocabularies.GetValueOrDefault(vocabulary.Name),7:N0} {used,13}  {vocabulary.Name}  ({(users.Count > 0 ? string.Join(", ", users) : "unused")}{changed})");
        }

        return report.ToString();
    }

    /// <summary>The C# source of the named glyphs, vocabularies and markers in the working definition.</summary>
    /// <param name="names">Comma- or space-separated names.</param>
    public string Show(string names)
    {
        var definition = Workbench.WorkingDefinition;
        var status = Workbench.Changes.ToDictionary(x => x.Name, x => x.Change);
        var report = new StringBuilder();

        foreach (var name in SplitNames(names))
        {
            var note = status.TryGetValue(name, out var change) ? $"// {change.ToString().ToLowerInvariant()} since the last commit{Environment.NewLine}" : "";

            if (definition.Glyphs.FirstOrDefault(x => x.Name == name) is GlyphDefinition glyph)
                report.Append(note).AppendLine(GlyphSourceWriter.WriteGlyph(glyph));
            else if (definition.Vocabularies.FirstOrDefault(x => x.Name == name) is VocabularyDefinition vocabulary)
                report.Append(note).AppendLine(GlyphSourceWriter.WriteVocabulary(vocabulary));
            else if (definition.Markers.Contains(name))
                report.Append(note).AppendLine(GlyphSourceWriter.WriteMarker(name));
            else
                report.AppendLine($"// {name}: no glyph, vocabulary or marker has this name{Suggest(name, definition)}").AppendLine();
        }

        return report.ToString().TrimEnd() + Environment.NewLine;
    }

    // ---- Sessions ----

    /// <summary>
    /// Starts (or, after a check-in, resumes) a working session: everything the agent needs in one brief - the person's
    /// instructions, the session's rules as the app configures them, and where the grammar stands. Resets the counts
    /// that decide when to check in.
    /// </summary>
    /// <param name="instructions">What the person asked for this session, in their words - may be empty.</param>
    public async Task<string> StartSessionAsync(string instructions = null, CancellationToken cancellation = default)
    {
        var continuing = _continueRound;
        _continueRound = false;
        _handoffDue = false;

        // A session continuing a round after a handoff keeps the round, and its count toward the check-in.
        if (!continuing)
        {
            Interlocked.Exchange(ref _stepsSinceCheckIn, 0);
            _round = null;
        }

        Interlocked.Exchange(ref _attemptsSinceStep, 0);

        var report = new StringBuilder();

        report.AppendLine(continuing
            ? "Session started, continuing the round the last session handed off when its context filled up. What it learned, and what it was in the middle of, is in the journal below."
                + (Settings.StepsBeforeCheckIn > 0 ? $" {_stepsSinceCheckIn} of the round's {Settings.StepsBeforeCheckIn} steps are taken." : "")
            : "Session started.");
        report.AppendLine($"The person's instructions: {(string.IsNullOrWhiteSpace(instructions) ? "none given - improve the grammar in the active workspace as it stands." : instructions.Trim())}");
        report.AppendLine();
        report.AppendLine("How this session works (set in the app, and enforced by the tools):");
        report.AppendLine("- The goal is full coverage: every word of the corpus inside a match. Coverage decides whether to cover text, and bits decide how - among the ways to cover it, the cheapest and most general. " +
            "Text with no cheap model still has to be covered: a step that costs bits to cover it is progress, not a failure.");
        report.AppendLine($"- The loop: find recurring unmatched text, draft a glyph, check it with `tokenize`/`explain_mismatch`, `evaluate` it, then `apply` it with a one-line description of why. Before each step, say in one line what you're targeting. If you haven't read `guide` in this conversation, read it first.");

        var checkIn = new List<string>();

        if (Settings.StepsBeforeCheckIn > 0)
            checkIn.Add($"after {Settings.StepsBeforeCheckIn} applied step{S(Settings.StepsBeforeCheckIn)}");

        if (Settings.AttemptsBeforeCheckIn > 0)
            checkIn.Add($"after {Settings.AttemptsBeforeCheckIn} evaluations in a row without an applied step");

        report.AppendLine(checkIn.Count > 0
            ? $"- Check in {string.Join(", or ", checkIn)} (the tools say when): stop, summarize each step with its bit and coverage change, say what you'd try next, and wait. When the person says to continue, call `start_session` again. " +
                "Don't stop before then: text that's expensive to cover is something to evaluate and, if no step can afford it, to journal as an open problem - then move on to the next target."
            : "- There's no check-in limit: keep going until the corpus is covered or you're stuck, then summarize and wait. Uncovered text that only costs bits to cover isn't a reason to stop.");

        var stepRule = Settings.MaxBitsPerCoveredWord > 0
            ? $"take at least {Settings.MinimumGainBits:N0} bit{S(Settings.MinimumGainBits)} off the total, or cover more words for at most {Settings.MaxBitsPerCoveredWord:N0} bit{S(Settings.MaxBitsPerCoveredWord)} per word gained"
            : $"take at least {Settings.MinimumGainBits:N0} bit{S(Settings.MinimumGainBits)} off the total";
        report.AppendLine($"- A step must {stepRule}{(Settings.AllowLostLines ? "" : ", and lose no lines")}. `apply` refuses anything else unless you pass `override_reason` - for a deliberate refactor, never to force a loss through.");
        if (DocumentationRule is { } documentationRule)
            report.AppendLine($"- {documentationRule}");

        report.AppendLine("- Stay in the active workspace. Create or switch workspaces only if the instructions ask - to start from scratch, `create_workspace` with start=vocabularies.");
        report.AppendLine("- Never commit, checkpoint or export: the person does that in the app.");

        if (_journal() is not null)
            report.AppendLine("- Keep the workspace's journal (`journal_add`, `journal_update`, `journal_remove`): what a later session would want to know before doing the work you did - open problems, dead ends (what you tried and why it didn't work), and hints. " +
                "A way of covering text that cost too many bits is an open problem, not a dead end: say what it covered, what it cost, and the best form you found, since the text still needs covering. " +
                "Write each down when you find it, not at the end - you can be stopped at any time. It's not a log of what you did (the step history is that), and facts about the tools or the engine in general don't belong in it. " +
                $"Remove or rewrite entries that no longer hold{(Settings.JournalWordLimit > 0 ? $", and keep it under {Settings.JournalWordLimit:N0} words" : "")}.");

        if (_workspaces?.GetGuidance() is { Length: > 0 } guidance)
        {
            report.AppendLine();
            report.AppendLine("Guidance for this workspace's grammar, written by the person - what it is for and how to approach iterating it. Follow it alongside the guide:");
            report.AppendLine(guidance);
        }

        if (_journal() is { } journal)
        {
            var entries = journal.Entries;
            report.AppendLine();

            if (entries.Count == 0)
                report.AppendLine("The workspace's journal is empty: nothing's been learned here yet.");
            else
            {
                report.AppendLine($"The workspace's journal ({journal.Words:N0} words) - what earlier sessions learned. Read it before choosing what to work on. An entry marked \"may be stale\" is about a glyph or vocabulary that's changed since it was written: check it, then update or remove it.");
                report.AppendLine(WorkspaceJournal.Render(entries, Workbench.WorkingDefinition));
            }
        }

        if (Workbench.History is { Count: > 0 } history)
        {
            report.AppendLine();
            report.AppendLine("The latest steps, newest first:");

            foreach (var step in history.AsEnumerable().Reverse().Take(5))
                report.AppendLine($"  #{step.Number,-4}{(step.Round is int round ? $" (AI round {round})" : "")} {step.Description}");
        }

        report.AppendLine();
        report.Append(await OverviewAsync(cancellation));

        return report.ToString();
    }

    /// <summary>
    /// Has the session hand off to a fresh one - for a host whose agent's context is filling up: from the next report on,
    /// the agent is told to put what the next session needs in the journal and stop, and no step is applied until a
    /// session starts again.
    /// </summary>
    public void RequestHandoff() => _handoffDue = true;

    public bool IsHandoffDue => _handoffDue;

    /// <summary>Steps applied since the session started or last checked in - what <see cref="AgentSessionSettings.StepsBeforeCheckIn"/> counts.</summary>
    public int StepsSinceCheckIn => _stepsSinceCheckIn;

    /// <summary>Has the next <see cref="StartSessionAsync"/> continue this session's round - and its count toward the check-in - rather than start a new one.</summary>
    public void ContinueRoundInNextSession() => _continueRound = true;

    /// <summary>Whether the session has reached a check-in (see <see cref="AgentSessionSettings"/>), so it's over until the person says to continue.</summary>
    public bool IsCheckInDue =>
        _checkInsSuspended == 0
        && ((Settings.StepsBeforeCheckIn > 0 && _stepsSinceCheckIn >= Settings.StepsBeforeCheckIn)
            || (Settings.AttemptsBeforeCheckIn > 0 && _attemptsSinceStep >= Settings.AttemptsBeforeCheckIn));

    /// <summary>
    /// What an agent is told about documenting the glyphs it writes, when <see cref="AgentSessionSettings.DocumentGlyphs"/>
    /// says it must - null when it needn't. For any brief that sets an agent writing glyphs, a session's or otherwise.
    /// </summary>
    public string DocumentationRule => Settings.DocumentGlyphs
        ? "Document every glyph you add or change, in its doc comment: `/// <summary>` saying what it's for and why it exists, " +
          "`/// <exampledoc>` naming one corpus document it's meant for (as the tools name documents), and `/// <examplecapture>` " +
          "with just the text in that document it captures. Keep a glyph's documentation when you change it, updating what's out of date. " +
          "`evaluate` and `apply` remind you of a glyph missing any of them; a step that only documents glyphs needn't take any bits off, and isn't counted among the round's steps. " +
          "A doc comment goes above everything in the declaration, attributes included."
        : null;

    /// <summary>The glyphs <paramref name="changes"/> declares without complete documentation, when the session requires it - empty otherwise.</summary>
    List<string> Undocumented(ChangeSet changes) =>
        Settings.DocumentGlyphs
            ? changes.Declarations.Glyphs.Where(x => !GlyphDocumentation.IsComplete(x.Documentation)).Select(x => x.Name).ToList()
            : [];

    /// <summary>A reminder, for a report's end, of the glyphs <paramref name="changes"/> declares without complete documentation - null when there are none.</summary>
    string DocumentationReminder(ChangeSet changes) =>
        Undocumented(changes) is { Count: > 0 } names
            ? $"Documentation: {string.Join(", ", names)} {(names.Count == 1 ? "lacks" : "lack")} a `/// <summary>`, `/// <exampledoc>` or `/// <examplecapture>`. Every glyph you add or change should have all three - add them in a later step if not in this one."
            : null;

    /// <summary>A warning, for a report's end, of declarations in <paramref name="source"/> whose doc comment isn't above them all, and so was dropped - null when there are none.</summary>
    static string MisplacedDocumentation(string source) =>
        GlyphDocComment.Misplaced(source) is { Count: > 0 } names
            ? $"Documentation dropped: the `///` doc comment on {string.Join(", ", names)} comes after {(names.Count == 1 ? "its" : "their")} attributes, where it isn't read. " +
              "Put a declaration's doc comment above everything else, attributes included, and send it again."
            : null;

    /// <summary>Whether <paramref name="changes"/> changes nothing but the documentation of glyphs <paramref name="working"/> already has.</summary>
    static bool OnlyDocuments(ChangeSet changes, GrammarDefinition working) =>
        changes.Removals.Count == 0
        && changes.Declarations.Vocabularies.Count == 0
        && changes.Declarations.Markers.Count == 0
        && changes.Declarations.Glyphs.All(x => working.Glyphs.FirstOrDefault(y => y.Name == x.Name) is { } existing
            && DefinitionJson.Serialize(existing with { Documentation = x.Documentation }) == DefinitionJson.Serialize(x));

    /// <summary>The session's progress toward a check-in, for a report's last lines - or a check-in, when it's due.</summary>
    string SessionStatus(bool applied)
    {
        if (_checkInsSuspended > 0)
            return null;

        if (_handoffDue && !IsCheckInDue)
            return HandoffInstruction;

        if (applied && Settings.StepsBeforeCheckIn > 0)
        {
            var steps = _stepsSinceCheckIn;

            return steps >= Settings.StepsBeforeCheckIn
                ? $"Check-in due ({steps} step{S(steps)} applied): stop now, summarize the steps for the person, and wait. When they say to continue, call `start_session`."
                : $"Session: {steps} of {Settings.StepsBeforeCheckIn} steps applied - keep going: the check-in comes after step {Settings.StepsBeforeCheckIn}, and the tools say when it's due.";
        }

        if (!applied && Settings.AttemptsBeforeCheckIn > 0 && _attemptsSinceStep >= Settings.AttemptsBeforeCheckIn)
            return $"Check-in due: {_attemptsSinceStep} evaluations without an applied step. Stop, tell the person what you tried and why none of it could be applied, and wait. When they say to continue, call `start_session`.";

        return null;
    }

    const string HandoffInstruction =
        "Handoff due: your context is filling up, so this round goes on in a fresh session that starts from the journal. " +
        "Put in the journal what it needs - what you were in the middle of, what you'd try next, and anything you've learned that isn't there yet - " +
        "then end your turn with one line saying you've handed off. Don't apply anything more.";

    /// <summary>
    /// Sets the session's check-ins aside until disposed: for work the person directs as it goes (the glyphs for one
    /// line, say), where stopping to check in after so many steps would only get in the way. Its steps are a round of
    /// their own, and the session's counts are as they were once it's over.
    /// </summary>
    public IDisposable SuspendCheckIns()
    {
        Interlocked.Increment(ref _checkInsSuspended);

        var (steps, attempts, round) = (_stepsSinceCheckIn, _attemptsSinceStep, _round);
        _round = null;

        return new Resumption(() =>
        {
            (_stepsSinceCheckIn, _attemptsSinceStep, _round) = (steps, attempts, round);
            Interlocked.Decrement(ref _checkInsSuspended);
        });
    }

    sealed class Resumption(Action resume) : IDisposable
    {
        Action _resume = resume;

        public void Dispose() => Interlocked.Exchange(ref _resume, null)?.Invoke();
    }

    static string WithStatus(string report, string status) =>
        status is null ? report : report.TrimEnd() + Environment.NewLine + Environment.NewLine + status + Environment.NewLine;

    // ---- Workspaces ----

    /// <summary>Every workspace, which one is active, and what each kind means.</summary>
    public string ListWorkspaces()
    {
        var workspaces = RequireWorkspaces();
        var report = new StringBuilder();

        report.AppendLine("Workspaces (* is active - it's what you and the person are both looking at):");

        foreach (var workspace in workspaces.Workspaces)
            report.AppendLine($"  {(workspace == workspaces.ActiveWorkspace ? "*" : " ")} {DescribeWorkspace(workspace)}");

        report.AppendLine();
        report.AppendLine("A source workspace's grammar is the app's C# sources; a scratch workspace's is kept as JSON and exported as C# by the person.");
        return report.ToString();
    }

    /// <summary>Creates a scratch workspace and makes it active - only when the person asked for one.</summary>
    /// <param name="start">"empty", "vocabularies" (another workspace's vocabularies and nothing else) or "copy".</param>
    /// <param name="from">The workspace to take vocabularies from, or copy - the active one by default.</param>
    public async Task<string> CreateWorkspaceAsync(string name, string start = "empty", string from = null, bool copyGuidance = false, bool copyJournal = false, CancellationToken cancellation = default)
    {
        var workspaces = RequireWorkspaces();

        if (!Enum.TryParse<WorkspaceSeed>(start, ignoreCase: true, out var seed))
            throw new AgentRequestException($"Unknown start '{start}': use empty, vocabularies or copy.");

        var workspace = Try(() => workspaces.Create(name, seed, NullIfBlank(from), copyGuidance, copyJournal));
        return $"Created and switched to {DescribeWorkspace(workspace)}.{Environment.NewLine}{Environment.NewLine}{await OverviewAsync(cancellation)}";
    }

    /// <summary>Makes another workspace active - only when the person asked for it, since it changes what they see too.</summary>
    public async Task<string> SwitchWorkspaceAsync(string name, CancellationToken cancellation = default)
    {
        var workspaces = RequireWorkspaces();
        Try(() => workspaces.Switch(name));

        return $"Switched to {DescribeWorkspace(workspaces.ActiveWorkspace)}.{Environment.NewLine}{Environment.NewLine}{await OverviewAsync(cancellation)}";
    }

    WorkspaceManager RequireWorkspaces() =>
        _workspaces ?? throw new AgentRequestException("This host serves one grammar, without workspaces.");

    static string DescribeWorkspace(WorkspaceInfo workspace) =>
        $"{workspace.Name} ({(workspace.Kind == WorkspaceKind.Source ? "source - the app's C# sources" : "scratch - kept as JSON")})";

    // ---- Journal ----

    /// <summary>The workspace's journal, with each entry's id and whether what it's about has changed since.</summary>
    public string Journal()
    {
        var journal = RequireJournal();
        var entries = journal.Entries;

        return $"The journal: {entries.Count} entr{(entries.Count == 1 ? "y" : "ies")}, {journal.Words:N0} words{(Settings.JournalWordLimit > 0 ? $" of {Settings.JournalWordLimit:N0}" : "")}.{Environment.NewLine}"
            + WorkspaceJournal.Render(entries, Workbench.WorkingDefinition) + Environment.NewLine;
    }

    /// <summary>Adds an entry to the workspace's journal.</summary>
    /// <param name="section">"open_problem", "dead_end" or "hint".</param>
    /// <param name="names">The glyphs and vocabularies it's about, comma-separated.</param>
    public string JournalAdd(string section, string text, string names = null)
    {
        var journal = RequireJournal();
        var parsed = ParseSection(section);
        RequireRoom(journal, WorkspaceJournal.CountWords(text));

        var entry = Try(() => journal.Add(parsed, text, SplitNames(names), Workbench.WorkingDefinition));
        _journalEditedSinceStep = true;

        return $"Added #{entry.Id} to {WorkspaceJournal.SectionTitle(entry.Section).ToLowerInvariant()} ({journal.Words:N0} words in all).";
    }

    /// <summary>Rewrites an entry of the workspace's journal: whatever's given of its text, section and names.</summary>
    public string JournalUpdate(int id, string text = null, string section = null, string names = null)
    {
        var journal = RequireJournal();
        var old = journal.Entries.FirstOrDefault(x => x.Id == id) ?? throw new AgentRequestException($"The journal has no entry #{id}.");

        if (!string.IsNullOrWhiteSpace(text))
            RequireRoom(journal, WorkspaceJournal.CountWords(text) - old.Words);

        var entry = Try(() => journal.Update(id, string.IsNullOrWhiteSpace(section) ? null : ParseSection(section), text, names is null ? null : SplitNames(names), Workbench.WorkingDefinition));
        _journalEditedSinceStep = true;

        return $"Updated #{entry.Id} ({journal.Words:N0} words in all).";
    }

    /// <summary>Removes an entry from the workspace's journal - one that no longer holds.</summary>
    public string JournalRemove(int id)
    {
        var journal = RequireJournal();
        var entry = Try(() => journal.Remove(id));
        _journalEditedSinceStep = true;

        return $"Removed #{entry.Id} ({journal.Words:N0} words left).";
    }

    WorkspaceJournal RequireJournal() =>
        _journal() ?? throw new AgentRequestException("This host keeps no journal.");

    /// <summary>Refuses an edit that would take the journal past its word limit, adding <paramref name="added"/> words.</summary>
    void RequireRoom(WorkspaceJournal journal, int added)
    {
        var words = journal.Words + added;

        if (Settings.JournalWordLimit > 0 && added > 0 && words > Settings.JournalWordLimit)
            throw new AgentRequestException($"Not written: the journal would come to {words:N0} words, past its limit of {Settings.JournalWordLimit:N0}. " +
                "Remove entries that no longer hold, or shorten some (`journal_update`), then try again.");
    }

    static JournalSection ParseSection(string section) =>
        WorkspaceJournal.TryParseSection(section, out var parsed)
            ? parsed
            : throw new AgentRequestException($"Unknown section '{section}': use open_problem, dead_end or hint.");

    /// <summary>After every few evaluations without a step, while nothing's been written to the journal since the last: a reminder to record what didn't work.</summary>
    string DeadEndReminder() =>
        _journal() is not null && !_journalEditedSinceStep && _attemptsSinceStep > 0 && _attemptsSinceStep % 3 == 0
            ? $"Journal: {_attemptsSinceStep} evaluations without a step. If you've learned why these drafts don't work, record it (`journal_add`) before moving on - " +
              "as a dead end if the approach is wrong, or as an open problem if it covers text but costs too much, since that text still needs covering."
            : null;

    /// <summary>A reminder of the journal entries about what <paramref name="changes"/> changed, for the agent to check - null when there are none.</summary>
    string JournalReminder(ChangeSet changes)
    {
        if (_journal() is not { } journal)
            return null;

        var changed = changes.Declarations.Glyphs.Select(x => x.Name)
            .Concat(changes.Declarations.Vocabularies.Select(x => x.Name))
            .Concat(changes.Declarations.Markers)
            .Concat(changes.Removals)
            .ToHashSet();

        var about = journal.Entries.Where(x => x.Names.Any(changed.Contains)).ToList();

        if (about.Count == 0)
            return null;

        var one = about.Count == 1;
        return $"Journal: {string.Join(", ", about.Select(x => $"#{x.Id}"))} {(one ? "is" : "are")} about what this step changed ({string.Join(", ", about.SelectMany(x => x.Names).Where(changed.Contains).Distinct())}). " +
            $"Update or remove {(one ? "it if it no longer holds" : "them if they no longer hold")}.";
    }

    // ---- Stepping ----

    /// <summary>What applying a change set would do - to the score, and to how lines tokenize - without applying it.</summary>
    /// <param name="source">C# declarations to add or replace (see the guide).</param>
    /// <param name="remove">Comma- or space-separated names of definitions to remove.</param>
    public async Task<string> EvaluateAsync(string source, string remove = null, CancellationToken cancellation = default)
    {
        var changes = ReadChanges(source, remove);
        var evaluation = await Evaluate(changes, cancellation);
        Interlocked.Increment(ref _attemptsSinceStep);

        var report = WithStatus($"Evaluated, not applied: {changes.Describe()}{Environment.NewLine}{DescribeEvaluation(evaluation)}", MisplacedDocumentation(source));

        if (evaluation.After.Succeeded)
            report += RuleViolations(evaluation) is { Count: > 0 } violations
                ? $"{Environment.NewLine}`apply` would refuse this: {string.Join("; ", violations)}."
                : $"{Environment.NewLine}To make this change, `apply` the same source and removals.";

        return WithStatus(WithStatus(WithStatus(report, DocumentationReminder(changes)), DeadEndReminder()), SessionStatus(applied: false));
    }

    /// <summary>
    /// Makes a change set as one step of the working definition - refused if the result wouldn't build, if the session's
    /// check-in is due, or if it breaks the session's step rules (see <see cref="Settings"/>) without an override reason.
    /// </summary>
    /// <param name="description">Why: shown to the person in the step history.</param>
    /// <param name="overrideReason">Why a step that breaks the step rules should be applied anyway - recorded in its description.</param>
    public async Task<string> ApplyAsync(string source, string remove = null, string description = null, string overrideReason = null, CancellationToken cancellation = default)
    {
        var changes = ReadChanges(source, remove);

        // A step that only documents glyphs is housekeeping, not one of the round's steps: it counts toward neither check-in, and isn't held up by one.
        var onlyDocuments = OnlyDocuments(changes, Workbench.WorkingDefinition);

        if (_checkInsSuspended == 0 && !onlyDocuments && Settings.StepsBeforeCheckIn > 0 && _stepsSinceCheckIn >= Settings.StepsBeforeCheckIn)
            throw new AgentRequestException($"Not applied: a check-in is due after {Settings.StepsBeforeCheckIn} steps. Summarize the steps for the person and wait; when they say to continue, call `start_session`.");

        if (_checkInsSuspended == 0 && _handoffDue)
            throw new AgentRequestException("Not applied. " + HandoffInstruction + " If this change is worth making, describe it in the journal for the next session.");

        var evaluation = await Evaluate(changes, cancellation);
        Interlocked.Increment(ref _attemptsSinceStep);

        if (!evaluation.After.Succeeded)
            return WithStatus($"Not applied - the result wouldn't build: {changes.Describe()}{Environment.NewLine}{DescribeEvaluation(evaluation)}", SessionStatus(applied: false));

        var violations = RuleViolations(evaluation);

        if (violations.Count > 0 && string.IsNullOrWhiteSpace(overrideReason))
            return WithStatus($"Not applied - {string.Join("; ", violations)}: {changes.Describe()}{Environment.NewLine}{DescribeEvaluation(evaluation)}"
                + $"{Environment.NewLine}Rework it, or undo toward something better. Pass `override_reason` only for a deliberate refactor.", SessionStatus(applied: false));

        description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();

        if (violations.Count > 0)
            description = $"{description ?? changes.Describe()} (override: {overrideReason.Trim()})";

        var workbench = Workbench;

        // A session's steps are one round, numbered after the last round the workbench's history holds.
        if (_round?.Workbench != workbench)
            _round = (workbench, (workbench.History.Max(x => x.Round) ?? 0) + 1);

        var step = Try(() => workbench.Apply(changes, description, _round.Value.Number))
            ?? throw new AgentRequestException("Nothing to apply: the working definition already reads exactly like this.");

        if (onlyDocuments)
            Interlocked.Decrement(ref _attemptsSinceStep);
        else
        {
            Interlocked.Increment(ref _stepsSinceCheckIn);
            Interlocked.Exchange(ref _attemptsSinceStep, 0);
            _journalEditedSinceStep = false;
        }

        var report = WithStatus($"Applied as step {step.Number}: {step.Description}{Environment.NewLine}{DescribeEvaluation(evaluation)}", MisplacedDocumentation(source));
        return WithStatus(WithStatus(WithStatus(report, DocumentationReminder(changes)), JournalReminder(changes)), SessionStatus(applied: !onlyDocuments || _stepsSinceCheckIn >= Settings.StepsBeforeCheckIn));
    }

    /// <summary>How <paramref name="evaluation"/> falls short of the session's step rules - empty when it doesn't.</summary>
    List<string> RuleViolations(Evaluation evaluation)
    {
        List<string> violations = [];
        var gain = evaluation.Before.Score.TotalBits - evaluation.After.Score.TotalBits;
        var wordsGained = evaluation.After.Score.CapturedWords - evaluation.Before.Score.CapturedWords;

        // Coverage decides whether to cover text, and bits decide how: a step covering more words may cost bits, up to a price per word.
        var coversAffordably = Settings.MaxBitsPerCoveredWord > 0 && wordsGained > 0 && -gain <= Settings.MaxBitsPerCoveredWord * wordsGained;

        // Documentation isn't grammar: a step that only documents takes nothing off, and needn't.
        if (gain < Settings.MinimumGainBits && !coversAffordably && !OnlyDocuments(evaluation.Changes, evaluation.Before.Definition))
            violations.Add($"it takes {gain:N1} bits off the total, and a step must take at least {Settings.MinimumGainBits:N0}" + (Settings.MaxBitsPerCoveredWord > 0
                ? $" or cover more words for at most {Settings.MaxBitsPerCoveredWord:N0} bits each ({(wordsGained > 0 ? $"this costs {-gain / wordsGained:N1} per word it gains" : "this covers no more")})"
                : ""));

        if (!Settings.AllowLostLines && CorpusQueries.CompareTokenizations(evaluation.Before.Documents, evaluation.After.Documents, limit: 0).LostLines is int lost and > 0)
            violations.Add($"it loses {lost} line{S(lost)}, and a step must lose none");

        return violations;
    }

    /// <summary>Takes back the latest step, whoever made it.</summary>
    public async Task<string> UndoAsync(CancellationToken cancellation = default)
    {
        var before = await Workbench.GetCurrentTrialAsync(cancellation);
        var step = Try(Workbench.Undo);
        var after = await Workbench.GetCurrentTrialAsync(cancellation);

        return $"Undid step {step.Number}: {step.Description}{Environment.NewLine}{DescribeEvaluation(new(new ChangeSet(), before, after), lineLimit: 5)}";
    }

    /// <summary>Returns the named glyph or vocabulary to its committed state, as one step.</summary>
    public async Task<string> RevertAsync(string name, CancellationToken cancellation = default)
    {
        var before = await Workbench.GetCurrentTrialAsync(cancellation);
        var kind = Workbench.Changes.FirstOrDefault(x => x.Name == name)?.Kind
            ?? throw new AgentRequestException($"{name} has no uncommitted changes to revert{Suggest(name, Workbench.WorkingDefinition)}");

        Try(() => Workbench.Revert(kind, name));
        var after = await Workbench.GetCurrentTrialAsync(cancellation);

        return $"Reverted {kind.ToString().ToLowerInvariant()} {name} to its committed state{Environment.NewLine}{DescribeEvaluation(new(new ChangeSet(), before, after), lineLimit: 5)}";
    }

    /// <summary>The steps made this session, newest first, and what's uncommitted.</summary>
    public string History(int limit = 20)
    {
        var history = Workbench.History;
        var report = new StringBuilder();

        report.AppendLine(history.Count == 0
            ? "No steps this session."
            : $"Steps this session, newest first ({history.Count}; `undo` takes back the top one):");

        foreach (var step in history.AsEnumerable().Reverse().Take(limit))
            report.AppendLine($"  #{step.Number,-4} {step.At:HH:mm:ss}  {step.Description}");

        if (history.Count > limit)
            report.AppendLine($"  … {history.Count - limit} older");

        report.AppendLine();
        report.AppendLine(Workbench.Changes.Count == 0
            ? "The working definition matches the committed grammar."
            : $"Uncommitted changes against the committed grammar: {string.Join(", ", Workbench.Changes.Select(x => $"{x.Name} ({x.Change.ToString().ToLowerInvariant()})"))}");

        return report.ToString();
    }

    // ---- Inspection ----

    /// <summary>Distinct lines matching a regex, most frequent first, as the working grammar tokenizes them.</summary>
    public async Task<string> SearchLinesAsync(string pattern, LineScope scope = LineScope.All, string glyph = null, int limit = 20, bool nested = false, CancellationToken cancellation = default)
    {
        var trial = await GetScoredTrialAsync(cancellation);
        var result = Try(() => CorpusQueries.SearchLines(trial.Documents, pattern, scope, NullIfBlank(glyph), limit, nested));

        var report = new StringBuilder();
        report.AppendLine($"{result.Lines:N0} line{S(result.Lines)} ({result.DistinctLines:N0} distinct){(result.DistinctLines > result.Samples.Count ? $", showing the {result.Samples.Count} most frequent" : "")}:");
        AppendLines(report, result.Samples);
        return report.ToString();
    }

    /// <summary>Where and how a glyph or vocabulary matched: counts, the shapes its matches took, and example lines.</summary>
    public async Task<string> MatchesAsync(string name, int shapes = 15, int lines = 8, CancellationToken cancellation = default)
    {
        var trial = await GetScoredTrialAsync(cancellation);

        if (!trial.Definition.Glyphs.Any(x => x.Name == name) && !trial.Definition.Vocabularies.Any(x => x.Name == name))
            throw new AgentRequestException($"There's no glyph or vocabulary named {name}{Suggest(name, trial.Definition)}");

        var matches = CorpusQueries.GetMatches(trial.Documents, name, shapes, lines);
        var report = new StringBuilder();

        report.AppendLine($"{name}: {matches.TopLevel:N0} top-level and {matches.Nested:N0} nested match{(matches.Nested == 1 ? "" : "es")}, in {matches.Documents:N0} document{S(matches.Documents)}.");

        if (matches.Shapes.Count > 0)
        {
            report.AppendLine("Shapes (frames with captures masked, or a terminal's text), most frequent first:");

            foreach (var shape in matches.Shapes)
                report.AppendLine($"  ×{shape.Occurrences,-5} {shape.Shape}{(shape.Example is null ? "" : $"      e.g. {shape.Example}")}");

            report.AppendLine("Lines:");
            AppendLines(report, matches.Lines);
        }

        return report.ToString();
    }

    /// <summary>The costliest distinct spans of unmatched text.</summary>
    public async Task<string> ResidualsAsync(int limit = 30, int offset = 0, int minWords = 1, CancellationToken cancellation = default)
    {
        var score = (await GetScoredTrialAsync(cancellation)).Score;
        var residuals = score.Residuals.Where(x => x.Words >= minWords).OrderByDescending(x => x.Bits).ToList();

        var report = new StringBuilder();
        report.AppendLine($"{residuals.Count:N0} distinct unmatched spans ({score.ComponentBits[DataComponent.Residual]:N0} bits in all), costliest first{(offset > 0 ? $", from #{offset + 1}" : "")}:");
        AppendResiduals(report, residuals.Skip(offset).Take(limit), score);
        return report.ToString();
    }

    /// <summary>Word runs that recur in unmatched text - the raw material for new glyphs.</summary>
    public async Task<string> ResidualPhrasesAsync(int minWords = 2, int maxWords = 8, int minOccurrences = 3, int limit = 40, CancellationToken cancellation = default)
    {
        var trial = await GetScoredTrialAsync(cancellation);
        var phrases = CorpusQueries.GetResidualPhrases(trial.Documents, minWords, maxWords, minOccurrences, limit);

        var report = new StringBuilder();
        report.AppendLine($"Recurring phrases in unmatched text ({minWords}-{maxWords} words, at least {minOccurrences}×), by words they'd account for:");
        report.AppendLine($"  {"count",6} {"docs",5}  phrase");

        foreach (var phrase in phrases)
            report.AppendLine($"  {phrase.Occurrences,6:N0} {phrase.Documents,5:N0}  {phrase.Text}");

        if (phrases.Count == 0)
            report.AppendLine("  (none)");

        return report.ToString();
    }

    // ---- Probing ----

    /// <summary>How text tokenizes - under the working grammar, or with a draft change set applied on top of it.</summary>
    public async Task<string> TokenizeAsync(string text, string source = null, string remove = null, CancellationToken cancellation = default)
    {
        var grammar = await GetGrammarAsync(source, remove, cancellation);
        return string.Join(Environment.NewLine, CorpusQueries.Tokenize(grammar, text));
    }

    /// <summary>How far one glyph's pattern gets through a piece of text on its own, and which part stops it.</summary>
    public async Task<string> ExplainMismatchAsync(string glyph, string text, string source = null, string remove = null, CancellationToken cancellation = default)
    {
        var grammar = await GetGrammarAsync(source, remove, cancellation);
        var explanation = Try(() => CorpusQueries.ExplainMismatch(grammar, glyph, text));

        var report = new StringBuilder();

        if (explanation.IsFullMatch)
        {
            report.AppendLine($"{glyph}'s pattern matches {(explanation.MatchedWords == explanation.TotalWords ? "all of the text" : $"the first {explanation.MatchedWords} of {explanation.TotalWords} words: \"{explanation.MatchedText}\"")}.");

            if (explanation.MatchedWords < explanation.TotalWords)
                report.AppendLine("A top-level glyph must match a whole clause, so unless it's nested (or allows partial matches), the rest of the clause stops it.");
        }
        else
        {
            report.AppendLine($"{glyph}'s pattern matches {explanation.MatchedWords} of {explanation.TotalWords} words, then fails.");
            report.AppendLine($"  matched: \"{explanation.MatchedText}\"");
            report.AppendLine($"  first failing part: {explanation.FirstFailure}");
        }

        report.AppendLine($"The whole grammar tokenizes it as: {explanation.Tokenization}");
        return report.ToString();
    }

    // ---- Helpers ----

    ChangeSet ReadChanges(string source, string remove)
    {
        var changes = Try(() => ChangeSet.FromSource(source, SplitNames(remove), Workbench.WorkingDefinition));

        if (changes.IsEmpty)
            throw new AgentRequestException("The change set is empty: pass C# declarations as source, names to remove, or both.");

        return changes;
    }

    async Task<Evaluation> Evaluate(ChangeSet changes, CancellationToken cancellation)
    {
        var evaluation = await TryAsync(() => Workbench.EvaluateAsync(changes, cancellation));

        if (!evaluation.Before.Succeeded)
            throw new AgentRequestException("The working definition doesn't build, so there's nothing to compare against. Fix it or undo the breaking step first:"
                + Environment.NewLine + string.Join(Environment.NewLine, evaluation.Before.Errors.Select(x => "  " + x)));

        return evaluation;
    }

    static string DescribeEvaluation(Evaluation evaluation, int lineLimit = 8)
    {
        var report = new StringBuilder();

        if (!evaluation.After.Succeeded)
        {
            report.AppendLine("The grammar doesn't build:");
            AppendErrors(report, evaluation.After.Errors);
            return report.ToString();
        }

        if (!evaluation.Before.Succeeded)
        {
            report.AppendLine($"Now builds again: {evaluation.After.Score.TotalBits:N0} bits, coverage {evaluation.After.Score.Coverage:P2}.");
            return report.ToString();
        }

        report.Append(new ScoreComparison(evaluation.Before.Score, evaluation.After.Score).ToReport());

        var diff = CorpusQueries.CompareTokenizations(evaluation.Before.Documents, evaluation.After.Documents, lineLimit);

        report.AppendLine($"Lines: {diff.GainedLines:N0} gained, {diff.LostLines:N0} lost, {diff.ReshapedLines:N0} reshaped, of {diff.Lines:N0}.");
        AppendChanges(report, "Gained (more words covered)", diff.Gained, diff.GainedLines);
        AppendChanges(report, "Lost (fewer words covered)", diff.Lost, diff.LostLines);
        AppendChanges(report, "Reshaped (same coverage, different parse)", diff.Reshaped, diff.ReshapedLines);

        return report.ToString();
    }

    static void AppendChanges(StringBuilder report, string heading, IReadOnlyList<LineChange> changes, int total)
    {
        if (changes.Count == 0)
            return;

        report.AppendLine($"{heading}:");

        foreach (var change in changes)
        {
            report.AppendLine($"  ×{change.Occurrences,-4} {change.Before}");
            report.AppendLine($"     → {change.After}{(change.CapturedWordsDelta != 0 ? $"   ({(change.CapturedWordsDelta > 0 ? "+" : "")}{change.CapturedWordsDelta} words)" : "")}");
        }

        var shown = changes.Sum(x => x.Occurrences);

        if (total > shown)
            report.AppendLine($"  … {total - shown:N0} more line{S(total - shown)}");
    }

    static void AppendLines(StringBuilder report, IEnumerable<LineSample> lines)
    {
        foreach (var line in lines)
            report.AppendLine($"  ×{line.Occurrences,-4} {line.Rendering}   [{line.Document}]");
    }

    static void AppendResiduals(StringBuilder report, IEnumerable<ResidualSpan> residuals, MdlScore score)
    {
        report.AppendLine($"  {"bits",8} {"count",6} {"words",6}  text");

        foreach (var span in residuals)
            report.AppendLine($"  {span.Bits,8:N0} {span.Occurrences,6:N0} {span.Words,6:N0}  {Truncate(span.Text, 160)}");
    }

    static void AppendErrors(StringBuilder report, IEnumerable<string> errors)
    {
        foreach (var error in errors)
            report.AppendLine("  " + error);
    }

    async Task<WorkingScore> GetScoredTrialAsync(CancellationToken cancellation)
    {
        var trial = await Workbench.GetCurrentTrialAsync(cancellation);

        if (!trial.Succeeded)
            throw new AgentRequestException("The working definition doesn't build, so the corpus can't be inspected. Fix it (`apply`) or `undo` the breaking step:"
                + Environment.NewLine + string.Join(Environment.NewLine, trial.Errors.Select(x => "  " + x)));

        return trial;
    }

    async Task<GlyphGrammar> GetGrammarAsync(string source, string remove, CancellationToken cancellation)
    {
        if (string.IsNullOrWhiteSpace(source) && string.IsNullOrWhiteSpace(remove))
            return (await GetScoredTrialAsync(cancellation)).Grammar;

        var definition = Try(() => ReadChanges(source, remove).ApplyTo(Workbench.WorkingDefinition));

        try
        {
            return Workbench.BuildGrammar(definition);
        }
        // Any failure to build is the draft's, as scoring it would report (see GrammarWorkbench.ScoreUncached) - so
        // it reaches the agent as a message, never as an exception the tool host would hide.
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            throw new AgentRequestException("The draft doesn't build:" + Environment.NewLine
                + string.Join(Environment.NewLine, exception.Message.Split('\n').Select(x => x.Trim()).Where(x => x.Length > 0).Select(x => "  " + x)), exception);
        }
    }

    /// <summary>Runs <paramref name="action"/>, turning the exceptions a bad request causes into <see cref="AgentRequestException"/>s.</summary>
    static T Try<T>(Func<T> action)
    {
        try
        {
            return action();
        }
        catch (Exception exception) when (IsBadRequest(exception))
        {
            throw new AgentRequestException(exception.Message, exception);
        }
    }

    static void Try(Action action) => Try(() => { action(); return 0; });

    static async Task<T> TryAsync<T>(Func<Task<T>> action)
    {
        try
        {
            return await action();
        }
        catch (Exception exception) when (IsBadRequest(exception))
        {
            throw new AgentRequestException(exception.Message, exception);
        }
    }

    static bool IsBadRequest(Exception exception) =>
        exception is GlyphSourceException or InvalidOperationException or ArgumentException or RegexMatchTimeoutException;

    static IEnumerable<string> SplitNames(string names) =>
        (names ?? "").Split([',', ' ', '\n', '\r', '\t', ';'], StringSplitOptions.RemoveEmptyEntries).Distinct();

    static string Suggest(string name, GrammarDefinition definition)
    {
        var similar = definition.Glyphs.Select(x => x.Name).Concat(definition.Vocabularies.Select(x => x.Name))
            .Where(x => x.Contains(name, StringComparison.OrdinalIgnoreCase) || name.Contains(x, StringComparison.OrdinalIgnoreCase))
            .Take(5)
            .ToList();

        return similar.Count > 0 ? $" - did you mean {string.Join(", ", similar)}?" : "";
    }

    static string NullIfBlank(string text) => string.IsNullOrWhiteSpace(text) ? null : text.Trim();

    static string S(int count) => count == 1 ? "" : "s";

    static string S(double count) => count == 1 ? "" : "s";

    static string Truncate(string text, int length) => text.Length <= length ? text : text[..(length - 1)] + "…";
}
