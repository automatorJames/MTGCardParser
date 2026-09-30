namespace Glyphotype.Distiller.Scoring;

/// <summary>
/// Whether an authored regex is <em>open-ended</em>: able to match text the grammar never spelled out. A closed
/// regex only chooses among texts written in it - <c>an?</c> is "a" or "an", <c>(wash|dry)</c> one of two words -
/// so the grammar paid for every text it can match by writing the regex down. An open-ended one - a character class
/// (<c>[^.]</c>, <c>\w</c>, <c>.</c>), or unbounded repetition (<c>*</c>, <c>+</c>, <c>{2,}</c>) - matches text the
/// grammar never wrote, and the corpus encoding has to pay to spell it (see <see cref="CorpusEncoding"/>).
/// </summary>
public static class RegexOpenness
{
    static readonly System.Collections.Concurrent.ConcurrentDictionary<string, bool> _cache = new();

    public static bool IsOpenEnded(string pattern) =>
        pattern is not null && _cache.GetOrAdd(pattern, Classify);

    static bool Classify(string pattern)
    {
        for (int i = 0; i < pattern.Length; i++)
        {
            switch (pattern[i])
            {
                case '\\' when i + 1 < pattern.Length:
                    var escaped = pattern[++i];

                    // A shorthand class (\d, \w, \s, \p{..} and their negations) matches a set of characters; anything
                    // else escaped is a literal character, an anchor (\b, \A, \z) or a backreference.
                    if ("dDwWsSpP".Contains(escaped))
                        return true;
                    break;

                case '[':
                    // A class of one literal character - "[ ]" is how spaces are written - is that character.
                    var close = FindClassEnd(pattern, i);

                    if (close < 0 || !IsSingleCharacterClass(pattern[(i + 1)..close]))
                        return true;

                    i = close;
                    break;

                case '.' or '*' or '+':
                    return true;

                case '{':
                    // A quantifier with no upper bound; a bounded one ({2}, {1,3}) repeats a closed thing finitely.
                    var end = pattern.IndexOf('}', i);

                    if (end > 0 && System.Text.RegularExpressions.Regex.IsMatch(pattern[(i + 1)..end], @"^\d+,$"))
                        return true;
                    break;
            }
        }

        return false;
    }

    static int FindClassEnd(string pattern, int open)
    {
        for (int i = open + 1; i < pattern.Length; i++)
        {
            if (pattern[i] == '\\')
                i++;
            else if (pattern[i] == ']' && i > open + 1)
                return i;
        }

        return -1;
    }

    static bool IsSingleCharacterClass(string content) =>
        content.Length == 1 && content != "^"
        || content.Length == 2 && content[0] == '\\' && !"dDwWsSpP".Contains(content[1]);
}
