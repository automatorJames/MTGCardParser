using System.ComponentModel;
using ModelContextProtocol.Server;

namespace DocumentAnalysisInterface.Agent;

/// <summary>
/// The whole instruction an agent needs to start working, as an MCP prompt - which a client like Claude Code offers as
/// a command (<c>/mcp__glyphotype__grammar</c>). Deliberately one line: everything else, including the session's rules
/// from appsettings, comes from <see cref="GrammarAgentTools.StartSession"/>.
/// </summary>
[McpServerPromptType]
public sealed class GrammarAgentPrompts
{
    /// <summary>The same instruction the repo's <c>/grammar</c> skill gives.</summary>
    public static string Instruction(string instructions) =>
        $"Work on the grammar through the glyphotype tools: call `start_session` with instructions \"{instructions?.Trim()}\", then follow the brief it returns.";

    [McpServerPrompt(Name = "grammar"), Description("Work on the grammar step by step: evaluate and apply glyphs, checking in as the app's settings say.")]
    public static string Grammar(
        [Description("Anything for this session, e.g. \"from scratch\" or \"focus on triggers\".")] string instructions = null) =>
        Instruction(instructions);
}
