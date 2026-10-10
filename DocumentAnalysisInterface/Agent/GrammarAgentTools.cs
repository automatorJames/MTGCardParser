using System.ComponentModel;
using Glyphotype.Distiller.Agent;
using Glyphotype.Distiller.Inspection;
using ModelContextProtocol;
using ModelContextProtocol.Server;

namespace DocumentAnalysisInterface.Agent;

/// <summary>
/// The <see cref="GrammarAgent"/>'s methods as MCP tools, served from this app at <c>/mcp</c> - so an agent works on
/// the same live workbench the Grammar Tools tab shows (whichever workspace is active), and a person there sees each
/// step as it lands. No tool commits to C#, checkpoints or exports: those stay the person's decisions, in the app.
/// </summary>
[McpServerToolType]
public sealed class GrammarAgentTools(GrammarAgent agent)
{
    const string _sourceDescription =
        "C# declarations, without namespace or usings: glyph classes, vocabulary enums, marker interfaces. " +
        "A declaration replaces the working definition of the same name, or adds one. See `guide` for the syntax.";

    const string _removeDescription = "Names of glyphs, vocabularies or markers to remove, comma-separated.";

    [McpServerTool(Name = "start_session"), Description(
        "Starts (or resumes, after a check-in) a working session: returns the person's instructions, the session's rules as the app configures them " +
        "(when to check in, what a step must achieve), and where the grammar stands. Call this first, and again whenever the person says to continue.")]
    public Task<string> StartSession(
        [Description("What the person asked for this session, in their words - e.g. \"from scratch\" or \"focus on triggers\". Empty if nothing.")] string instructions = null,
        CancellationToken cancellation = default) =>
        Run(() => agent.StartSessionAsync(instructions, cancellation));

    [McpServerTool(Name = "overview", ReadOnly = true), Description("The corpus, the working grammar's score and coverage, glyphs that cost more than they save, and the costliest unmatched text. Start here.")]
    public Task<string> Overview(CancellationToken cancellation) =>
        Run(() => agent.OverviewAsync(cancellation));

    [McpServerTool(Name = "guide", ReadOnly = true, Idempotent = true), Description("How to work: the loop, how the score works, how to write glyphs in C#, and habits that work. Read once before the first change.")]
    public static string Guide() => GrammarAgent.Guide;

    [McpServerTool(Name = "list_workspaces", ReadOnly = true), Description("The grammars (workspaces) the app holds, which one is active - the one you and the person both see - and what each kind is.")]
    public Task<string> ListWorkspaces() =>
        Run(() => Task.FromResult(agent.ListWorkspaces()));

    [McpServerTool(Name = "create_workspace"), Description(
        "Creates a scratch workspace - a grammar kept as JSON, apart from the app's C# sources - and makes it active for you and the person. " +
        "Only when the person asks, e.g. to start from scratch.")]
    public Task<string> CreateWorkspace(
        [Description("A name for it.")] string name,
        [Description("empty: nothing. vocabularies (for starting from scratch): another workspace's vocabularies and nothing else. copy: everything another workspace has.")] string start = "empty",
        [Description("The workspace to take vocabularies from, or copy - the active one by default.")] string from = null,
        [Description("Whether to start with that workspace's guidance too (the person's notes on the grammar). Usually yes, when there is some.")] bool copyGuidance = true,
        [Description("Whether to start with that workspace's journal too (what agents learned there). Usually yes when copying the grammar; no when starting from scratch, where its dead ends mostly don't apply.")] bool copyJournal = true,
        CancellationToken cancellation = default) =>
        Run(() => agent.CreateWorkspaceAsync(name, start, from, copyGuidance, copyJournal, cancellation));

    [McpServerTool(Name = "switch_workspace"), Description("Makes another workspace active - for the person too, so only when they ask.")]
    public Task<string> SwitchWorkspace([Description("The workspace's name.")] string name, CancellationToken cancellation = default) =>
        Run(() => agent.SwitchWorkspaceAsync(name, cancellation));

    [McpServerTool(Name = "list_glyphs", ReadOnly = true), Description("Every glyph with its net bits, top-level matches, words covered and definition cost; every vocabulary with its cost and the glyphs using it.")]
    public Task<string> ListGlyphs(
        [Description("Sort glyphs by: words covered (default), net, matches, cost or name.")] string sort = "words",
        CancellationToken cancellation = default) =>
        Run(() => agent.ListGlyphsAsync(sort, cancellation));

    [McpServerTool(Name = "show", ReadOnly = true), Description("The C# source of glyphs, vocabularies or markers in the working definition.")]
    public Task<string> Show([Description("Names, comma-separated.")] string names) =>
        Run(() => Task.FromResult(agent.Show(names)));

