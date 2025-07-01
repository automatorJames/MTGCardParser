namespace MTGCardParser.RegexSegmentDTOs.Interfaces;

public interface IRegexSegment
{
    public Regex Regex { get; }
    public string RegexString { get; }

    static string AddOptionalPluralization(string word)
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
}