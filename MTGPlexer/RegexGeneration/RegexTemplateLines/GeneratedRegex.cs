namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

public record GeneratedRegex
{
    const int _hashSeparatorPadding = 6;
    const int _boxContentLeftPadding = 1;
    const int _spacesPerIndent = 4;

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
        MinifiedRegex = MinifyRegex(string.Join("", lines.Select(x => x.Regex)));
    }

    void FormatCommentedLines(List<RegexTemplateLine> templateLines)
    {
        foreach (var line in templateLines)
        {
            int indentSpaces = GetIndentDepth(line) * _spacesPerIndent;
            var indentedRegex = new string(' ', indentSpaces) + line.Regex;
            var paddedRegex = indentedRegex.PadRight(CommentColumn);
            var commentPrefix = $"#{new string(' ', _hashSeparatorPadding)}";
            var commentBody = GetFormattedComment(line);
            CommentedLines.Add(new(paddedRegex, commentPrefix + commentBody, line.Palette));
        }
    }

    private string GetFormattedComment(RegexTemplateLine line)
    {
        if (line.Enclosures.Length == 0) return line.Comment ?? string.Empty;

        var parentEnclosures = line.Enclosures.Take(line.Enclosures.Length - 1);
        var prefix = new StringBuilder();
        var suffix = new StringBuilder();
        foreach (var parent in parentEnclosures)
        {
            char wall = BoxChars.Get(parent.Treatment).Wall;
            prefix.Append($"{wall} ");
            suffix.Insert(0, $" {wall}");
        }

        int parentDepth = parentEnclosures.Count();
        int currentLevelWidth = CommentBoxLength - (parentDepth * 4);
        string coreContent;
        var currentEnclosure = line.Enclosures.Last();
        var chars = BoxChars.Get(currentEnclosure.Treatment);
        bool isBookend = line is GroupOpen or GroupClose or NamedGroupOpen or NamedGroupClose;

        if (isBookend)
        {
            int availableWidth = currentLevelWidth - 2;
            switch (line)
            {
                case NamedGroupOpen ngo:
                    string openComment = $" {ngo.Comment} ";
                    string fillerOpen = new string(chars.Top, Math.Max(0, availableWidth - openComment.Length));
                    coreContent = $"{chars.TopLeft}{openComment}{fillerOpen}{chars.TopRight}";
                    break;
                case GroupOpen:
                    coreContent = $"{chars.TopLeft}{new string(chars.Top, availableWidth)}{chars.TopRight}";
                    break;
                case NamedGroupClose ngc:
                    string closeComment = $" {ngc.Comment} ";
                    string fillerClose = new string(chars.Bottom, Math.Max(0, availableWidth - closeComment.Length));
                    coreContent = $"{chars.BottomLeft}{fillerClose}{closeComment}{chars.BottomRight}";
                    break;
                case GroupClose gc:
                    string quantComment = gc.Comment != null ? $" {gc.Comment} " : "";
                    string fillerQuant = new string(chars.Bottom, Math.Max(0, availableWidth - quantComment.Length));
                    coreContent = $"{chars.BottomLeft}{fillerQuant}{quantComment}{chars.BottomRight}";
                    break;
                default:
                    coreContent = new string(' ', currentLevelWidth);
                    break;
            }
        }
        else
        {
            int innerWidth = currentLevelWidth - 4;
            string textContent;
            switch (line)
            {
                case AlternateValue av:
                    string altComment = $" {av.Comment} ";
                    int totalPad = Math.Max(0, innerWidth - altComment.Length);
                    textContent = $"{new string(' ', totalPad / 2)}{altComment}{new string(' ', totalPad - (totalPad / 2))}";
                    break;
                default:
                    if (string.IsNullOrEmpty(line.Comment))
                    {
                        textContent = new string(' ', innerWidth);
                    }
                    else
                    {
                        textContent = (new string(' ', _boxContentLeftPadding) + line.Comment).PadRight(innerWidth);
                    }
                    break;
            }
            coreContent = $"{chars.Wall} {textContent} {chars.Wall}";
        }
        return prefix + coreContent + suffix;
    }

    void CalculateColumnWidths(List<RegexTemplateLine> lines)
    {
        int maxRegexLen = lines.Any() ? lines.Max(l => GetIndentDepth(l) * _spacesPerIndent + l.Regex.Length) : 0;
        HashSeparatorColumn = maxRegexLen + _hashSeparatorPadding;
        CommentColumn = HashSeparatorColumn + _hashSeparatorPadding;

        var uniquePaths = lines
            .SelectMany(l => l.Enclosures.Select((e, i) => l.Enclosures.Take(i + 1)))
            .GroupBy(p => string.Join(",", p.Select(e => e.Ordinal)))
            .Select(g => g.First())
            .Where(p => p.Any())
            .ToList();

        var boxWidths = uniquePaths.ToDictionary(p => string.Join(",", p.Select(e => e.Ordinal)), p => 0);

        // Pass 1: Determine the minimum content width required by each box for its own lines.
        foreach (var line in lines.Where(l => l.Enclosures.Any()))
        {
            string pathKey = string.Join(",", line.Enclosures.Select(e => e.Ordinal));
            int requiredWidth = 0;
            string comment = line.Comment;

            if (!string.IsNullOrEmpty(comment))
            {
                bool isBookend = line is GroupOpen or GroupClose or NamedGroupOpen or NamedGroupClose;
                int textWidth;

                switch (line)
                {
                    case AlternateValue or NamedGroupOpen or NamedGroupClose or GroupClose:
                        textWidth = comment.Length + 2; // For " comment "
                        break;
                    default:
                        textWidth = _boxContentLeftPadding + comment.Length; // For " comment"
                        break;
                }

                requiredWidth = isBookend ? textWidth + 2 : textWidth + 4;
            }
            boxWidths[pathKey] = Math.Max(boxWidths[pathKey], requiredWidth);
        }

        // Pass 2: Propagate widths upwards. A parent must be wide enough to contain its children's boxes.
        var sortedPaths = uniquePaths.OrderByDescending(p => p.Count());
        foreach (var path in sortedPaths)
        {
            if (path.Count() <= 1) continue;
            string childPathKey = string.Join(",", path.Select(e => e.Ordinal));
            string parentPathKey = string.Join(",", path.Take(path.Count() - 1).Select(e => e.Ordinal));
            int childFootprint = boxWidths[childPathKey] + 4; // Child's box width + parent's walls
            boxWidths[parentPathKey] = Math.Max(boxWidths[parentPathKey], childFootprint);
        }

        // Pass 3: Find the maximum width among all root-level boxes.
        var rootPaths = uniquePaths.Where(p => p.Count() == 1);
        CommentBoxLength = rootPaths.Any() ? rootPaths.Max(p => boxWidths[string.Join(",", p.Select(e => e.Ordinal))]) : 0;
    }

    private int GetIndentDepth(RegexTemplateLine line)
    {
        if (line.Enclosures.Length == 0) return 0;
        bool isBookend = line is GroupOpen or GroupClose or NamedGroupOpen or NamedGroupClose;
        return isBookend ? line.Enclosures.Length - 1 : line.Enclosures.Length;
    }

    private string MinifyRegex(string pattern)
    {
        string placeholder = Guid.NewGuid().ToString();
        string protectedPattern = pattern.Replace("[ ]", placeholder);
        string strippedPattern = Regex.Replace(protectedPattern, @"\s", "");
        return strippedPattern.Replace(placeholder, " ");
    }

    private record BoxCharSet(char TopLeft, char TopRight, char BottomLeft, char BottomRight, char Top, char Bottom, char Wall);

    private static class BoxChars
    {
        private static readonly BoxCharSet Closed = new('┌', '┐', '└', '┘', '─', '─', '│');
        private static readonly BoxCharSet Dashed = new('┌', '┐', '└', '┘', '─', '─', '╎');
        private static readonly BoxCharSet Brace = new('╭', '╮', '╰', '╯', ' ', ' ', '┊');

        public static BoxCharSet Get(GroupBorderTreatment treatment) => treatment switch
        {
            GroupBorderTreatment.ClosedBox => Closed,
            GroupBorderTreatment.DashedBox => Dashed,
            GroupBorderTreatment.Brace => Brace,
            _ => Closed,
        };
    }
}