using System.Diagnostics;

public class Tokenizer
{
    private readonly List<Type> _orderedTokenTypes;
    private readonly Regex _whitespaceRegex = new(@"\G\s+", RegexOptions.Compiled);
    private static readonly Dictionary<Type, Regex> _anchoredRegexCache = [];
    private static readonly Dictionary<int, Regex> _unmatchedRegexCache = [];

    public Tokenizer(List<Type> orderedTokenTypes)
    {
        _orderedTokenTypes = orderedTokenTypes;
        foreach (var type in orderedTokenTypes)
        {
            if (TokenTypeRegistry.Templates.TryGetValue(type, out var template) && template.Regex != null)
            {
                _anchoredRegexCache[type] = new Regex($"\\G({template.Regex})", RegexOptions.Compiled);
            }
        }
    }

    public List<TokenUnit> Tokenize(string sourceText)
    {
        var tokens = new List<TokenUnit>();
        int currentIndex = 0;
        int unmatchedStartIndex = -1;

        while (currentIndex < sourceText.Length)
        {
            bool matched = false;

            // **Step 1: Prioritize matching a known token.**
            foreach (var tokenType in _orderedTokenTypes)
            {
                if (_anchoredRegexCache.TryGetValue(tokenType, out var anchoredRegex))
                {
                    var match = anchoredRegex.Match(sourceText, currentIndex);
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
                            match = anchoredRegex.Match(sourceText, currentIndex);
                        }

                        // Check if the match is still valid after skipping whitespace
                        if (match.Success && match.Length > 0)
                        {
                            var token = TokenUnit.HydrateFromMatch(tokenType, match);
                            tokens.Add(token);
                            currentIndex += match.Length;
                            matched = true;
                            break; // Exit foreach and continue the main while loop
                        }
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