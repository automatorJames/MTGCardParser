using System.Text.Json;
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

/// <summary>What a conversation has used so far, and where the account stands against its allowances.</summary>
public sealed record ChatStats
{
    /// <summary>How many tokens the conversation comes to now, of the <see cref="ContextWindow"/> the model can hold (null until the CLI has said).</summary>
    public long ContextTokens { get; init; }
    public long? ContextWindow { get; init; }

    /// <summary>Tokens every request of the conversation used, added up: read in new, written to and read from the cache, and written out.</summary>
    public long InputTokens { get; init; }
    public long CacheWriteTokens { get; init; }
    public long CacheReadTokens { get; init; }
    public long OutputTokens { get; init; }

    public long TotalTokens => InputTokens + CacheWriteTokens + CacheReadTokens + OutputTokens;

    /// <summary>What the conversation's requests would cost at list prices.</summary>
    public double CostUsd { get; init; }

    /// <summary>The account's allowances, as last heard - null until then, and for an account without them.</summary>
    public UsageWindow FiveHour { get; init; }
    public UsageWindow SevenDay { get; init; }
}

/// <summary>What a conversation runs on, as the CLI names them.</summary>
public sealed record ChatOptions(string Model, string Effort);

/// <summary>
/// A conversation with the local agent (see <see cref="LocalAgent"/>), which works through the same tools this app
/// serves at <c>/mcp</c>. It runs apart from any browser tab: every tab showing it sees the same thing, and closing
/// one doesn't stop it. <see cref="Changed"/> fires, from whatever thread, whenever anything a tab shows changes.
/// </summary>
public abstract class AgentChat(LocalAgent localAgent, IServer server, string workingDirectory)
{
    const string _mcpServerName = "glyphotype";

    protected readonly object _gate = new();
    readonly List<ChatEntry> _entries = [];

    LocalAgentSession _session;
    CancellationTokenSource _turn;
    ChatStats _stats = new();
    ChatOptions _chosen;

    /// <summary>The options of the conversation under way - null before it begins.</summary>
    ChatOptions _settled;

    public event Action Changed;

    protected LocalAgent LocalAgent => localAgent;

    protected string WorkingDirectory => workingDirectory;

    /// <summary>What the agent is told it is, and how to work - read when the conversation begins.</summary>
    protected abstract string SystemPrompt { get; }

    /// <summary>The file in <see cref="WorkingDirectory"/> the system prompt is handed to the CLI in.</summary>
    protected abstract string SystemPromptFileName { get; }

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

    /// <summary>Whether the conversation has begun: what it runs on is settled from then on.</summary>
    public bool HasBegun => _settled is not null;

    /// <summary>What the conversation has used so far.</summary>
    public ChatStats Stats => _stats;

    /// <summary>
    /// The model the conversation runs on, as the CLI names it (an alias like "sonnet", or a full id). Settable until
    /// the conversation begins, and settled from then until <see cref="Reset"/>.
    /// </summary>
    public string Model
    {
        get => Options.Model;
        set => Choose(value, x => x with { Model = value.Trim() });
    }

    /// <summary>The effort level the conversation runs at, as the CLI names it. Settable until the conversation begins, as <see cref="Model"/> is.</summary>
    public string Effort
    {
        get => Options.Effort;
        set => Choose(value, x => x with { Effort = value.Trim() });
    }

    ChatOptions Options
    {
        get
        {
            lock (_gate)
                return _settled ?? ChosenOptions;
        }
    }

    /// <summary>The options set for the conversation to come - the settings' model and effort, until others are chosen. Called under the gate.</summary>
    protected virtual ChatOptions ChosenOptions
    {
        get => _chosen ??= DefaultOptions;
        set => _chosen = value;
    }

    protected ChatOptions DefaultOptions =>
        new(NullIfBlank(localAgent.Settings.Model) ?? "sonnet", NullIfBlank(localAgent.Settings.Effort) ?? "medium");

