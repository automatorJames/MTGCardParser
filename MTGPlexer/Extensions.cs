using MTGPlexer.CommonDTOs.StructuredMatches;
using System.ComponentModel;
using System.Text;

namespace MTGPlexer;

public static class Extensions
{
    public static string AddOptionalPluralization(this string word)
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

    public static string Dot(this string parentPath, string nextPathPart) => parentPath + "." + nextPathPart;
    public static string Colon(this string parentPath, string nextPathPart) => parentPath + ":" + nextPathPart;
    public static string ToIndexString(this Match match) => $"idx[{match.Index}]";
    public static string ToIndexString(this StructuredMatchBase match) => $"idx[{match.AbsoluteStartInSource}]";

    public static Type UnderlyingType(this PropertyInfo prop) => prop.PropertyType.UnderlyingType();
    public static Type UnderlyingType(this Type type) => Nullable.GetUnderlyingType(type) ?? type;

    public static void CombineDictionaryCounts<T>(this Dictionary<T, int> dictToAddTo, Dictionary<T, int> dictToAddFrom)
    {
        foreach (var key in dictToAddFrom.Keys)
        {
            if (!dictToAddTo.ContainsKey(key))
                dictToAddTo[key] = 0;

            dictToAddTo[key] += dictToAddFrom[key];
        }
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
}

