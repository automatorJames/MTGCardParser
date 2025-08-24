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

    /// <summary>
    /// The zero-based character index where the '#' comment column begins for all lines.
    /// </summary>
    public int HashColumnIndex { get; }

    /// <summary>
    /// The zero-based character index where the ':' for the type annotation (e.g., ": bool") begins.
    /// </summary>
    public int TypeColumnIndex { get; }

    public PrettifiedRegex(string originalRegex)
    {
        OriginalRegex = originalRegex;
        var initialLines = GetLines(OriginalRegex);

        // The line building method now returns the calculated lines and the column indices.
        var formatResult = BuildFormattedLines(initialLines);
        Lines = formatResult.FormattedLines;
        HashColumnIndex = formatResult.HashIndex;
        TypeColumnIndex = formatResult.TypeIndex;

        DisplayText = string.Join(Environment.NewLine, Lines.Select(l => l.DisplayText));
    }

    private static (List<PrettifiedRegexLine> FormattedLines, int HashIndex, int TypeIndex) BuildFormattedLines(List<PrettifiedRegexLine> initialLines)
    {
        if (initialLines.Count == 0)
        {
            return ([], -1, -1);
        }

        static string PrettifyInternalText(string fragment)
        {
            return Regex.Replace(fragment, @"(?<!\[) (?!\])", "[ ]");
        }

        // Step 1: Pre-compute capture group types.
        var groupTypes = new Dictionary<string, string>();
        foreach (var line in initialLines.Where(l => !string.IsNullOrEmpty(l.CaptureGroupName)))
        {
            if (line.Role == PrettifiedRegexLineRole.LiteralMatch)
            {
                groupTypes[line.CaptureGroupName] = "bool";
            }
            else if (line.Role == PrettifiedRegexLineRole.FirstEnumValueInGroup)
            {
                groupTypes[line.CaptureGroupName] = "enum";
            }
        }

        // Step 2: Create an intermediate representation.
        var lineParts = new List<(string Left, string Comment, string Type, bool IsInGroup)>();
        bool isFirstBoundary = true;

        foreach (var line in initialLines)
        {
            switch (line.Role)
            {
                case PrettifiedRegexLineRole.WordBoundary:
                    lineParts.Add((line.Text, $"{(isFirstBoundary ? "Start" : "End")} (word boundary)", "", false));
                    isFirstBoundary = false;
                    break;
                case PrettifiedRegexLineRole.Empty:
                    lineParts.Add(("", "---", "", false));
                    break;
                case PrettifiedRegexLineRole.ConnectiveMatch:
                    lineParts.Add((PrettifyInternalText(line.Text), "connective match", "", false));
                    break;
                case PrettifiedRegexLineRole.CaptureGroupStart:
                    lineParts.Add((line.Text, line.CaptureGroupName, groupTypes.GetValueOrDefault(line.CaptureGroupName, ""), false));
                    break;
                case PrettifiedRegexLineRole.LiteralMatch:
                    lineParts.Add(($"    {PrettifyInternalText(line.Text)}", "literal match", "", true));
                    break;
                case PrettifiedRegexLineRole.FirstEnumValueInGroup:
                    lineParts.Add(($"      {PrettifyInternalText(line.Text)}", "enum member", "", true));
                    break;
                case PrettifiedRegexLineRole.NonFirstEnumValueInGroup:
                    lineParts.Add(($"    | {PrettifyInternalText(line.Text)}", "enum member", "", true));
                    break;
                default:
                    lineParts.Add((line.Text, "", "", false));
                    break;
            }
        }

        // Step 3: Calculate maximum widths and final column indices.
        const string typeColumnPrefix = "    : ";
        const int globalCommentIndent = 4;

        int maxLeftWidth = lineParts.Select(p => p.Left.Length).DefaultIfEmpty(0).Max();
        int maxGroupNameWidth = lineParts
            .Where(p => !string.IsNullOrEmpty(p.Type))
            .Select(p => p.Comment.Length)
            .DefaultIfEmpty(0)
            .Max();

        int hashIndex = maxLeftWidth + 4;
        int typeIndex = hashIndex + 1 + globalCommentIndent + maxGroupNameWidth + typeColumnPrefix.IndexOf(':');

        // Calculate total width of the right-hand side for the horizontal rule.
        int maxTypeLength = groupTypes.Values.Select(v => v.Length).DefaultIfEmpty(0).Max();
        int totalCommentWidth = maxGroupNameWidth + typeColumnPrefix.Length + maxTypeLength;
        string hr = new('-', totalCommentWidth + globalCommentIndent);

        // Step 4: Build the final list of lines with all padding applied.
        var finalLines = new List<PrettifiedRegexLine>();
        for (int i = 0; i < initialLines.Count; i++)
        {
            var (originalLine, parts) = (initialLines[i], lineParts[i]);
            var sb = new StringBuilder();

            sb.Append(parts.Left.PadRight(maxLeftWidth));

            if (!string.IsNullOrEmpty(parts.Comment))
            {
                if (parts.Comment == "---")
                {
                    sb.Append("    # ");
                    sb.Append(hr);
                }
                else
                {
                    sb.Append("    #".PadRight(5 + globalCommentIndent));
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

    static List<PrettifiedRegexLine> GetLines(string regex)
    {
        List<PrettifiedRegexLine> lines = [];
        if (string.IsNullOrEmpty(regex)) return lines;

        int lineNumber = 0;
        string workRegex = regex;

        void AddLine(string captureGroupName, string text, string regexMatchPattern, PrettifiedRegexLineRole role)
        {
            lines.Add(new PrettifiedRegexLine(lineNumber++, captureGroupName, text, regexMatchPattern, role));
        }

        void AddEmptyLine()
        {
            if (lines.Count > 0 && lines.Last().Role != PrettifiedRegexLineRole.Empty)
            {
                AddLine(null, "", null, PrettifiedRegexLineRole.Empty);
            }
        }

        var groupParserRegex = new Regex(
            @"\(\?<(?<name>\w+)>(?<content>(?>[^()]+|\((?<DEPTH>)|\)(?<-DEPTH>))*(?(DEPTH)(?!)))\)(?<optional>\?)?",
            RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline
        );

        if (workRegex.StartsWith(@"\b"))
        {
            AddLine(null, @"\b", null, PrettifiedRegexLineRole.WordBoundary);
            workRegex = workRegex.Substring(2);
        }

        bool hasEndBoundary = workRegex.EndsWith(@"\b");
        if (hasEndBoundary)
        {
            workRegex = workRegex.Substring(0, workRegex.Length - 2);
        }

        int lastIndex = 0;
        foreach (Match match in groupParserRegex.Matches(workRegex))
        {
            if (match.Index > lastIndex)
            {
                AddEmptyLine();
                string connective = workRegex.Substring(lastIndex, match.Index - lastIndex);
                AddLine(null, connective, null, PrettifiedRegexLineRole.ConnectiveMatch);
            }

            AddEmptyLine();
            string groupName = match.Groups["name"].Value;
            string content = match.Groups["content"].Value;
            bool isOptional = match.Groups["optional"].Success;

            AddLine(groupName, $"(?<{groupName}>", null, PrettifiedRegexLineRole.CaptureGroupStart);

            if (content.Contains('|'))
            {
                string[] alternatives = content.Split('|');
                AddLine(groupName, alternatives[0].Trim(), $@"\b{alternatives[0].Trim()}\b", PrettifiedRegexLineRole.FirstEnumValueInGroup);
                for (int i = 1; i < alternatives.Length; i++)
                {
                    string alt = alternatives[i].Trim();
                    AddLine(groupName, alt, $@"\b{alt}\b", PrettifiedRegexLineRole.NonFirstEnumValueInGroup);
                }
            }
            else
            {
                AddLine(groupName, content, $@"\b{content.Trim()}\b", PrettifiedRegexLineRole.LiteralMatch);
            }

            AddLine(groupName, isOptional ? ")?" : ")", null, PrettifiedRegexLineRole.CaptureGroupEnd);
            lastIndex = match.Index + match.Length;
        }

        if (lastIndex < workRegex.Length)
        {
            AddEmptyLine();
            string finalConnective = workRegex.Substring(lastIndex);
            AddLine(null, finalConnective, null, PrettifiedRegexLineRole.ConnectiveMatch);
        }

        if (hasEndBoundary)
        {
            AddEmptyLine();
            AddLine(null, @"\b", null, PrettifiedRegexLineRole.WordBoundary);
        }

        if (lines.Count > 0 && lines[0].Role == PrettifiedRegexLineRole.Empty) lines.RemoveAt(0);
        if (lines.Count > 0 && lines.Last().Role == PrettifiedRegexLineRole.Empty) lines.RemoveAt(lines.Count - 1);

        for (int i = 0; i < lines.Count; i++)
        {
            lines[i] = lines[i] with { LineNumber = i };
        }

        return lines;
    }
}