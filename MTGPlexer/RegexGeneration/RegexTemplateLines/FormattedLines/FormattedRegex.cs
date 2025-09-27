namespace MTGPlexer.RegexGeneration.RegexTemplateLines.FormattedLines;

public class FormattedRegex
{
    private readonly FormattedRegexColoringRules _colors = new();
    private readonly FormattedRegexTreatmentRules _treatments = new();

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
            var spans = new List<RegexCommentedLineSpan>();

            // 1. REGEX PART (Left of '#')
            int indentSpaces = GetIndentDepth(line) * _spacesPerIndent;
            var indentedRegex = new string(' ', indentSpaces) + line.Regex;
            var paddedRegex = indentedRegex.PadRight(HashSeparatorColumn);

            var primaryContentColor = GetPrimaryContentColorForLine(line);
            var primaryContentPalette = DeterministicPalette.GetStaticPalette(new HexColor(primaryContentColor));
            var highlightTreatment = _treatments.GetRegexHighlightTreatment(line);

            string pathForRegexSpan = line.NamedPath;
            if (line is IMatchableAlternate alt)
            {
                pathForRegexSpan = $"{pathForRegexSpan}.{alt.CanonicalValue}";
            }
            string relativePath = RegexCommentedLine.GetRelativePath(pathForRegexSpan);

            if (relativePath == null)
            {
                highlightTreatment = SpanHighlightTreatment.None;
            }

            spans.Add(new RegexCommentedLineSpan(
                SpanText: paddedRegex,
                Palette: primaryContentPalette,
                PathRelativeToRoot: relativePath,
                HighlightTreatment: highlightTreatment,
                // --- MODIFIED LOGIC: Apply the same lowlight rule to the left side ---
                LowlightTreatment: _treatments.CommentLowlightTreatment
            ));

            // 2. COMMENT PART (Right of '#')
            var commentPrefix = $"#{new string(' ', _hashSeparatorPadding)}";
            var hashPalette = DeterministicPalette.GetStaticPalette(new HexColor(_colors.HashSeparatorColor));

            spans.Add(new RegexCommentedLineSpan(
                SpanText: commentPrefix,
                Palette: hashPalette,
                PathRelativeToRoot: null,
                HighlightTreatment: SpanHighlightTreatment.None,
                LowlightTreatment: SpanLowlightTreatment.None
            ));

            var commentSpans = GenerateCommentSpans(line);
            spans.AddRange(commentSpans);

            string commentText = commentPrefix + string.Join("", commentSpans.Select(s => s.SpanText));

            RegexCommentedLine commentedLine = line is IMatchableAlternate matchableAlt
                ? new RegexCommentedAlternateLine(paddedRegex, commentText, line.NamedPath, i, spans, matchableAlt)
                : new RegexCommentedLine(paddedRegex, commentText, line.NamedPath, i, spans);