    [McpServerTool(Name = "evaluate", ReadOnly = true), Description(
        "Scores the working grammar with a change set applied, without applying it: the bit and coverage deltas, per-glyph contribution changes, " +
        "and the lines whose tokenization changed (gained, lost, reshaped). Takes as long as scoring the corpus.")]
    public Task<string> Evaluate(
        [Description(_sourceDescription)] string source = null,
        [Description(_removeDescription)] string remove = null,
        CancellationToken cancellation = default) =>
        Run(() => agent.EvaluateAsync(source, remove, cancellation));

    [McpServerTool(Name = "apply", Destructive = false), Description(
        "Makes a change set as one step of the working definition, reporting what it did as `evaluate` does. Refused if the result wouldn't build, " +
        "if a check-in is due, or if it breaks the session's step rules (see `start_session`) without an override reason. " +
        "The person sees the step in the app; `undo` takes it back.")]
    public Task<string> Apply(
        [Description(_sourceDescription)] string source = null,
        [Description(_removeDescription)] string remove = null,
        [Description("Why you're making this change, in a short phrase - shown in the step history.")] string description = null,
        [Description("Only for a deliberate refactor that breaks the step rules (e.g. a merge that costs a few bits now): why it's worth it. Recorded in the step.")] string override_reason = null,
        CancellationToken cancellation = default) =>
        Run(() => agent.ApplyAsync(source, remove, description, override_reason, cancellation));

    [McpServerTool(Name = "undo"), Description("Takes back the latest step of the working definition (whoever made it), and reports what that did.")]
    public Task<string> Undo(CancellationToken cancellation) =>
        Run(() => agent.UndoAsync(cancellation));

    [McpServerTool(Name = "revert"), Description("Returns one glyph or vocabulary to its committed state, as a step.")]
    public Task<string> Revert([Description("The glyph or vocabulary name.")] string name, CancellationToken cancellation) =>
        Run(() => agent.RevertAsync(name, cancellation));

    [McpServerTool(Name = "history", ReadOnly = true), Description("The steps made this session, newest first, and what's uncommitted.")]
    public Task<string> History([Description("How many steps to list.")] int limit = 20) =>
        Run(() => Task.FromResult(agent.History(limit)));

    [McpServerTool(Name = "journal", ReadOnly = true), Description(
        "The workspace's journal - open problems, dead ends and hints earlier sessions recorded - with each entry's id, and which are about a glyph or vocabulary that's changed since. " +
        "`start_session` includes it; read it again to see it as it stands.")]
    public Task<string> Journal() =>
        Run(() => Task.FromResult(agent.Journal()));

    [McpServerTool(Name = "journal_add"), Description(
        "Records something a later session would want to know before doing the work: an open problem (including text whose best cover so far costs too much), a dead end (what you tried, and why it didn't work), or a hint. " +
        "Write it when you find it. Not for logging what you did - the step history does that.")]
    public Task<string> JournalAdd(
        [Description("open_problem, dead_end or hint.")] string section,
        [Description("The finding, in a sentence or two: specific enough to act on without redoing the work.")] string text,
        [Description("The glyphs and vocabularies it's about, comma-separated - so it's flagged when they change.")] string names = null) =>
        Run(() => Task.FromResult(agent.JournalAdd(section, text, names)));

    [McpServerTool(Name = "journal_update"), Description("Rewrites a journal entry that's partly out of date - whatever's given of its text, section and names.")]
    public Task<string> JournalUpdate(
        [Description("The entry's id.")] int id,
        [Description("Its new text, if it changes.")] string text = null,
        [Description("Its new section, if it changes: open_problem, dead_end or hint.")] string section = null,
        [Description("The glyphs and vocabularies it's about now, comma-separated, if they change.")] string names = null) =>
        Run(() => Task.FromResult(agent.JournalUpdate(id, text, section, names)));

    [McpServerTool(Name = "journal_remove"), Description("Removes a journal entry that no longer holds - a problem since solved, a dead end the grammar has moved past.")]
    public Task<string> JournalRemove([Description("The entry's id.")] int id) =>
        Run(() => Task.FromResult(agent.JournalRemove(id)));

