namespace MTGPlexer.TokenAnalysis.RegexDTOs.Internal;

/// <summary>
/// Digests a raw regex string into a hierarchical tree of fragments using a recursive descent parser.
/// </summary>
public static class RegexParser
{
    private static string _regex;
    private static int _position;

    public static RegexRoot Parse(string regex)
    {
        if (string.IsNullOrEmpty(regex))
        {
            return new RegexRoot([]);
        }

        _regex = regex;
        _position = 0;

        var children = ParseChildrenUntil((char)0); // Parse until the end of the string
        var root = new RegexRoot(children);
        SetParents(root);
        CoalesceOptionalSuffixes(root); // Post-processing step
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

    private static void CoalesceOptionalSuffixes(RegexGroupFragment group)
    {
        foreach (var child in group.Children.OfType<RegexGroupFragment>())
        {
            CoalesceOptionalSuffixes(child);
        }

        bool isEnum = group.Type == RegexGroupType.NamedCapture &&
                      group.Children.Any(c => c is RegexTextFragment { Text: "|" });

        if (!isEnum) return;

        var newChildren = new List<RegexFragment>();
        for (int i = 0; i < group.Children.Count; i++)
        {
            if (i + 1 < group.Children.Count &&
                group.Children[i] is RegexTextFragment currentText &&
                group.Children[i + 1] is RegexGroupFragment nextGroup &&
                nextGroup.Type == RegexGroupType.AnonymousCapture &&
                nextGroup.Quantifier == "?" &&
                nextGroup.Children.Count == 1 &&
                nextGroup.Children[0] is RegexTextFragment { Text.Length: <= 2 })
            {
                var combinedText = currentText.Text + nextGroup.ToString();
                newChildren.Add(new RegexTextFragment(combinedText) { Parent = group });
                i++;
            }
            else
            {
                newChildren.Add(group.Children[i]);
            }
        }
        group.Children.Clear();
        group.Children.AddRange(newChildren);
    }

    private static List<RegexFragment> ParseChildrenUntil(char terminator)
    {
        var children = new List<RegexFragment>();
        var textBuffer = new StringBuilder();

        while (_position < _regex.Length && _regex[_position] != terminator)
        {
            if (_position + 1 < _regex.Length && _regex[_position] == '\\' && _regex[_position + 1] == 'b')
            {
                if (textBuffer.Length > 0) { children.Add(new RegexTextFragment(textBuffer.ToString())); textBuffer.Clear(); }
                children.Add(new RegexTextFragment(@"\b"));
                _position += 2;
                continue;
            }

            if (_position < _regex.Length && _regex[_position] == '\\' && _position + 1 < _regex.Length)
            {
                textBuffer.Append(_regex, _position, 2);
                _position += 2;
                continue;
            }

            char c = _regex[_position];
            if ("()[]|".Contains(c))
            {
                if (textBuffer.Length > 0) { children.Add(new RegexTextFragment(textBuffer.ToString())); textBuffer.Clear(); }

                switch (c)
                {
                    case '(': children.Add(ParseGroup()); break;
                    case '[': children.Add(ParseCharClass()); break;
                    case '|': children.Add(new RegexTextFragment("|")); _position++; break;
                    case ')': return children;
                }
            }
            else
            {
                textBuffer.Append(c);
                _position++;
            }
        }
        if (textBuffer.Length > 0) { children.Add(new RegexTextFragment(textBuffer.ToString())); }
        return children;
    }

    private static RegexGroupFragment ParseGroup()
    {
        int groupStartPos = _position;
        _position++; // Consume '('

        string name = null, comment = null, openingDelimiter = "(";
        var type = RegexGroupType.AnonymousCapture;

        if (_position < _regex.Length && _regex[_position] == '?')
        {
            int tagStart = groupStartPos;
            if (_position + 2 < _regex.Length && _regex.Substring(_position, 2) == "?<")
            {
                type = RegexGroupType.NamedCapture;
                int nameEnd = _regex.IndexOf('>', _position);
                name = _regex.Substring(_position + 2, nameEnd - (_position + 2));
                _position = nameEnd + 1;
                openingDelimiter = _regex.Substring(tagStart, _position - tagStart);
            }
            else if (_position + 2 < _regex.Length && _regex.Substring(_position, 2) == "?#")
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
        if (_position < _regex.Length && _regex[_position] == ')') _position++;

        string quantifier = null;
        if (_position < _regex.Length && "?*+".Contains(_regex[_position]))
        {
            quantifier = _regex[_position].ToString();
            _position++;
        }
        else if (_position < _regex.Length && _regex[_position] == '{')
        {
            int quantEnd = _regex.IndexOf('}', _position);
            if (quantEnd != -1)
            {
                quantifier = _regex.Substring(_position, quantEnd - _position + 1);
                _position = quantEnd + 1;
            }
        }

        return new RegexGroupFragment(type, openingDelimiter, ")", children, name, comment, quantifier);
    }

    /// <summary>
    /// Parses a character class as an atomic text fragment, including its quantifier.
    /// </summary>
    private static RegexTextFragment ParseCharClass()
    {
        int startPos = _position;
        int endPos = _regex.IndexOf(']', startPos + 1);
        if (endPos == -1) endPos = _regex.Length - 1;
        _position = endPos + 1;

        // Check for a quantifier immediately after the character class
        if (_position < _regex.Length && "?*+".Contains(_regex[_position]))
        {
            _position++;
        }

        return new RegexTextFragment(_regex.Substring(startPos, _position - startPos));
    }
}