namespace MTGPlexer.TokenAnalysis.RegexDTOs;

using Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public record PrettifiedRegex
{
    public string OriginalRegex { get; }
    public List<PrettifiedRegexLine> Lines { get; }
    public string DisplayText { get; }
    public int HashColumnIndex { get; }
    public int TypeColumnIndex { get; }

    public PrettifiedRegex(string originalRegex)
    {
        OriginalRegex = originalRegex;
        try
        {
            var rootFragment = RegexParser.Parse(OriginalRegex);
            var initialLines = LineGenerator.GenerateLines(rootFragment);
            var formatResult = BuildFormattedLines(initialLines);
            Lines = formatResult.FormattedLines;
            HashColumnIndex = formatResult.HashIndex;
            TypeColumnIndex = formatResult.TypeIndex;
            DisplayText = string.Join(Environment.NewLine, Lines.Select(l => l.DisplayText));
        }
        catch (Exception ex)
        {
            Lines =
            [
                new PrettifiedRegexLine(0, null, $"// Failed to prettify: {ex.Message}", null, PrettifiedRegexLineRole.Error),
                new PrettifiedRegexLine(1, null, originalRegex, null, PrettifiedRegexLineRole.EnumValue)
            ];
            DisplayText = string.Join(Environment.NewLine, Lines.Select(l => l.Text));
            HashColumnIndex = -1;
            TypeColumnIndex = -1;
        }
    }

    private class LineFormattingInfo
    {
        internal string Left { get; set; }
        internal string Comment { get; init; }
        internal string Type { get; init; }
        internal PrettifiedRegexLine OriginalLine { get; init; }
    }

    private static (List<PrettifiedRegexLine> FormattedLines, int HashIndex, int TypeIndex) BuildFormattedLines(List<PrettifiedRegexLine> initialLines)
    {
        if (initialLines.Count == 0) return ([], -1, -1);

        // --- Configuration Constants ---
        const int indentSpaces = 4;
        const int paddingBeforeCommentDivider = 2;
        const int paddingAfterCommentDivider = 1;
        const int commentIndentBaseline = 2;

        static string PrettifyInternalText(string fragment) => Regex.Replace(fragment, @"(?<!\[) (?!\])", "[ ]").Replace(@"\s", "[ ]");

        // --- Step 1: Determine Group Types ---
        var groupTypes = new Dictionary<string, string>();
        var allGroupNames = new HashSet<string>(initialLines.Where(l => !string.IsNullOrEmpty(l.CaptureGroupName)).Select(l => l.CaptureGroupName));
        foreach (var line in initialLines.Where(l => l.Role == PrettifiedRegexLineRole.EnumValue && !string.IsNullOrEmpty(l.CaptureGroupName)))
        {
            groupTypes[line.CaptureGroupName] = "enum";
        }
        foreach (var name in allGroupNames.Where(n => !groupTypes.ContainsKey(n)))
        {
            groupTypes[name] = "placeholder"; // Default type
        }

        // --- Step 2: Pre-process lines to generate intermediate parts for formatting ---
        var lineParts = new List<LineFormattingInfo>();
        for (int i = 0; i < initialLines.Count; i++)
        {
            var line = initialLines[i];
            var prevLine = i > 0 ? initialLines[i - 1] : null;
            var indent = new string(' ', line.IndentLevel * indentSpaces);
            var groupName = line.CaptureGroupName;

            // Skip inline alternation markers; they are handled by the line that follows them.
            if (line.Role == PrettifiedRegexLineRole.Alternation) continue;

            switch (line.Role)
            {
                case PrettifiedRegexLineRole.Separator:
                    if (lineParts.LastOrDefault()?.OriginalLine.Role != PrettifiedRegexLineRole.Separator)
                    {
                        lineParts.Add(new() { Left = "", Comment = "", Type = "", OriginalLine = line });
                    }
                    break;
                case PrettifiedRegexLineRole.GroupAlternation:
                    // Add separators around the group alternator for spacing.
                    lineParts.Add(new() { Left = "", Comment = "", Type = "", OriginalLine = line with { Role = PrettifiedRegexLineRole.Separator } });
                    lineParts.Add(new() { Left = indent + "|", Comment = "", Type = "", OriginalLine = line });
                    lineParts.Add(new() { Left = "", Comment = "", Type = "", OriginalLine = line with { Role = PrettifiedRegexLineRole.Separator } });
                    break;
                case PrettifiedRegexLineRole.WordBoundary: lineParts.Add(new() { Left = line.Text, Comment = "word boundary", Type = "", OriginalLine = line }); break;
                case PrettifiedRegexLineRole.ConnectiveMatch: lineParts.Add(new() { Left = indent + PrettifyInternalText(line.Text), Comment = "connective match", Type = "", OriginalLine = line }); break;
                case PrettifiedRegexLineRole.CaptureGroupStart: lineParts.Add(new() { Left = $"{indent}{line.Text}", Comment = groupName, Type = groupTypes.GetValueOrDefault(groupName, ""), OriginalLine = line }); break;
                case PrettifiedRegexLineRole.CaptureGroupEnd: lineParts.Add(new() { Left = $"{indent}{line.Text}", Comment = groupName, Type = "", OriginalLine = line }); break;
                case PrettifiedRegexLineRole.Comment: lineParts.Add(new() { Left = indent + line.Text, Comment = line.Comment, Type = "", OriginalLine = line }); break;
                case PrettifiedRegexLineRole.EnumValue:
                case PrettifiedRegexLineRole.CharacterRange:
                    string leftText = indent + PrettifyInternalText(line.Text);
                    if (prevLine?.Role == PrettifiedRegexLineRole.Alternation && prevLine.IndentLevel == line.IndentLevel)
                    {
                        leftText = $"{indent.Substring(2)}| {PrettifyInternalText(line.Text).Trim()}";
                    }
                    lineParts.Add(new() { Left = leftText, Comment = line.Role == PrettifiedRegexLineRole.EnumValue ? "enum member" : "match range", OriginalLine = line });
                    break;
                default: lineParts.Add(new() { Left = $"{indent}{line.Text}", Comment = "", Type = "", OriginalLine = line }); break;
            }
        }

        // --- Step 3: Calculate uniform comment width ---
        int maxCommentWidth = 0;
        var groupStackForWidthCalc = new Stack<LineFormattingInfo>();
        foreach (var p in lineParts)
        {
            string currentContent = "";
            var currentGroup = groupStackForWidthCalc.Any() ? groupStackForWidthCalc.Peek() : null;

            if (p.OriginalLine.Role == PrettifiedRegexLineRole.CaptureGroupStart && !string.IsNullOrEmpty(p.Comment))
            {
                string headerText = $" {p.Comment} ";
                string typeText = $" : {p.Type} ";
                currentContent = headerText + "─" + typeText;
                groupStackForWidthCalc.Push(p);
            }
            else if (p.OriginalLine.Role == PrettifiedRegexLineRole.CaptureGroupEnd && currentGroup?.Comment == p.Comment)
            {
                currentContent = $" {p.Comment} ";
                groupStackForWidthCalc.Pop();
            }
            else if (!string.IsNullOrWhiteSpace(p.Comment))
            {
                int indentLevel = (currentGroup != null)
                    ? commentIndentBaseline + (p.OriginalLine.IndentLevel - currentGroup.OriginalLine.IndentLevel - 1) * indentSpaces
                    : commentIndentBaseline;
                currentContent = $"{new string(' ', indentLevel)}{p.Comment}";
            }

            int currentFullWidth = currentContent.Length > 0 ? currentContent.Length + 2 : 0; // +2 for side padding
            maxCommentWidth = Math.Max(maxCommentWidth, currentFullWidth);
        }

        int maxLeftWidth = lineParts.Select(p => p.Left.Length).DefaultIfEmpty(0).Max();
        int hashIndex = maxLeftWidth + paddingBeforeCommentDivider;

        // --- Final Rendering ---
        var finalLines = new List<PrettifiedRegexLine>();
        var boxStack = new Stack<string>();
        foreach (var p in lineParts)
        {
            var sb = new StringBuilder();
            sb.Append(p.Left.PadRight(hashIndex));
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

            sb.Append(new string(' ', paddingAfterCommentDivider));

            if (isHeader)
            {
                string textPart = $" {p.Comment} ";
                string typePart = $" : {p.Type} ";
                int dashCount = Math.Max(0, maxCommentWidth - textPart.Length - typePart.Length - 2); // -2 for corners
                string dashes = new string('─', dashCount);
                sb.Append($"┌{textPart}{dashes}{typePart}┐");
            }
            else if (isFooter)
            {
                string footerText = $" {p.Comment} ";
                int dashCount = Math.Max(0, maxCommentWidth - footerText.Length - 2); // -2 for corners
                string dashes = new string('─', dashCount);
                sb.Append($"└{dashes}{footerText}┘");
            }
            else if (isInsideBox)
            {
                var groupStartLine = lineParts.First(lp => lp.Comment == currentBoxName && lp.OriginalLine.Role == PrettifiedRegexLineRole.CaptureGroupStart);
                var parentIndent = groupStartLine.OriginalLine.IndentLevel;
                var relativeIndent = (p.OriginalLine.IndentLevel - parentIndent - 1) * indentSpaces;
                string content = $"{new string(' ', commentIndentBaseline + relativeIndent)}{p.Comment}";
                string paddedContent = $" {content}".PadRight(maxCommentWidth - 2); // Corrected padding
                sb.Append($"│{paddedContent}│"); // Corrected wall alignment
            }
            else if (!string.IsNullOrWhiteSpace(p.Comment) || p.OriginalLine.Role == PrettifiedRegexLineRole.GroupAlternation)
            {
                sb.Append($"{new string(' ', commentIndentBaseline)}{p.Comment}");
            }

            if (isFooter) { boxStack.Pop(); }
            finalLines.Add(p.OriginalLine with { DisplayText = sb.ToString().TrimEnd() });
        }
        return (finalLines, hashIndex, -1);
    }
}