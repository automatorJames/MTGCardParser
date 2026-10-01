using System.Text.Json;
using Glyphotype.Distiller.Agent;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace DocumentAnalysisInterface.Agent;

public enum ChatEntryKind
{
    Person,
    Agent,
    Tool,
    Error,
}

/// <summary>One thing in the chat: something the person or the agent said, a tool the agent called, or what went wrong.</summary>
/// <param name="Text">What was said - the agent's as Markdown. For a tool, a few words on what the call was for.</param>
/// <param name="Streaming">An agent's entry still being added to.</param>
public sealed record ChatEntry(ChatEntryKind Kind, string Text, bool Streaming = false)
{
    public string ToolUseId { get; init; }
    public string Tool { get; init; }

    /// <summary>The C# a tool call passed, if any.</summary>
    public string Source { get; init; }

    /// <summary>What the tool answered - null until it has.</summary>
    public string Result { get; init; }

    public bool Failed { get; init; }
}

/// <summary>
/// The Grammar Tools tab's chat with a local agent (see <see cref="LocalAgent"/>), which works through the same tools
/// this app serves at <c>/mcp</c> - so running a round here is what <c>/grammar</c> is in a terminal. One conversation
/// for the whole app, running apart from any browser tab: every tab shows it, and closing one doesn't stop it.
/// <see cref="Changed"/> fires, from whatever thread, whenever anything a tab shows changes.
/// </summary>
public sealed class GrammarChat(LocalAgent localAgent, GrammarAgent agent, IServer server, string workingDirectory)
{
    const string _mcpServerName = "glyphotype";

    const string _systemPrompt = """
        You are the grammar agent built into the Grammar Tools page of an app for composing a grammar over a text corpus. You work only through the glyphotype tools, on a working definition the person watches change live beside this chat: every step you apply appears on their page as it lands.

        Your replies are shown as Markdown in a narrow chat pane, and each tool call is shown to the person as a line they can open to read its report. So keep replies short: before each step, one line on what it targets; at a check-in, one line per step with its bit and coverage change, then what you'd try next. Don't repeat the tools' reports.

        When the person asks a question rather than for more steps, answer it - using the tools to look things up where that helps - without changing the grammar.
        """;

    readonly object _gate = new();
    readonly List<ChatEntry> _entries = [];

    LocalAgentSession _session;
    CancellationTokenSource _turn;
    string _instructions;
    string _model = NullIfBlank(localAgent.Settings.Model);
    string _effort = NullIfBlank(localAgent.Settings.Effort);

    public event Action Changed;

    /// <summary>Whether a local agent is there to chat with - checked afresh at most once a minute.</summary>
    public Task<LocalAgentStatus> GetStatusAsync() => localAgent.GetStatusAsync();

    public IReadOnlyList<ChatEntry> Entries
    {
        get
        {
            lock (_gate)
                return _entries.ToList();
        }
    }

    /// <summary>Whether the agent is working on a message.</summary>
    public bool IsRunning => _turn is not null;

    /// <summary>Whether a round has been run in this conversation - so the next is a continuation.</summary>
    public bool HasRound { get; private set; }

    /// <summary>The model the conversation runs on, as the CLI names it (an alias like "sonnet", or a full id) - null for the CLI's own default. Settable until the conversation begins, and settled from then until <see cref="Reset"/>.</summary>
    public string Model
    {
        get => _model;
        set => SetOption(ref _model, value);
    }

    /// <summary>The effort level the conversation runs at, as the CLI names it - null for the CLI's own default. Settable until the conversation begins, as <see cref="Model"/> is.</summary>
    public string Effort
    {
        get => _effort;
        set => SetOption(ref _effort, value);
    }

    void SetOption(ref string option, string value)
    {
        lock (_gate)
        {
            if (_entries.Count > 0)
                return;

            option = NullIfBlank(value);
        }

        Changed?.Invoke();
    }

