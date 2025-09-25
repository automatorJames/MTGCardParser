namespace MTGPlexer.RegexGeneration.RegexTemplateLines.FormattedLines;

public record FormattedRegex
{
    private readonly FormattedRegexColoringRules _colors = new();

    const string DefaultWhite = "#FFFFFF";
    private const int _hashSeparatorPadding = 6;
    private const int _boxContentLeftPadding = 1;
    private const int _spacesPerIndent = 4;

    public List<RegexCommentedLine> CommentedLines { get; private set; } = [];
    public string PrettifiedRegex { get; set; }
    public string MinifiedRegex { get; set; }
    public int HashSeparatorColumn { get; private set; }
    public int CommentBoxLength { get; private set; }

    public RegexCommentedAlternateLine this[string pathToTerminalProp, object terminalValue]
    {
        get
        {
            var alternateLinesAtPath = CommentedLines
                .OfType<RegexCommentedAlternateLine>()
                .Where(x => x.EnclosurePath == pathToTerminalProp);

            return alternateLinesAtPath.FirstOrDefault(x => x.CanonicalValue.Equals(terminalValue));
        }
    }

    public FormattedRegex(List<RegexTemplateLine> lines)
    {
        if (lines == null || !lines.Any())
            return;

        CalculateColumnWidths(lines);
        FormatCommentedLines(lines);
        PrettifiedRegex = string.Join(Environment.NewLine, CommentedLines.Select(x => x.FormattedText));
        MinifiedRegex = MinifyRegex(string.Join("", lines.Select(x => x.Regex)));
    }

    void FormatCommentedLines(List<RegexTemplateLine> templateLines)
    {
        for (int i = 0; i < templateLines.Count; i++)
        {
            RegexTemplateLine line = templateLines[i];

            // Calculate comment body, its spans, AND the primary color for the regex text
            var (commentBody, commentSpans, regexPrimaryColor) = GetFormattedCommentAndColorSpans(line);

            // Initialize colorSpans with the dynamically determined primary color for the regex text
            var colorSpans = new Dictionary<int, string> { [0] = regexPrimaryColor };

            int indentSpaces = GetIndentDepth(line) * _spacesPerIndent;
            var indentedRegex = new string(' ', indentSpaces) + line.Regex;
            var paddedRegex = indentedRegex.PadRight(HashSeparatorColumn);
            var commentPrefix = $"#{new string(' ', _hashSeparatorPadding)}";

            // Set the hash separator color
            colorSpans[HashSeparatorColumn] = _colors.HashSeparatorColor;

            int commentOffset = paddedRegex.Length + commentPrefix.Length;

            foreach (var span in commentSpans)
                colorSpans[commentOffset + span.Key] = span.Value;

            RegexCommentedLine commentedLine = line is IMatchableAlternate matchableAlt
                ? new RegexCommentedAlternateLine(paddedRegex, commentPrefix + commentBody, line.NamedPath, i, colorSpans, matchableAlt)
                : new RegexCommentedLine(paddedRegex, commentPrefix + commentBody, line.NamedPath, i, colorSpans);

            CommentedLines.Add(commentedLine);
        }
    }

