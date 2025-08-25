namespace MTGPlexer.TokenAnalysis.RegexDTOs.Internal;

using System.Collections.Generic;
using System.Text;

/// <summary>
/// Static factory class to digest a raw regex string into a hierarchical tree of fragments.
/// </summary>
public static class RegexParser
{
    public static RegexGroupFragment Parse(string regex)
    {
        if (string.IsNullOrEmpty(regex))
            return new RegexGroupFragment(RegexGroupType.Root, "", "", []);

        var root = new RegexGroupFragment(RegexGroupType.Root, "", "", []);
        var stack = new Stack<RegexGroupFragment>();
        stack.Push(root);

        int i = 0;
        while (i < regex.Length)
        {
            var currentGroup = stack.Peek();
            char c = regex[i];

            if (c == '(')
            {
                var (newGroup, consumedLength) = CreateGroup(regex, i);
                i += consumedLength;
                currentGroup.Children.Add(newGroup);
                stack.Push(newGroup);
            }
            else if (c == ')')
            {
                if (stack.Count > 1) // Don't pop the root
                {
                    var finishedGroup = stack.Pop();
                    i++; // Consume ')'

                    if (i < regex.Length && "?*+".Contains(regex[i]))
                    {
                        // Use `with` expression to create a new, updated record
                        var updatedGroup = finishedGroup with { Quantifier = regex[i].ToString() };
                        // Replace the old group in the parent's children list
                        var parent = stack.Peek();
                        int index = parent.Children.IndexOf(finishedGroup);
                        parent.Children[index] = updatedGroup;
                        i++;
                    }
                }
                else i++; // Unmatched closing paren
            }
            else if (c == '[')
            {
                var (charClass, consumed) = CreateCharClass(regex, i);
                i += consumed;
                currentGroup.Children.Add(charClass);
            }
            else
            {
                var (textFragment, consumed) = ReadText(regex, i);
                if (!string.IsNullOrEmpty(textFragment.Text))
                    currentGroup.Children.Add(textFragment);
                i += consumed;
            }
        }
        return root;
    }

    private static (RegexGroupFragment group, int consumed) CreateGroup(string regex, int start)
    {
        int i = start + 1; // Consume '('
        if (regex.Length > i + 2 && regex.Substring(i, 2) == "?<") // Named Capture Group
        {
            i += 2;
            int nameEnd = regex.IndexOf('>', i);
            string name = regex.Substring(i, nameEnd - i);
            return (new RegexGroupFragment(RegexGroupType.NamedCapture, $"(?<{name}>", ")", [], Name: name), nameEnd + 1 - start);
        }
        if (regex.Length > i + 2 && regex.Substring(i, 2) == "?#") // Comment Group (for TokenUnitOneOf)
        {
            i += 2;
            int commentEnd = regex.IndexOf(')', i);
            string comment = regex.Substring(i, commentEnd - i);
            i = commentEnd + 1;
            if (i < regex.Length && regex[i] == '(') i++;
            return (new RegexGroupFragment(RegexGroupType.TokenUnitOneOf, "((?#...)", "))", [], Comment: comment), i - start);
        }
        return (new RegexGroupFragment(RegexGroupType.AnonymousCapture, "(", ")", []), 1);
    }

    private static (RegexFragment fragment, int consumed) CreateCharClass(string regex, int start)
    {
        int i = start + 1; // consume '['
        int end = regex.IndexOf(']', i);
        if (end == -1) end = regex.Length;
        var children = new List<RegexFragment> { new RegexTextFragment(regex.Substring(i, end - i)) };
        var group = new RegexGroupFragment(RegexGroupType.CharacterClass, "[", "]", children);
        return (group, end + 1 - start);
    }

    private static (RegexTextFragment fragment, int consumed) ReadText(string regex, int start)
    {
        var sb = new StringBuilder();
        int i = start;
        while (i < regex.Length && !"()[]".Contains(regex[i]))
        {
            if (regex[i] == '\\' && i + 1 < regex.Length)
            {
                sb.Append(regex, i, 2);
                i += 2;
            }
            else
            {
                sb.Append(regex[i]);
                i++;
            }
        }
        return (new RegexTextFragment(sb.ToString()), i - start);
    }
}
