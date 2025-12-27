using System.ComponentModel;
using System.Diagnostics;

namespace MTGPlexer;

public static class Extensions
{
    public static string AddPluralization(this string word, bool makeOptional)
    {
        if (string.IsNullOrWhiteSpace(word))
            throw new ArgumentNullException(nameof(word));

        string root = word;
        string suffix;

        if (word.EndsWith("y", StringComparison.OrdinalIgnoreCase) &&
            word.Length > 1 &&
            !"aeiou".Contains(char.ToLower(word[^2])))
        {
            root = word[..^1];  // drop the 'y'
            suffix = "ies";
        }
        else if (word.EndsWith("s", StringComparison.OrdinalIgnoreCase) ||
                 word.EndsWith("x", StringComparison.OrdinalIgnoreCase) ||
                 word.EndsWith("z", StringComparison.OrdinalIgnoreCase) ||
                 word.EndsWith("ch", StringComparison.OrdinalIgnoreCase) ||
                 word.EndsWith("sh", StringComparison.OrdinalIgnoreCase))
        {
            suffix = "es";
        }
        else
        {
            suffix = "s";
        }

        if (makeOptional)
            suffix = $"({suffix})?";

        return root + suffix;
    }

    public static string Description(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attr = field?.GetCustomAttribute<DescriptionAttribute>();
        return attr?.Description ?? value.ToString();
    }

    public static string ToFriendlyCase(this string input, TitleDisplayOption option = TitleDisplayOption.Lower)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        // 1) Insert spaces at boundaries while preserving acronyms:
        // - (?<=[A-Z])(?=[A-Z][a-z])  → split between an acronym and the next normal word (e.g., "HTMLParser" → "HTML Parser")
        // - (?<=[a-z0-9])(?=[A-Z])    → split between lower/digit and upper (e.g., "myURL" → "my URL")
        var withSpaces = Regex.Replace(input, @"(?<=[A-Z])(?=[A-Z][a-z])|(?<=[a-z0-9])(?=[A-Z])", " ");

        // 2) Normalize to lower as the base format.
        var lowerCaseResult = withSpaces.ToLowerInvariant();