    /// <summary>
    /// Gets the formatted comment body, its color spans, and the primary content color
    /// that should be used for the regex text itself on the left.
    /// </summary>
    private (string commentBody, Dictionary<int, string> commentSpans, string primaryContentColor) GetFormattedCommentAndColorSpans(RegexTemplateLine line)
    {
        var sb = new StringBuilder();
        var spans = new Dictionary<int, string>();
        string currentPrimaryContentColor = _colors.DefaultRegexTextColor; // Default if no specific comment color found

        Action<string, string> append = (text, color) => {
            if (string.IsNullOrEmpty(text)) return;
            if (!spans.Any() || spans.Last().Value != color)
            {
                spans[sb.Length] = color;
            }
            sb.Append(text);
        };

        if (line.PropEnclosures.Length == 0)
        {
            // Determine primary content color for unenclosed lines
            currentPrimaryContentColor = line switch
            {
                TextLine => _colors.UnenclosedTextLineCommentColor,
                SpaceLine => _colors.UnenclosedSpaceLineCommentColor,
                BoundaryBase => _colors.BoundaryCommentColor,
                _ => _colors.DefaultFallbackColor
            };
            append(line.Comment ?? string.Empty, currentPrimaryContentColor);
            return (sb.ToString(), spans, currentPrimaryContentColor);
        }

        var parentEnclosures = line.PropEnclosures.Take(line.PropEnclosures.Length - 1);
        var currentEnclosure = line.PropEnclosures.Last();
        var chars = BoxChars.Get(currentEnclosure.Treatment);
        var palette = currentEnclosure.Palette;
        string currentBorderColor = _colors.GetBorderColor(currentEnclosure.Treatment, palette);

        foreach (var parent in parentEnclosures)
        {
            char wall = BoxChars.Get(parent.Treatment).Wall;
            string parentBorderColor = _colors.GetBorderColor(parent.Treatment, parent.Palette);
            append(wall.ToString(), parentBorderColor);
            append(" ", DefaultWhite); // Padding spaces between walls are white
        }

        int parentDepth = parentEnclosures.Count();
        int currentLevelWidth = CommentBoxLength - (parentDepth * 4);

        if (line is EncloureBookend)
        {
            int availableWidth = currentLevelWidth - 2;
            switch (line)
            {
                case NamedGroupOpen ngo:
                    currentPrimaryContentColor = _colors.NamedGroupBookendCommentColor(palette);
                    string openComment = $" {ngo.Comment} ";
                    string fillerOpen = new string(chars.Top, Math.Max(0, availableWidth - openComment.Length));
                    append(chars.TopLeft.ToString(), currentBorderColor);
                    append(openComment, currentPrimaryContentColor);
                    append(fillerOpen, currentBorderColor);
                    append(chars.TopRight.ToString(), currentBorderColor);
                    break;
                case GroupOpen:
                    // GroupOpen has no "inner content" comment text. Its comment section is purely structural.
                    // We'll treat the padding/default content as white for the regex text side.
                    currentPrimaryContentColor = DefaultWhite;
                    append(chars.TopLeft.ToString(), currentBorderColor);
                    append(new string(chars.Top, availableWidth), currentBorderColor);
                    append(chars.TopRight.ToString(), currentBorderColor);
                    break;
                case NamedGroupClose ngc:
                    currentPrimaryContentColor = _colors.NamedGroupBookendCommentColor(palette);
                    string closeComment = $" {ngc.Comment} ";
                    string fillerClose = new string(chars.Bottom, Math.Max(0, availableWidth - closeComment.Length));
                    append(chars.BottomLeft.ToString(), currentBorderColor);
                    append(fillerClose, currentBorderColor);
                    append(closeComment, currentPrimaryContentColor);
                    append(chars.BottomRight.ToString(), currentBorderColor);
                    break;
                case GroupClose gc:
                    currentPrimaryContentColor = _colors.GroupCloseQuantifierColor;
                    string quantComment = gc.Comment != null ? $" {gc.Comment} " : "";
                    string fillerQuant = new string(chars.Bottom, Math.Max(0, availableWidth - quantComment.Length));
                    append(chars.BottomLeft.ToString(), currentBorderColor);
                    append(fillerQuant, currentBorderColor);
                    append(quantComment, currentPrimaryContentColor);
                    append(chars.BottomRight.ToString(), currentBorderColor);
                    break;
                default:
                    currentPrimaryContentColor = DefaultWhite; // Default for other bookends with no specific comment
                    append(new string(' ', currentLevelWidth), DefaultWhite);
                    break;
            }
        }
        else // Not a bookend line (e.g., TextLine, AlternateValue, etc. inside an enclosure)
        {
            int innerWidth = currentLevelWidth - 4;
            append(chars.Wall.ToString(), currentBorderColor);
            append(" ", DefaultWhite); // Space between wall and inner content is white

            switch (line)
            {
                case AlternateValue av:
                    currentPrimaryContentColor = _colors.AlternateValueCommentColor(palette);
                    string altComment = $" {av.Comment} ";
                    int totalPad = Math.Max(0, innerWidth - altComment.Length);
                    append(new string(' ', totalPad / 2), DefaultWhite);
                    append(altComment, currentPrimaryContentColor);
                    append(new string(' ', totalPad - (totalPad / 2)), DefaultWhite);
                    break;
                default:
                    // Rule: if a TextLine or SpaceLine or GroupAlternativePipe has a NamedEnclosure parent, 
                    // color its comment, otherwise default to White.
                    string commentContentColor = DefaultWhite;
                    if (line is TextLine or SpaceLine or GroupAlternativePipe)
                    {
                        var nearestNamedEnclosure = line.PropEnclosures.LastOrDefault(e => e is NamedEnclosure) as NamedEnclosure;
                        if (nearestNamedEnclosure != null)
                        {
                            commentContentColor = _colors.EnclosedTextColor(nearestNamedEnclosure.Palette);
                        }
                    }
                    currentPrimaryContentColor = commentContentColor; // This is the inner content color

                    var content = string.IsNullOrEmpty(line.Comment)
                        ? new string(' ', innerWidth)
                        : (new string(' ', _boxContentLeftPadding) + line.Comment).PadRight(innerWidth);
                    append(content, currentPrimaryContentColor);
                    break;
            }

            append(" ", DefaultWhite); // Space between inner content and wall is white
            append(chars.Wall.ToString(), currentBorderColor);
        }

        // 3. Build Suffix (Walls from outer to inner)
        foreach (var parent in parentEnclosures.Reverse())
        {
            char wall = BoxChars.Get(parent.Treatment).Wall;
            string parentBorderColor = _colors.GetBorderColor(parent.Treatment, parent.Palette);
            append(" ", DefaultWhite); // Padding space is white
            append(wall.ToString(), parentBorderColor);
        }

        return (sb.ToString(), spans, currentPrimaryContentColor);
    }

