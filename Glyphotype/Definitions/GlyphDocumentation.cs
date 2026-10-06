using System.Net;
using System.Text.RegularExpressions;

namespace Glyphotype.Definitions;

/// <summary>
/// What a glyph's <c>///</c> doc comment says about it - written above its class by <see cref="GlyphSourceWriter"/>,
/// and read back from the comment by <see cref="Read"/>: the one place either is done. None of it is grammar: it
/// doesn't change what the glyph matches, and costs nothing to score.
/// <para>
/// Best effort, and never required: a comment is read for whichever of <c>&lt;summary&gt;</c>, <c>&lt;exampledoc&gt;</c>
/// and <c>&lt;examplecapture&gt;</c> it has, and whatever else it holds - other tags, loose text, a tag left unclosed -
/// is no part of the documentation, but is kept (see <see cref="ReadComment.Rest"/>) so writing the comment again
/// doesn't lose it.
/// </para>
/// </summary>
public sealed record GlyphDocumentation
{
    const string _summaryTag = "summary";
    const string _exampleDocumentTag = "exampledoc";
    const string _exampleCaptureTag = "examplecapture";

    /// <summary>
    /// What the glyph is for: the <c>&lt;summary&gt;</c>'s content, which is doc-comment markup - it may hold
    /// <c>&lt;see cref="..."/&gt;</c>, <c>&lt;c&gt;</c> and the like, and is written as it is.
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

    /// <summary>A doc comment as read: the documentation in it (null when there's none), and the lines of everything else it holds.</summary>
    /// <param name="Rest">The comment's other content, a line per line without its <c>///</c> - empty when it holds nothing else.</param>
    public sealed record ReadComment(GlyphDocumentation Documentation, IReadOnlyList<string> Rest);

    /// <summary>
    /// The doc comment's lines, each starting <c>///</c>: <paramref name="documentation"/>'s tags, then <paramref name="rest"/>
    /// (the rest of a comment it was read from, kept) - none when there's nothing to write.
    /// </summary>
    public static IEnumerable<string> ToComment(GlyphDocumentation documentation, IEnumerable<string> rest = null)
    {
        IEnumerable<string> Tag(string tag, string content)
        {
            if (content is null)
                return [];

            var lines = content.Split('\n');

            return lines.Length == 1
                ? [$"/// <{tag}>{lines[0]}</{tag}>"]
                : [$"/// <{tag}>", .. lines.Select(Line), $"/// </{tag}>"];
        }

        static string Line(string text) => text.Length == 0 ? "///" : "/// " + text;

        return Tag(_summaryTag, documentation?.Summary)
            .Concat(Tag(_exampleDocumentTag, Escape(documentation?.ExampleDocument)))
            .Concat(Tag(_exampleCaptureTag, Escape(documentation?.ExampleCapture)))
            .Concat((rest ?? []).Select(Line));
    }

    /// <summary>
    /// What <paramref name="comment"/> - a <c>///</c> doc comment as written in source, indentation and all - says. Each
    /// tag's content is kept a line per line of the comment, without the blank lines around it or the indentation of
    /// each; where a tag appears more than once, the first is the documentation and the others are the rest.
    /// </summary>
    public static ReadComment Read(string comment)
    {
        // The comment's text without its "///"s: a "/// " opening each line, the indentation before it aside.
        var text = string.Join("\n", (comment ?? "").ReplaceLineEndings("\n").Split('\n')
            .Select(x => x.TrimStart())
            .Select(x => x.StartsWith("///") ? x[3..] : x)
            .Select(x => x.StartsWith(' ') ? x[1..] : x));

        string Take(string tag, bool isText)
        {
            var match = Regex.Match(text, $@"<{tag}>(?<content>.*?)</{tag}>", RegexOptions.Singleline);

            if (!match.Success)
                return null;

            text = text.Remove(match.Index, match.Length);
            var content = Normalize(match.Groups["content"].Value);
            return isText && content is not null ? WebUtility.HtmlDecode(content) : content;
        }

        var documentation = new GlyphDocumentation
        {
            Summary = Take(_summaryTag, isText: false),
            ExampleDocument = Take(_exampleDocumentTag, isText: true),
            ExampleCapture = Take(_exampleCaptureTag, isText: true),
        };

        return new(documentation.IsEmpty ? null : documentation, Lines(text));
    }

    /// <summary>Each line of <paramref name="content"/> trimmed, and the blank lines around them dropped - null when that leaves nothing.</summary>
    static string Normalize(string content)
    {
        var lines = Lines(content);
        return lines.Count == 0 ? null : string.Join("\n", lines);
    }

    /// <summary><paramref name="text"/>'s lines, each trimmed, without blank lines around them or more than one in a row between.</summary>
    static List<string> Lines(string text)
    {
        var lines = new List<string>();

        foreach (var line in text.Split('\n').Select(x => x.Trim()))
            if (line.Length > 0 || lines.Count > 0 && lines[^1].Length > 0)
                lines.Add(line);

        while (lines.Count > 0 && lines[^1].Length == 0)
            lines.RemoveAt(lines.Count - 1);

        return lines;
    }

    static string Escape(string text) =>
        text?.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
}