    static string NullIfBlank(string value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    /// <summary>Steps the agent applies in a round before it checks in (0 for no limit) - the session setting itself, so it holds for an agent in a terminal too.</summary>
    public int StepsPerRound
    {
        get => agent.Settings.StepsBeforeCheckIn;
        set
        {
            agent.Settings = agent.Settings with { StepsBeforeCheckIn = Math.Max(0, value) };
            Changed?.Invoke();
        }
    }

    /// <summary>Has the agent work a round of steps: the first starts a session as <c>/grammar</c> does, later ones continue it.</summary>
    /// <param name="instructions">What to work on, in the person's words - when blank, a later round keeps the instructions of the one before.</param>
    public void RunRound(string instructions)
    {
        instructions = string.IsNullOrWhiteSpace(instructions) ? null : instructions.Trim();
        var steps = StepsPerRound;
        var said = $"{(HasRound ? "Next round" : "Start")}: {(steps > 0 ? $"{steps} step{(steps == 1 ? "" : "s")}" : "no step limit")}{(instructions is null ? "" : $" - {instructions}")}";

        var message = HasRound
            ? $"Continue with another round: call `start_session` again with instructions \"{instructions ?? _instructions}\", then follow the brief it returns."
            : GrammarAgentPrompts.Instruction(instructions);

        if (Start(said, message))
        {
            _instructions = instructions ?? _instructions;
            HasRound = true;
        }
    }

    /// <summary>Says something to the agent outside of a round: a question, or a direction for it to take.</summary>
    public void Send(string message)
    {
        if (!string.IsNullOrWhiteSpace(message))
            Start(message.Trim(), message.Trim());
    }

    /// <summary>Stops the agent where it is. Steps it has applied stay applied.</summary>
    public void Stop() => _turn?.Cancel();

    /// <summary>Starts a new conversation, forgetting this one.</summary>
    public void Reset()
    {
        lock (_gate)
        {
            if (_turn is not null)
                return;

            _entries.Clear();
            _session = null;
            _instructions = null;
            HasRound = false;
        }

        Changed?.Invoke();
    }

    bool Start(string said, string message)
    {
        CancellationTokenSource turn;

        lock (_gate)
        {
            if (_turn is not null)
                return false;

            _turn = turn = new();
            _entries.Add(new(ChatEntryKind.Person, said));
        }

        Changed?.Invoke();
        _ = Task.Run(() => RunTurnAsync(message, turn));
        return true;
    }

    async Task RunTurnAsync(string message, CancellationTokenSource turn)
    {
        try
        {
            _session ??= CreateSession();

            await foreach (var agentEvent in _session.SendAsync(message, turn.Token))
            {
                lock (_gate)
                    Record(agentEvent);

                Changed?.Invoke();
            }
        }
        catch (Exception exception)
        {
            lock (_gate)
                _entries.Add(new(ChatEntryKind.Error, exception.Message));
        }
        finally
        {
            lock (_gate)
            {
                EndStreaming();

                // A call the agent was stopped in the middle of never gets its answer.
                for (int i = 0; i < _entries.Count; i++)
                    if (_entries[i] is { Kind: ChatEntryKind.Tool, Result: null } call)
                        _entries[i] = call with { Result = "", Failed = true };

                if (turn.IsCancellationRequested)
                    _entries.Add(new(ChatEntryKind.Error, "Stopped."));

                _turn = null;
            }

            turn.Dispose();
            Changed?.Invoke();
        }
    }

    void Record(LocalAgentEvent agentEvent)
    {
        switch (agentEvent)
        {
            case AgentText text when !text.StartsMessage && _entries is [.., { Kind: ChatEntryKind.Agent, Streaming: true } last]:
                _entries[^1] = last with { Text = last.Text + text.Text };
                break;

            case AgentText text:
                EndStreaming();
                _entries.Add(new(ChatEntryKind.Agent, text.Text, Streaming: true));
                break;

            case AgentToolCall call:
                EndStreaming();
                _entries.Add(new(ChatEntryKind.Tool, Summarize(call.Arguments)) { ToolUseId = call.ToolUseId, Tool = call.Tool, Source = GetString(call.Arguments, "source") });
                break;

            case AgentToolResult result when _entries.FindIndex(x => x.ToolUseId == result.ToolUseId) is int index and >= 0:
                _entries[index] = _entries[index] with { Result = result.Text ?? "", Failed = result.Failed };
                break;

            case AgentTurnFailed failed:
                EndStreaming();
                _entries.Add(new(ChatEntryKind.Error, failed.Message));
                break;
        }
    }

    void EndStreaming()
    {
        if (_entries is [.., { Streaming: true } last])
            _entries[^1] = last with { Streaming = false };
    }

    /// <summary>A few words on what a tool call is for: the argument that says the most about it.</summary>
    static string Summarize(JsonElement arguments)
    {
        string[] telling = ["description", "instructions", "name", "names", "glyph", "pattern", "text", "remove"];

        var summary = telling.Select(x => GetString(arguments, x)).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)) ?? "";
        summary = summary.ReplaceLineEndings(" ").Trim();

        return summary.Length > 120 ? summary[..120] + "…" : summary;
    }

    static string GetString(JsonElement element, string name) =>
        element.ValueKind == JsonValueKind.Object && element.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;

    LocalAgentSession CreateSession()
    {
        // A directory of the agent's own, so the CLI's sessions here stay apart from anyone's work in a repo.
        Directory.CreateDirectory(workingDirectory);

        var mcpConfigPath = Path.Combine(workingDirectory, "mcp.json");
        var systemPromptPath = Path.Combine(workingDirectory, "system-prompt.md");

        var mcpConfig = new
        {
            mcpServers = new Dictionary<string, object>
            {
                [_mcpServerName] = new { type = "http", url = GetMcpUrl() },
            },
        };

        File.WriteAllText(mcpConfigPath, JsonSerializer.Serialize(mcpConfig));
        File.WriteAllText(systemPromptPath, _systemPrompt);

        return localAgent.CreateSession(workingDirectory,
        [
            "--system-prompt-file", systemPromptPath,
            "--mcp-config", mcpConfigPath,
            "--strict-mcp-config",

            // No built-in tools (files, shell, web): the agent gets this app's tools and nothing else, and the
            // machine's own Claude Code settings, hooks and skills stay out of it.
            "--tools", "",
            "--allowedTools", $"mcp__{_mcpServerName}",
            "--permission-mode", "dontAsk",
            "--setting-sources", "",
            "--disable-slash-commands",
        ], _model, _effort);
    }

    /// <summary>Where this app serves its tools, as a local client reaches it: over plain http where there's a choice, since a CLI doesn't trust the dev certificate.</summary>
    string GetMcpUrl()
    {
        var addresses = server.Features.Get<IServerAddressesFeature>()?.Addresses ?? [];

        var address = addresses.FirstOrDefault(x => x.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
            ?? addresses.FirstOrDefault()
            ?? throw new InvalidOperationException("The app has no address for a local agent to reach its tools at.");

        // A wildcard host is every address of this machine.
        foreach (var wildcard in (string[])["*", "+", "0.0.0.0", "[::]"])
            address = address.Replace("://" + wildcard, "://localhost");

        return address.TrimEnd('/') + "/mcp";
    }
}
