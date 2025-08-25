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
        internal int LineNumber { get; set; } = 0;
    }

    public static List<PrettifiedRegexLine> GenerateLines(RegexGroupFragment root)
    {
        var state = new TraversalState();
        Traverse(state, root, 0, null, true);
        return state.Lines;
    }

    private static void Traverse(TraversalState state, RegexFragment fragment, int indent, string parentGroupName, bool isFirstInAlternation)
    {
        if (fragment is RegexGroupFragment group)
        {
            HandleGroup(state, group, indent, parentGroupName);
        }
        else if (fragment is RegexTextFragment text)
        {
            HandleText(state, text, indent, parentGroupName, isFirstInAlternation);
        }
    }

    private static void HandleGroup(TraversalState state, RegexGroupFragment group, int indent, string parentGroupName)
    {
        AddGroupLine(state, group, indent, true);

        bool isFirstChild = true;
        foreach (var child in group.Children)
        {
            if (child is RegexTextFragment txt && txt.Text == "|")
            {
                isFirstChild = false;
                continue;
            }
            Traverse(state, child, indent + 1, group.Name ?? parentGroupName, isFirstChild);
            isFirstChild = false;
        }

        AddGroupLine(state, group, indent, false);
    }

    private static void HandleText(TraversalState state, RegexTextFragment text, int indent, string parentGroupName, bool isFirstInAlternation)
    {
        var role = text.Text == @"\b" ? PrettifiedRegexLineRole.WordBoundary : PrettifiedRegexLineRole.ConnectiveMatch;

        if (text.Text != @"\b")
        {
            if (isFirstInAlternation) role = PrettifiedRegexLineRole.FirstEnumValueInGroup;
            else role = PrettifiedRegexLineRole.NonFirstEnumValueInGroup;
        }

        AddLine(state, parentGroupName, text.Text, text.Text, indent, role);
    }

    private static void AddGroupLine(TraversalState state, RegexGroupFragment group, int indent, bool isOpening)
    {
        if (group.Type == RegexGroupType.Root) return;

        string text = isOpening ? group.OpeningDelimiter : (group.ClosingDelimiter + group.Quantifier);
        PrettifiedRegexLineRole role;

        switch (group.Type)
        {
            case RegexGroupType.NamedCapture:
                role = isOpening ? PrettifiedRegexLineRole.CaptureGroupStart : PrettifiedRegexLineRole.CaptureGroupEnd;
                break;
            case RegexGroupType.TokenUnitOneOf:
                text = $"(?# {group.Comment})";
                role = PrettifiedRegexLineRole.TokenUnitOneOfHeader;
                if (!isOpening) return; // Rendered as a single header line
                break;
            case RegexGroupType.CharacterClass:
                text = $"[{string.Join("", group.Children.OfType<RegexTextFragment>().Select(t => t.Text))}]";
                role = PrettifiedRegexLineRole.CharacterClass;
                if (!isOpening) return; // Rendered as a single line
                break;
            default: // AnonymousCapture
                role = isOpening ? PrettifiedRegexLineRole.GenericGroupStart : PrettifiedRegexLineRole.GenericGroupEnd;
                break;
        }

        AddLine(state, group.Name, text, null, indent, role);
    }

    private static void AddLine(TraversalState state, string groupName, string text, string matchPattern, int indent, PrettifiedRegexLineRole role)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        state.Lines.Add(new PrettifiedRegexLine(state.LineNumber++, groupName, text.Trim(), $@"\b{matchPattern}\b", role) { IndentLevel = indent });
    }
}