            CommentedLines.Add(commentedLine);
        }
    }

    private string GetPrimaryContentColorForLine(RegexTemplateLine line)
    {
        if (line.PropEnclosures.Length == 0)
        {
            return line switch
            {
                TextLine => _colors.UnenclosedTextLineCommentColor,
                SpaceLine => _colors.UnenclosedSpaceLineCommentColor,
                BoundaryBase => _colors.BoundaryCommentColor,
                _ => _colors.DefaultFallbackColor
            };
        }

        var currentEnclosure = line.PropEnclosures.Last();
        var palette = currentEnclosure.Palette;

        switch (line)
        {
            case NamedGroupOpen:
            case NamedGroupClose:
                return _colors.NamedGroupBookendCommentColor(palette);
            case GroupOpen:
                return DefaultWhite;
            case GroupClose:
                return _colors.GroupCloseQuantifierColor;
            case AlternateValue:
                return _colors.AlternateValueCommentColor(palette);
            case TextLine or SpaceLine or GroupAlternativePipe:
                var nearestNamedEnclosure = line.PropEnclosures.LastOrDefault(e => e is NamedEnclosure) as NamedEnclosure;
                if (nearestNamedEnclosure != null)
                {
                    return _colors.EnclosedTextColor(nearestNamedEnclosure.Palette);
                }
                return DefaultWhite;
            default:
                return DefaultWhite;
        }
    }

    private List<RegexCommentedLineSpan> GenerateCommentSpans(RegexTemplateLine line)
    {
        var spans = new List<RegexCommentedLineSpan>();
        var lowlight = _treatments.CommentLowlightTreatment;

        Action<string, Palette, SpanHighlightTreatment, IEnumerable<Enclosure>> addSpanForEnclosurePath = (text, palette, highlight, enclosureScope) =>
        {
            if (string.IsNullOrEmpty(text)) return;
            string rootName = line.Enclosures.OfType<RootEnclosure>().FirstOrDefault()?.RootTypeName ?? "";
            string namedPath = string.Join('.', enclosureScope.OfType<NamedEnclosure>().Select(x => x.Name));
            string fullPath = string.IsNullOrEmpty(namedPath) ? rootName : $"{rootName}.{namedPath}";
            string relativePath = RegexCommentedLine.GetRelativePath(fullPath);

            var finalHighlight = (relativePath == null) ? SpanHighlightTreatment.None : highlight;

            spans.Add(new RegexCommentedLineSpan(text, palette, relativePath, finalHighlight, lowlight));
        };

        Action<string, Palette, bool> addSpanForCurrentLine = (text, palette, isTextSpan) =>
        {
            if (string.IsNullOrEmpty(text)) return;

            string pathForSpan = line.NamedPath;
            if (line is IMatchableAlternate alt)
            {
                pathForSpan = $"{pathForSpan}.{alt.CanonicalValue}";
            }
            string relativePath = RegexCommentedLine.GetRelativePath(pathForSpan);

            var highlight = _treatments.GetCommentHighlightTreatment(line, isTextSpan);
            var finalHighlight = (relativePath == null) ? SpanHighlightTreatment.None : highlight;

            spans.Add(new RegexCommentedLineSpan(text, palette, relativePath, finalHighlight, lowlight));
        };

        var defaultWhitePalette = DeterministicPalette.GetStaticPalette(new HexColor(DefaultWhite));

        if (line.PropEnclosures.Length == 0)
        {
            var color = GetPrimaryContentColorForLine(line);
            var unenclosedPalette = DeterministicPalette.GetStaticPalette(new HexColor(color));

            spans.Add(new RegexCommentedLineSpan(line.Comment ?? string.Empty, unenclosedPalette, null, SpanHighlightTreatment.None, lowlight));
            return spans;
        }

        var parentEnclosures = line.PropEnclosures.Take(line.PropEnclosures.Length - 1).ToList();
        var currentEnclosure = line.PropEnclosures.Last();
        var chars = BoxChars.Get(currentEnclosure.Treatment);
        var palette = currentEnclosure.Palette;
        var borderPalette = DeterministicPalette.GetStaticPalette(new HexColor(_colors.GetBorderColor(currentEnclosure.Treatment, palette)));
        var borderHighlight = _treatments.GetCommentHighlightTreatment(line, isTextSpan: false);

        var currentPathParts = new List<Enclosure>();
        foreach (var parent in parentEnclosures)
        {
            currentPathParts.Add(parent);
            char wall = BoxChars.Get(parent.Treatment).Wall;
            var parentBorderPalette = DeterministicPalette.GetStaticPalette(new HexColor(_colors.GetBorderColor(parent.Treatment, parent.Palette)));
            addSpanForEnclosurePath(wall.ToString(), parentBorderPalette, borderHighlight, currentPathParts);
            addSpanForEnclosurePath(" ", defaultWhitePalette, borderHighlight, currentPathParts);
        }

        int parentDepth = parentEnclosures.Count;
        int currentLevelWidth = CommentBoxLength - (parentDepth * 4);

        if (line is EncloureBookend)
        {
            int availableWidth = currentLevelWidth - 2;
            switch (line)
            {
                case NamedGroupOpen ngo:
                    var openBookendPalette = DeterministicPalette.GetStaticPalette(new HexColor(_colors.NamedGroupBookendCommentColor(palette)));
                    string openComment = $" {ngo.Comment} ";
                    string fillerOpen = new string(chars.Top, Math.Max(0, availableWidth - openComment.Length));
                    addSpanForCurrentLine(chars.TopLeft.ToString(), borderPalette, false);
                    addSpanForCurrentLine(openComment, openBookendPalette, true);
                    addSpanForCurrentLine(fillerOpen, borderPalette, false);
                    addSpanForCurrentLine(chars.TopRight.ToString(), borderPalette, false);
                    break;
                case GroupOpen:
                    addSpanForCurrentLine(chars.TopLeft.ToString(), borderPalette, false);
                    addSpanForCurrentLine(new string(chars.Top, availableWidth), borderPalette, false);
                    addSpanForCurrentLine(chars.TopRight.ToString(), borderPalette, false);
                    break;
                case NamedGroupClose ngc:
                    var closeBookendPalette = DeterministicPalette.GetStaticPalette(new HexColor(_colors.NamedGroupBookendCommentColor(palette)));
                    string closeComment = $" {ngc.Comment} ";
                    string fillerClose = new string(chars.Bottom, Math.Max(0, availableWidth - closeComment.Length));
                    addSpanForCurrentLine(chars.BottomLeft.ToString(), borderPalette, false);
                    addSpanForCurrentLine(fillerClose, borderPalette, false);
                    addSpanForCurrentLine(closeComment, closeBookendPalette, true);
                    addSpanForCurrentLine(chars.BottomRight.ToString(), borderPalette, false);
                    break;
                case GroupClose gc:
                    var quantPalette = DeterministicPalette.GetStaticPalette(new HexColor(_colors.GroupCloseQuantifierColor));
                    string quantComment = gc.Comment != null ? $" {gc.Comment} " : "";
                    string fillerQuant = new string(chars.Bottom, Math.Max(0, availableWidth - quantComment.Length));
                    addSpanForCurrentLine(chars.BottomLeft.ToString(), borderPalette, false);
                    addSpanForCurrentLine(fillerQuant, borderPalette, false);
                    addSpanForCurrentLine(quantComment, quantPalette, true);
                    addSpanForCurrentLine(chars.BottomRight.ToString(), borderPalette, false);
                    break;
            }
        }
        else
        {
            int innerWidth = currentLevelWidth - 4;
            addSpanForEnclosurePath(chars.Wall.ToString(), borderPalette, borderHighlight, line.PropEnclosures);
            addSpanForEnclosurePath(" ", defaultWhitePalette, borderHighlight, line.PropEnclosures);

            switch (line)
            {
                case AlternateValue av:
                    var altPalette = DeterministicPalette.GetStaticPalette(new HexColor(_colors.AlternateValueCommentColor(palette)));
                    string altCommentText = $" {av.Comment} ";
                    int totalPad = Math.Max(0, innerWidth - altCommentText.Length);
                    string leftPad = new string(' ', totalPad / 2);
                    string rightPad = new string(' ', totalPad - (totalPad / 2));
                    string fullContent = $"{leftPad}{altCommentText}{rightPad}";
                    addSpanForCurrentLine(fullContent, altPalette, true);
                    break;
                default:
                    var nearestNamedEnclosure = line.PropEnclosures.LastOrDefault(e => e is NamedEnclosure) as NamedEnclosure;
                    var contentPalette = (nearestNamedEnclosure != null)
                        ? DeterministicPalette.GetStaticPalette(new HexColor(_colors.EnclosedTextColor(nearestNamedEnclosure.Palette)))
                        : defaultWhitePalette;
                    var content = string.IsNullOrEmpty(line.Comment)
                        ? new string(' ', innerWidth)
                        : (new string(' ', _boxContentLeftPadding) + line.Comment).PadRight(innerWidth);
                    addSpanForCurrentLine(content, contentPalette, true);
                    break;
            }

            addSpanForEnclosurePath(" ", defaultWhitePalette, borderHighlight, line.PropEnclosures);
            addSpanForEnclosurePath(chars.Wall.ToString(), borderPalette, borderHighlight, line.PropEnclosures);
        }

        var reversedParents = parentEnclosures.AsEnumerable().Reverse().ToList();
        for (int j = 0; j < reversedParents.Count(); j++)
        {
            var parent = reversedParents[j];
            var wallPathScope = parentEnclosures.Take(parentEnclosures.Count - j).ToList();
            char wall = BoxChars.Get(parent.Treatment).Wall;
            var parentBorderPalette = DeterministicPalette.GetStaticPalette(new HexColor(_colors.GetBorderColor(parent.Treatment, parent.Palette)));
            addSpanForEnclosurePath(" ", defaultWhitePalette, borderHighlight, wallPathScope);
            addSpanForEnclosurePath(wall.ToString(), parentBorderPalette, borderHighlight, wallPathScope);
        }

        return spans;
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
                        textWidth = comment.Length + 2;
                        break;
                    default:
                        textWidth = _boxContentLeftPadding + comment.Length;
                        break;
                }
                requiredWidth = line is EncloureBookend ? textWidth + 2 : textWidth + 4;
            }
            boxWidths[pathKey] = Math.Max(boxWidths[pathKey], requiredWidth);
        }

        var sortedPaths = uniquePaths.OrderByDescending(p => p.Count());
        foreach (var path in sortedPaths)
        {
            if (path.Count() <= 1) continue;
            string childPathKey = string.Join(",", path.Select(e => e.Ordinal));
            string parentPathKey = string.Join(",", path.Take(path.Count() - 1).Select(e => e.Ordinal));
            int childFootprint = boxWidths[childPathKey] + 4;
            boxWidths[parentPathKey] = Math.Max(boxWidths[parentPathKey], childFootprint);
        }

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