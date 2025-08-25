namespace MTGPlexer.TokenAnalysis.RegexDTOs.Internal;

using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Digests a raw regex string into a hierarchical tree of fragments using a recursive descent parser.
/// </summary>
public static class RegexParser
{
    private static string _regex;
    private static int _position;

    public static RegexGroupFragment Parse(string regex)
    {
        if (string.IsNullOrEmpty(regex))
        {
            return new RegexGroupFragment(RegexGroupType.Root, "", "", []);
        }

        _regex = regex;
        _position = 0;

        var children = ParseChildrenUntil((char)0); // Parse until the end of the string
        var root = new RegexGroupFragment(RegexGroupType.Root, "", "", children);
        SetParents(root);
        CoalesceOptionalSuffixes(root); // Post-processing step to merge items like "lose(s)?"
        return root;
    }

    private static void SetParents(RegexGroupFragment group)
    {
        foreach (var child in group.Children)
        {
            child.Parent = group;
            if (child is RegexGroupFragment childGroup)
            {
                SetParents(childGroup);
            }
        }
    }

    /// <summary>
    /// Post-processes the tree to merge simple optional groups (e.g., "(s)?")
    /// with their preceding text fragment within enum-like capture groups.
    /// </summary>
    private static void CoalesceOptionalSuffixes(RegexGroupFragment group)
    {
        // Recurse first to process from the inside out.
        foreach (var child in group.Children.OfType<RegexGroupFragment>())
        {
            CoalesceOptionalSuffixes(child);
        }

        // An "enum group" is a named capture that contains alternation pipes.
        bool isEnum = group.Type == RegexGroupType.NamedCapture &&
                      group.Children.Any(c => c is RegexTextFragment { Text: "|" });

        if (!isEnum) return;

        var newChildren = new List<RegexFragment>();
        for (int i = 0; i < group.Children.Count; i++)
        {
            // Pattern: TextFragment followed by a simple, optional, anonymous Group.
            if (i + 1 < group.Children.Count &&
                group.Children[i] is RegexTextFragment currentText &&
                group.Children[i + 1] is RegexGroupFragment nextGroup &&
                nextGroup.Type == RegexGroupType.AnonymousCapture &&
                nextGroup.Quantifier == "?" &&
                nextGroup.Children.Count == 1 &&
                nextGroup.Children[0] is RegexTextFragment { Text.Length: <= 2 }) // Heuristic for "s", "es", etc.
            {
                // Coalesce them into a single new text fragment.
                var combinedText = currentText.Text + nextGroup.ToString();
                newChildren.Add(new RegexTextFragment(combinedText) { Parent = group });
                i++; // Skip the next element, as it has been consumed.
            }
            else
            {
                // If the pattern doesn't match, just add the current child.
                newChildren.Add(group.Children[i]);
            }
        }

        // Replace the old children list with the new, coalesced list.
        group.Children.Clear();
        group.Children.AddRange(newChildren);
    }

    private static List<RegexFragment> ParseChildrenUntil(char terminator)
    {
        var children = new List<RegexFragment>();
        var textBuffer = new StringBuilder();

        while (_position < _regex.Length && _regex[_position] != terminator)
        {
            // Specifically handle \b as its own token.
            if (_position + 1 < _regex.Length && _regex[_position] == '\\' && _regex[_position + 1] == 'b')
            {
                if (textBuffer.Length > 0)
                {
                    children.Add(new RegexTextFragment(textBuffer.ToString()));
                    textBuffer.Clear();
                }
                children.Add(new RegexTextFragment(@"\b"));
                _position += 2;
                continue;
            }

            if (_position < _regex.Length && _regex[_position] == '\\' && _position + 1 < _regex.Length)
            {
                textBuffer.Append(_regex[_position]);
                textBuffer.Append(_regex[_position + 1]);
                _position += 2;
                continue;
            }

            char c = _regex[_position];
            if ("()[]|".Contains(c))
            {
                if (textBuffer.Length > 0)
                {
                    children.Add(new RegexTextFragment(textBuffer.ToString()));
                    textBuffer.Clear();
                }

                switch (c)
                {
                    case '(': children.Add(ParseGroup()); break;
                    case '[': children.Add(ParseCharClass()); break;
                    case '|': children.Add(new RegexTextFragment("|")); _position++; break;
                    case ')': return children; // End of current group
                }
            }
            else
            {
                textBuffer.Append(c);
                _position++;
            }
        }

        if (textBuffer.Length > 0)
        {
            children.Add(new RegexTextFragment(textBuffer.ToString()));
        }

        return children;
    }

    private static RegexGroupFragment ParseGroup()
    {
        int groupStartPos = _position;
        _position++; // Consume '('

        string name = null;
        string comment = null;
        var type = RegexGroupType.AnonymousCapture;
        string openingDelimiter = "(";

        if (_position < _regex.Length && _regex[_position] == '?')
        {
            int tagStart = groupStartPos;
            if (_position + 2 < _regex.Length && _regex.Substring(_position, 2) == "?<") // Named Capture
            {
                type = RegexGroupType.NamedCapture;
                int nameEnd = _regex.IndexOf('>', _position);
                name = _regex.Substring(_position + 2, nameEnd - (_position + 2));
                _position = nameEnd + 1;
                openingDelimiter = _regex.Substring(tagStart, _position - tagStart);
            }
            else if (_position + 2 < _regex.Length && _regex.Substring(_position, 2) == "?#") // Comment
            {
                type = RegexGroupType.Comment;
                int commentEnd = _regex.IndexOf(')', _position);
                comment = _regex.Substring(_position + 2, commentEnd - (_position + 2));
                _position = commentEnd + 1;
                openingDelimiter = _regex.Substring(tagStart, _position - tagStart);
                return new RegexGroupFragment(type, openingDelimiter, "", [], Comment: comment);
            }
        }

        var children = ParseChildrenUntil(')');
        if (_position < _regex.Length && _regex[_position] == ')')
        {
            _position++; // Consume ')'
        }

        string quantifier = null;
        if (_position < _regex.Length && "?*+".Contains(_regex[_position]))
        {
            quantifier = _regex[_position].ToString();
            _position++;
        }

        return new RegexGroupFragment(type, openingDelimiter, ")", children, name, comment, quantifier);
    }

    private static RegexGroupFragment ParseCharClass()
    {
        int startPos = _position;
        int endPos = _regex.IndexOf(']', startPos + 1);
        if (endPos == -1) endPos = _regex.Length - 1;

        endPos = System.Math.Min(endPos, _regex.Length - 1);

        string content = _regex.Substring(startPos + 1, endPos - startPos - 1);
        _position = endPos + 1;

        string quantifier = null;
        if (_position < _regex.Length && "?*+".Contains(_regex[_position]))
        {
            quantifier = _regex[_position].ToString();
            _position++;
        }

        var children = new List<RegexFragment> { new RegexTextFragment(content) };
        return new RegexGroupFragment(RegexGroupType.CharacterClass, "[", "]", children, Quantifier: quantifier);
    }
}