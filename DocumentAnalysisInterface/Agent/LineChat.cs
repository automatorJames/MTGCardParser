using System.Text;
using Glyphotype.Distiller.Agent;
using Glyphotype.Distiller.Workbench;
using Glyphotype.Distiller.Workspaces;
using Microsoft.AspNetCore.Hosting.Server;

namespace DocumentAnalysisInterface.Agent;

/// <summary>A glyph or vocabulary an agent declared in a step it applied.</summary>
public sealed record ProducedDefinition(DefinitionKind Kind, string Name);

/// <summary>
/// A chat about one corpus line, opened from Corpus Captures: the agent writes the glyphs that capture the line, in the
/// active workspace, and the person pushes back until they're satisfied. Its steps land as working changes, as the
/// Grammar Tools chat's do - but they're the person's to direct, so no check-in interrupts them.
/// </summary>
public sealed class LineChat : AgentChat
{
    const string _systemPrompt = """
        You are the grammar assistant built into the Corpus Captures page of an app for composing a grammar over a text corpus. The person picked one line of the corpus and asked you for the glyph or glyphs that capture it. You work only through the glyphotype tools, on the active workspace's working definition: each step you apply lands as a working change on the Grammar Tools page, and the line re-renders beside this chat as the grammar now tokenizes it.

        This is a request the person directs, not an autonomous session: don't call `start_session`, and there are no check-ins. Read `guide` before your first change if you haven't in this conversation.

        What you must deliver: when you're done, the whole line - every clause of it - is covered by your glyphs under the working grammar. Check that with `tokenize` on the line's text (no `source`) before you finish.

        How to get there:
        - Look before writing. `list_glyphs` and `show` the glyphs and vocabularies that already say part of what the line says, and `search_lines` for lines like it, so the glyph fits the family of lines and not just this one.
        - Compose. The variable data in the line - the words and numbers another line of the same shape would have differently - belong in properties: terminals from vocabularies (existing ones, extended where they're missing a member, or new ones for a closed set of words) and nested glyphs for sub-phrases. The top-level glyph should capture the line's key information in its properties, not as literal text.
        - Draft with `tokenize` (your draft as `source`) and `explain_mismatch` until the line matches, then `evaluate`, then `apply`. Apply a glyph and the vocabularies and glyphs it uses together, as one change set, where you can.
        - `apply` enforces rules meant for an autonomous loop. Because the person asked for this line, when it refuses only because the change takes too few bits off the total, apply it anyway with `override_reason` saying it was requested for this line. Don't override a change that loses other lines: narrow it so it doesn't steal their matches, unless the person says otherwise.
        - If composing can't be made to cover the line, fall back rather than leave it uncovered: a glyph named {0} whose only nib is the whole line as one literal, with no properties. Periods may sit inside a literal nib. That's the worst case, and you should say so if it's what you did.

        Your replies are shown as Markdown in a small panel under the line, and each tool call is shown as a line the person can open. Keep replies short. When you're done, list each glyph and vocabulary you added or changed, with what each property of the top-level glyph captures, then stop and wait. The person may push back on your choices or ask questions: answer, and rework with further steps (or `undo`) as they direct.
        """;

    readonly GrammarAgent _agent;
    readonly WorkspaceManager _workspaces;
    readonly string _id = Guid.NewGuid().ToString("N");
    readonly List<ProducedDefinition> _produced = [];

    public LineChat(LocalAgent localAgent, GrammarAgent agent, WorkspaceManager workspaces, IServer server, string workingDirectory,
        string documentName, int lineIndex, string lineText)
        : base(localAgent, server, workingDirectory)
    {
        _agent = agent;
        _workspaces = workspaces;
        DocumentName = documentName;
        LineIndex = lineIndex;
        LineText = lineText;
        Draft = $"Create GlyphDefinition (or multiple as necessary for composition) in the current workspace that would capture this line. Do so with respect to variable data from existing or not-yet-existing vocabularies that should reasonably be used to extract key information into Glyph and terminal sub-properties in the top-level glyph. Here's the text: {lineText}";
    }

    public string DocumentName { get; }

    /// <summary>The line's index in its document, from 0.</summary>
    public int LineIndex { get; }

    /// <summary>The line as the grammar tokenizes it.</summary>
    public string LineText { get; }

    /// <summary>What the person is writing to send next - kept here, so it outlives the panel it's typed in.</summary>
    public string Draft { get; set; }

    /// <summary>What the worst-case glyph is named: for the document and the line.</summary>
    public string FallbackGlyphName => FallbackName(DocumentName, LineIndex);

