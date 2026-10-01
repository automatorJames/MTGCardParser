using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace DocumentAnalysisInterface.Agent;

/// <summary>How the app runs a local agent itself (see <see cref="LocalAgent"/>). Optional - each setting has a default.</summary>
public sealed record LocalAgentSettings
{
    /// <summary>The full path of the claude executable, when it isn't on the PATH.</summary>
    public string ClaudePath { get; init; }

    /// <summary>The model the agent runs on, as the CLI's <c>--model</c> takes it (an alias like "sonnet", or a full id) - the CLI's own default when blank.</summary>
    public string Model { get; init; }

    /// <summary>The effort level, as the CLI's <c>--effort</c> takes it - the CLI's own default when blank.</summary>
    public string Effort { get; init; }
}

public sealed record LocalAgentStatus(bool IsAvailable, string Reason)
{
    public static readonly LocalAgentStatus Available = new(true, null);

    public static LocalAgentStatus Unavailable(string reason) => new(false, reason);
}

/// <summary>Something that happened while the agent worked on one message.</summary>
public abstract record LocalAgentEvent;

/// <summary>A piece of the agent's reply, as Markdown, to append to what has arrived so far.</summary>
public sealed record AgentText(string Text, bool StartsMessage) : LocalAgentEvent;

/// <summary>The agent called a tool. <paramref name="Arguments"/> is the tool's input as JSON.</summary>
public sealed record AgentToolCall(string ToolUseId, string Tool, JsonElement Arguments) : LocalAgentEvent;

public sealed record AgentToolResult(string ToolUseId, bool Failed, string Text) : LocalAgentEvent;

public sealed record AgentTurnFailed(string Message) : LocalAgentEvent;

/// <summary>
/// The local agent: the Claude Code CLI, run headless. It only exists where someone has it installed and signed in,
/// so everything here answers "not available" rather than throwing.
/// </summary>
public sealed class LocalAgent(LocalAgentSettings settings)
{
    static readonly TimeSpan _statusLifetime = TimeSpan.FromMinutes(1);

    readonly SemaphoreSlim _statusLock = new(1, 1);

    LocalAgentStatus _status;
    DateTime _statusCheckedUtc;
    string _path;

    public LocalAgentSettings Settings => settings;

    /// <param name="recheck">Look again now, rather than reuse the answer of the last minute.</param>
    public async Task<LocalAgentStatus> GetStatusAsync(bool recheck = false)
    {
        await _statusLock.WaitAsync();

        try
        {
            if (recheck || _status is null || DateTime.UtcNow - _statusCheckedUtc > _statusLifetime)
            {
                _status = await CheckAsync();
                _statusCheckedUtc = DateTime.UtcNow;
            }

            return _status;
        }
        catch (Exception exception)
        {
            return LocalAgentStatus.Unavailable("The check for a local agent failed: " + exception.Message);
        }
        finally
        {
            _statusLock.Release();
        }
    }

    /// <summary>Starts a new conversation, run with <paramref name="arguments"/> from <paramref name="workingDirectory"/>. Call only when <see cref="GetStatusAsync"/> says the agent is available.</summary>
    public LocalAgentSession CreateSession(string workingDirectory, IEnumerable<string> arguments)
    {
        if (_path is null)
            throw new InvalidOperationException("No local agent is available.");

        List<string> all = ["--print", "--output-format", "stream-json", "--verbose", "--include-partial-messages", .. arguments];

        if (!string.IsNullOrWhiteSpace(settings.Model))
            all.AddRange(["--model", settings.Model.Trim()]);

        if (!string.IsNullOrWhiteSpace(settings.Effort))
            all.AddRange(["--effort", settings.Effort.Trim()]);

        return new LocalAgentSession(_path, workingDirectory, all);
    }

    async Task<LocalAgentStatus> CheckAsync()
    {
        _path = Find();

        if (_path is null)
            return LocalAgentStatus.Unavailable("The Claude Code CLI ('claude') was not found on this machine. Install it, or set LocalAgent:ClaudePath to its full path.");

        var (exitCode, output) = await RunAsync(_path, ["auth", "status"], TimeSpan.FromSeconds(15));

        return IsSignedIn(exitCode, output)
            ? LocalAgentStatus.Available
            : LocalAgentStatus.Unavailable("Claude Code is installed but not signed in. Run 'claude' in a terminal and sign in.");
    }

