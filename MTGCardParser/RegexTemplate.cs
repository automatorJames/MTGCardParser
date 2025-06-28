using System.Text;

namespace MTGCardParser;

public class RegexTemplate
{
    public Dictionary<PropertyInfo, string> PropRegexPatterns { get; set; } = new();
    public Dictionary<Type, Dictionary<object, Regex>> EnumRegexes { get; set; } = new();
    public string RenderedRegex { get; set; }
}

public class RegexTemplate<T> : RegexTemplate where T : ITokenCapture
{
    public RegexTemplate(params string[] templateSnippets)
    {
        List<string> renderedSnippets = new();
        var type = typeof(T);
        var props = type.GetPropertiesForCapture();

        for (int i = 0; i < templateSnippets.Length; i++)
        {
            string snippet = templateSnippets[i];
            string renderedSnippet = "";
            var matchingProp = props.FirstOrDefault(x => x.Name == snippet);

            if (matchingProp != null)
            {
                bool prependSpace = false;
                bool appendSpace = false;

                if (matchingProp.PropertyType == typeof(bool))
                {
                    if (i > 0)
                        prependSpace = true;

                    if (i < templateSnippets.Length - 1)
                        appendSpace = true;
                }

                var underlyingType = Nullable.GetUnderlyingType(matchingProp.PropertyType) ?? matchingProp.PropertyType;

                if (underlyingType.IsEnum)
                    renderedSnippet = EnumToRegexPattern(underlyingType);
                else if (underlyingType.IsAssignableTo(typeof(ITokenCapture)))
                    renderedSnippet = TokenCaptureSubPropertyToRegexPattern(matchingProp);
                else
                    renderedSnippet = PropertyToRegexPattern(matchingProp, prependSpace, appendSpace);

                PropRegexPatterns[matchingProp] = renderedSnippet;
            }
            else
                renderedSnippet = snippet.ToLower();

            renderedSnippets.Add(renderedSnippet);
        }

        var renderedRegex = JoinWithSmartSpacing(renderedSnippets);
        renderedRegex = WrapInSmartWordBoundaries(renderedRegex);

        //if (type != typeof(Punctuation))
        //    renderedRegex = @"\b" + renderedRegex + @"\b";

        RenderedRegex = renderedRegex;
    }

    string EnumToRegexPattern(Type underlyingEnumType)
    {
        List<string> enumSnippets = new();
        EnumRegexes[underlyingEnumType] = new();
        var enumRegOptions = underlyingEnumType.GetCustomAttribute<RegexOptionsAttribute>() ?? new();
        var enumValues = Enum.GetValues(underlyingEnumType).Cast<object>();

        // Local helper to add snippet to list and dictionary
        void AddSnippet(object enumValue, string pattern)
        {
            enumSnippets.Add(pattern);
            EnumRegexes[underlyingEnumType][enumValue] = new Regex(pattern);
        }

        foreach (var enumValue in enumValues)
        {
            var enumAsString = enumValue.ToString();
            var regexPatternAttribute = underlyingEnumType.GetField(enumAsString).GetCustomAttribute<RegexPatternAttribute>();

            // Check if enum member has a specified string[] of patterns
            if (regexPatternAttribute != null)
            {
                var alternatedPattern = GetAlternatives(regexPatternAttribute.Patterns, wrapInWordBoundaries: false);
                AddSnippet(enumValue, alternatedPattern);
                enumSnippets.Add(alternatedPattern);
            }
            // Otherwise add the stringified enum iteself
            else
            {
                var valueLower = enumAsString.ToLower();

                if (enumRegOptions.OptionalPlural)
                    valueLower = Pluralize(valueLower);

                AddSnippet(enumValue, valueLower);
            }
        }

        return GetCaptureGroupOfAlternatives(enumSnippets, underlyingEnumType.Name, wrapInWordBoundaries: enumRegOptions.WrapInWordBoundaries, groupIsOptional: false);
    }

    string TokenCaptureSubPropertyToRegexPattern(PropertyInfo subTokenCaptureProp)
    {
        var instanceOfPropType = (ITokenCapture)Activator.CreateInstance(subTokenCaptureProp.PropertyType);
        return instanceOfPropType.RenderedRegex;
    }

    string PropertyToRegexPattern(PropertyInfo prop, bool prependSpace, bool appendSpace)
    {
        var regexPatternAttribute = prop.GetCustomAttribute<RegexPatternAttribute>();
        var patterns = regexPatternAttribute?.Patterns ?? [prop.Name];
        var groupIsOptional = prop.PropertyType == typeof(bool);

        return GetCaptureGroupOfAlternatives(
            patterns, 
            prop.Name, 
            wrapInWordBoundaries: true, 
            groupIsOptional: groupIsOptional,
            prependSpace: prependSpace,
            appendSpace: appendSpace
            );
    }

