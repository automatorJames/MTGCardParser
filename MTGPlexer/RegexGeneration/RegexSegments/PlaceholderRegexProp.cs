namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// Represents a placeholder text property of type PlaceholderCapture. This property type will typically have
/// a RegexPattern attribute defining its pattern, but in the absence of one the normalized property name will
/// be used as a pattern instead. This record is tightly coupled with the PlaceholderCapture type, which represents
/// a capture that's a placeholder in the sense that the caller wants to capture the given pattern but doesn't 
/// know how to decompose it yet, or the containing TokenUnit overrides SetPropertiesFromMatch and needs a property
/// to store an interim text value.
/// </summary>
public record PlaceholderRegexProp : ScalarCapturePropBase
{
    public PlaceholderRegexProp(RegexPropInfo captureProp) : base(captureProp)
    {
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        var regexString = ScalarAlternativeSet.Alternates.Single();

        try
        {
            // Attempt to parse the regex string into a series of actions on the collector.
            var actions = Parse(regexString);

            // If parsing succeeds without error, we can confidently modify the collector.
            builder.OpenGroup(RegexPropInfo, spaceDisposition: SpaceDisposition.DisallowedGlobal);
            actions.ForEach(action => action(builder));
            builder.CloseGroup();
        }
        catch (Exception)
        {
            // If parsing fails for any reason (e.g., complexity, malformed pattern),
            // fall back to the original behavior of rendering the regex as a single literal line.
            builder.OpenGroup(RegexPropInfo, spaceDisposition: SpaceDisposition.DisallowedGlobal);
            builder.AddTextLine(regexString);
            builder.CloseGroup();
        }
    }

    /// <summary>
    /// Kicks off the parsing process for the entire regex string.
    /// </summary>
    private List<Action<RegexBuilder>> Parse(string regex)
    {
        int index = 0;
        var actions = ParseTopLevel(regex, ref index, regex.Length);

        // If the entire string wasn't consumed, the pattern is too complex or malformed.
        if (index != regex.Length)
        {
            throw new FormatException("Did not parse the entire regex string.");
        }
        return actions;
    }

    /// <summary>
    /// Parses a substring, determining if it's an alternation set or a sequence of items.
    /// </summary>
    private List<Action<RegexBuilder>> ParseTopLevel(string regex, ref int startIndex, int endIndex)
    {
        var alternatives = SplitByTopLevelAlternator(regex, startIndex, endIndex);

        if (alternatives.Count > 1)
        {
            var altStrings = alternatives.Select(pair => regex.Substring(pair.Item1, pair.Item2 - pair.Item1)).ToList();
            startIndex = endIndex; // Consume the entire block for the caller.
            return new List<Action<RegexBuilder>> { c => c.AddAlternateValues(altStrings) };
        }
        else
        {
            // Not an alternation, so treat it as a sequence.
            return ParseSequence(regex, ref startIndex, endIndex);
        }
    }

    /// <summary>
    /// Parses a sequence of regex elements, such as literals and groups.
    /// </summary>
    private List<Action<RegexBuilder>> ParseSequence(string regex, ref int index, int end)
    {
        var actions = new List<Action<RegexBuilder>>();
        var literalBuffer = new StringBuilder();

        while (index < end)
        {
            char c = regex[index];

            if (c == '(')
            {
                FlushLiteralBufferToAction(literalBuffer, actions);

                int groupContentStart = index + 1;
                int groupEnd = FindMatchingParen(regex, index, end);

                // Recursively parse the content within the parentheses.
                int contentIndex = groupContentStart;
                var groupActions = ParseTopLevel(regex, ref contentIndex, groupEnd);
                if (contentIndex != groupEnd)
                {
                    throw new FormatException("Failed to parse group content fully.");
                }

                index = groupEnd + 1; // Move past the closing parenthesis.
                var quantifier = GetQuantifier(regex, ref index);

                actions.Add(c => c.OpenGroup());
                actions.AddRange(groupActions);
                actions.Add(c => c.CloseGroup(quantifier));
            }
            else if (c == ')' || c == '|')
            {
                throw new FormatException($"Unexpected character '{c}' while parsing a sequence.");
            }
            else if (c == '\\')
            {
                if (index + 1 < end)
                {
                    literalBuffer.Append(regex[index]);
                    literalBuffer.Append(regex[index + 1]);
                    index += 2;
                }
                else
                {
                    throw new FormatException("Invalid escape sequence at the end of the regex.");
                }
            }
            else
            {
                literalBuffer.Append(c);
                index++;
            }
        }

        FlushLiteralBufferToAction(literalBuffer, actions);
        return actions;
    }

    /// <summary>
    /// If the literal buffer contains text, this method adds an action to create a TextLine.
    /// </summary>
    private void FlushLiteralBufferToAction(StringBuilder buffer, List<Action<RegexBuilder>> actions)
    {
        if (buffer.Length > 0)
        {
            var literal = buffer.ToString();
            actions.Add(c => c.AddTextLine(literal));
            buffer.Clear();
        }
    }

    /// <summary>
    /// Finds the matching closing parenthesis for an opening one.
    /// </summary>
    private int FindMatchingParen(string regex, int start, int end)
    {
        if (regex[start] != '(') throw new ArgumentException("Start position must be '('.");
        int depth = 1;
        for (int i = start + 1; i < end; i++)
        {
            if (regex[i] == '(') depth++;
            else if (regex[i] == ')')
            {
                depth--;
                if (depth == 0) return i;
            }
        }
        throw new FormatException("Mismatched parentheses in regex segment.");
    }

    /// <summary>
    /// Checks for a quantifier (+, *, ?) immediately following a group.
    /// </summary>
    private GroupQuantifier? GetQuantifier(string regex, ref int index)
    {
        if (index >= regex.Length) return null;

        GroupQuantifier? quantifier = regex[index] switch
        {
            '*' => GroupQuantifier.AnyNumber,
            '+' => GroupQuantifier.OneOrMore,
            '?' => GroupQuantifier.Optional,
            _ => null
        };

        if (quantifier.HasValue)
        {
            index++;
        }
        return quantifier;
    }

    /// <summary>
    /// Splits a regex segment by the pipe '|' character, respecting nested parentheses.
    /// </summary>
    private List<Tuple<int, int>> SplitByTopLevelAlternator(string regex, int start, int end)
    {
        var splits = new List<Tuple<int, int>>();
        int depth = 0;
        int lastSplit = start;

        for (int i = start; i < end; i++)
        {
            char c = regex[i];
            if (c == '(') depth++;
            else if (c == ')') depth--;
            else if (c == '|' && depth == 0)
            {
                splits.Add(new Tuple<int, int>(lastSplit, i));
                lastSplit = i + 1;
            }
        }
        splits.Add(new Tuple<int, int>(lastSplit, end));
        return splits;
    }


    public override object GetValueToSet(TokenUnit parentTokenUnit, Group namedGroup)
    {
        var valueToSet = new PlaceholderCapture(namedGroup.Value);

        return valueToSet;
    }
}