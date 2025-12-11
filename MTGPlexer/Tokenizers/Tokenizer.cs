namespace MTGPlexer.Tokenizers;

public class Tokenizer
{
    private readonly Dictionary<Type, Regex> _orderedAnchoredTypeRegexes;

    // A dictionary where each pattern simply matches int (Key) number of "." (any) chars (built as different lengths encountered)
    private static readonly Dictionary<int, Regex> _unmatchedRegexCache = [];

    public Tokenizer(List<Type> orderedTypes)
    {
        _orderedAnchoredTypeRegexes = orderedTypes.ToDictionary(x => x, x => new Regex($"\\G({TokenTypeRegistry.Templates[x].Regex})"));
    }

    public List<TokenUnit> Tokenize(SourceTextDTO sourceText)
    {
        var formattedText = sourceText.FormattedText; 
        var tokens = new List<TokenUnit>();
        int currentIndex = 0;
        int unmatchedStartIndex = -1;

        while (currentIndex < formattedText.Length)
        {
            bool matched = false;

            // **Step 1: Prioritize matching a known token.**
            foreach (var (type, regex) in _orderedAnchoredTypeRegexes)
            {
                var match = regex.Match(formattedText, currentIndex);
                if (match.Success && match.Length > 0)
                {
                    // A token was found. Flush any preceding unmatched text.
                    FlushUnmatched(sourceText, tokens, ref unmatchedStartIndex, currentIndex);

                    TokenUnitMatch typeMatch = new(type, match, sourceText, new CaptureGroupPropPath(type.Name));
                    var token = TokenUnit.InstantiateFromMatch(typeMatch);
                    tokens.Add(token);
                    currentIndex += match.Length;
                    matched = true;
                    break; // Exit foreach and continue the main while loop
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

    /// <summary>
    /// Intended for use by DynamicRegexProp instances to check for a sub-capture for a given match among all possible TokenUnit types.
    /// </summary>
    /// <returns></returns>
    public TokenUnit TokenizeDynamicSubContent(TokenUnit parentToken, Capture captureToTokenize, Match parentMatch, CaptureGroupPropPath ancestorCapturePath, Type constrainToType = null)
    {
        // Filter the regexes to only include types that are assignable to the constraint type, or all types if no constraint is provided.
        Dictionary<Type, Regex> filteredOrderedTypeRegexes =
            constrainToType == null ? _orderedAnchoredTypeRegexes
            : _orderedAnchoredTypeRegexes.Where(x => x.Key.IsAssignableTo(constrainToType)).ToDictionary(x => x.Key, x => x.Value);

        // Iterate through the filtered regexes to find a match.
        foreach (var (type, regex) in filteredOrderedTypeRegexes)
        {
            var captureMatch = regex.Match(captureToTokenize.Value);

            // A successful match must consume the entire sourceText.
            // The \G anchor in the regex ensures the match starts at the beginning (index 0).
            // This check ensures it ends at the end of the string.
            if (captureMatch.Success && captureMatch.Length == captureToTokenize.Length)
            {
                // If a full match is found, hydrate the token and return it immediately.
                TokenUnitMatch typeMatch = new(type, captureMatch, parentToken.Match.SourceText, ancestorCapturePath);

                return TokenUnit.InstantiateFromMatch(typeMatch);
            }
        }

        // If no regex resulted in a match that consumed the entire string, return null.
        return null;
    }

    private void FlushUnmatched(SourceTextDTO sourceText, List<TokenUnit> tokens, ref int unmatchedStartIndex, int currentIndex)
    {
        if (unmatchedStartIndex == -1) return;

        int length = currentIndex - unmatchedStartIndex;
        if (length > 0)
        {
            if (!_unmatchedRegexCache.TryGetValue(length, out var regex))
            {
                regex = new Regex($".{{{length}}}", RegexOptions.Singleline | RegexOptions.Compiled);
                _unmatchedRegexCache[length] = regex;
            }

            Match unmatchedMatch = regex.Match(sourceText.FormattedText, unmatchedStartIndex);
            if (unmatchedMatch.Success)
            {
                TokenUnitMatch typeMatch = new(typeof(DefaultUnmatchedString), unmatchedMatch, sourceText);
                var unmatchedTokenUnit = TokenUnit.InstantiateFromMatch(typeMatch);
                tokens.Add(unmatchedTokenUnit);
            }
        }

        unmatchedStartIndex = -1; // Reset for the next sequence.
    }
}