    static bool IsSignedIn(int exitCode, string output)
    {
        try
        {
            using var json = JsonDocument.Parse(output);
            return json.RootElement.TryGetProperty("loggedIn", out var loggedIn) && loggedIn.ValueKind == JsonValueKind.True;
        }
        catch (JsonException)
        {
            // Not the JSON this was written against; go by the exit code.
            return exitCode == 0;
        }
    }

    string Find()
    {
        if (!string.IsNullOrWhiteSpace(settings.ClaudePath))
            return File.Exists(settings.ClaudePath) ? settings.ClaudePath : null;

        var fileName = OperatingSystem.IsWindows() ? "claude.exe" : "claude";

        var directories = (Environment.GetEnvironmentVariable("PATH") ?? "")
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            // Where the native installer puts it, in case the app was started with a PATH that predates the install.
            .Append(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local", "bin"));

        foreach (var directory in directories)
        {
            try
            {
                var candidate = Path.Combine(directory, fileName);

                if (File.Exists(candidate))
                    return candidate;
            }
            catch (ArgumentException)
            {
                // A malformed PATH entry.
            }
        }

        return null;
    }

    static async Task<(int ExitCode, string Output)> RunAsync(string fileName, string[] arguments, TimeSpan timeout)
    {
        var startInfo = new ProcessStartInfo(fileName)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        foreach (var argument in arguments)
            startInfo.ArgumentList.Add(argument);

        using var process = Process.Start(startInfo);
        using var timeoutSource = new CancellationTokenSource(timeout);

        try
        {
            var output = process.StandardOutput.ReadToEndAsync(timeoutSource.Token);
            _ = process.StandardError.ReadToEndAsync(timeoutSource.Token);
            await process.WaitForExitAsync(timeoutSource.Token);
            return (process.ExitCode, await output);
        }
        catch (OperationCanceledException)
        {
            process.Kill(entireProcessTree: true);
            return (-1, "");
        }
    }
}

/// <summary>
/// One conversation with the local agent. Each message runs the CLI once, resuming the CLI's own session, so the
/// conversation's memory lives in the CLI and a turn is stopped by ending its process.
/// </summary>
public sealed class LocalAgentSession
{
    readonly string _path;
    readonly string _workingDirectory;
    readonly IReadOnlyList<string> _arguments;
    readonly string _sessionId = Guid.NewGuid().ToString();

    bool _sessionExists;

    internal LocalAgentSession(string path, string workingDirectory, IReadOnlyList<string> arguments)
    {
        _path = path;
        _workingDirectory = workingDirectory;
        _arguments = arguments;
    }

