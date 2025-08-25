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
        bool isFirstTopLevelElement = true;
        foreach (var child in root.Children)
        {
            if (!isFirstTopLevelElement)
            {
                // Add a separator between each top-level element.
                AddLine(state, null, "", null, 0, PrettifiedRegexLineRole.Separator);
            }
            Traverse(state, child, 0);
            isFirstTopLevelElement = false;
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
        if (group.Type == RegexGroupType.CharacterClass)
        {
            var content = string.Concat(group.Children.OfType<RegexTextFragment>().Select(c => c.Text));
            AddLine(state, group.Parent?.Name, $"[{content}]{group.Quantifier}", $"[{content}]{group.Quantifier}", indent, PrettifiedRegexLineRole.CharacterClass);
            return;
        }

        AddLine(state, group.Name ?? group.Parent?.Name, group.OpeningDelimiter, null, indent, GetRole(group, true));

        // Process children
        bool isFirstInAlternation = true;
        bool hasAlternations = group.Children.Any(c => c is RegexTextFragment { Text: "|" });

        foreach (var child in group.Children)
        {
            if (child is RegexTextFragment tf && tf.Text == "|")
            {
                isFirstInAlternation = true;
                continue;
            }

            if (child is RegexTextFragment childText && hasAlternations)
            {
                var role = isFirstInAlternation ? PrettifiedRegexLineRole.FirstEnumValueInGroup : PrettifiedRegexLineRole.NonFirstEnumValueInGroup;
                AddLine(state, group.Name, childText.Text, childText.Text, indent + 1, role);
            }
            else
            {
                Traverse(state, child, indent + 1);
            }
            isFirstInAlternation = false;
        }

        AddLine(state, group.Name ?? group.Parent?.Name, group.ClosingDelimiter + group.Quantifier, null, indent, GetRole(group, false));
    }

    private static void HandleText(TraversalState state, RegexTextFragment text, int indent)
    {
        var role = text.Text == @"\b" ? PrettifiedRegexLineRole.WordBoundary : PrettifiedRegexLineRole.ConnectiveMatch;
        AddLine(state, text.Parent?.Name, text.Text, text.Text, indent, role);
    }

    private static PrettifiedRegexLineRole GetRole(RegexGroupFragment group, bool isOpening)
    {
        return group.Type switch
        {
            RegexGroupType.NamedCapture => isOpening ? PrettifiedRegexLineRole.CaptureGroupStart : PrettifiedRegexLineRole.CaptureGroupEnd,
            _ => isOpening ? PrettifiedRegexLineRole.GenericGroupStart : PrettifiedRegexLineRole.GenericGroupEnd
        };
    }

    private static void AddLine(TraversalState state, string groupName, string text, string matchPattern, int indent, PrettifiedRegexLineRole role)
    {
        state.Lines.Add(new PrettifiedRegexLine(state.LineNumber++, groupName, text.Trim(), matchPattern, role) { IndentLevel = indent });
    }
}