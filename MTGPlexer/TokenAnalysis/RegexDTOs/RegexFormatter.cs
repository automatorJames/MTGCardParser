namespace MTGPlexer.TokenAnalysis.RegexDTOs.Internal;

/// <summary>
/// Takes a list of semantic PrettifiedRegexLines and formats them into their final
/// display strings with proper alignment, padding, and box-drawing characters.
/// </summary>
public class RegexFormatter
{
    // --- Configuration Constants ---
    private const int IndentSpaces = 4;
    private const int PaddingBeforeCommentDivider = 4;
    private const int PaddingAfterCommentDivider = 4;
    private const int CommentIndentBaseline = 2;

    private readonly List<PrettifiedRegexLine> _initialLines;

    public record FormatResult(List<PrettifiedRegexLine> FormattedLines, int HashIndex);
    private record LayoutMetrics(int MaxLeftWidth, int MaxCommentWidth, int HashIndex);
    private record LineFormattingInfo(string Left, string Comment, string Type, PrettifiedRegexLine OriginalLine);

    public RegexFormatter(List<PrettifiedRegexLine> initialLines)
    {
        _initialLines = initialLines;
    }

    public FormatResult Format()
    {
        if (_initialLines.Count == 0) return new FormatResult([], -1);

        var lineParts = GenerateLineParts();
        var layout = CalculateLayout(lineParts);
        var finalLines = RenderFinalLines(lineParts, layout);

        return new FormatResult(finalLines, layout.HashIndex);
    }

    /// <summary>
    /// STEP 1: Convert semantic lines into intermediate formatting parts (Left, Comment, Type).
    /// </summary>
    private List<LineFormattingInfo> GenerateLineParts()
    {
        var lineParts = new List<LineFormattingInfo>();
        var groupTypes = DetermineGroupTypes();

        for (int i = 0; i < _initialLines.Count; i++)
        {
            var line = _initialLines[i];
            if (line.Role == PrettifiedRegexLineRole.Alternation) continue;

            var prevLine = i > 0 ? _initialLines[i - 1] : null;
            var indent = new string(' ', line.IndentLevel * IndentSpaces);
            var groupName = line.PropertyCaptureGroup;

            var newParts = new List<LineFormattingInfo>();

            switch (line.Role)
            {
                case PrettifiedRegexLineRole.Separator:
                    newParts.Add(new("", "", "", line));
                    break;
                case PrettifiedRegexLineRole.GroupAlternation:
                    newParts.AddRange(CreateGroupAlternationParts(line, indent).Parts);
                    break;
                case PrettifiedRegexLineRole.WordBoundary:
                    newParts.Add(new(indent + line.Text, "word boundary", "", line));
                    break;
                case PrettifiedRegexLineRole.ConnectiveMatch:
                    newParts.Add(new(indent + PrettifyInternalText(line.Text), "connective match", "", line));
                    break;
                case PrettifiedRegexLineRole.CaptureGroupStart:
                    newParts.Add(new($"{indent}{line.Text}", groupName, groupTypes.GetValueOrDefault(groupName, ""), line));
                    break;
                case PrettifiedRegexLineRole.CaptureGroupEnd:
                    newParts.Add(new($"{indent}{line.Text}", groupName, "", line));
                    break;
                case PrettifiedRegexLineRole.Comment:
                    newParts.Add(new(indent + line.Text, line.Comment.Trim(), "", line));
                    break;
                case PrettifiedRegexLineRole.EnumValue or PrettifiedRegexLineRole.CharacterRange:
                    newParts.Add(CreateEnumOrRangePart(line, prevLine, indent));
                    break;
                default:
                    newParts.Add(new($"{indent}{line.Text}", "", "", line));
                    break;
            }

            // Safely process the generated parts for the current line
            foreach (var part in newParts)
            {
                // Prevent stacking multiple separator lines
                if (part.OriginalLine.Role == PrettifiedRegexLineRole.Separator &&
                    lineParts.LastOrDefault()?.OriginalLine.Role == PrettifiedRegexLineRole.Separator)
                {
                    continue;
                }
                lineParts.Add(part);
            }
        }
        return lineParts;
    }


    /// <summary>
    /// STEP 2: Calculate the necessary widths and column indexes for alignment.
    /// </summary>
    private LayoutMetrics CalculateLayout(List<LineFormattingInfo> lineParts)
    {
        int maxCommentWidth = 0;
        var groupStackForWidthCalc = new Stack<LineFormattingInfo>();
        foreach (var p in lineParts)
        {
            string currentContent = "";
            var currentGroup = groupStackForWidthCalc.Any() ? groupStackForWidthCalc.Peek() : null;

            if (p.OriginalLine.Role == PrettifiedRegexLineRole.CaptureGroupStart && !string.IsNullOrEmpty(p.Comment))
            {
                string headerText = $" {p.Comment.ToFriendlyCase(TitleDisplayOption.Title)} ";
                string typeText = $" : {p.Type} ";
                currentContent = headerText + "─" + typeText;
                groupStackForWidthCalc.Push(p);
            }
            else if (p.OriginalLine.Role == PrettifiedRegexLineRole.CaptureGroupEnd && currentGroup?.Comment == p.Comment)
            {
                currentContent = $" {p.Comment.ToFriendlyCase(TitleDisplayOption.Title)} ";
                groupStackForWidthCalc.Pop();
            }
            else if (!string.IsNullOrWhiteSpace(p.Comment))
            {
                currentContent = $"{new string(' ', CommentIndentBaseline)}{p.Comment}";
            }

            int currentFullWidth = currentContent.Length > 0 ? currentContent.Length + 2 : 0; // +2 for side padding
            maxCommentWidth = Math.Max(maxCommentWidth, currentFullWidth);
        }

        int maxLeftWidth = lineParts.Select(p => p.Left.Length).DefaultIfEmpty(0).Max();
        int hashIndex = maxLeftWidth + PaddingBeforeCommentDivider;

        return new LayoutMetrics(maxLeftWidth, maxCommentWidth, hashIndex);
    }