    /// <summary>Sends one message and yields what the agent does with it. Cancel the token to stop the agent.</summary>
    public async IAsyncEnumerable<LocalAgentEvent> SendAsync(string message, [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        var startInfo = new ProcessStartInfo(_path)
        {
            WorkingDirectory = _workingDirectory,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            StandardInputEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        foreach (var argument in _arguments)
            startInfo.ArgumentList.Add(argument);

        startInfo.ArgumentList.Add(_sessionExists ? "--resume" : "--session-id");
        startInfo.ArgumentList.Add(_sessionId);

        using var process = Process.Start(startInfo);
        using var stopOnCancel = cancellation.Register(() => Kill(process));

        var errorOutput = process.StandardError.ReadToEndAsync(CancellationToken.None);

        // The message goes in on stdin, which spares it from command-line quoting.
        await process.StandardInput.WriteAsync(message);
        process.StandardInput.Close();

        var reader = new StreamEventReader();
        var finished = false;

        try
        {
            while (!cancellation.IsCancellationRequested)
            {
                var line = await ReadLineOrNullAsync(process, cancellation);

                if (line is null)
                    break;

                foreach (var agentEvent in reader.Read(line))
                {
                    finished |= agentEvent is AgentTurnFailed;
                    yield return agentEvent;
                }

                _sessionExists |= reader.SessionStarted;
                finished |= reader.TurnCompleted;
            }
        }
        finally
        {
            // Covers a caller that walks away mid-reply; after a normal finish there is nothing left to stop.
            if (!finished)
                Kill(process);
        }

        if (cancellation.IsCancellationRequested)
            yield break;

        await process.WaitForExitAsync(CancellationToken.None);

        if (!finished)
        {
            var error = (await errorOutput).Trim();
            yield return new AgentTurnFailed(error.Length > 0 ? error : $"The agent stopped unexpectedly (exit code {process.ExitCode}).");
        }
    }

    static async Task<string> ReadLineOrNullAsync(Process process, CancellationToken cancellation)
    {
        try
        {
            return await process.StandardOutput.ReadLineAsync(cancellation);
        }
        catch (OperationCanceledException)
        {
            return null;
        }
    }

    static void Kill(Process process)
    {
        try
        {
            if (!process.HasExited)
                process.Kill(entireProcessTree: true);
        }
        catch (InvalidOperationException)
        {
            // Already gone.
        }
    }

    /// <summary>Turns the CLI's stream-json output lines into events.</summary>
    sealed class StreamEventReader
    {
        bool _textStreamedForMessage;
        bool _messageHasText;

        public bool SessionStarted { get; private set; }
        public bool TurnCompleted { get; private set; }

        public IEnumerable<LocalAgentEvent> Read(string line)
        {
            JsonDocument document;

            try
            {
                document = JsonDocument.Parse(line);
            }
            catch (JsonException)
            {
                yield break;
            }

            using (document)
            {
                var root = document.RootElement;

                switch (GetString(root, "type"))
                {
                    case "system" when GetString(root, "subtype") == "init":
                        SessionStarted = true;
                        break;

                    case "stream_event" when root.TryGetProperty("event", out var streamEvent):
                        if (ReadStreamEvent(streamEvent) is LocalAgentEvent delta)
                            yield return delta;
                        break;

                    case "assistant" when TryGetContent(root, out var blocks):
                        foreach (var block in blocks.EnumerateArray())
                        {
                            switch (GetString(block, "type"))
                            {
                                case "tool_use":
                                    yield return new AgentToolCall(
                                        GetString(block, "id"),
                                        StripServerPrefix(GetString(block, "name")),
                                        block.TryGetProperty("input", out var input) ? input.Clone() : default);
                                    break;

                                // Normally the text has already arrived as deltas.
                                case "text" when !_textStreamedForMessage:
                                    yield return new AgentText(GetString(block, "text"), StartsMessage: true);
                                    break;
                            }
                        }
                        break;

                    case "user" when TryGetContent(root, out var blocks):
                        foreach (var block in blocks.EnumerateArray().Where(x => GetString(x, "type") == "tool_result"))
                        {
                            var failed = block.TryGetProperty("is_error", out var isError) && isError.ValueKind == JsonValueKind.True;
                            yield return new AgentToolResult(GetString(block, "tool_use_id"), failed, GetResultText(block));
                        }
                        break;

                    case "result":
                        TurnCompleted = true;

                        if (root.TryGetProperty("is_error", out var turnFailed) && turnFailed.ValueKind == JsonValueKind.True)
                            yield return new AgentTurnFailed(GetString(root, "result") ?? "The agent could not complete the request.");
                        break;
                }
            }
        }

        LocalAgentEvent ReadStreamEvent(JsonElement streamEvent)
        {
            switch (GetString(streamEvent, "type"))
            {
                case "message_start":
                    _textStreamedForMessage = false;
                    _messageHasText = false;
                    break;

                case "content_block_delta"
                    when streamEvent.TryGetProperty("delta", out var delta) && GetString(delta, "type") == "text_delta":
                    // What the agent says before a tool call and after it are separate messages.
                    var text = new AgentText(GetString(delta, "text"), StartsMessage: !_messageHasText);
                    _textStreamedForMessage = _messageHasText = true;
                    return text;
            }

            return null;
        }

        static bool TryGetContent(JsonElement root, out JsonElement content)
        {
            content = default;

            return root.TryGetProperty("message", out var message)
                && message.TryGetProperty("content", out content)
                && content.ValueKind == JsonValueKind.Array;
        }

        static string GetResultText(JsonElement toolResult)
        {
            if (!toolResult.TryGetProperty("content", out var content))
                return "";

            if (content.ValueKind == JsonValueKind.String)
                return content.GetString();

            if (content.ValueKind != JsonValueKind.Array)
                return "";

            return string.Join('\n', content.EnumerateArray()
                .Where(x => GetString(x, "type") == "text")
                .Select(x => GetString(x, "text")));
        }

        static string GetString(JsonElement element, string name) =>
            element.ValueKind == JsonValueKind.Object && element.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String
                ? value.GetString()
                : null;

        // mcp__glyphotype__evaluate -> evaluate
        static string StripServerPrefix(string toolName) =>
            toolName?.Split("__").Last();
    }
}
