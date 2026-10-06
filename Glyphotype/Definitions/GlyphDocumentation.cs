using System.Xml;
using System.Xml.Linq;

namespace Glyphotype.Definitions;

/// <summary>
/// What a glyph's <c>///</c> doc comment says about it - written above its class by <see cref="GlyphSourceWriter"/>,
/// and read back from the comment by <see cref="FromComment"/>: the one place either is done. None of it is grammar:
/// it doesn't change what the glyph matches, and costs nothing to score.
/// <para>
/// A glyph's doc comment holds these tags and nothing else, so it can always be written again from its definition
/// without losing anything: <c>&lt;summary&gt;</c>, <c>&lt;exampledoc&gt;</c> and <c>&lt;examplecapture&gt;</c>.
/// Each is optional, and each may run over several lines.
/// </para>
/// </summary>
public sealed record GlyphDocumentation
{
    const string _summaryTag = "summary";
    const string _exampleDocumentTag = "exampledoc";
    const string _exampleCaptureTag = "examplecapture";

    /// <summary>The tags a glyph's doc comment may hold, in the order they're written.</summary>
    public static IReadOnlyList<string> Tags { get; } = [_summaryTag, _exampleDocumentTag, _exampleCaptureTag];

    /// <summary>
    /// What the glyph is for: the <c>&lt;summary&gt;</c>'s content, which is doc-comment markup - it may hold
    /// <c>&lt;see cref="..."/&gt;</c>, <c>&lt;c&gt;</c> and the like, and a literal <c>&amp;</c> or <c>&lt;</c> must be escaped.
    /// </summary>
    public string Summary { get; init; }

    /// <summary><c>&lt;exampledoc&gt;</c>: the name of a corpus document the glyph is meant for. Plain text, and only a pointer - it may be out of date.</summary>
    public string ExampleDocument { get; init; }

    /// <summary><c>&lt;examplecapture&gt;</c>: the text in <see cref="ExampleDocument"/> the glyph is meant to capture - just that part. Plain text.</summary>
    public string ExampleCapture { get; init; }

    public bool IsEmpty => Summary is null && ExampleDocument is null && ExampleCapture is null;

    /// <summary>Whether <paramref name="documentation"/> has all three: a summary, an example document and an example capture.</summary>
    public static bool IsComplete(GlyphDocumentation documentation) =>
        documentation is { Summary: not null, ExampleDocument: not null, ExampleCapture: not null };

    /// <summary>The doc comment's lines, each starting <c>///</c> - none when there's nothing to say.</summary>
    public IEnumerable<string> ToComment()
    {
        IEnumerable<string> Tag(string tag, string content)
        {
            if (content is null)
                return [];

            var lines = content.Split('\n');

            return lines.Length == 1
                ? [$"/// <{tag}>{lines[0]}</{tag}>"]
                : [$"/// <{tag}>", .. lines.Select(x => x.Length == 0 ? "///" : "/// " + x), $"/// </{tag}>"];
        }

        return Tag(_summaryTag, Summary)
            .Concat(Tag(_exampleDocumentTag, Escape(ExampleDocument)))
            .Concat(Tag(_exampleCaptureTag, Escape(ExampleCapture)));
    }

    /// <summary>
    /// The documentation <paramref name="comment"/> holds - a <c>///</c> doc comment as written in source, indentation and
    /// all - or null when it holds none. Each tag's content is kept a line per line of the comment, without the blank
    /// lines around it or the indentation of each.
    /// </summary>
    /// <exception cref="FormatException">The comment isn't well-formed, or holds something besides the tags it may (see <see cref="Tags"/>).</exception>
    public static GlyphDocumentation FromComment(string comment)
    {
        // The comment's text without its "///"s: a "/// " opening each line, the indentation before it aside.
        var text = string.Join("\n", comment.ReplaceLineEndings("\n").Split('\n')
            .Select(x => x.TrimStart())
            .Select(x => x.StartsWith("///") ? x[3..] : x)
            .Select(x => x.StartsWith(' ') ? x[1..] : x));

        XElement root;

        try
        {
            root = XElement.Parse($"<doc>{text}</doc>", LoadOptions.PreserveWhitespace);
        }
        catch (XmlException e)
        {
            throw new FormatException($"the doc comment isn't well-formed: {e.Message}");
        }

        Dictionary<string, string> contents = [];

        foreach (var node in root.Nodes())
        {
            if (node is XText { Value: var value } && string.IsNullOrWhiteSpace(value))
                continue;

            if (node is not XElement element || !Tags.Contains(element.Name.LocalName) || element.HasAttributes)
                throw new FormatException($"a glyph's doc comment holds only {string.Join(", ", Tags.Select(x => $"<{x}>"))}, but this one also has {Describe(node)}");

            if (!contents.TryAdd(element.Name.LocalName, Normalize(element.Name.LocalName == _summaryTag ? RawSummary(text) : element.Value)))
                throw new FormatException($"the doc comment has more than one <{element.Name.LocalName}>");
        }

        var documentation = new GlyphDocumentation
        {
            Summary = contents.GetValueOrDefault(_summaryTag),
            ExampleDocument = contents.GetValueOrDefault(_exampleDocumentTag),
            ExampleCapture = contents.GetValueOrDefault(_exampleCaptureTag),
        };

        return documentation.IsEmpty ? null : documentation;
    }

    /// <summary>
    /// The <c>&lt;summary&gt;</c>'s markup exactly as written in <paramref name="text"/> - which parsing it would rewrite
    /// (<c>&lt;see cref="X"/&gt;</c> coming back as <c>&lt;see cref="X" /&gt;</c>).
    /// </summary>
    static string RawSummary(string text)
    {
        var start = text.IndexOf($"<{_summaryTag}>", StringComparison.Ordinal) + _summaryTag.Length + 2;
        return text[start..text.IndexOf($"</{_summaryTag}>", start, StringComparison.Ordinal)];
    }

    /// <summary>Each line of <paramref name="content"/> trimmed, and the blank lines around them dropped - null when that leaves nothing.</summary>
    static string Normalize(string content)
    {
        var lines = content.Split('\n').Select(x => x.Trim()).SkipWhile(x => x.Length == 0).Reverse().SkipWhile(x => x.Length == 0).Reverse().ToList();
        return lines.Count == 0 ? null : string.Join("\n", lines);
    }

    static string Escape(string text) =>
        text?.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");

    static string Describe(XNode node) =>
        node switch
        {
            XElement element when element.HasAttributes && Tags.Contains(element.Name.LocalName) => $"attributes on <{element.Name.LocalName}>",
            XElement element => $"<{element.Name.LocalName}>",
            _ => $"text outside any tag (\"{node.ToString().Trim()}\")",
        };
}