    /// <summary>
    /// STEP 3: Render the final, formatted lines using the calculated layout metrics.
    /// </summary>
    private List<PrettifiedRegexLine> RenderFinalLines(List<LineFormattingInfo> lineParts, LayoutMetrics layout)
    {
        var finalLines = new List<PrettifiedRegexLine>();
        var boxStack = new Stack<string>();
        foreach (var p in lineParts)
        {
            var sb = new StringBuilder();
            sb.Append(p.Left.PadRight(layout.HashIndex));
            sb.Append('#');

            string currentBoxName = boxStack.Any() ? boxStack.Peek() : null;
            if (p.OriginalLine.Role == PrettifiedRegexLineRole.CaptureGroupStart && !string.IsNullOrEmpty(p.Comment))
            {
                boxStack.Push(p.Comment);
                currentBoxName = p.Comment;
            }

            bool isInsideBox = currentBoxName != null;
            bool isHeader = isInsideBox && p.OriginalLine.Role == PrettifiedRegexLineRole.CaptureGroupStart && p.Comment == currentBoxName;
            bool isFooter = isInsideBox && p.OriginalLine.Role == PrettifiedRegexLineRole.CaptureGroupEnd && p.Comment == currentBoxName;

            if (p.OriginalLine.Role == PrettifiedRegexLineRole.Separator && !isInsideBox)
            {
                finalLines.Add(p.OriginalLine with { DisplayText = sb.ToString().TrimEnd() });
                continue;
            }

            sb.Append(new string(' ', PaddingAfterCommentDivider));

            if (isHeader)
            {
                string textPart = $" {p.Comment.ToFriendlyCase(TitleDisplayOption.Title)} ";
                string typePart = $" : {p.Type} ";
                int dashCount = Math.Max(0, layout.MaxCommentWidth - textPart.Length - typePart.Length - 2);
                sb.Append($"┌{textPart}{new string('─', dashCount)}{typePart}┐");
            }
            else if (isFooter)
            {
                string footerText = $" {p.Comment.ToFriendlyCase(TitleDisplayOption.Title)} ";
                int dashCount = Math.Max(0, layout.MaxCommentWidth - footerText.Length - 2);
                sb.Append($"└{new string('─', dashCount)}{footerText}┘");
            }
            else if (isInsideBox)
            {
                string content = $"{new string(' ', CommentIndentBaseline)}{p.Comment}";
                string paddedContent = $" {content}".PadRight(layout.MaxCommentWidth - 2);
                sb.Append($"│{paddedContent}│");
            }
            else if (!string.IsNullOrWhiteSpace(p.Comment) || p.OriginalLine.Role == PrettifiedRegexLineRole.GroupAlternation)
            {
                sb.Append($"{new string(' ', CommentIndentBaseline)}{p.Comment}");
            }

            if (isFooter) boxStack.Pop();
            finalLines.Add(p.OriginalLine with { DisplayText = sb.ToString().TrimEnd() });
        }
        return finalLines;
    }

    #region Helper Methods
    private record GroupAlternationParts(List<LineFormattingInfo> Parts) : LineFormattingInfo("", "", "", null);

    private GroupAlternationParts CreateGroupAlternationParts(PrettifiedRegexLine line, string indent)
    {
        return new GroupAlternationParts(
        [
            new("", "", "", line with { Role = PrettifiedRegexLineRole.Separator }),
            new(indent + "|", "", "", line),
            new("", "", "", line with { Role = PrettifiedRegexLineRole.Separator })
        ]);
    }

    private LineFormattingInfo CreateEnumOrRangePart(PrettifiedRegexLine line, PrettifiedRegexLine prevLine, string indent)
    {
        string leftText = indent + PrettifyInternalText(line.Text);
        // If preceded by an alternation pipe at the same level, adjust indentation for alignment.
        if (prevLine?.Role == PrettifiedRegexLineRole.Alternation && prevLine.IndentLevel == line.IndentLevel)
        {
            leftText = $"{indent.Substring(2)}| {PrettifyInternalText(line.Text).Trim()}";
        }
        string comment = line.Role == PrettifiedRegexLineRole.EnumValue ? "enum member" : "match range";
        return new(leftText, comment, "", line);
    }

    private Dictionary<string, string> DetermineGroupTypes()
    {
        var groupTypes = new Dictionary<string, string>();
        var allGroupNames = new HashSet<string>(_initialLines.Select(l => l.PropertyCaptureGroup).Where(n => !string.IsNullOrEmpty(n)));

        foreach (var line in _initialLines.Where(l => l.Role == PrettifiedRegexLineRole.EnumValue && !string.IsNullOrEmpty(l.PropertyCaptureGroup)))
        {
            groupTypes[line.PropertyCaptureGroup] = "enum";
        }

        foreach (var name in allGroupNames.Where(n => !groupTypes.ContainsKey(n)))
        {
            groupTypes[name] = "placeholder"; // Default type
        }
        return groupTypes;
    }

    private static string PrettifyInternalText(string fragment) => Regex.Replace(fragment, @"(?<!\[) (?!\])", "[ ]").Replace(@"\s", "[ ]");
    #endregion
}