    void Choose(string value, Func<ChatOptions, ChatOptions> change)
    {
        lock (_gate)
        {
            if (_settled is not null || string.IsNullOrWhiteSpace(value))
                return;

            ChosenOptions = change(Options);
        }

        NotifyChanged();
    }

    protected static string NullIfBlank(string value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    /// <summary>The conversation begins on <paramref name="options"/> - called under the gate.</summary>
    protected virtual void OnBegun(ChatOptions options)
    {
    }

    /// <summary>Something to hold for the length of one turn, disposed of once it's over - null for nothing.</summary>
    protected virtual IDisposable BeginTurn() => null;

    /// <summary>A tool call has its answer - called under the gate.</summary>
    protected virtual void OnToolAnswered(ChatEntry call)
    {
    }

    /// <summary>Says something to the agent.</summary>
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
            _settled = null;

            // The allowances are the account's, not the conversation's.
            _stats = new() { FiveHour = _stats.FiveHour, SevenDay = _stats.SevenDay };
            OnReset();
        }

        NotifyChanged();
    }

    /// <summary>The conversation is forgotten - called under the gate.</summary>
    protected virtual void OnReset()
    {
    }

    protected void NotifyChanged() => Changed?.Invoke();

    /// <summary>Has the agent take <paramref name="message"/>, shown in the chat as <paramref name="said"/> - false if it's busy with one already.</summary>
    protected bool Start(string said, string message)
    {
        CancellationTokenSource turn;

        lock (_gate)
        {
            if (_turn is not null)
                return false;

            if (_settled is null)
            {
                _settled = Options;
                OnBegun(_settled);
            }

            _turn = turn = new();
            _entries.Add(new(ChatEntryKind.Person, said));
        }

        NotifyChanged();
        _ = Task.Run(() => RunTurnAsync(message, turn));
        return true;
    }

    async Task RunTurnAsync(string message, CancellationTokenSource turn)
    {
        IDisposable held = null;

        try
        {
            held = BeginTurn();
            _session ??= CreateSession();

            await foreach (var agentEvent in _session.SendAsync(message, turn.Token))
            {
                lock (_gate)
                    Record(agentEvent);

                NotifyChanged();
            }
        }
        catch (Exception exception)
        {
            lock (_gate)
                _entries.Add(new(ChatEntryKind.Error, exception.Message));
        }
        finally
        {
            held?.Dispose();

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
            NotifyChanged();
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
                OnToolAnswered(_entries[index]);
                break;

            case AgentTurnFailed failed:
                EndStreaming();
                _entries.Add(new(ChatEntryKind.Error, failed.Message));
                break;

            case AgentContextSize context:
                _stats = _stats with { ContextTokens = context.Tokens };
                break;

            case AgentTokens tokens:
                _stats = _stats with
                {
                    InputTokens = _stats.InputTokens + tokens.Input,
                    CacheWriteTokens = _stats.CacheWriteTokens + tokens.CacheWrite,
                    CacheReadTokens = _stats.CacheReadTokens + tokens.CacheRead,
                    OutputTokens = _stats.OutputTokens + tokens.Output,
                };
                break;

            case AgentTurnCompleted completed:
                _stats = _stats with { CostUsd = _stats.CostUsd + completed.CostUsd, ContextWindow = completed.ContextWindow ?? _stats.ContextWindow };
                break;

            case AgentUsageLimits limits:
                _stats = _stats with { FiveHour = limits.FiveHour ?? _stats.FiveHour, SevenDay = limits.SevenDay ?? _stats.SevenDay };
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
        var systemPromptPath = Path.Combine(workingDirectory, SystemPromptFileName);

        var mcpConfig = new
        {
            mcpServers = new Dictionary<string, object>
            {
                [_mcpServerName] = new { type = "http", url = GetMcpUrl() },
            },
        };

        File.WriteAllText(mcpConfigPath, JsonSerializer.Serialize(mcpConfig));
        Directory.CreateDirectory(Path.GetDirectoryName(systemPromptPath));
        File.WriteAllText(systemPromptPath, SystemPrompt);

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
        ], _settled.Model, _settled.Effort);
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
