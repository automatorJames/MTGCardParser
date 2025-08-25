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

        var groupTypes = new Dictionary<string, string>();
        foreach (var line in initialLines.Where(l => !string.IsNullOrEmpty(l.CaptureGroupName) && l.Role == PrettifiedRegexLineRole.FirstEnumValueInGroup))
        {
            groupTypes[line.CaptureGroupName] = "enum";
        }

        var lineParts = new List<(string Left, string Comment, string Type, PrettifiedRegexLine OriginalLine)>();
        foreach (var line in initialLines)
        {
            var indent = new string(' ', line.IndentLevel * 4);
            var groupName = line.CaptureGroupName;
            switch (line.Role)
            {
                case PrettifiedRegexLineRole.Separator: lineParts.Add(("", "---", "", line)); break;
                case PrettifiedRegexLineRole.WordBoundary: lineParts.Add((line.Text, "(word boundary)", "", line)); break;
                case PrettifiedRegexLineRole.ConnectiveMatch: lineParts.Add((indent + PrettifyInternalText(line.Text), "connective match", "", line)); break;
                case PrettifiedRegexLineRole.CaptureGroupStart: lineParts.Add(($"{indent}{line.Text}", groupName, groupTypes.GetValueOrDefault(groupName, ""), line)); break;
                case PrettifiedRegexLineRole.CaptureGroupEnd:
                    var endComment = string.IsNullOrEmpty(groupName) ? "" : $"____END {groupName}____";
                    lineParts.Add(($"{indent}{line.Text}", endComment, "", line)); break;
                case PrettifiedRegexLineRole.FirstEnumValueInGroup: lineParts.Add(($"{indent}{PrettifyInternalText(line.Text)}", "enum member", "", line)); break;
                case PrettifiedRegexLineRole.NonFirstEnumValueInGroup: lineParts.Add(($"{indent.Substring(2)}| {PrettifyInternalText(line.Text)}", "enum member", "", line)); break;
                case PrettifiedRegexLineRole.GenericGroupStart:
                case PrettifiedRegexLineRole.GenericGroupEnd:
                case PrettifiedRegexLineRole.CharacterClass:
                case PrettifiedRegexLineRole.TokenUnitOneOfHeader: lineParts.Add(($"{indent}{line.Text}", "", "", line)); break;
                default: lineParts.Add((line.Text, "", "", line)); break;
            }
        }

        // Post-processing for Start/End comments
        var firstLineIndex = lineParts.FindIndex(p => !string.IsNullOrWhiteSpace(p.Left) || p.Comment == "---");
        if (firstLineIndex != -1)
        {
            var p = lineParts[firstLineIndex];
            var comment = "Start" + (p.Comment.Contains("word boundary") ? " (word boundary)" : "");
            if (p.Comment != "---") lineParts[firstLineIndex] = (p.Left, comment, p.Type, p.OriginalLine);
        }
        var lastLineIndex = lineParts.FindLastIndex(p => !string.IsNullOrWhiteSpace(p.Left) || p.Comment == "---");
        if (lastLineIndex != -1)
        {
            var p = lineParts[lastLineIndex];
            if (string.IsNullOrWhiteSpace(p.Comment) || p.Comment.Contains("word boundary"))
            {
                var comment = "End" + (p.Comment.Contains("word boundary") ? " (word boundary)" : "");
                lineParts[lastLineIndex] = (p.Left, comment, p.Type, p.OriginalLine);
            }
        }

        const string typeColumnPrefix = "    : ";
        const int globalCommentIndent = 4;

        int maxLeftWidth = lineParts.Select(p => p.Left.Length).DefaultIfEmpty(0).Max();
        int hashIndex = maxLeftWidth > 0 ? maxLeftWidth + 4 : 21;
        int maxGroupNameWidth = lineParts.Where(p => !string.IsNullOrEmpty(p.Type)).Select(p => p.Comment.Length).DefaultIfEmpty(0).Max();
        int typeIndex = hashIndex + 1 + globalCommentIndent + maxGroupNameWidth + typeColumnPrefix.IndexOf(':');

        int maxCommentSectionWidth = 0;
        foreach (var p in lineParts)
        {
            if (string.IsNullOrEmpty(p.Comment) || p.Comment == "---") continue;

            int currentCommentWidth;
            if (p.Comment.StartsWith("____END"))
            {
                currentCommentWidth = p.Comment.Length;
            }
            else
            {
                currentCommentWidth = globalCommentIndent;
                if (!string.IsNullOrEmpty(p.Type))
                {
                    currentCommentWidth += p.Comment.PadRight(maxGroupNameWidth).Length + typeColumnPrefix.Length + p.Type.Length;
                }
                else
                {
                    currentCommentWidth += p.Comment.Length;
                }
            }
            maxCommentSectionWidth = Math.Max(maxCommentSectionWidth, currentCommentWidth);
        }

        string hr = new('-', maxCommentSectionWidth);
        if (hr.Length == 0) hr = "----";

        var finalLines = new List<PrettifiedRegexLine>();
        foreach (var p in lineParts)
        {
            var sb = new StringBuilder();
            sb.Append(p.Left.PadRight(hashIndex - 4));
            sb.Append("    #");

            if (!string.IsNullOrWhiteSpace(p.Comment))
            {
                sb.Append(" ");
                if (p.Comment == "---")
                {
                    sb.Append(hr);
                }
                else if (p.Comment.StartsWith("____END"))
                {
                    sb.Append(p.Comment.PadRight(hr.Length, '_'));
                }
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
                        sb.Append(p.Comment);
                    }
                }
            }
            finalLines.Add(p.OriginalLine with { DisplayText = sb.ToString().TrimEnd() });
        }

        return (finalLines, hashIndex, typeIndex);
    }
}