        // 3) Apply the selected casing option.
        switch (option)
        {
            case TitleDisplayOption.Sentence:
                return char.ToUpper(lowerCaseResult[0]) + lowerCaseResult.Substring(1);

            case TitleDisplayOption.Title:
                var words = lowerCaseResult.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var resultBuilder = new StringBuilder();

                for (int i = 0; i < words.Length; i++)
                {
                    var word = words[i];
                    if (i == 0 || !MinorWords.Contains(word))
                        resultBuilder.Append(char.ToUpper(word[0]) + word.Substring(1));
                    else
                        resultBuilder.Append(word);

                    if (i < words.Length - 1)
                        resultBuilder.Append(" ");
                }
                return resultBuilder.ToString();

            case TitleDisplayOption.Lower:
            default:
                return lowerCaseResult;
        }
    }

    // A set for fast lookups of common English words that should remain lowercase in title case.
    private static readonly HashSet<string> MinorWords = new HashSet<string>
    {
        "a", "an", "the", "and", "but", "or", "for", "nor", "on", "at", "to", "from", "by", "of", "in", "with"
    };

    /// <summary>
    /// Defines the casing style for formatting a string.
    /// </summary>
    public enum TitleDisplayOption
    {
        /// <summary>
        /// Converts the string to all lowercase, e.g., "my awesome property".
        /// </summary>
        Lower,

        /// <summary>
        /// Converts the string to sentence case, e.g., "My awesome property".
        /// </summary>
        Sentence,

        /// <summary>
        /// Converts the string to title case, capitalizing major words, e.g., "My Awesome Property".
        /// </summary>
        Title
    }

    public static string CapitalizeFirstWord(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        return char.ToUpper(input[0]) + input.Substring(1);
    }

    public static PropertyInfo[] GetProps(this Type type) => 
        type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

    public static string Dot(this string parentPath, params string[] nextPathParts) =>
        nextPathParts == null || nextPathParts.Length == 0 ? parentPath
        : parentPath + "." + string.Join('.', nextPathParts);

    public static string LastPathPart(this string dotPathString) =>
        dotPathString is null ? null
        : !dotPathString.Contains(".") ? dotPathString
        : dotPathString.Split(".").Last();

    public static RegexPropType GetRegexPropType(this Type type) =>
        type switch
        {
            { IsEnum: true } => RegexPropType.Enum,
            { } t when t == typeof(PlaceholderCapture) => RegexPropType.Placeholder,
            { } t when t.IsAssignableTo(typeof(DynamicCapture)) => RegexPropType.Dynamic,
            { } t when t == typeof(bool) => RegexPropType.Bool,
            { } t when typeof(TokenUnitOneOf).IsAssignableFrom(t) => RegexPropType.TokenUnitOneOf,
            { } t when typeof(TokenUnit).IsAssignableFrom(t) => RegexPropType.TokenUnit,
            _ => throw new Exception($"{type.Name} is not a valid {nameof(RegexPropType)} type")
        };

    public static void CombineDictionaryCounts<T>(this Dictionary<T, int> dictToAddTo, Dictionary<T, int> dictToAddFrom)
    {
        foreach (var key in dictToAddFrom.Keys)
        {
            if (!dictToAddTo.ContainsKey(key))
                dictToAddTo[key] = 0;

            dictToAddTo[key] += dictToAddFrom[key];
        }
    }

    /// <summary>
    /// Removes all non-essential whitespace from a regex pattern, preserving literal spaces indicated by "[ ]".
    /// </summary>
    public static string MinifyRegex(string pattern)
    {
        if (string.IsNullOrEmpty(pattern)) return string.Empty;

        return Regex.Replace(pattern, @"(\[\ \])|(\s+)", match =>
        {
            if (match.Groups[1].Success)
            {
                return " ";
            }
            return string.Empty;
        });
    }

    public static string TrimStartAndEnd(this string target, string trimString)
    {
        if (string.IsNullOrEmpty(trimString)) return target;

        var result = target.TrimStart(trimString);
        result = result.TrimEnd(trimString);

        return result;
    }

    public static string TrimStart(this string target, string trimString)
    {
        if (string.IsNullOrEmpty(trimString)) return target;

        string result = target;
        while (result.StartsWith(trimString))
        {
            result = result.Substring(trimString.Length);
        }

        return result;
    }

    public static string TrimEnd(this string target, string trimString)
    {
        if (string.IsNullOrEmpty(trimString)) return target;

        string result = target;
        while (result.EndsWith(trimString))
        {
            result = result.Substring(0, result.Length - trimString.Length);
        }

        return result;
    }

    public static string ToFriendlyStringOrPattern(object value)
    {
        if (value is null)
            return string.Empty;

        var type = value.GetType();

        if (type.IsEnum)
        {
            var member = type.GetMember(value.ToString() ?? string.Empty);
            if (member.Length > 0)
            {
                var attr = member[0].GetCustomAttribute<RegexPatternAttribute>();
                if (attr != null && attr.Patterns.Length > 0)
                    return attr.Patterns.First();
            }
        }

        return value.ToString()?.ToFriendlyCase(TitleDisplayOption.Lower) ?? string.Empty;
    }

    /// <summary>
    /// Retrieves a list of all named groups from a regular expression match.
    /// </summary>
    /// <param name="match">The match object to inspect.</param>
    /// <param name="includeMatch">If true, the output format is "{Name}: '{Value}'". If false, only the Name is returned.</param>
    /// <param name="excludeUnsuccessfulMatches">If true, groups that did not capture a value (e.g., optional groups) are excluded.</param>
    /// <param name="orderByIndex">If true, sorts successful matches by their index in the input string. If false, all returned groups are sorted alphabetically by name. Unsuccessful matches are always placed last.</param>
    /// <returns>An ordered List of strings representing the desired group information.</returns>
    public static List<string> GetGroupNames(
        this Match match,
        bool includeMatch = false,
        bool excludeUnsuccessfulMatches = true,
        bool orderByIndex = true)
    {
        if (match == null) throw new ArgumentNullException(nameof(match));

        var regexField = typeof(Match).GetField("_regex", BindingFlags.NonPublic | BindingFlags.Instance);
        if (regexField?.GetValue(match) is not Regex regex)
        {
            return new List<string>();
        }

        // Project to an intermediate anonymous type. The compiler knows the types of Name and Group.
        var groupsQuery = regex.GetGroupNames()
            .Select(name => new { Name = name, Group = match.Groups[name] })
            .Where(g => !string.IsNullOrEmpty(g.Name) && !char.IsDigit(g.Name[0]));

        if (excludeUnsuccessfulMatches)
        {
            groupsQuery = groupsQuery.Where(g => g.Group.Success);
        }

        // Apply the sorting logic. `var` allows the compiler to infer the IOrderedEnumerable<T> type.
        var sortedGroups = orderByIndex
            ? groupsQuery
                .OrderByDescending(g => g.Group.Success)
                .ThenBy(g => g.Group.Index)
                .ThenBy(g => g.Name)
            : groupsQuery.OrderBy(g => g.Name);

        // Now, g.Name is correctly inferred as a string at compile-time.
        if (includeMatch)
        {
            return sortedGroups
                .Select(g => $"{g.Name}: '{g.Group.Value}'")
                .ToList();
        }

        // This now correctly returns List<string> because g.Name is a string.
        return sortedGroups.Select(g => g.Name).ToList();
    }

    public static string Debug(this object obj) => DebugSerializer.Serialize(obj);
}

