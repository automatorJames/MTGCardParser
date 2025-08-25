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

    private static List<RegexFragment> ParseChildrenUntil(char terminator)
    {
        var children = new List<RegexFragment>();
        var textBuffer = new StringBuilder();

        while (_position < _regex.Length && _regex[_position] != terminator)
        {
            char c = _regex[_position];

            if (c == '\\' && _position + 1 < _regex.Length)
            {
                textBuffer.Append(c);
                textBuffer.Append(_regex[_position + 1]);
                _position += 2;
                continue;
            }

            // If we hit a special character, first flush any accumulated text.
            if ("()[]|".Contains(c))
            {
                if (textBuffer.Length > 0)
                {
                    children.Add(new RegexTextFragment(textBuffer.ToString()));
                    textBuffer.Clear();
                }

                switch (c)
                {
                    case '(':
                        children.Add(ParseGroup());
                        break;
                    case '[':
                        children.Add(ParseCharClass());
                        break;
                    case '|':
                        children.Add(new RegexTextFragment("|"));
                        _position++;
                        break;
                    case ')': // Should only be hit if it's the terminator
                        return children;
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
            int tagStart = _position - 1;
            if (_position + 1 < _regex.Length && _regex[_position + 1] == '<') // Named Capture
            {
                type = RegexGroupType.NamedCapture;
                int nameStart = _position + 2;
                int nameEnd = _regex.IndexOf('>', nameStart);
                name = _regex.Substring(nameStart, nameEnd - nameStart);
                _position = nameEnd + 1;
                openingDelimiter = _regex.Substring(tagStart, _position - tagStart);
            }
            else if (_position + 1 < _regex.Length && _regex[_position + 1] == '#') // Comment
            {
                type = RegexGroupType.Comment;
                int commentStart = _position + 2;
                int commentEnd = _regex.IndexOf(')', commentStart);
                comment = _regex.Substring(commentStart, commentEnd - commentStart);
                _position = commentEnd + 1; // Consume comment and ')'
                openingDelimiter = _regex.Substring(tagStart, _position - tagStart);
                // Comment groups have no children and are self-closing in the parser's view.
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

        // Ensure we don't read past the end of the string if ']' is missing
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