using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text.RegularExpressions;

namespace Glyphotype.Distiller.Workbench;

/// <summary>
/// The <c>/// &lt;summary&gt;</c> above a declaration, as <see cref="GlyphDefinition.Summary"/> holds it: the element's
/// content, a line of it per line of the comment, with the comment's <c>///</c> and the space after it - and any blank
/// lines around the content - left out. The inverse of <see cref="GlyphSourceWriter.WriteSummary"/>.
/// </summary>
/// <param name="Summary">The summary, or null when the declaration has none.</param>
/// <param name="Comment">The doc comment it's in, or null when the declaration has none.</param>
/// <param name="HoldsMore">Whether <paramref name="Comment"/> holds anything besides the summary, e.g. <c>&lt;remarks&gt;</c>.</param>
sealed record DocumentationSummary(string Summary, SyntaxTrivia? Comment, bool HoldsMore)
{
    static readonly Regex _summary = new(@"<summary>(?<content>.*?)</summary>", RegexOptions.Singleline);

    public static DocumentationSummary Read(SyntaxNode declaration)
    {
        var comment = declaration.GetLeadingTrivia().LastOrDefault(x => x.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia));

        if (comment == default)
            return new(null, null, false);

        // The comment's text without its "///"s: a "/// " opening each line, the indentation before it aside.
        var text = string.Join("\n", comment.ToFullString().ReplaceLineEndings("\n").Split('\n')
            .Select(x => x.TrimStart())
            .Select(x => x.StartsWith("///") ? x[3..] : x)
            .Select(x => x.StartsWith(' ') ? x[1..] : x)
            .Select(x => x.TrimEnd()));

        var match = _summary.Match(text);

        if (!match.Success)
            return new(null, comment, !string.IsNullOrWhiteSpace(text));

        var lines = match.Groups["content"].Value.Split('\n').Select(x => x.Trim()).ToList();

        while (lines.Count > 0 && lines[0].Length == 0)
            lines.RemoveAt(0);

        while (lines.Count > 0 && lines[^1].Length == 0)
            lines.RemoveAt(lines.Count - 1);

        var holdsMore = !string.IsNullOrWhiteSpace(text.Remove(match.Index, match.Length));

        return new(lines.Count == 0 ? null : string.Join("\n", lines), comment, holdsMore);
    }
}
