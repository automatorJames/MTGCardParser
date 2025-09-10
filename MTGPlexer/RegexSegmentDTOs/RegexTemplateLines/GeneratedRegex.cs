using System.Text.RegularExpressions;

namespace MTGPlexer.RegexSegmentDTOs.RegexTemplateLines;

public record GeneratedRegex
{
    const int _hashSeparatorPadding = 2;
    const int _alternateIndent = 3;

    public List<RegexCommentedLine> CommentedLines { get; private set; } = [];
    public string FormattedRegex { get; set; }
    public string MinifiedRegex { get; set; }
    public int HashSeparatorColumn { get; private set; }
    public int CommentColumn { get; private set; }
    public int CommentBoxLength { get; private set; }

    public GeneratedRegex(List<RegexTemplateLine> lines)
    {
        if (lines == null || !lines.Any())
            return;

        CalculateColumnWidths(lines);
        FormatCommentedLines(lines);
        FormattedRegex = string.Join(Environment.NewLine, CommentedLines.Select(x => x.FormattedText));
        var regexWithoutComments = string.Join("", CommentedLines.Select(x => x.Regex));
        MinifiedRegex = MinifyRegex(regexWithoutComments);
    }

    /// <summary>
    /// Builds the final formatted string for each line, aligning the regex, hash separator, and comments into columns.
    /// </summary>
    void FormatCommentedLines(List<RegexTemplateLine> templateLines)
    {
        foreach (var line in templateLines)
        {
            // Pad the regex part so its total length reaches the column where the '#' and comment will start.
            var regex = line.IndentedValue.PadRight(CommentColumn);

            // The comment string starts with a '#' followed by padding.
            var commentPrefix = $"#{new string(' ', _hashSeparatorPadding)}";

            // Generate the main body of the comment (which could be a plain comment or a unicode box).
            var commentBody = GetCommentBodyForLine(line);

            CommentedLines.Add(new(regex, commentPrefix + commentBody, line.Palette));
        }
    }

    /// <summary>
    // Determines the appropriate comment body string for a given template line.
    /// </summary>
    private string GetCommentBodyForLine(RegexTemplateLine line)
    {
        return line switch
        {
            NamedGroupOpen namedGroupOpen => FormatNamedGroupOpenComment(namedGroupOpen),
            AlternateValue alternateValue => FormatAlternateValueComment(alternateValue),
            GroupClose groupClose when !string.IsNullOrEmpty(groupClose.CommentTwo) => FormatGroupCloseComment(groupClose),
            _ => line.CommentOne ?? string.Empty,
        };
    }

    /// <summary>
    /// Formats a comment for a named group opening, like: ┌ Group Name : Type ┐
    /// </summary>
    private string FormatNamedGroupOpenComment(NamedGroupOpen namedGroupOpen)
    {
        string leftContent = $" {namedGroupOpen.CommentOne} ";
        string rightContent = $" {namedGroupOpen.CommentTwo} ";

        // Calculate filler needed to span the full box width, accounting for Unicode box chars (┌, ┐) and content.
        int fillerLength = CommentBoxLength - 2 - leftContent.Length - rightContent.Length;
        string filler = new string('─', Math.Max(0, fillerLength));

        return $"┌{leftContent}{filler}{rightContent}┐";
    }

    /// <summary>
    /// Formats a comment for an alternate value, like: │   match         │
    /// </summary>
    private string FormatAlternateValueComment(AlternateValue alternateValue)
    {
        string content = $"{new string(' ', _alternateIndent)}{alternateValue.CommentOne}";

        // Calculate filler needed to span the full box width, accounting for Unicode box chars (│, │) and content.
        int fillerLength = CommentBoxLength - 2 - content.Length;
        string filler = new string(' ', Math.Max(0, fillerLength));

        return $"│{content}{filler}│";
    }

    /// <summary>
    /// Formats a comment for a group closing, like: └──────── Group Name ┘
    /// </summary>
    private string FormatGroupCloseComment(GroupClose groupClose)
    {
        string content = $" {groupClose.CommentTwo} ";

        // Calculate filler needed to span the full box width, accounting for Unicode box chars (└, ┘) and content.
        int fillerLength = CommentBoxLength - 2 - content.Length;
        string filler = new string('─', Math.Max(0, fillerLength));

        return $"└{filler}{content}┘";
    }

    void CalculateColumnWidths(List<RegexTemplateLine> lines)
    {
        HashSeparatorColumn = lines.Max(x => x.End) + _hashSeparatorPadding;
        CommentColumn = HashSeparatorColumn + _hashSeparatorPadding;

        var longestGroupNamePlusTypeWithPadding = lines
            .OfType<NamedGroupOpen>()
            .Select(x => x.CommentOneLength + x.CommentTwoLength)
            .DefaultIfEmpty()
            .Max()
            + 2 // top left corner and top right corner
            + 1 // at least one '─' char between name and type
            + 4; // spacing for both sides of both comment parts

        CommentBoxLength = longestGroupNamePlusTypeWithPadding;
    }

    /// <summary>
    /// Removes all non-essential whitespace from a regex pattern, preserving literal spaces indicated by "[ ]".
    /// </summary>
    string MinifyRegex(string pattern)
    {
        if (string.IsNullOrEmpty(pattern)) return string.Empty;

        // This regex matches one of two things:
        // 1. (\\[ \\]): The literal sequence "[ ]", captured in Group 1. This is the token we want to preserve.
        // 2. (\\s+): Any sequence of one or more whitespace characters, captured in Group 2. This is the whitespace we want to remove.
        return Regex.Replace(pattern, @"(\[\ \])|(\s+)", match =>
        {
            // If Group 1 succeeded, we matched "[ ]". Replace it with a single literal space.
            if (match.Groups[1].Success)
            {
                return " ";
            }

            // Otherwise, Group 2 must have succeeded. We matched disposable whitespace, so replace it with nothing.
            return string.Empty;
        });
    }
}