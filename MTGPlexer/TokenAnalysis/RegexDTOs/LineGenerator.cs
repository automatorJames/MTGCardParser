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
        if (fragment is RegexGroupFragment group)
        {
            HandleGroup(state, group, indent);
        }
        else if (fragment is RegexTextFragment text)
        {
            HandleText(state, text, indent);
        }
    }

    private static void HandleGroup(TraversalState state, RegexGroupFragment group, int indent)
    {
        if (group.Type == RegexGroupType.Root)
        {
            ProcessChildren(state, group, indent, false);
            return;
        }

        // Special case for character classes, which are rendered on a single line
        if (group.Type == RegexGroupType.CharacterClass)
        {
            string content = string.Concat(group.Children.OfType<RegexTextFragment>().Select(c => c.Text));
            AddLine(state, group.Parent?.Name, $"[{content}]{group.Quantifier}", $"[{content}]{group.Quantifier}", indent, PrettifiedRegexLineRole.CharacterClass);
            return;
        }

        // Handle the special ((?#...)...) structure
        var firstChild = group.Children.FirstOrDefault();
        if (group.Type == RegexGroupType.AnonymousCapture && firstChild is RegexGroupFragment commentGroup && commentGroup.Type == RegexGroupType.Comment)
        {
            AddLine(state, group.Parent?.Name, group.OpeningDelimiter + commentGroup.OpeningDelimiter, null, indent, PrettifiedRegexLineRole.TokenUnitOneOfHeader);
            ProcessChildren(state, group, indent + 1, true); // Process inner children
            AddLine(state, group.Parent?.Name, group.ClosingDelimiter + group.Quantifier, null, indent, PrettifiedRegexLineRole.GenericGroupEnd);
            return;
        }

        AddLine(state, group.Name ?? group.Parent?.Name, group.OpeningDelimiter, null, indent, GetRole(group, true));
        ProcessChildren(state, group, indent + 1, false);
        AddLine(state, group.Name ?? group.Parent?.Name, group.ClosingDelimiter + group.Quantifier, null, indent, GetRole(group, false));
    }

    private static void ProcessChildren(TraversalState state, RegexGroupFragment group, int indent, bool isInsideTokenUnit)
    {
        bool isFirstInAlternation = true;

        foreach (var child in group.Children)
        {
            if (isInsideTokenUnit && child is RegexGroupFragment commentGroup && commentGroup.Type == RegexGroupType.Comment)
            {
                continue; // Skip rendering the comment again, it was handled in the header
            }

            if (child is RegexTextFragment txt && txt.Text == "|")
            {
                // The next non-text item will be the start of a new alternation.
                isFirstInAlternation = true;
                continue; // The "|" is handled by the NonFirstEnumValueInGroup role now
            }

            // For text fragments inside a group, determine their role
            if (child is RegexTextFragment textFragment)
            {
                string parentName = group.Name ?? group.Parent?.Name;
                PrettifiedRegexLineRole role;

                if (group.Children.Any(c => c is RegexTextFragment t && t.Text == "|"))
                {
                    role = isFirstInAlternation ? PrettifiedRegexLineRole.FirstEnumValueInGroup : PrettifiedRegexLineRole.NonFirstEnumValueInGroup;
                }
                else
                {
                    role = textFragment.Text == @"\b" ? PrettifiedRegexLineRole.WordBoundary : PrettifiedRegexLineRole.ConnectiveMatch;
                }
                AddLine(state, parentName, textFragment.Text, textFragment.Text, indent, role);
            }
            else
            {
                Traverse(state, child, indent);
            }

            isFirstInAlternation = false;
        }
    }

    private static void HandleText(TraversalState state, RegexTextFragment text, int indent)
    {
        // This handles text fragments that are direct children of the root
        var role = text.Text == @"\b" ? PrettifiedRegexLineRole.WordBoundary : PrettifiedRegexLineRole.ConnectiveMatch;
        AddLine(state, text.Parent?.Name, text.Text, text.Text, indent, role);
    }

    private static PrettifiedRegexLineRole GetRole(RegexGroupFragment group, bool isOpening)
    {
        return group.Type switch
        {
            RegexGroupType.NamedCapture => isOpening ? PrettifiedRegexLineRole.CaptureGroupStart : PrettifiedRegexLineRole.CaptureGroupEnd,
            _ => isOpening ? PrettifiedRegexLineRole.GenericGroupStart : PrettifiedRegexLineRole.GenericGroupEnd,
        };
    }

    private static void AddLine(TraversalState state, string groupName, string text, string matchPattern, int indent, PrettifiedRegexLineRole role)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        state.Lines.Add(new PrettifiedRegexLine(state.LineNumber++, groupName, text.Trim(), matchPattern != null ? $@"\b{matchPattern}\b" : null, role) { IndentLevel = indent });
    }
}