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


    /// <summary>
    /// Converts a PascalCase or camelCase string into a human-readable format with a specified casing.
    /// </summary>
    /// <param name="input">The string to convert, e.g., "MyAwesomeProperty".</param>
    /// <param name="option">The desired output casing (Lower, Sentence, or Title). Defaults to Lower.</param>
    /// <returns>A formatted string.</returns>
    public static string ToFriendlyCase(this string input, TitleDisplayOption option = TitleDisplayOption.Lower)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        // 1. Split on capital letters to insert spaces and convert everything to lowercase.
        // This creates a consistent base format, e.g., "my awesome property".
        var lowerCaseResult = Regex.Replace(input, "(?<!^)([A-Z])", " $1").ToLower();

        // 2. Apply the selected casing option.
        switch (option)
        {
            case TitleDisplayOption.Sentence:
                // Capitalize the very first letter and return.
                return char.ToUpper(lowerCaseResult[0]) + lowerCaseResult.Substring(1);

            case TitleDisplayOption.Title:
                var words = lowerCaseResult.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var resultBuilder = new StringBuilder();

                for (int i = 0; i < words.Length; i++)
                {
                    var word = words[i];

                    // Always capitalize the first word, or any word not in the "minor words" list.
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
                // Already in the correct format.
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

    public static PropertyInfo[] GetPublicDeclaredProps(this Type type) => 
        type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

    public static string Dot(this string parentPath, string nextPathPart) => parentPath + "." + nextPathPart;

    public static Type UnderlyingType(this PropertyInfo prop) => prop.PropertyType.UnderlyingType();
    public static Type UnderlyingType(this Type type) => Nullable.GetUnderlyingType(type) ?? type;
}

