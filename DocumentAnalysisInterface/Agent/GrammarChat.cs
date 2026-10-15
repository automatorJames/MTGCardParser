using System.Text.Json;
using System.Text.RegularExpressions;
using Glyphotype.Distiller.Agent;
using Glyphotype.Distiller.Workbench;
using Glyphotype.Distiller.Workspaces;
using Microsoft.AspNetCore.Hosting.Server;

namespace DocumentAnalysisInterface.Agent;

/// <summary>
/// The Grammar Tools tab's chat with the local agent - running a round here is what <c>/grammar</c> is in a terminal.
/// One conversation for the whole app.
/// </summary>
public sealed class GrammarChat(LocalAgent localAgent, GrammarAgent agent, WorkspaceManager workspaces, IServer server, string workingDirectory)
    : AgentChat(localAgent, server, workingDirectory)
{
    const string _systemPrompt = """
        You are the grammar agent built into the Grammar Tools page of an app for composing a grammar over a text corpus. You work only through the glyphotype tools, on a working definition the person watches change live beside this chat: every step you apply appears on their page as it lands.

        Your replies are shown as Markdown in a narrow chat pane, and each tool call is shown to the person as a line they can open to read its report. So keep replies short: before each step, one line on what it targets; at a check-in, one line per step with its bit and coverage change, then what you'd try next. Don't repeat the tools' reports.

        Every request rereads the whole conversation, so make fewer of them: when you need several lookups that don't depend on each other (`show`, `search_lines`, `matches`, `tokenize`), call them together in one request rather than one at a time.

        When the person asks a question rather than for more steps, answer it - using the tools to look things up where that helps - without changing the grammar.
        """;

    string _instructions;

    /// <summary>Whether the agent has been told to hand off to a fresh session (see <see cref="HandoffThousands"/>) in this turn.</summary>
    bool _handingOff;

    /// <summary>How many times in this round the agent has been told to go on after stopping with no check-in due.</summary>
    int _nudges;

    /// <summary>How many times a round tells an agent that stopped early to go on, before taking the stop as meant.</summary>
    const int _maxNudges = 2;

    int? _handoffThousands;

    /// <summary>Where a round hands off by default, in thousands of tokens of context - see <see cref="HandoffThousands"/>.</summary>
    const int _defaultHandoffThousands = 60;

    /// <summary>The options set for a conversation to come, by the folder of the workspace they were set in.</summary>
    readonly Dictionary<string, ChatOptions> _chosen = [];

    protected override string SystemPrompt => _systemPrompt;

    protected override string SystemPromptFileName => "system-prompt.md";

    /// <summary>Whether a round has been run in this conversation - so the next is a continuation.</summary>
    public bool HasRound { get; private set; }

    /// <summary>The number of the AI round under way - the round its steps are grouped under in the working changes - or null when none is.</summary>
    public int? Round { get; private set; }

    /// <summary>Before the conversation begins: what the active workspace's last conversation ran on - or the settings' model and effort, where there hasn't been one.</summary>
    protected override ChatOptions ChosenOptions
    {
        get
        {
            var folder = workspaces.ActiveWorkspace.Folder;

            if (!_chosen.TryGetValue(folder, out var options))
                _chosen[folder] = options = ReadLastUsed().GetValueOrDefault(folder) ?? DefaultOptions;

            return options;
        }
        set => _chosen[workspaces.ActiveWorkspace.Folder] = value;
    }

    /// <summary>What it runs on is remembered for the workspace's next conversation.</summary>
    protected override void OnBegun(ChatOptions options) => SaveLastUsed(workspaces.ActiveWorkspace.Folder, options);

    protected override void OnReset()
    {
        _instructions = null;
        HasRound = false;
        _nudges = 0;
        (_workedBeforeTurn, _workedAtStep, _tokensAtStep, _weeklyStart) = (TimeSpan.Zero, TimeSpan.Zero, 0, null);
    }

    // ---- What each step took ----

    /// <summary>How long the agent has worked in this conversation, in the turns before the one under way - and when that one began.</summary>
    TimeSpan _workedBeforeTurn;
    DateTimeOffset? _turnBegan;

    /// <summary>How long the agent had worked, and how many tokens the conversation had used, when its last step was applied.</summary>
    TimeSpan _workedAtStep;
    long _tokensAtStep;

    /// <summary>The conversation's tokens, and the weekly allowance's use, when the allowance was first heard of - what <see cref="EstimateCost"/> measures from.</summary>
    (long Tokens, double Utilization)? _weeklyStart;

    /// <summary>What a week's allowance is reckoned to be worth, for <see cref="EstimateCost"/>: a rough figure, for a sense of scale.</summary>
    const double _weeklyAllowanceUsd = 20;

    /// <summary>The share of the weekly allowance that must have been used before a rate is measured from it - the allowance is reported coarsely.</summary>
    const double _minimumMeasuredUtilization = 0.01;

    double? _dollarsPerMillionTokens;

    /// <summary>What a million tokens cost, as last measured (see <see cref="EstimateCost"/>) - kept with the chat's settings, so a step has an estimate before this conversation has used enough to measure one.</summary>
    double? DollarsPerMillionTokens
    {
        get => _dollarsPerMillionTokens ??= ReadSettings()?.DollarsPerMillionTokens;
        set
        {
            _dollarsPerMillionTokens = value;
            SaveSettings();
        }
    }

    TimeSpan Worked => _workedBeforeTurn + (_turnBegan is DateTimeOffset began ? DateTimeOffset.Now - began : TimeSpan.Zero);

    /// <summary>A turn's time counts toward the steps applied in it - the time between turns doesn't.</summary>
    protected override IDisposable BeginTurn()
    {
        lock (_gate)
            _turnBegan = DateTimeOffset.Now;

        return new TurnEnd(() =>
        {
            lock (_gate)
            {
                _workedBeforeTurn = Worked;
                _turnBegan = null;
            }
        });
    }

    sealed class TurnEnd(Action end) : IDisposable
    {
        Action _end = end;

        public void Dispose() => Interlocked.Exchange(ref _end, null)?.Invoke();
    }

    /// <summary>
    /// Where <paramref name="call"/> applied a step: puts a rule under it in the conversation, with what the step took -
    /// the time worked and tokens used since the step before (or the conversation's start), and their estimated cost -
    /// and what it did to the score, and records what it took on the step itself, for the working changes to show.
    /// </summary>
    void RecordStep(ChatEntry call)
    {
        if (call is not { Tool: "apply", Failed: false, Result: { } result } || _appliedStep.Match(result) is not { Success: true } match)
            return;

        var number = int.Parse(match.Groups[1].Value);
        var (worked, tokens) = (Worked, Stats.TotalTokens);
        var effort = new StepEffort(worked - _workedAtStep, tokens - _tokensAtStep, EstimateCost(tokens - _tokensAtStep));
        (_workedAtStep, _tokensAtStep) = (worked, tokens);

        var workbench = workspaces.Active;
        var step = workbench.History.LastOrDefault(x => x.Number == number);
        var figures = StepFigures.Of((step ?? new(number, null, null, null, DateTimeOffset.Now)) with { Effort = effort });

        AddEntry(new(ChatEntryKind.Step, figures.Line()) { StepNumber = number, StepDescription = step?.Description });

        // Off the gate: the workbench tells the page, which reads the chat too.
        _ = Task.Run(() => workbench.SetStepEffort(number, effort));
    }

    static readonly Regex _appliedStep = new(@"^\s*Applied as step (\d+)", RegexOptions.Compiled);

    /// <summary>
    /// What <paramref name="tokens"/> cost, reckoned naively as the share of the weekly allowance they used, of what the
    /// allowance is worth (<see cref="_weeklyAllowanceUsd"/>). The share isn't reported by the token, only for the
    /// allowance as a whole, so a rate is measured from the conversation's tokens against how far the allowance's use
    /// has moved while it ran - counting whatever else used the allowance meanwhile. Null until a rate is known.
    /// </summary>
    double? EstimateCost(long tokens)
    {
        if (Stats.SevenDay is { } weekly)
        {
            var total = Stats.TotalTokens;

            // A use lower than at the start is the allowance having started over.
            if (_weeklyStart is not { } start || weekly.Utilization < start.Utilization)
                _weeklyStart = (total, weekly.Utilization);
            else if (weekly.Utilization - start.Utilization >= _minimumMeasuredUtilization && total > start.Tokens)
                DollarsPerMillionTokens = _weeklyAllowanceUsd * (weekly.Utilization - start.Utilization) / (total - start.Tokens) * 1_000_000;
        }

        return DollarsPerMillionTokens is double rate ? tokens * rate / 1_000_000 : null;
    }

    /// <summary>
    /// How many tokens of context, in thousands, a round may build up before it hands off to a fresh session: the agent
    /// puts what the next one needs in the workspace's journal, and the round goes on from there. A count rather than a
    /// share of the model's window, since what it governs is the cost: every request reads the whole context again, so a
    /// round's cost grows with the square of its length, whatever the model could hold. Kept with the app's agent settings.
    /// </summary>
    public int HandoffThousands
    {
        get
        {
            lock (_gate)
                return _handoffThousands ??= ReadSettings()?.HandoffThousands ?? _defaultHandoffThousands;
        }
        set
        {
            lock (_gate)
            {
                _handoffThousands = Math.Clamp(value, 20, 900);
                SaveSettings();
            }

            NotifyChanged();
        }
    }

    /// <summary>
    /// Once a call is answered: a step it applied closes off the work that led to it (see <see cref="RecordStep"/>), and
    /// in a round whose context has passed <see cref="HandoffThousands"/>, the agent is told to hand off.
    /// </summary>
    protected override void OnToolAnswered(ChatEntry call)
    {
        RecordStep(call);

        if (Round is null || _handingOff || agent.IsHandoffDue)
            return;

        var tokens = Stats.ContextTokens;

        if (tokens < HandoffThousands * 1000L)
            return;

        _handingOff = true;
        agent.RequestHandoff();
        AddNote($"Context at {tokens / 1000:N0}K tokens, past the {HandoffThousands:N0}K handoff point: the agent is writing what the next session needs into the journal.");
    }

    /// <summary>
    /// A round lasts until a check-in is due, or the agent is stopped - unless it stopped to hand off, when it goes on in
    /// a fresh session. An agent that stops on its own before a check-in is due is told to go on, up to
    /// <see cref="_maxNudges"/> times a round: agents tend to stop at text that's expensive to cover, which the session
    /// says to journal and move past.
    /// </summary>
    protected override void OnTurnEnded(bool stopped)
    {
        var handingOff = _handingOff;
        _handingOff = false;

        if (handingOff && !stopped && Round is int round && !agent.IsCheckInDue)
        {
            StartFreshSession();
            _ = Task.Run(() => ContinueRound(round));
            return;
        }

        // An agent asked to stop after its step that stops before it is stopping as asked.
        if (!stopped && Round is int unfinished && !agent.IsCheckInDue && !agent.IsHandoffDue && !agent.IsStopAfterStepRequested && _nudges < _maxNudges && !EndedInError)
        {
            _nudges++;
            _ = Task.Run(() => Nudge(unfinished));
            return;
        }

        Round = null;
    }

    /// <summary>Whether the turn just ended with an error - when going on would only meet it again.</summary>
    bool EndedInError => Entries is [.., { Kind: ChatEntryKind.Error }];

    /// <summary>Tells the agent, which stopped with no check-in due, to go on with AI round <paramref name="round"/>.</summary>
    void Nudge(int round)
    {
        var steps = agent.Settings.StepsBeforeCheckIn;
        var taken = agent.StepsSinceCheckIn;
        var progress = steps > 0 ? $"{taken} of the round's {steps} steps are applied" : "this round has no step limit";
        var message = $"No check-in is due yet - {progress}, so go on with AI round {round}. " +
            "Text that's expensive to cover isn't a reason to stop: evaluate the best form you can find, apply it if the step rules allow, " +
            "and if they don't, journal it as an open problem and move on to the next target. Stop when the tools say a check-in is due.";

        if (!Start($"The agent stopped with no check-in due ({progress}): told it to go on.", message, ChatEntryKind.Note))
        {
            lock (_gate)
                Round = null;
        }

        NotifyChanged();
    }

    /// <summary>Starts AI round <paramref name="round"/> again in the fresh session a handoff left, where the last session left it.</summary>
    void ContinueRound(int round)
    {
        agent.ContinueRoundInNextSession();

        var message = $"Continue AI round {round} - the session before this one handed off when its context filled up: call `start_session` with instructions \"{_instructions}\", then follow the brief it returns.";

        if (Start($"Handed off to a fresh session: AI round {round} goes on from the journal.", message, ChatEntryKind.Note))
        {
            lock (_gate)
            {
                if (IsRunning)
                    Round = round;
            }
        }
        else
        {
            lock (_gate)
                Round = null;
        }

        NotifyChanged();
    }

    /// <summary>
    /// The chat's settings as kept between runs of the app. The session's two are null where they were never set here,
    /// which leaves the app's configured ones; 0 is "none", as in <see cref="AgentSessionSettings"/>.
    /// </summary>
    sealed record Settings(int? HandoffThousands = null, int? StepsBeforeCheckIn = null, AgentFocus? Focus = null, bool StopsAfterStep = false, double? DollarsPerMillionTokens = null);

    const string _settingsFileName = "chat-settings.json";

    string SettingsPath => Path.Combine(WorkingDirectory, _settingsFileName);

    /// <summary>The session settings last set here, put back on <paramref name="grammarAgent"/> as the chat is made - so they hold from the app's start, for an agent in a terminal too.</summary>
    readonly bool _sessionSettingsRestored = RestoreSessionSettings(grammarAgent: agent, Path.Combine(workingDirectory, _settingsFileName));

    static bool RestoreSessionSettings(GrammarAgent grammarAgent, string path)
    {
        if (ReadSettings(path) is not { } saved)
            return false;

        grammarAgent.Settings = grammarAgent.Settings with
        {
            StepsBeforeCheckIn = saved.StepsBeforeCheckIn ?? grammarAgent.Settings.StepsBeforeCheckIn,
            Focus = saved.Focus ?? grammarAgent.Settings.Focus,
        };

        return true;
    }

    Settings ReadSettings() => ReadSettings(SettingsPath);

    static Settings ReadSettings(string path)
    {
        try
        {
            return File.Exists(path) ? JsonSerializer.Deserialize<Settings>(File.ReadAllText(path)) : null;
        }
        catch (Exception exception) when (exception is IOException or JsonException or UnauthorizedAccessException)
        {
            return null;
        }
    }

    /// <summary>Keeps the chat's settings as they stand now, for the app's next run.</summary>
    void SaveSettings() => SaveSettings(new(HandoffThousands, agent.Settings.StepsBeforeCheckIn, agent.Settings.Focus, StopsAfterStep, DollarsPerMillionTokens));

    void SaveSettings(Settings settings)
    {
        try
        {
            Directory.CreateDirectory(WorkingDirectory);
            File.WriteAllText(SettingsPath, JsonSerializer.Serialize(settings));
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            // Remembering is a convenience: the setting holds for this session either way.
        }
    }

    string LastUsedPath => Path.Combine(WorkingDirectory, "last-used.json");

    /// <summary>What each workspace's last conversation ran on, by the workspace's folder - nothing, where that can't be read.</summary>
    Dictionary<string, ChatOptions> ReadLastUsed()
    {
        try
        {
            return File.Exists(LastUsedPath) ? JsonSerializer.Deserialize<Dictionary<string, ChatOptions>>(File.ReadAllText(LastUsedPath)) ?? [] : [];
        }
        catch (Exception exception) when (exception is IOException or JsonException or UnauthorizedAccessException)
        {
            return [];
        }
    }

    void SaveLastUsed(string folder, ChatOptions options)
    {
        try
        {
            var lastUsed = ReadLastUsed();
            lastUsed[folder] = options;

            Directory.CreateDirectory(WorkingDirectory);
            File.WriteAllText(LastUsedPath, JsonSerializer.Serialize(lastUsed));
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            // Remembering is a convenience: the conversation runs on what was chosen either way.
        }
    }

    /// <summary>
    /// Steps the agent applies in a round before it stops - null for no limit, when it works until it runs out of
    /// improvements or is stopped. The session setting itself, so it holds for an agent in a terminal too - and kept for the app's next run.
    /// </summary>
    public int? MaxSteps
    {
        get => agent.Settings.StepsBeforeCheckIn > 0 ? agent.Settings.StepsBeforeCheckIn : null;
        set
        {
            agent.Settings = agent.Settings with { StepsBeforeCheckIn = Math.Max(0, value ?? 0) };
            SaveSettings();
            NotifyChanged();
        }
    }

    /// <summary>
    /// What the agent weighs most in choosing targets and steps, as its session brief tells it - read when a session
    /// starts, so a change holds from the next round (or handoff) on. The session setting itself, as <see cref="MaxSteps"/> is.
    /// </summary>
    public AgentFocus Focus
    {
        get => agent.Settings.Focus;
        set
        {
            agent.Settings = agent.Settings with { Focus = value };
            SaveSettings();
            NotifyChanged();
        }
    }

    bool? _stopsAfterStep;

    /// <summary>Whether the stop button last used stops after the agent's next step rather than at once - what it offers first. Kept with the chat's settings.</summary>
    public bool StopsAfterStep
    {
        get
        {
            lock (_gate)
                return _stopsAfterStep ??= ReadSettings()?.StopsAfterStep ?? false;
        }
        private set
        {
            lock (_gate)
            {
                if (_stopsAfterStep == value)
                    return;

                _stopsAfterStep = value;
                SaveSettings();
            }
        }
    }

    /// <summary>Stops the agent where it is, as <see cref="AgentChat.Stop"/> does - and remembers that's the stop used.</summary>
    public void StopNow()
    {
        StopsAfterStep = false;
        Stop();
        NotifyChanged();
    }

    /// <summary>
    /// Lets the agent finish the step it's working toward, then has it check in as at the end of its steps - so the round
    /// ends with the step applied and summarized. Remembers that's the stop used.
    /// </summary>
    public void StopAfterStep()
    {
        StopsAfterStep = true;

        if (!IsRunning || agent.IsStopAfterStepRequested)
            return;

        agent.StopAfterStep();

        lock (_gate)
            AddNote("The agent will stop once it has applied the step it's working on.");

        NotifyChanged();
    }

    /// <summary>A focus as the pane names it: <see cref="AgentFocus.HotSpots"/> as "Hot spots".</summary>
    public static string FocusName(AgentFocus focus) => focus == AgentFocus.HotSpots ? "Hot spots" : focus.ToString();

    /// <summary>Whether the agent is working toward a step it's to stop after (see <see cref="StopAfterStep"/>).</summary>
    public bool IsStoppingAfterStep => IsRunning && agent.IsStopAfterStepRequested && !agent.IsCheckInDue;

    /// <summary>
    /// Starts an AI round: the agent works until it has applied <see cref="MaxSteps"/>, runs out of improvements, or is
    /// stopped. The first starts a session as <c>/grammar</c> does, later ones continue it.
    /// </summary>
    /// <param name="instructions">What to work on, in the person's words - when blank, a later round keeps the instructions of the one before.</param>
    public void StartRound(string instructions)
    {
        instructions = string.IsNullOrWhiteSpace(instructions) ? null : instructions.Trim();

        // The agent's first step in the round is numbered after the last round the workbench's history holds (see GrammarAgent.ApplyAsync).
        var round = (workspaces.Active.History.Max(x => x.Round) ?? 0) + 1;
        var steps = MaxSteps;
        var focus = Focus == AgentFocus.Default ? "" : $", {FocusName(Focus)} focus";
        var said = $"AI round {round}: {(steps is int limit ? $"up to {limit} step{(limit == 1 ? "" : "s")}" : "no step limit")}{focus}{(instructions is null ? "" : $" - {instructions}")}";

        var message = HasRound
            ? $"Continue with another round: call `start_session` again with instructions \"{instructions ?? _instructions}\", then follow the brief it returns."
            : GrammarAgentPrompts.Instruction(instructions);

        _nudges = 0;

        if (Start(said, message))
        {
            lock (_gate)
            {
                // Unless the agent was quick enough to finish already.
                if (IsRunning)
                    Round = round;
            }

            _instructions = instructions ?? _instructions;
            HasRound = true;
            NotifyChanged();
        }
    }
}
