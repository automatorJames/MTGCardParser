namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

public record GeneratedRegex
{
    const int _hashSeparatorPadding = 6;
    const int _boxContentLeftPadding = 1;
    const int _spacesPerIndent = 4;
    const string DarkerGrey = "#505050";
    const string DarkGrey = "#808080";
    const string Grey = "#BEBEBE";
    const string LightGrey = "#A9A9A9";
    const string White = "#DCDCDC";

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
            var colorSpans = new Dictionary<int, string> { [0] = White };

            int indentSpaces = GetIndentDepth(line) * _spacesPerIndent;
            var indentedRegex = new string(' ', indentSpaces) + line.Regex;
            var paddedRegex = indentedRegex.PadRight(CommentColumn);
            var commentPrefix = $"#{new string(' ', _hashSeparatorPadding)}";

            // Rule: The position of the hash separator "#" should be a darker grey for all lines.
            colorSpans[CommentColumn] = DarkerGrey;

            var (commentBody, commentSpans) = GetFormattedCommentAndColorSpans(line);

            int commentOffset = paddedRegex.Length + commentPrefix.Length;
            foreach (var span in commentSpans)
            {
                colorSpans[commentOffset + span.Key] = span.Value;
            }
            CommentedLines.Add(new(paddedRegex, commentPrefix + commentBody, colorSpans));
        }
    }

    /// <summary>
    /// Gets the consistent color for all border elements of an enclosure based on its treatment type.
    /// </summary>
    private string GetBorderColor(GroupBorderTreatment treatment, Palette palette) => treatment switch
    {
        GroupBorderTreatment.ClosedBox => palette.Hex,
        GroupBorderTreatment.DashedBox => palette.HexDark,
        GroupBorderTreatment.Brace => DarkGrey,
        _ => DarkGrey
    };

    private (string, Dictionary<int, string>) GetFormattedCommentAndColorSpans(RegexTemplateLine line)
    {
        var sb = new StringBuilder();
        var spans = new Dictionary<int, string>();

        Action<string, string> append = (text, color) => {
            if (string.IsNullOrEmpty(text)) return;
            // Only add a new span if the color is different from the last one
            if (!spans.Any() || spans[spans.Keys.Last()] != color)
            {
                spans[sb.Length] = color;
            }
            sb.Append(text);
        };

        // Rule: TextLine comments should be white.
        // Rule: Boundary comments should be dark grey.
        if (line.Enclosures.Length == 0)
        {
            string color = line is TextLine ? White : DarkGrey;
            append(line.Comment ?? string.Empty, color);
            return (sb.ToString(), spans);
        }

        var parentEnclosures = line.Enclosures.Take(line.Enclosures.Length - 1);
        var currentEnclosure = line.Enclosures.Last();
        var chars = BoxChars.Get(currentEnclosure.Treatment);
        var palette = currentEnclosure.Palette;
        string currentBorderColor = GetBorderColor(currentEnclosure.Treatment, palette);

        // 1. Build Prefix (Walls from inner to outer)
        foreach (var parent in parentEnclosures)
        {
            char wall = BoxChars.Get(parent.Treatment).Wall;
            string parentBorderColor = GetBorderColor(parent.Treatment, parent.Palette);
            append(wall.ToString(), parentBorderColor);
            append(" ", White);
        }

        // 2. Build Core Content
        int parentDepth = parentEnclosures.Count();
        int currentLevelWidth = CommentBoxLength - (parentDepth * 4);
        bool isBookend = line is GroupOpen or GroupClose or NamedGroupOpen or NamedGroupClose;

        if (isBookend)
        {
            int availableWidth = currentLevelWidth - 2;
            switch (line)
            {
                case NamedGroupOpen ngo:
                    string openComment = $" {ngo.Comment} ";
                    string fillerOpen = new string(chars.Top, Math.Max(0, availableWidth - openComment.Length));
                    append(chars.TopLeft.ToString(), currentBorderColor);
                    append(openComment, palette.HexSat); // Rule: NamedGroupOpen comments are HexSat
                    append(fillerOpen, currentBorderColor);
                    append(chars.TopRight.ToString(), currentBorderColor);
                    break;
                case GroupOpen:
                    append(chars.TopLeft.ToString(), currentBorderColor);
                    append(new string(chars.Top, availableWidth), currentBorderColor);
                    append(chars.TopRight.ToString(), currentBorderColor);
                    break;
                case NamedGroupClose ngc:
                    string closeComment = $" {ngc.Comment} ";
                    string fillerClose = new string(chars.Bottom, Math.Max(0, availableWidth - closeComment.Length));
                    append(chars.BottomLeft.ToString(), currentBorderColor);
                    append(fillerClose, currentBorderColor);
                    append(closeComment, palette.HexSat); // Rule: NamedGroupClose comments are HexSat
                    append(chars.BottomRight.ToString(), currentBorderColor);
                    break;
                case GroupClose gc:
                    string quantComment = gc.Comment != null ? $" {gc.Comment} " : "";
                    string fillerQuant = new string(chars.Bottom, Math.Max(0, availableWidth - quantComment.Length));
                    append(chars.BottomLeft.ToString(), currentBorderColor);
                    append(fillerQuant, currentBorderColor);
                    append(quantComment, DarkGrey); // Rule: GroupClose comments are dark grey
                    append(chars.BottomRight.ToString(), currentBorderColor);
                    break;
                default:
                    append(new string(' ', currentLevelWidth), White);
                    break;
            }
        }
        else // Not a bookend line (e.g., TextLine, AlternateValue)
        {
            int innerWidth = currentLevelWidth - 4;
            append(chars.Wall.ToString(), currentBorderColor);
            append(" ", White);

            switch (line)
            {
                case AlternateValue av:
                    string altComment = $" {av.Comment} ";
                    int totalPad = Math.Max(0, innerWidth - altComment.Length);
                    append(new string(' ', totalPad / 2), White);
                    append(altComment, palette.HexLight); // Rule: AlternateValue comments are HexLight
                    append(new string(' ', totalPad - (totalPad / 2)), White);
                    break;
                default:
                    string textColor = White; // Default to white
                    // Rule: if a TextLine or SpaceLine has a NamedEnclosure parent, color its comment
                    if (line is TextLine or SpaceLine or GroupAlternativePipe)
                    {
                        var nearestNamedEnclosure = line.Enclosures.LastOrDefault(e => e is NamedEnclosure) as NamedEnclosure;
                        if (nearestNamedEnclosure != null)
                        {
                            textColor = nearestNamedEnclosure.Palette.HexLight;
                        }
                    }

                    var content = string.IsNullOrEmpty(line.Comment)
                        ? new string(' ', innerWidth)
                        : (new string(' ', _boxContentLeftPadding) + line.Comment).PadRight(innerWidth);
                    append(content, textColor);
                    break;
            }

            append(" ", White);
            append(chars.Wall.ToString(), currentBorderColor);
        }

        // 3. Build Suffix (Walls from outer to inner)
        foreach (var parent in parentEnclosures.Reverse())
        {
            char wall = BoxChars.Get(parent.Treatment).Wall;
            string parentBorderColor = GetBorderColor(parent.Treatment, parent.Palette);
            append(" ", White);
            append(wall.ToString(), parentBorderColor);
        }

        return (sb.ToString(), spans);
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