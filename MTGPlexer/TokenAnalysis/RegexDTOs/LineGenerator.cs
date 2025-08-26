namespace MTGPlexer.TokenAnalysis.RegexDTOs.Internal;

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Traverses the RegexFragment tree and flattens it into a list of semantic lines for rendering.
/// </summary>
public static class LineGenerator
{
    private class TraversalState
    {
        internal List<PrettifiedRegexLine> Lines { get; } = [];
        internal int LineNumber { get; set; }
    }

    public static List<PrettifiedRegexLine> GenerateLines(RegexGroupFragment root)
    {
        var state = new TraversalState();
        bool isFirstElement = true;
        foreach (var child in root.Children)
        {
            if (!isFirstElement)
            {
                AddLine(state, null, "", null, 0, PrettifiedRegexLineRole.Separator);
            }
            Traverse(state, child, 0);
            isFirstElement = false;
        }
        return state.Lines;
    }

    private static void Traverse(TraversalState state, RegexFragment fragment, int indent)
    {
        switch (fragment)
        {
            case RegexGroupFragment group:
                HandleGroup(state, group, indent);
                break;
            case RegexTextFragment text:
                HandleText(state, text, indent);
                break;
        }
    }

    private static void HandleGroup(TraversalState state, RegexGroupFragment group, int indent)
    {
        // Add a separator before nested named capture groups.
        if (group.Type == RegexGroupType.NamedCapture && indent > 0)
        {
            AddLine(state, null, "", null, indent, PrettifiedRegexLineRole.Separator);
        }

        // Handle the special ((?#...)...) structure
        var firstChild = group.Children.FirstOrDefault();
        if (group.Type == RegexGroupType.AnonymousCapture && firstChild is RegexGroupFragment commentGroup && commentGroup.Type == RegexGroupType.Comment)
        {
            AddLine(state, group.Parent?.Name, group.OpeningDelimiter + commentGroup.OpeningDelimiter, null, indent, PrettifiedRegexLineRole.TokenUnitOneOfHeader);
            ProcessChildren(state, group, indent + 1);
            AddLine(state, group.Parent?.Name, group.ClosingDelimiter + group.Quantifier, null, indent, PrettifiedRegexLineRole.GenericGroupEnd);
            return;
        }

        AddLine(state, group.Name ?? group.Parent?.Name, group.OpeningDelimiter, null, indent, GetRole(group, true));
        ProcessChildren(state, group, indent + 1);
        AddLine(state, group.Name ?? group.Parent?.Name, group.ClosingDelimiter + group.Quantifier, null, indent, GetRole(group, false));
    }

    private static void ProcessChildren(TraversalState state, RegexGroupFragment group, int indent)
    {
        bool hasAlternations = group.Children.Any(c => c is RegexTextFragment { Text: "|" });

        foreach (var child in group.Children)
        {
            // The formatter will now handle prepending "|", so we just pass fragments through.
            Traverse(state, child, indent);
        }
    }

    private static void HandleText(TraversalState state, RegexTextFragment text, int indent)
    {
        string parentName = text.Parent?.Name;
        PrettifiedRegexLineRole role;

        switch (text.Text)
        {
            case @"\b":
                role = PrettifiedRegexLineRole.WordBoundary;
                break;
            case "|":
                role = PrettifiedRegexLineRole.Alternation;
                break;
            default:
                // If the parent group has alternations, its text children are enum members.
                bool parentHasAlternations = text.Parent?.Children.Any(c => c is RegexTextFragment { Text: "|" }) ?? false;
                role = parentHasAlternations ? PrettifiedRegexLineRole.FirstEnumValueInGroup : PrettifiedRegexLineRole.ConnectiveMatch;
                break;
        }

        AddLine(state, parentName, text.Text, text.Text, indent, role);
    }

    private static PrettifiedRegexLineRole GetRole(RegexGroupFragment group, bool isOpening)
    {
        return group.Type switch
        {
            RegexGroupType.Comment => PrettifiedRegexLineRole.Comment,
            RegexGroupType.NamedCapture => isOpening ? PrettifiedRegexLineRole.CaptureGroupStart : PrettifiedRegexLineRole.CaptureGroupEnd,
            _ => isOpening ? PrettifiedRegexLineRole.GenericGroupStart : PrettifiedRegexLineRole.GenericGroupEnd,
        };
    }

    private static void AddLine(TraversalState state, string groupName, string text, string matchPattern, int indent, PrettifiedRegexLineRole role)
    {
        state.Lines.Add(new PrettifiedRegexLine(state.LineNumber++, groupName, text.Trim(), matchPattern, role) { IndentLevel = indent });
    }
}