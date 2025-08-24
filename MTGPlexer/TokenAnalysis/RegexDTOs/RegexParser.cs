namespace MTGPlexer.TokenAnalysis.RegexDTOs;

using MTGPlexer.RegexSegmentDTOs;
using System;
using System.Text;

// Static factory class to digest a raw regex string into a hierarchical tree of fragments.
public static class RegexParser
{
    public static RegexGroupFragment Parse(string regex)
    {
        if (string.IsNullOrEmpty(regex)) return new RegexGroupFragment(null, RegexGroupType.Root, "", "");

        var root = new RegexGroupFragment(null, RegexGroupType.Root, "", "");
        var stack = new Stack<RegexGroupFragment>();
        stack.Push(root);

        int i = 0;
        while (i < regex.Length)
        {
            var currentGroup = stack.Peek();
            char c = regex[i];

            if (c == '(')
            {
                var newGroup = CreateGroup(currentGroup, regex, ref i);
                currentGroup.Children.Add(newGroup);
                stack.Push(newGroup);
            }
            else if (c == ')' || c == ']')
            {
                if (stack.Count > 1) // Don't pop the root
                {
                    var finishedGroup = stack.Pop();
                    i++; // Consume closing char
                    // Check for a quantifier immediately after the group
                    if (i < regex.Length && "?*+".Contains(regex[i]))
                    {
                        finishedGroup.Quantifier = regex[i].ToString();
                        i++;
                    }
                }
                else i++; // Unmatched closing paren, just consume it
            }
            else if (c == '[')
            {
                var charClassGroup = new RegexGroupFragment(currentGroup, RegexGroupType.CharacterClass, "[", "]");
                i++; // consume '['
                int end = regex.IndexOf(']', i);
                if (end == -1) end = regex.Length;
                charClassGroup.Children.Add(new RegexTextFragment(charClassGroup, regex.Substring(i, end - i)));
                i = end;
                currentGroup.Children.Add(charClassGroup);
            }
            else if (c == '|')
            {
                currentGroup.Children.Add(new RegexTextFragment(currentGroup, "|"));
                i++;
            }
            else
            {
                var textFragment = ReadText(regex, ref i);
                currentGroup.Children.Add(new RegexTextFragment(currentGroup, textFragment));
            }
        }
        return root;
    }

    private static RegexGroupFragment CreateGroup(RegexGroupFragment parent, string regex, ref int i)
    {
        i++; // Consume '('
        if (regex.Length > i + 2 && regex.Substring(i, 2) == "?<") // Named Capture Group
        {
            i += 2;
            int nameEnd = regex.IndexOf('>', i);
            string name = regex.Substring(i, nameEnd - i);
            i = nameEnd + 1;
            return new RegexGroupFragment(parent, RegexGroupType.NamedCapture, $"(?<{name}>", ")") { Name = name };
        }
        if (regex.Length > i + 2 && regex.Substring(i, 2) == "?#") // Comment Group (for TokenUnitOneOf)
        {
            i += 2;
            int commentEnd = regex.IndexOf(')', i);
            string comment = regex.Substring(i, commentEnd - i);
            i = commentEnd + 1;
            // The structure is ((?#...)...) so we expect another '('
            if (i < regex.Length && regex[i] == '(') i++;
            return new RegexGroupFragment(parent, RegexGroupType.TokenUnitOneOf, "((?#...)", "))") { Comment = comment };
        }
        // Anonymous Capture Group
        return new RegexGroupFragment(parent, RegexGroupType.AnonymousCapture, "(", ")");
    }

    private static string ReadText(string regex, ref int i)
    {
        var sb = new StringBuilder();
        while (i < regex.Length && !"()[]|".Contains(regex[i]))
        {
            if (regex[i] == '\\' && i + 1 < regex.Length) // Handle escaped characters
            {
                sb.Append(regex[i]);
                sb.Append(regex[i + 1]);
                i += 2;
            }
            else
            {
                sb.Append(regex[i]);
                i++;
            }
        }
        return sb.ToString();
    }
}