    string GetCaptureGroupOfAlternatives(IEnumerable<string> items, string namedCaptureGroup, bool wrapInWordBoundaries, bool groupIsOptional, bool prependSpace = false, bool appendSpace = false)
    {
        var str = $@"(?<{namedCaptureGroup}>{(prependSpace ? " " : "")}{string.Join("|", items.OrderByDescending(s => s.Length))}{(appendSpace ? " " : "")})";

        if (groupIsOptional)
            str += "?";

        return str;
    }

    string GetAlternatives(IEnumerable<string> items, bool wrapInWordBoundaries)
    {
        var str = "(" + string.Join("|", items.OrderByDescending(s => s.Length)) + ")";

        //if (wrapInWordBoundaries)
        //    str = @"\b" + str + @"\b";

        return str;
    }


    public static string Pluralize(string word)
    {
        if (string.IsNullOrWhiteSpace(word))
            throw new ArgumentNullException(nameof(word));

        if (word.EndsWith("y", StringComparison.OrdinalIgnoreCase) && word.Length > 1 && !"aeiou".Contains(char.ToLower(word[word.Length - 2])))
            word = word.Substring(0, word.Length - 1) + "(ies)";
        else if (word.EndsWith("s", StringComparison.OrdinalIgnoreCase) ||
            word.EndsWith("x", StringComparison.OrdinalIgnoreCase) ||
            word.EndsWith("z", StringComparison.OrdinalIgnoreCase) ||
            word.EndsWith("ch", StringComparison.OrdinalIgnoreCase) ||
            word.EndsWith("sh", StringComparison.OrdinalIgnoreCase))
            word += "(es)";
        else word += "(s)";

        return word + "?";
    }

    static readonly HashSet<string> TerminalPunctuation = new()
    {
        ".", ",", ";"
    };

    static readonly HashSet<string> MatchedPairChars = new()
    {
        "\""
    };


    string JoinWithSmartSpacing(List<string> segments)
    {
        var str = "";
        int matchedPairCharCount = 0;

        for (int i = 0; i < segments.Count; i++)
        {
            var segment = segments[i];

            if (i > 0)
            {
                bool spaceBefore = !TerminalPunctuation.Contains(segment) && matchedPairCharCount % 2 == 0;
                str += spaceBefore ? " " : "";
            }

            matchedPairCharCount += MatchedPairChars.Contains(segment) ? 1 : 0;
            str += segment;
        }

        if (matchedPairCharCount % 2 != 0)
            throw new Exception("Paired chars not matched");

        return str.Trim();
    }

    public static string WrapInSmartWordBoundaries(string regexPattern)
    {
        // Return immediately for null or empty strings.
        if (string.IsNullOrEmpty(regexPattern))
            return regexPattern;

        bool prependBoundary = false;
        bool appendBoundary = false;

        // --- Check if the pattern STARTS with a word character ---

        char firstChar = regexPattern[0];
        if (firstChar == '\\')
        {
            // It's an escape sequence. Check what is being escaped.
            if (regexPattern.Length > 1)
            {
                char escapedChar = regexPattern[1];
                // In regex, \w (word) and \d (digit) represent word characters.
                // Other common escapes like \s, \n, \t, \. are not word characters.
                if (escapedChar == 'w' || escapedChar == 'd')
                {
                    prependBoundary = true;
                }
            }
        }
        // In C#, the \w shorthand is equivalent to [a-zA-Z0-9_].
        else if (char.IsLetterOrDigit(firstChar) || firstChar == '_')
        {
            // It's a literal character that is part of a word.
            prependBoundary = true;
        }

        // --- Check if the pattern ENDS with a word character ---

        char lastChar = regexPattern[regexPattern.Length - 1];

        // Check for single-character patterns that are word characters, like "a" or "_".
        if (regexPattern.Length == 1)
        {
            if (char.IsLetterOrDigit(lastChar) || lastChar == '_')
            {
                appendBoundary = true;
            }
        }
        else // Pattern has 2 or more characters
        {
            char penultimateChar = regexPattern[regexPattern.Length - 2];
            if (penultimateChar == '\\')
            {
                // The end of the string is an escape sequence, like \d or \n or \\.
                // Check if the sequence itself represents a word character.
                if (lastChar == 'w' || lastChar == 'd')
                {
                    appendBoundary = true;
                }
                // Note: If the pattern ends in \\, 'lastChar' is '\', which is not a 
                // letter/digit, so it's correctly ignored.
            }
            else if (char.IsLetterOrDigit(lastChar) || lastChar == '_')
            {
                // The last character is a literal that is part of a word.
                appendBoundary = true;
            }
        }

        // --- Build the final string ---

        // If no changes are needed, return the original string to avoid allocation.
        if (!prependBoundary && !appendBoundary)
        {
            return regexPattern;
        }

        var sb = new StringBuilder();
        if (prependBoundary)
        {
            sb.Append(@"\b");
        }

        sb.Append(regexPattern);

        if (appendBoundary)
        {
            sb.Append(@"\b");
        }

        return sb.ToString();
    }
}

