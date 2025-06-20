using System.Text;

namespace MTGCardParser.Static;

public static class StringExtensions
{
    public static string RenderRegex(this string regexTemplate, Type type, string delimiter = "§", bool wrapInWordBoundaries = true)
    {
        var delimiterCount = Regex.Count(regexTemplate, delimiter);
        if (delimiterCount % 2 != 0)
            throw new Exception($"Regex templates must have an even number of {delimiter} delimiters");

        var finalRegex = regexTemplate;

        var autoCaptureGroupNames = Regex.Matches(regexTemplate, @"§(?<name>[^§]+)§");

        foreach (Match autoCaptureGroupName in autoCaptureGroupNames.Cast<Match>())
        {
            var name = autoCaptureGroupName.Groups["name"].Value;
            var property = type.GetProperty(name);

            if (property is null)
                throw new Exception($"No property named {name} found on type {type.Name}");

            var propertyType = property.PropertyType;
            var enumType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

            if (!enumType.IsEnum)
                throw new Exception($"Property {name} on type {type.Name} is not an enum (or nullable enum)");

            var enumValues = Enum.GetValues(enumType)
                .Cast<object>()
                .Select(x => x.ToRegexPattern());

            var autoEnumCaptureGroupStr = GetAlternation(enumValues, name);
            finalRegex = Regex.Replace(finalRegex, autoCaptureGroupName.Value, autoEnumCaptureGroupStr);
        }

        if (wrapInWordBoundaries)
            finalRegex = finalRegex.WrapInWordBoundaries();

        return finalRegex;
    }

    public static string ToRegexPattern(this object obj, bool toLower = true)
    {
        if (obj is null)
            return null;

        string val = null;

        var type = obj.GetType();
        if (type.GetField(obj.ToString()).IsDefined(typeof(RegPatAttribute), false))
        {
            var descriptionAttribute = type.GetField(obj.ToString()).GetCustomAttributes(typeof(RegPatAttribute), false)[0] as RegPatAttribute;
            val = descriptionAttribute.Patterns.GetAlternation();
        }
        else
            val = obj.ToString();

        return val.ToLower();
    }

    public static string GetAlternation(this IEnumerable<string> items, string namedCaptureGroup = null)
    {
        var alternation = "(" + string.Join("|", items.OrderByDescending(s => s.Length)) + ")";

        if (namedCaptureGroup is not null)
            alternation = $@"\b(?<{namedCaptureGroup}>{alternation})\b";

        return alternation;
    }

    public static object ParseTokenEnumIgnoreCase(this string input, Type enumType)
    {
        if (!enumType.IsEnum)
            throw new ArgumentException($"Type {enumType.Name} is not an enum.");

        foreach (var val in Enum.GetValues(enumType))
        {
            var field = enumType.GetField(val.ToString());

            if 
            (
                field is not null 
                && field.GetCustomAttributes(typeof(RegPatAttribute), false).FirstOrDefault() is RegPatAttribute attr 
                && attr.Patterns is not null && attr.Patterns.Length > 0
            )
            {
                var pattern = attr.Patterns.GetAlternation();
                if (Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase))
                    return val;
            }
            else if (val.ToString().Equals(input, StringComparison.OrdinalIgnoreCase))
            {
                return val;
            }
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

    public static string WrapInWordBoundaries(this string regexPattern)
    {
        // Return immediately for null or empty strings.
        if (string.IsNullOrEmpty(regexPattern))
        {
            return regexPattern;
        }

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

