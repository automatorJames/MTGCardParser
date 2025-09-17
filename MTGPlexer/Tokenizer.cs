public class Tokenizer
{
    private readonly Dictionary<Type, Regex> _orderedAnchoredTypeRegexes;
    private readonly Regex _whitespaceRegex = new(@"\G\s+", RegexOptions.Compiled);

    // A dictionary where each pattern simply matches int (Key) number of "." (any) chars (built as different lengths encountered)
    private static readonly Dictionary<int, Regex> _unmatchedRegexCache = [];

    public Tokenizer(List<Type> orderedTypes)
    {
        _orderedAnchoredTypeRegexes = orderedTypes.ToDictionary(x => x, x => new Regex($"\\G({TokenTypeRegistry.Templates[x].Regex})"));
    }

    public List<TokenUnit> Tokenize(string sourceText, Type constrainToType = null)
    {
        Dictionary<Type, Regex> filteredOrderedTypeRegexes =
            constrainToType == null ? _orderedAnchoredTypeRegexes
            : _orderedAnchoredTypeRegexes.Where(x => x.Key.IsAssignableTo(constrainToType)).ToDictionary(x => x.Key, x => x.Value);

        var tokens = new List<TokenUnit>();
        int currentIndex = 0;
        int unmatchedStartIndex = -1;

        while (currentIndex < sourceText.Length)
        {
            bool matched = false;

            // **Step 1: Prioritize matching a known token.**
            foreach (var (type, regex) in filteredOrderedTypeRegexes)
            {
                var match = regex.Match(sourceText, currentIndex);
                if (match.Success && match.Length > 0)
                {
                    // A token was found. Flush any preceding unmatched text.
                    FlushUnmatched(sourceText, tokens, ref unmatchedStartIndex, currentIndex);

                    // Now, we must skip any ignorable whitespace that follows the flushed text
                    // and precedes the token we just found.
                    var spaceMatch = _whitespaceRegex.Match(sourceText, currentIndex);
                    if (spaceMatch.Success)
                    {
                        currentIndex += spaceMatch.Length;
                        // Re-run the match at the new position
                        match = regex.Match(sourceText, currentIndex);
                    }

                    // Check if the match is still valid after skipping whitespace
                    if (match.Success && match.Length > 0)
                    {
                        var token = TokenUnit.HydrateFromMatch(type, match);
                        tokens.Add(token);
                        currentIndex += match.Length;
                        matched = true;
                        break; // Exit foreach and continue the main while loop
                    }
                }
            }

            // **Step 2: If no token matched, consume one character as part of an unmatched string.**
            if (!matched)
            {
                if (unmatchedStartIndex == -1)
                {
                    // Start a new unmatched sequence.
                    unmatchedStartIndex = currentIndex;
                }
                // Advance the index by one to continue the sequence.
                currentIndex++;
            }
        }

        // **Step 3: After the loop, flush any remaining unmatched text.**
        FlushUnmatched(sourceText, tokens, ref unmatchedStartIndex, currentIndex);

        return tokens;
    }

    public TokenUnit TokenizeSingleNonDefault(Capture captureToTokenize, Match parentMatch, Type constrainToType = null)
    {
        // Filter the regexes to only include types that are assignable to the constraint type, or all types if no constraint is provided.
        Dictionary<Type, Regex> filteredOrderedTypeRegexes =
            constrainToType == null ? _orderedAnchoredTypeRegexes
            : _orderedAnchoredTypeRegexes.Where(x => x.Key.IsAssignableTo(constrainToType)).ToDictionary(x => x.Key, x => x.Value);

        // Iterate through the filtered regexes to find a match.
        foreach (var (type, regex) in filteredOrderedTypeRegexes)
        {
            var captureMatch = regex.Match(captureToTokenize.Value, 0);

            // A successful match must consume the entire sourceText.
            // The \G anchor in the regex ensures the match starts at the beginning (index 0).
            // This check ensures it ends at the end of the string.
            if (captureMatch.Success && captureMatch.Length == captureToTokenize.Length)
            {
                // If a full match is found, hydrate the token and return it immediately.
                return TokenUnit.HydrateFromMatch(type, parentMatch, captureMatch);
            }
        }

        // If no regex resulted in a match that consumed the entire string, return null.
        return null;
    }

    private void FlushUnmatched(string sourceText, List<TokenUnit> tokens, ref int unmatchedStartIndex, int currentIndex)
    {
        if (unmatchedStartIndex == -1) return;

        // Find the actual start by skipping leading whitespace from the unmatched buffer
        var spaceMatch = _whitespaceRegex.Match(sourceText, unmatchedStartIndex);

        if (spaceMatch.Success)
            unmatchedStartIndex += spaceMatch.Length;

        // Find the actual end by trimming trailing whitespace
        string tempSubstring = sourceText.Substring(unmatchedStartIndex, currentIndex - unmatchedStartIndex);
        int finalLength = tempSubstring.TrimEnd().Length;

        if (finalLength > 0)
        {
            int finalEndIndex = unmatchedStartIndex + finalLength;

            if (!_unmatchedRegexCache.TryGetValue(finalLength, out var regex))
            {
                regex = new Regex($".{{{finalLength}}}", RegexOptions.Singleline | RegexOptions.Compiled);
                _unmatchedRegexCache[finalLength] = regex;
            }

            Match unmatchedMatch = regex.Match(sourceText, unmatchedStartIndex);
            if (unmatchedMatch.Success)
            {
                var unmatchedTokenUnit = TokenUnit.HydrateFromMatch(typeof(DefaultUnmatchedString), unmatchedMatch);
                tokens.Add(unmatchedTokenUnit);
            }
        }

        unmatchedStartIndex = -1; // Reset for the next sequence.
    }
}