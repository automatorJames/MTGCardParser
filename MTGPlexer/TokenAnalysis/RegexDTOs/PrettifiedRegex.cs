namespace MTGPlexer.TokenAnalysis.RegexDTOs;

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

    // Static factory method as requested.
    public static PrettifiedRegex Create(string originalRegex)
    {
        return new PrettifiedRegex(originalRegex);
    }

    private PrettifiedRegex(string originalRegex)
    {
        OriginalRegex = originalRegex;
        try
        {
            // GOAL 1: Create an intermediate form (hierarchical tree).
            var rootFragment = RegexParser.Parse(OriginalRegex);

            // GOAL 2 & 3 (partially): Traverse the tree to generate a flat list of semantic lines.
            var initialLines = LineGenerator.GenerateLines(rootFragment);

            // GOAL 3 & 4: Format the lines, calculate column indices, and produce the final text.
            var formatResult = BuildFormattedLines(initialLines);
            Lines = formatResult.FormattedLines;
            HashColumnIndex = formatResult.HashIndex;
            TypeColumnIndex = formatResult.TypeIndex;
            DisplayText = string.Join(Environment.NewLine, Lines.Select(l => l.DisplayText));
        }
        catch (Exception ex) // Failsafe for any parsing/formatting error.
        {
            // Log the exception if a logger is available.
            Lines = [new PrettifiedRegexLine(0, null, $"// Failed to prettify: {ex.Message}", null, PrettifiedRegexLineRole.Error),
                     new PrettifiedRegexLine(1, null, originalRegex, null, PrettifiedRegexLineRole.LiteralMatch)];
            DisplayText = string.Join(Environment.NewLine, Lines.Select(l => l.Text));
            HashColumnIndex = -1;
            TypeColumnIndex = -1;
        }
    }

    private static (List<PrettifiedRegexLine> FormattedLines, int HashIndex, int TypeIndex) BuildFormattedLines(List<PrettifiedRegexLine> initialLines)
    {
        if (initialLines.Count == 0) return ([], -1, -1);

        // This function remains largely the same as your well-designed version,
        // but is adapted to use the IndentLevel from the new LineGenerator.
        static string PrettifyInternalText(string fragment) => Regex.Replace(fragment, @"(?<!\[) (?!\])", "[ ]").Replace(@"\s", "[ ]");

        var groupTypes = new Dictionary<string, string>();
        foreach (var line in initialLines.Where(l => !string.IsNullOrEmpty(l.CaptureGroupName)))
        {
            if (line.Role == PrettifiedRegexLineRole.LiteralMatch) groupTypes[line.CaptureGroupName] = "bool";
            else if (line.Role == PrettifiedRegexLineRole.FirstEnumValueInGroup) groupTypes[line.CaptureGroupName] = "enum";
        }

        var lineParts = new List<(string Left, string Comment, string Type, bool IsInGroup)>();
        foreach (var line in initialLines)
        {
            var indent = new string(' ', line.IndentLevel * 4); // Use spaces for consistent alignment
            switch (line.Role)
            {
                case PrettifiedRegexLineRole.WordBoundary: lineParts.Add((line.Text, "Word Boundary", "", false)); break;
                case PrettifiedRegexLineRole.Empty: lineParts.Add(("", "---", "", false)); break;
                case PrettifiedRegexLineRole.ConnectiveMatch: lineParts.Add((PrettifyInternalText(line.Text), "connective match", "", false)); break;
                case PrettifiedRegexLineRole.CaptureGroupStart: lineParts.Add(($"{indent}{line.Text}", line.CaptureGroupName, groupTypes.GetValueOrDefault(line.CaptureGroupName, ""), false)); break;
                case PrettifiedRegexLineRole.LiteralMatch: lineParts.Add(($"{indent}{PrettifyInternalText(line.Text)}", "literal match", "", true)); break;
                case PrettifiedRegexLineRole.FirstEnumValueInGroup: lineParts.Add(($"{indent}{PrettifyInternalText(line.Text)}", "enum member", "", true)); break;
                case PrettifiedRegexLineRole.NonFirstEnumValueInGroup: lineParts.Add(($"{indent}| {PrettifyInternalText(line.Text)}", "enum member", "", true)); break;
                case PrettifiedRegexLineRole.GenericGroupStart:
                case PrettifiedRegexLineRole.GenericGroupEnd:
                case PrettifiedRegexLineRole.Quantifier:
                case PrettifiedRegexLineRole.CharacterClass:
                case PrettifiedRegexLineRole.TokenUnitOneOfHeader:
                case PrettifiedRegexLineRole.CaptureGroupEnd:
                    lineParts.Add(($"{indent}{line.Text}", "", "", false)); break;
                default: lineParts.Add((line.Text, "", "", false)); break;
            }
        }

        const string typeColumnPrefix = "    : ";
        const int globalCommentIndent = 4;

        int maxLeftWidth = lineParts.Select(p => p.Left.Length).DefaultIfEmpty(0).Max();
        int maxGroupNameWidth = lineParts.Where(p => !string.IsNullOrEmpty(p.Type)).Select(p => p.Comment.Length).DefaultIfEmpty(0).Max();

        int hashIndex = maxLeftWidth + 4;
        int typeIndex = hashIndex + 1 + globalCommentIndent + maxGroupNameWidth + typeColumnPrefix.IndexOf(':');

        int maxTypeLength = groupTypes.Values.Select(v => v.Length).DefaultIfEmpty(0).Max();
        int totalCommentWidth = maxGroupNameWidth + typeColumnPrefix.Length + maxTypeLength;
        string hr = new('-', totalCommentWidth + globalCommentIndent);

        var finalLines = new List<PrettifiedRegexLine>();
        for (int i = 0; i < initialLines.Count; i++)
        {
            var (originalLine, parts) = (initialLines[i], lineParts[i]);
            var sb = new StringBuilder();
            sb.Append(parts.Left.PadRight(maxLeftWidth));

            if (!string.IsNullOrEmpty(parts.Comment))
            {
                sb.Append("    # ");
                if (parts.Comment == "---") sb.Append(hr);
                else
                {
                    sb.Append("".PadRight(globalCommentIndent));
                    if (!string.IsNullOrEmpty(parts.Type))
                    {
                        sb.Append(parts.Comment.PadRight(maxGroupNameWidth));
                        sb.Append($"{typeColumnPrefix}{parts.Type}");
                    }
                    else
                    {
                        if (parts.IsInGroup) sb.Append("    ");
                        sb.Append(parts.Comment);
                    }
                }
            }
            finalLines.Add(originalLine with { DisplayText = sb.ToString().TrimEnd() });
        }
        return (finalLines, hashIndex, typeIndex);
    }
}