    /// <summary>The glyphs and vocabularies the agent's applied steps declared, in the order it first declared them.</summary>
    public IReadOnlyList<ProducedDefinition> Produced
    {
        get
        {
            lock (_gate)
                return _produced.ToList();
        }
    }

    protected override string SystemPromptFileName => Path.Combine("line-chats", $"{_id}.md");

    protected override string SystemPrompt
    {
        get
        {
            var prompt = new StringBuilder(string.Format(_systemPrompt, $"`{FallbackGlyphName}`"));
            prompt.AppendLine().AppendLine($"The line is line {LineIndex + 1} of the document \"{DocumentName}\". Its text, as the grammar tokenizes it:");
            prompt.AppendLine(LineText);

            // This chat skips start_session, whose brief would otherwise carry the rule.
            if (_agent.DocumentationRule is { } documentationRule)
                prompt.AppendLine().AppendLine(documentationRule + $" For the glyphs that capture this line, the example document is \"{DocumentName}\".");

            if (_workspaces.GetGuidance() is { Length: > 0 } guidance)
            {
                prompt.AppendLine().AppendLine("Guidance for this workspace's grammar, written by the person - what it is for and how to approach it. Follow it alongside the guide:");
                prompt.AppendLine(guidance);
            }

            return prompt.ToString();
        }
    }

    protected override IDisposable BeginTurn() => _agent.SuspendCheckIns();

    protected override void OnReset() => _produced.Clear();

    protected override void OnToolAnswered(ChatEntry call)
    {
        if (call is not { Tool: "apply", Failed: false } || string.IsNullOrWhiteSpace(call.Source) || call.Result?.StartsWith("Applied") != true)
            return;

        SourceDeclarations declarations;

        try
        {
            declarations = GlyphSourceReader.Read(call.Source, _workspaces.Active.WorkingDefinition);
        }
        catch (Exception)
        {
            // It applied, so it read; should it not read again, there's only less to list.
            return;
        }

        var declared = declarations.Glyphs.Select(x => new ProducedDefinition(DefinitionKind.Glyph, x.Name))
            .Concat(declarations.Vocabularies.Select(x => new ProducedDefinition(DefinitionKind.Vocabulary, x.Name)));

        foreach (var definition in declared)
            if (!_produced.Contains(definition))
                _produced.Add(definition);
    }

    /// <summary>"Ancestral Recall", line 0 → AncestralRecallLine1.</summary>
    static string FallbackName(string documentName, int lineIndex)
    {
        var words = new string((documentName ?? "").Select(x => char.IsLetterOrDigit(x) ? x : ' ').ToArray())
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => char.ToUpperInvariant(x[0]) + x[1..]);

        var name = string.Concat(words);

        if (name.Length == 0 || !char.IsLetter(name[0]))
            name = "Document" + name;

        return $"{name}Line{lineIndex + 1}";
    }
}

/// <summary>
/// The open line chats, one per line at most, kept app-wide: Corpus Captures rebuilds a document's view every time the
/// grammar changes - which is what a line chat does - so a chat can't live in the view. <see cref="Changed"/> fires when
/// one opens or closes.
/// </summary>
public sealed class LineChats(Func<string, int, string, LineChat> create)
{
    readonly object _gate = new();
    readonly Dictionary<(string Document, int Line), LineChat> _chats = [];

    public event Action Changed;

    public LineChat Get(string documentName, int lineIndex)
    {
        lock (_gate)
            return _chats.GetValueOrDefault((documentName, lineIndex));
    }

    /// <summary>Whether any of the document's lines has a chat open.</summary>
    public bool AnyFor(string documentName)
    {
        lock (_gate)
            return _chats.Keys.Any(x => x.Document == documentName);
    }

    /// <summary>The line's chat - a new one, if it hasn't one open.</summary>
    public LineChat Open(string documentName, int lineIndex, string lineText)
    {
        LineChat chat;

        lock (_gate)
        {
            if (_chats.TryGetValue((documentName, lineIndex), out chat))
                return chat;

            _chats[(documentName, lineIndex)] = chat = create(documentName, lineIndex, lineText);
        }

        Changed?.Invoke();
        return chat;
    }

    /// <summary>Closes the line's chat, stopping the agent if it's working. Steps it applied stay applied.</summary>
    public void Close(LineChat chat)
    {
        lock (_gate)
        {
            if (!_chats.Remove((chat.DocumentName, chat.LineIndex)))
                return;
        }

        chat.Stop();
        Changed?.Invoke();
    }
}
