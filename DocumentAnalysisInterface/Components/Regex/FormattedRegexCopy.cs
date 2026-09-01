using Glyphotype.PresentationRules;
using Glyphotype.RegexGeneration.Presentation;

namespace DocumentAnalysisInterface.Components.Regex;

/// <summary>
/// What a formatted-regex copy button copies: the rendered lines with each line's "  #  "-style comment
/// separator and everything to its right stripped off, leaving the bare extended/whitespace regex a
/// copy-paster downstream typically wants without the commentary. Shared by GlyphRegexPage's cards and
/// DebugRegexDialog's preview windows so both copy identically. Done via plain string manipulation
/// (rather than a Glyphotype rendering option) since this is purely a "what the copy button copies"
/// GUI concern.
/// </summary>
public static class FormattedRegexCopy
{
    public static string GetCopyText(List<SmartLine> lines) =>
        StripCommentColumn(string.Join("\r\n", lines));

    /// <summary>
    /// Cuts each line at its first occurrence of <see cref="SmartRegexStaticRules.CommentBorderLineWithBuffer"/>
    /// (dropping the comment separator and everything after it), then trims trailing padding spaces per
    /// line - left indentation is kept, only the right-hand padding that lined the comment column up goes.
    /// A line with no separator (e.g. a blank spacer line) is just trimmed as-is.
    /// </summary>
    public static string StripCommentColumn(string formattedRegex)
    {
        var lines = formattedRegex.Split(["\r\n"], StringSplitOptions.None);

        for (int i = 0; i < lines.Length; i++)
        {
            var separatorIndex = lines[i].IndexOf(SmartRegexStaticRules.CommentBorderLineWithBuffer, StringComparison.Ordinal);
            var line = separatorIndex >= 0 ? lines[i][..separatorIndex] : lines[i];
            lines[i] = line.TrimEnd();
        }

        return string.Join("\r\n", lines);
    }
}