    void CalculateColumnWidths(List<RegexTemplateLine> lines)
    {
        int maxRegexLen = lines.Any() ? lines.Max(x => (GetIndentDepth(x) * _spacesPerIndent) + x.Regex.Length) : 0;
        HashSeparatorColumn = maxRegexLen + _hashSeparatorPadding;

        var uniquePaths = lines
            .SelectMany(l => l.PropEnclosures.Select((e, i) => l.PropEnclosures.Take(i + 1)))
            .GroupBy(p => string.Join(",", p.Select(e => e.Ordinal)))
            .Select(g => g.First())
            .Where(p => p.Any())
            .ToList();

        var boxWidths = uniquePaths.ToDictionary(p => string.Join(",", p.Select(e => e.Ordinal)), p => 0);

        // Pass 1: Determine the minimum content width required by each box for its own lines.
        foreach (var line in lines.Where(l => l.PropEnclosures.Any()))
        {
            string pathKey = string.Join(",", line.PropEnclosures.Select(e => e.Ordinal));
            int requiredWidth = 0;
            string comment = line.Comment;

            if (!string.IsNullOrEmpty(comment))
            {
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

                requiredWidth = line is EncloureBookend ? textWidth + 2 : textWidth + 4;
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
        if (line.PropEnclosures.Length == 0) 
            return 0;

        return line is EncloureBookend ? line.PropEnclosures.Length - 1 : line.PropEnclosures.Length;
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
        private static readonly BoxCharSet Dashed = new('┌', '┐', '└', '┘', '─', '─', '┆');
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