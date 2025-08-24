namespace MTGPlexer.TokenAnalysis.RegexDTOs;

using System.Collections.Generic;

// Traverses the RegexFragment tree and flattens it into a list of semantic lines.
public static class LineGenerator
{
    private static List<PrettifiedRegexLine> _lines;
    private static int _lineNumber;

    public static List<PrettifiedRegexLine> GenerateLines(RegexGroupFragment root)
    {
        _lines = new List<PrettifiedRegexLine>();
        _lineNumber = 0;
        Traverse(root, 0);
        return _lines;
    }

    private static void Traverse(IRegexFragment fragment, int indent)
    {
        if (fragment is RegexGroupFragment group)
        {
            // Add opening line for the group
            AddGroupLine(group, indent, isOpening: true);

            bool isFirstInAlternatives = true;
            foreach (var child in group.Children)
            {
                if (child is RegexTextFragment text && text.Text == "|")
                {
                    isFirstInAlternatives = false;
                    continue; // The "|" is handled by the role of the next sibling
                }

                if (child is IRegexFragment)
                {
                    // This is where we determine if it's the first member of an enum-like group
                    if (group.Type == RegexGroupType.NamedCapture || group.Type == RegexGroupType.AnonymousCapture)
                    {
                        if (isFirstInAlternatives)
                        {
                            (child as dynamic).IsFirstAlternative = true;
                        }
                    }
                    Traverse(child, indent + 1);
                    isFirstInAlternatives = true; // Reset after processing a non-'|' fragment
                }
            }

            // Add closing line for the group
            AddGroupLine(group, indent, isOpening: false);

            if (!string.IsNullOrEmpty(group.Quantifier))
            {
                AddLine(null, group.Quantifier, null, indent, PrettifiedRegexLineRole.Quantifier);
            }
        }
        else if (fragment is RegexTextFragment text)
        {
            var role = text.Text == @"\b" ? PrettifiedRegexLineRole.WordBoundary : PrettifiedRegexLineRole.ConnectiveMatch;

            // This is a hacky way to check the flag set by the parent. A better system would pass this context down.
            if ((fragment as dynamic).IsFirstAlternative is true)
            {
                role = PrettifiedRegexLineRole.FirstEnumValueInGroup;
            }
            else if (text.Parent is RegexGroupFragment g && g.Children.IndexOf(text) > 0 && g.Children[g.Children.IndexOf(text) - 1] is RegexTextFragment t && t.Text == "|")
            {
                role = PrettifiedRegexLineRole.NonFirstEnumValueInGroup;
            }

            AddLine(GetParentGroupName(text), text.Text, text.Text, indent, role);
        }
    }

    private static void AddGroupLine(RegexGroupFragment group, int indent, bool isOpening)
    {
        if (group.Type == RegexGroupType.Root) return;

        string text = isOpening ? group.OpeningDelimiter : group.ClosingDelimiter;
        var role = PrettifiedRegexLineRole.GenericGroupStart; // Default

        switch (group.Type)
        {
            case RegexGroupType.NamedCapture:
                role = isOpening ? PrettifiedRegexLineRole.CaptureGroupStart : PrettifiedRegexLineRole.CaptureGroupEnd;
                break;
            case RegexGroupType.TokenUnitOneOf:
                text = isOpening ? $"(?# {group.Comment})" : ")"; // The outer parens are handled by children
                role = PrettifiedRegexLineRole.TokenUnitOneOfHeader;
                if (!isOpening) return; // Only show the header
                break;
            case RegexGroupType.CharacterClass:
                text = $"[{string.Join("", group.Children.OfType<RegexTextFragment>().Select(t => t.Text))}]";
                role = PrettifiedRegexLineRole.CharacterClass;
                if (!isOpening) return; // Rendered as a single line
                break;
            case RegexGroupType.AnonymousCapture:
                role = isOpening ? PrettifiedRegexLineRole.GenericGroupStart : PrettifiedRegexLineRole.GenericGroupEnd;
                break;
        }

        AddLine(group.Name, text, null, indent, role);
    }

    private static void AddLine(string groupName, string text, string matchPattern, int indent, PrettifiedRegexLineRole role)
    {
        if (string.IsNullOrEmpty(text)) return;
        _lines.Add(new PrettifiedRegexLine(_lineNumber++, groupName, text, $@"\b{matchPattern}\b", role) { IndentLevel = indent });
    }

    private static string GetParentGroupName(IRegexFragment fragment)
    {
        var current = fragment.Parent;
        while (current != null)
        {
            if (current is RegexGroupFragment group && group.Type == RegexGroupType.NamedCapture)
            {
                return group.Name;
            }
            current = current.Parent;
        }
        return null;
    }
}