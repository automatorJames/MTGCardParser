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
        Traverse(state, root, 0);
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
        if (group.Type == RegexGroupType.Root)
        {
            ProcessChildren(state, group, indent, true);
            return;
        }

        // Handle comments as a single, atomic line.
        if (group.Type == RegexGroupType.Comment)
        {
            // The OpeningDelimiter is the full "(?#...)", and the Comment property has the inner text.
            AddLine(state, group.Parent?.Name, group.OpeningDelimiter, null, indent, PrettifiedRegexLineRole.Comment, group.Comment);
            return;
        }

        // Add a separator before any non-root group
        if (indent > 0)
        {
            AddLine(state, null, "", null, indent, PrettifiedRegexLineRole.Separator);
        }

        AddLine(state, group.Name ?? group.Parent?.Name, group.OpeningDelimiter, null, indent, GetRole(group, true));
        ProcessChildren(state, group, indent + 1, false);
        AddLine(state, group.Name ?? group.Parent?.Name, group.ClosingDelimiter + group.Quantifier, null, indent, GetRole(group, false));
    }

    private static void ProcessChildren(TraversalState state, RegexGroupFragment group, int indent, bool isTopLevel, List<RegexFragment> childrenOverride = null)
    {
        var children = childrenOverride ?? group.Children;
        for (int i = 0; i < children.Count; i++)
        {
            if (isTopLevel && i > 0)
            {
                AddLine(state, null, "", null, indent, PrettifiedRegexLineRole.Separator);
            }
            Traverse(state, children[i], indent);
        }
    }

    private static void HandleText(TraversalState state, RegexTextFragment text, int indent)
    {
        string parentName = text.Parent?.Name;
        PrettifiedRegexLineRole role;

        switch (text.Text)
        {
            case @"\b": role = PrettifiedRegexLineRole.WordBoundary; break;
            case "|": role = PrettifiedRegexLineRole.Alternation; break;
            default:
                if (text.Text.StartsWith("["))
                {
                    role = PrettifiedRegexLineRole.CharacterRange;
                }
                else
                {
                    // It's an enum value if it's part of an alternation sequence.
                    bool isEnumContext = false;
                    if (text.Parent != null)
                    {
                        var siblings = text.Parent.Children;
                        int myIndex = siblings.IndexOf(text);

                        bool prevIsAlternation = myIndex > 0 && siblings[myIndex - 1] is RegexTextFragment { Text: "|" };
                        bool nextIsAlternation = myIndex < siblings.Count - 1 && siblings[myIndex + 1] is RegexTextFragment { Text: "|" };
                        bool isFirstInAlternation = myIndex == 0 && siblings.Count > 1 && siblings[1] is RegexTextFragment { Text: "|" };

                        isEnumContext = prevIsAlternation || nextIsAlternation || isFirstInAlternation;
                    }
                    role = isEnumContext ? PrettifiedRegexLineRole.EnumValue : PrettifiedRegexLineRole.ConnectiveMatch;
                }
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

    private static void AddLine(TraversalState state, string groupName, string text, string matchPattern, int indent, PrettifiedRegexLineRole role, string comment = null)
    {
        // CORRECTED: Create the line with distinct Text and Comment properties.
        var line = new PrettifiedRegexLine(state.LineNumber++, groupName, text, matchPattern, role)
        {
            IndentLevel = indent,
            Comment = comment
        };
        state.Lines.Add(line);
    }
}