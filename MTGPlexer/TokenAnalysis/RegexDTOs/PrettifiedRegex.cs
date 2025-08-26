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

    private static (List<PrettifiedRegexLine> FormattedLines, int HashIndex, int TypeIndex) BuildFormattedLines(List<PrettifiedRegexLine> initialLines)
    {
        if (initialLines.Count == 0) return ([], -1, -1);

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

        // --- Step 2: Pre-process lines to generate parts for formatting ---
        var lineParts = new List<(string Left, string Comment, string Type, bool IsInGroup, PrettifiedRegexLine OriginalLine)>();

        foreach (var line in initialLines.Where(l => l.Role != PrettifiedRegexLineRole.Alternation))
        {
            var indent = new string(' ', line.IndentLevel * 4);
            var groupName = line.CaptureGroupName;

            switch (line.Role)
            {
                case PrettifiedRegexLineRole.Separator: lineParts.Add(("", "---", "", false, line)); break;
                case PrettifiedRegexLineRole.WordBoundary: lineParts.Add((line.Text, "(word boundary)", "", false, line)); break;
                case PrettifiedRegexLineRole.ConnectiveMatch: lineParts.Add((indent + PrettifyInternalText(line.Text), "connective match", "", false, line)); break;
                case PrettifiedRegexLineRole.CharacterRange: lineParts.Add((indent + PrettifyInternalText(line.Text), "match range", "", true, line)); break;
                case PrettifiedRegexLineRole.CaptureGroupStart: lineParts.Add(($"{indent}{line.Text}", groupName, groupTypes.GetValueOrDefault(groupName, ""), false, line)); break;
                case PrettifiedRegexLineRole.CaptureGroupEnd:
                    var endComment = string.IsNullOrEmpty(groupName) ? "" : $"____END {groupName}____";
                    lineParts.Add(($"{indent}{line.Text}", endComment, "", false, line)); break;
                case PrettifiedRegexLineRole.EnumValue:
                    lineParts.Add((indent + PrettifyInternalText(line.Text), "enum member", "", true, line));
                    break;
                case PrettifiedRegexLineRole.Comment:
                    // FIXED: Use line.Text for the regex part and line.Comment for the semantic part.
                    lineParts.Add((indent + line.Text, line.Comment, "", false, line)); break;
                default:
                    lineParts.Add(($"{indent}{line.Text}", "", "", false, line)); break;
            }
        }

        // Post-process to inject alternation symbols
        for (int i = 1; i < lineParts.Count; i++)
        {
            var current = lineParts[i];
            var prev = lineParts[i - 1];
            // FIXED: Broaden the roles that are considered part of an alternation sequence
            var alternationRoles = new[] { PrettifiedRegexLineRole.EnumValue, PrettifiedRegexLineRole.CharacterRange };
            if (alternationRoles.Contains(current.OriginalLine.Role) &&
                alternationRoles.Contains(prev.OriginalLine.Role) &&
                current.OriginalLine.IndentLevel == prev.OriginalLine.IndentLevel)
            {
                var indent = new string(' ', current.OriginalLine.IndentLevel * 4);
                lineParts[i] = ($"{indent.Substring(2)}| {current.Left.Trim()}", current.Comment, current.Type, current.IsInGroup, current.OriginalLine);
            }
        }

        // --- Step 3: Final Formatting Pass ---
        const string typeColumnPrefix = "    : ";
        const int globalCommentIndent = 4;
        const int childCommentIndent = 4;

        var firstLineIndex = lineParts.FindIndex(p => !string.IsNullOrWhiteSpace(p.Left) || p.Comment == "---");
        if (firstLineIndex != -1) { var p = lineParts[firstLineIndex]; lineParts[firstLineIndex] = (p.Left, "Start" + (p.Comment.Contains("word boundary") ? " (word boundary)" : ""), p.Type, p.IsInGroup, p.OriginalLine); }
        var lastLineIndex = lineParts.FindLastIndex(p => !string.IsNullOrWhiteSpace(p.Left) || p.Comment.StartsWith("____END") || p.Comment.Contains("word boundary"));
        if (lastLineIndex != -1) { var p = lineParts[lastLineIndex]; if (string.IsNullOrWhiteSpace(p.Comment) || p.Comment.Contains("word boundary")) lineParts[lastLineIndex] = (p.Left, "End" + (p.Comment.Contains("word boundary") ? " (word boundary)" : ""), p.Type, p.IsInGroup, p.OriginalLine); }

        // --- Width Calculation ---
        int maxLeftWidth = lineParts.Select(p => p.Left.Length).DefaultIfEmpty(0).Max();
        int hashIndex = maxLeftWidth > 0 ? maxLeftWidth + 4 : 21;
        int maxGroupNameWidth = lineParts.Select(p => p.Comment).Where(c => groupTypes.ContainsKey(c)).Select(c => c.Length).DefaultIfEmpty(0).Max();

        var commentStrings = new List<string>();
        foreach (var p in lineParts)
        {
            if (string.IsNullOrEmpty(p.Comment) || p.Comment == "---") continue;
            var sb = new StringBuilder();
            if (p.Comment.StartsWith("____END")) { sb.Append(p.Comment); }
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
            commentStrings.Add(sb.ToString());
        }
        int maxCommentWidth = commentStrings.Select(s => s.Length).DefaultIfEmpty(0).Max();
        string hr = new('-', maxCommentWidth);

        // --- Final Rendering ---
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
                    var commentContent = new StringBuilder();
                    commentContent.Append("".PadRight(globalCommentIndent));
                    if (!string.IsNullOrEmpty(p.Type))
                    {
                        commentContent.Append(p.Comment.PadRight(maxGroupNameWidth));
                        commentContent.Append($"{typeColumnPrefix}{p.Type}");
                    }
                    else
                    {
                        if (p.IsInGroup) commentContent.Append("".PadRight(childCommentIndent));
                        commentContent.Append(p.Comment);
                    }
                    sb.Append(commentContent.ToString());
                }
            }
            finalLines.Add(p.OriginalLine with { DisplayText = sb.ToString().TrimEnd() });
        }

        return (finalLines, hashIndex, -1);
    }
}