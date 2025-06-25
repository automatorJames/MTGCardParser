using System.Text;

namespace MTGCardParser.Static;

public static class Extensions
{
    public static string ToRegexPattern(this object obj, bool toLower = true, bool addOptionalPlural = false)
    {
        if (obj is null)
            return null;

        string val = null;

        var type = obj.GetType();
        if (type.GetField(obj.ToString()).IsDefined(typeof(RegPatAttribute), false))
        {
            var regPatAttribute = type.GetField(obj.ToString()).GetCustomAttributes(typeof(RegPatAttribute), false)[0] as RegPatAttribute;
            val = regPatAttribute.Patterns.GetAlternation();
        }
        else
            val = obj.ToString();

        if (addOptionalPlural)
            val = val.Pluralize();

        return val.ToLower();
    }

    public static string ToRegexPattern(this PropertyInfo prop, bool toLower = true, bool spaceAfter = true)
    {
        if (prop is null)
            return null;

        string val = prop.GetCustomAttribute<RegPatAttribute>()?.Patterns?.GetAlternation() ?? prop.Name;

        if (spaceAfter)
            val += " ";

        val = $@"(?<{prop.Name}>{val.ToLower()})?";

        //if (isOptionalMatch)
        //    val = $"({val})?";

        return val;
    }

    public static string GetAlternation(this IEnumerable<string> items, string namedCaptureGroup = null, bool wrapInWordBoundaries = true)
    {
        if (items.Count() == 1)
            return items.First();

        var alternation = "(" + string.Join("|", items.OrderByDescending(s => s.Length)) + ")";

        if (namedCaptureGroup is not null)
            alternation = $@"(?<{namedCaptureGroup}>{alternation})";

        if (wrapInWordBoundaries)
            alternation = $@"\b{alternation}\b";

        return alternation;
    }

    public static object ParseTokenEnumValue(this string input, Type enumType, Type containingType)
    {
        if (!enumType.IsEnum)
            throw new ArgumentException($"Type {enumType.Name} is not an enum.");

        foreach (var val in Enum.GetValues(enumType))
        {
            var field = enumType.GetField(val.ToString());

            if 
            (
                field.GetCustomAttributes(typeof(RegPatAttribute), false).FirstOrDefault() is RegPatAttribute attr 
                && attr.Patterns is not null 
                && attr.Patterns.Length > 0
            )
            {
                var pattern = attr.Patterns.GetAlternation();
                if (Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase))
                    return val;
            }
            else if (enumType.ShouldPluralize(containingType))
            {
                var pluralizedVal = val.ToString().ToLower().Pluralize();
                if (Regex.IsMatch(input, pluralizedVal, RegexOptions.IgnoreCase))
                    return val;
            }
            else if (val.ToString().Equals(input, StringComparison.OrdinalIgnoreCase))
                return val;
        }

        throw new ArgumentException($"No matching enum value of type {enumType.Name} for string '{input}'.");
    }

    public static string GetRegexTemplate(this Type type)
    {
        if (!typeof(ITokenCapture).IsAssignableFrom(type))
            throw new ArgumentException($"Type {type.Name} does not implement {nameof(ITokenCapture)}.");

        var instance = (ITokenCapture)Activator.CreateInstance(type)!;
        return instance.RegexTemplate;
    }

    public static string GetRenderedRegex(this Type type)
    {
        if (!typeof(ITokenCapture).IsAssignableFrom(type))
            throw new ArgumentException($"Type {type.Name} does not implement {nameof(ITokenCapture)}.");

        var instance = (ITokenCapture)Activator.CreateInstance(type)!;
        return instance.RenderedRegex;
    }

    public static string WrapInWordBoundaries(this string regexPattern)
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

    public static bool ShouldPluralize(this Type enumType, Type containingType)
    {
        var shouldPluralizeEnum = enumType.GetCustomAttribute<RegOptAttribute>()?.OptionalPlural ?? false;

        if (shouldPluralizeEnum)
            return true;

        return containingType.GetCustomAttribute<RegOptAttribute>()?.OptionalPlural ?? false;
    }
        
    public static string Pluralize(this string word, bool appendQuestionMark = true)
    {
        if (string.IsNullOrWhiteSpace(word))
            return word;

        // Basic rules (can be expanded for more accuracy)
        if (word.EndsWith("y", StringComparison.OrdinalIgnoreCase) && word.Length > 1 && !"aeiou".Contains(char.ToLower(word[word.Length - 2])))
            return word.Substring(0, word.Length - 1) + "ies";
        if (word.EndsWith("s", StringComparison.OrdinalIgnoreCase) ||
            word.EndsWith("x", StringComparison.OrdinalIgnoreCase) ||
            word.EndsWith("z", StringComparison.OrdinalIgnoreCase) ||
            word.EndsWith("ch", StringComparison.OrdinalIgnoreCase) ||
            word.EndsWith("sh", StringComparison.OrdinalIgnoreCase))
            return word + "es";

        return word + "s" + (appendQuestionMark ? "?" : "");
    }
}