    [McpServerTool(Name = "search_lines", ReadOnly = true), Description("Distinct corpus lines matching a regex, most frequent first, as the working grammar tokenizes them.")]
    public Task<string> SearchLines(
        [Description("A case-insensitive .NET regex; omit to match every line in scope.")] string pattern = null,
        [Description("all (default): test the whole line. unmatched: test only text no glyph covers. unparsed: whole lines no glyph matched any of.")] string scope = "all",
        [Description("Only lines where this glyph or vocabulary matched, at any depth.")] string glyph = null,
        [Description("How many distinct lines to list.")] int limit = 20,
        [Description("Show captures inside matches, not just the top-level glyphs.")] bool nested = false,
        CancellationToken cancellation = default) =>
        Run(() => agent.SearchLinesAsync(pattern, ParseScope(scope), glyph, limit, nested, cancellation));

    [McpServerTool(Name = "matches", ReadOnly = true), Description("Where and how a glyph or vocabulary matched: counts, the shapes its matches took (frames, or a vocabulary's words), and example lines.")]
    public Task<string> Matches(
        [Description("The glyph or vocabulary name.")] string name,
        [Description("How many shapes to list.")] int shapes = 15,
        [Description("How many example lines to list.")] int lines = 8,
        CancellationToken cancellation = default) =>
        Run(() => agent.MatchesAsync(name, shapes, lines, cancellation));

    [McpServerTool(Name = "residuals", ReadOnly = true), Description("The costliest distinct spans of unmatched text.")]
    public Task<string> Residuals(
        [Description("How many spans to list.")] int limit = 30,
        [Description("How many of the costliest to skip, to page further down.")] int offset = 0,
        [Description("Only spans of at least this many words.")] int minWords = 1,
        CancellationToken cancellation = default) =>
        Run(() => agent.ResidualsAsync(limit, offset, minWords, cancellation));

    [McpServerTool(Name = "unmatched_openings", ReadOnly = true), Description("Uncovered text - unmatched, or held unresolved in a match - grouped by its opening words, ranked by the uncovered words it holds: the constructions whose frame, or slot, covers the most.")]
    public Task<string> UnmatchedOpenings(
        [Description("Longest opening, in words.")] int maxWords = 3,
        [Description("Only openings starting at least this many spans.")] int minOccurrences = 3,
        [Description("How many openings to list.")] int limit = 25,
        CancellationToken cancellation = default) =>
        Run(() => agent.UnmatchedOpeningsAsync(maxWords, minOccurrences, limit, cancellation));

    [McpServerTool(Name = "residual_phrases", ReadOnly = true), Description("Word runs that recur in unmatched text, at their longest form, ranked by the words they'd account for: where the next glyph probably is.")]
    public Task<string> ResidualPhrases(
        [Description("Shortest phrase, in words.")] int minWords = 2,
        [Description("Longest phrase, in words.")] int maxWords = 8,
        [Description("Only phrases occurring at least this often.")] int minOccurrences = 3,
        [Description("How many phrases to list.")] int limit = 40,
        CancellationToken cancellation = default) =>
        Run(() => agent.ResidualPhrasesAsync(minWords, maxWords, minOccurrences, limit, cancellation));

    [McpServerTool(Name = "tokenize", ReadOnly = true), Description("How text tokenizes under the working grammar - or with a draft change set applied on top - with captures shown. Instant: nothing is scored.")]
    public Task<string> Tokenize(
        [Description("The text; one line per line. Lower-cased, as corpus lines are.")] string text,
        [Description("Optional draft: " + _sourceDescription)] string source = null,
        [Description("Optional: " + _removeDescription)] string remove = null,
        CancellationToken cancellation = default) =>
        Run(() => agent.TokenizeAsync(text, source, remove, cancellation));

    [McpServerTool(Name = "explain_mismatch", ReadOnly = true), Description("How far one glyph's pattern gets through a piece of text on its own, which part of it fails first, and how the whole grammar tokenizes the text instead.")]
    public Task<string> ExplainMismatch(
        [Description("The glyph whose pattern to test.")] string glyph,
        [Description("The text it should match - one clause.")] string text,
        [Description("Optional draft: " + _sourceDescription)] string source = null,
        [Description("Optional: " + _removeDescription)] string remove = null,
        CancellationToken cancellation = default) =>
        Run(() => agent.ExplainMismatchAsync(glyph, text, source, remove, cancellation));

    static LineScope ParseScope(string scope) =>
        Enum.TryParse<LineScope>(scope, ignoreCase: true, out var parsed)
            ? parsed
            : throw new AgentRequestException($"Unknown scope '{scope}': use all, unmatched or unparsed.");

    /// <summary>Surfaces a bad request's message to the agent as a tool error - the SDK hides other exceptions' messages.</summary>
    static async Task<string> Run(Func<Task<string>> tool)
    {
        try
        {
            return await tool();
        }
        catch (AgentRequestException exception)
        {
            throw new McpException(exception.Message, exception);
        }
    }
}
