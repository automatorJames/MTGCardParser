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

    void FormatCommentedLines(List<RegexTemplateLine> templateLines)
    {
        foreach (var line in templateLines)
        {
            var regex = line.IndentedValue;
            var comment = "#" + string.Empty.PadLeft(_hashSeparatorPadding);

            if (line is NamedGroupOpen namedGroupOpen)
            {
                var firstPart = "┌" + " " + namedGroupOpen.CommentOne + " ";
                var secondPart = " " + namedGroupOpen.CommentTwo + " " + "┐";
                var spacesToFill = CommentBoxLength - firstPart.Length - secondPart.Length;
                comment += firstPart + string.Empty.PadLeft(spacesToFill, '─');
            }
            else if (line is AlternateValue alternateValue)
            {
                var formattedLine = "│" + string.Empty.PadLeft(_alternateIndent) + alternateValue.CommentOne;
                comment += formattedLine.PadRight(CommentBoxLength - formattedLine.Length, ' ') + "│";
            }
            else if (line is GroupClose groupClose && groupClose.CommentTwo is string closeComment)
            {
                var spacesToFill = CommentBoxLength - closeComment.Length + 4; // spacing for both sides of comment plus corners
                comment += "└" + string.Empty.PadLeft(spacesToFill, '─') + " " + closeComment + " " + "┘";

            }
            else
                comment += line.CommentOne;

            CommentedLines.Add(new(regex, comment, line.Palette));
        }


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

    string MinifyRegex(string pattern)
    {
        if (pattern is null) return string.Empty;

        const string sentinel = "\uE000SPACE\uE000"; // unlikely to appear in user patterns

        // 1) Protect literal "[ ]" tokens
        string protectedPattern = Regex.Replace(pattern, @"\[\ \]", sentinel);

        // 2) Collapse/remove all whitespace everywhere else
        string noWhitespace = Regex.Replace(protectedPattern, @"\s+", "");

        // 3) Replace protected "[ ]" tokens with a single literal space
        return Regex.Replace(noWhitespace, Regex.Escape(sentinel), " ");
    }
}

