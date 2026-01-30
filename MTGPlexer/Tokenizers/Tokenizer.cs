namespace MTGPlexer.Tokenizers;

using System.Text.RegularExpressions;

public class Tokenizer
{
    List<Type> _orderedTypes;
    private static readonly Dictionary<int, Regex> _unmatchedRegexCache = [];

    // Characters that are allowed to immediately follow a valid token match
    private static readonly char[] _boundaryChars = [' ', '.'];

    public Tokenizer(List<Type> orderedTypes)
    {
        _orderedTypes = orderedTypes;
    }

    public List<TokenUnit> Tokenize(string sourceText, int? scopeStart = null, int? scopeEnd = null, Type scopeToType = null)
    {
        if (string.IsNullOrEmpty(sourceText))
            throw new Exception("Source text may not be null or empty");

        var tokens = new List<TokenUnit>();
        int currentIndex = scopeStart ?? 0;
        int endIndex = scopeEnd ?? sourceText.Length;
        int unmatchedStartIndex = -1;

        while (currentIndex < endIndex)
        {
            bool matched = false;
            var filteredTypes = _orderedTypes;

            if (scopeToType != null && scopeToType != typeof(TokenUnit))
                filteredTypes = _orderedTypes
                    .Where(x => x.IsAssignableTo(scopeToType)).ToList();

            foreach (var type in filteredTypes)
            {
                var rootNode = TokenTypeRegistry.RootNodes[type];
                var match = rootNode.BuiltRegex.Regex.Match(sourceText, currentIndex);    

                // Validation:
                // 1. Regex must succeed.
                // 2. We manually enforce anchoring: the match MUST start at currentIndex.
                // 3. The match must not exceed our current scope (endIndex).
                if (match.Success && match.Index == currentIndex && match.Length > 0 && (match.Index + match.Length <= endIndex))
                {
                    int matchEndIndex = match.Index + match.Length;

                    // **Boundary Check**: 
                    // To avoid mid-word partial matches, the match is only valid if it extends 
                    // exactly to the end of the line, or is followed by a space or period.
                    bool endsAtBoundary = matchEndIndex == endIndex || (matchEndIndex < endIndex && _boundaryChars.Contains(sourceText[matchEndIndex]));

                    if (!endsAtBoundary)
                        goto NextIteration;

                    // --- COMMIT PHASE ---
                    FlushUnmatched(sourceText, tokens, ref unmatchedStartIndex, match.Index);

                    var token = rootNode.Hydrate(new CaptureDictionary(match));

                    tokens.Add(token);
                    currentIndex = match.Index + match.Length;
                    unmatchedStartIndex = -1;
                    matched = true;
                    break;
                }

            NextIteration:;
            }

            // If no token matched at this boundary, "ratchet" up to the next space and begin again.
            if (!matched)
            {
                if (unmatchedStartIndex == -1)
                    unmatchedStartIndex = currentIndex;

                int nextSpaceIndex = sourceText.IndexOf(' ', currentIndex);

                if (nextSpaceIndex == -1 || nextSpaceIndex >= endIndex)
                    currentIndex = endIndex;
                else
                    currentIndex = nextSpaceIndex + 1;
            }
        }

        FlushUnmatched(sourceText, tokens, ref unmatchedStartIndex, endIndex);

        return tokens;
    }

    private void FlushUnmatched(string sourceText, List<TokenUnit> tokens, ref int unmatchedStartIndex, int flushUntilIndex)
    {
        if (unmatchedStartIndex == -1 || unmatchedStartIndex >= flushUntilIndex)
        {
            unmatchedStartIndex = -1;
            return;
        }

        int length = flushUntilIndex - unmatchedStartIndex;
        if (length > 0)
        {
            if (!_unmatchedRegexCache.TryGetValue(length, out var regex))
            {
                regex = new Regex($".{{{length}}}", RegexOptions.Singleline | RegexOptions.Compiled);
                _unmatchedRegexCache[length] = regex;
            }

            Match unmatchedMatch = regex.Match(sourceText, unmatchedStartIndex);
            if (unmatchedMatch.Success)
            {
                var unmatchedStringRootNode = TokenTypeRegistry.RootNodes[typeof(DefaultUnmatchedString)];
                var unmatchedTokenUnit = unmatchedStringRootNode.Hydrate(new CaptureDictionary(unmatchedMatch));
                tokens.Add(unmatchedTokenUnit);
            }
        }

        unmatchedStartIndex = -1;
    }
}