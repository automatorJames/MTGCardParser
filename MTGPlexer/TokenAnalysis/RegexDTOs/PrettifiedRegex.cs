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
                new PrettifiedRegexLine(1, null, originalRegex, null, PrettifiedRegexLineRole.LiteralMatch)
            ];
            DisplayText = string.Join(Environment.NewLine, Lines.Select(l => l.Text));
            HashColumnIndex = -1;
            TypeColumnIndex = -1;
        }
    }

    private static (List<PrettifiedRegexLine> FormattedLines, int HashIndex, int TypeIndex) BuildFormattedLines(List<PrettifiedRegexLine> initialLines)
    {
        if (initialLines.Count == 0) return ([], -1, -1);

        static string PrettifyInternalText(string fragment) => Regex.Replace(fragment, @"(?<!\[) (?!\])", "[ ]").Replace(@"\s", "[ ]");

        // --- Step 1: Determine Group Types ---
        var groupTypes = new Dictionary<string, string>();
        var allGroupNames = new HashSet<string>(initialLines.Where(l => !string.IsNullOrEmpty(l.CaptureGroupName)).Select(l => l.CaptureGroupName));

        foreach (var line in initialLines.Where(l => l.Role == PrettifiedRegexLineRole.FirstEnumValueInGroup && !string.IsNullOrEmpty(l.CaptureGroupName)))
        {
            groupTypes[line.CaptureGroupName] = "enum";
        }
        foreach (var name in allGroupNames.Where(n => !groupTypes.ContainsKey(n)))
        {
            groupTypes[name] = "placeholder"; // Default type for non-enum groups
        }

        // --- Step 2: Pre-process lines to generate parts for formatting ---
        var lineParts = new List<(string Left, string Comment, string Type, bool IsInGroup, PrettifiedRegexLine OriginalLine)>();
        bool isFirstInAlternation = true;
        for (int i = 0; i < initialLines.Count; i++)
        {
            var line = initialLines[i];
            var indent = new string(' ', line.IndentLevel * 4);
            var groupName = line.CaptureGroupName;

            // Reset alternation tracking when exiting a group
            if (line.Role is PrettifiedRegexLineRole.CaptureGroupEnd or PrettifiedRegexLineRole.GenericGroupEnd)
            {
                isFirstInAlternation = true;
            }

            switch (line.Role)
            {
                case PrettifiedRegexLineRole.Separator: lineParts.Add(("", "---", "", false, line)); break;
                case PrettifiedRegexLineRole.WordBoundary: lineParts.Add((line.Text, "(word boundary)", "", false, line)); break;
                case PrettifiedRegexLineRole.ConnectiveMatch: lineParts.Add((indent + PrettifyInternalText(line.Text), "connective match", "", true, line)); break;
                case PrettifiedRegexLineRole.CaptureGroupStart:
                    lineParts.Add(($"{indent}{line.Text}", groupName, groupTypes.GetValueOrDefault(groupName, ""), false, line));
                    isFirstInAlternation = true; // The next item inside is the first
                    break;
                case PrettifiedRegexLineRole.CaptureGroupEnd:
                    var endComment = string.IsNullOrEmpty(groupName) ? "" : $"____END {groupName}____";
                    lineParts.Add(($"{indent}{line.Text}", endComment, "", false, line)); break;
                case PrettifiedRegexLineRole.FirstEnumValueInGroup:
                    string text = PrettifyInternalText(line.Text);
                    string leftPart = isFirstInAlternation ? $"{indent}{text}" : $"{indent.Substring(2)}| {text}";
                    lineParts.Add((leftPart, "enum member", "", true, line));
                    isFirstInAlternation = false;
                    break;
                case PrettifiedRegexLineRole.Alternation:
                    // This is handled by the logic in FirstEnumValueInGroup now, so we can skip adding a separate line for the symbol.
                    continue;
                case PrettifiedRegexLineRole.Comment:
                    lineParts.Add(($"{indent}{line.Text}", "", "", false, line)); break;
                default:
                    lineParts.Add(($"{indent}{line.Text}", "", "", false, line)); break;
            }
        }

        // --- Step 3: Final Formatting Pass ---
        const string typeColumnPrefix = "    : ";
        const int globalCommentIndent = 4;
        const int childCommentIndent = 4;

        // Apply Start/End comments
        var firstLineIndex = lineParts.FindIndex(p => !string.IsNullOrWhiteSpace(p.Left) || p.Comment == "---");
        if (firstLineIndex != -1) { var p = lineParts[firstLineIndex]; lineParts[firstLineIndex] = (p.Left, "Start" + (p.Comment.Contains("word boundary") ? " (word boundary)" : ""), p.Type, p.IsInGroup, p.OriginalLine); }
        var lastLineIndex = lineParts.FindLastIndex(p => !string.IsNullOrWhiteSpace(p.Left) || p.Comment.StartsWith("____END") || p.Comment.Contains("word boundary"));
        if (lastLineIndex != -1) { var p = lineParts[lastLineIndex]; if (string.IsNullOrWhiteSpace(p.Comment) || p.Comment.Contains("word boundary")) lineParts[lastLineIndex] = (p.Left, "End" + (p.Comment.Contains("word boundary") ? " (word boundary)" : ""), p.Type, p.IsInGroup, p.OriginalLine); }

        // Calculate dynamic widths for alignment
        int maxLeftWidth = lineParts.Select(p => p.Left.Length).DefaultIfEmpty(0).Max();
        int hashIndex = maxLeftWidth > 0 ? maxLeftWidth + 4 : 21;
        int maxGroupNameWidth = lineParts.Select(p => p.Comment).Where(c => groupTypes.ContainsKey(c)).Select(c => c.Length).DefaultIfEmpty(0).Max();

        int maxCommentSectionWidth = 0;
        foreach (var p in lineParts)
        {
            if (string.IsNullOrEmpty(p.Comment) || p.Comment == "---") continue;
            int currentCommentWidth = p.Comment.StartsWith("____END") ? p.Comment.Length : globalCommentIndent + (p.IsInGroup ? childCommentIndent : 0) + p.Comment.Length;
            if (!string.IsNullOrEmpty(p.Type)) currentCommentWidth += typeColumnPrefix.Length + p.Type.Length;
            maxCommentSectionWidth = Math.Max(maxCommentSectionWidth, currentCommentWidth);
        }

        string hr = new('-', maxCommentSectionWidth);

        // Build final display strings
        var finalLines = new List<PrettifiedRegexLine>();
        foreach (var p in lineParts)
        {
            var sb = new StringBuilder();
            sb.Append(p.Left.PadRight(hashIndex - 4));
            sb.Append("    #");

            if (!string.IsNullOrWhiteSpace(p.Comment))
            {
                sb.Append(" ");
                if (p.Comment == "---") { sb.Append(hr); }
                else if (p.Comment.StartsWith("____END")) { sb.Append(p.Comment.PadRight(hr.Length, '_')); }
                else
                {
                    sb.Append("".PadRight(globalCommentIndent));
                    if (!string.IsNullOrEmpty(p.Type))
                    {
                        sb.Append(p.Comment.PadRight(maxGroupNameWidth));
                        sb.Append($"{typeColumnPrefix}{p.Type}");
                    }
                    else
                    {
                        if (p.IsInGroup) sb.Append("".PadRight(childCommentIndent));
                        sb.Append(p.Comment);
                    }
                }
            }
            finalLines.Add(p.OriginalLine with { DisplayText = sb.ToString().TrimEnd() });
        }

        return (finalLines, hashIndex, -1);
    }
}