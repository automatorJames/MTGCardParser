using System.Text.Json;
using Glyphotype.Distiller.Agent;
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

        When the person asks a question rather than for more steps, answer it - using the tools to look things up where that helps - without changing the grammar.
        """;

    string _instructions;

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
    }

    /// <summary>A round lasts until the agent stops, or is stopped.</summary>
    protected override void OnTurnEnded() => Round = null;

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
    /// improvements or is stopped. The session setting itself, so it holds for an agent in a terminal too.
    /// </summary>
    public int? MaxSteps
    {
        get => agent.Settings.StepsBeforeCheckIn > 0 ? agent.Settings.StepsBeforeCheckIn : null;
        set
        {
            agent.Settings = agent.Settings with { StepsBeforeCheckIn = Math.Max(0, value ?? 0) };
            NotifyChanged();
        }
    }

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
        var said = $"AI round {round}: {(steps is int limit ? $"up to {limit} step{(limit == 1 ? "" : "s")}" : "no step limit")}{(instructions is null ? "" : $" - {instructions}")}";

        var message = HasRound
            ? $"Continue with another round: call `start_session` again with instructions \"{instructions ?? _instructions}\", then follow the brief it returns."
            : GrammarAgentPrompts.Instruction(instructions);

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
