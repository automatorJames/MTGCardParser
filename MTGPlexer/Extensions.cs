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

    public static string GetDescription(this Enum value)
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

    public static PropertyInfo[] GetProps(this Type type) => 
        type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

    public static string Dot(this string parentPath, params string[] nextPathParts) =>
        nextPathParts == null || nextPathParts.Length == 0 ? parentPath
        : parentPath + "." + string.Join('.', nextPathParts);


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

    public static string[] GetPublicPropNames(this Type type) =>
        type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Select(x => x.Name)
            .ToArray();

    public static object[] GetSetFlags(this Enum value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        var type = value.GetType();

        if (!Attribute.IsDefined(type, typeof(FlagsAttribute)))
            throw new ArgumentException($"Enum type '{type.Name}' is not marked with [Flags].", nameof(value));

        return Enum.GetValues(type)
            .Cast<Enum>()
            .Where(flag =>
            Convert.ToUInt64(flag) != 0 &&
            value.HasFlag(flag))
            .Cast<object>()
            .ToArray();
    }

    public static string PrintDebug(this object obj) => DebugSerializer.Serialize(obj);
}

