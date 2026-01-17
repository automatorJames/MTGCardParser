namespace MTGPlexer.Tokenizers;

using System.Text.RegularExpressions;

public class Tokenizer
{
    private readonly Dictionary<Type, Regex> _orderedTypeRegexes = [];
    private static readonly Dictionary<int, Regex> _unmatchedRegexCache = [];

    // Characters that are allowed to immediately follow a valid token match
    private static readonly char[] _boundaryChars = [' ', '.'];

    public Tokenizer(List<Type> orderedTypes)
    {
        foreach (var type in orderedTypes)
        {
            var regex = TokenTypeRegistry.Templates[type].Regex;
            _orderedTypeRegexes[type] = regex;
        }
    }

    public List<TokenUnit> Tokenize(SourceTextDTO sourceText, int? scopeStart = null, int? scopeEnd = null, Type scopeToType = null)
    {
        if (string.IsNullOrEmpty(sourceText.FormattedText))
            throw new Exception("Source text may not be null or empty");

        var tokens = new List<TokenUnit>();
        int currentIndex = scopeStart ?? 0;
        int endIndex = scopeEnd ?? sourceText.FormattedText.Length;
        int unmatchedStartIndex = -1;

        while (currentIndex < endIndex)
        {
            bool matched = false;

            var filteredTypeRegexes = _orderedTypeRegexes;
            if (scopeToType != null && scopeToType != typeof(TokenUnit))
            {
                filteredTypeRegexes = _orderedTypeRegexes
                    .Where(x => x.Key.IsAssignableTo(scopeToType))
                    .ToDictionary(x => x.Key, x => x.Value);
            }

            // **Step 1: Prioritize matching a known token.**
            foreach (var (type, regex) in filteredTypeRegexes)
            {
                var match = regex.Match(sourceText.FormattedText, currentIndex);    

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
                    bool endsAtBoundary = matchEndIndex == endIndex || (matchEndIndex < endIndex && _boundaryChars.Contains(sourceText.FormattedText[matchEndIndex]));

                    if (!endsAtBoundary)
                        goto NextIteration;

                    // --- COMMIT PHASE ---
                    FlushUnmatched(sourceText, tokens, ref unmatchedStartIndex, match.Index);

                    MatchTraversalState typeMatch = new(type, match, sourceText);
                    var token = TokenUnit.InstantiateFromMatch(typeMatch, out var result);

                    if (result == ValueResult.Success)
                    {
                        tokens.Add(token);
                        currentIndex = match.Index + match.Length;
                        unmatchedStartIndex = -1;
                        matched = true;
                        break;
                    }
                }

            NextIteration:;
            }

            // If no token matched at this boundary, "ratchet" up to the next space and begin again.
            if (!matched)
            {
                if (unmatchedStartIndex == -1)
                    unmatchedStartIndex = currentIndex;

                int nextSpaceIndex = sourceText.FormattedText.IndexOf(' ', currentIndex);

                if (nextSpaceIndex == -1 || nextSpaceIndex >= endIndex)
                    currentIndex = endIndex;
                else
                    currentIndex = nextSpaceIndex + 1;
            }
        }

        FlushUnmatched(sourceText, tokens, ref unmatchedStartIndex, endIndex);

        return tokens;
    }

    private void FlushUnmatched(SourceTextDTO sourceText, List<TokenUnit> tokens, ref int unmatchedStartIndex, int flushUntilIndex)
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

            Match unmatchedMatch = regex.Match(sourceText.FormattedText, unmatchedStartIndex);
            if (unmatchedMatch.Success)
            {
                MatchTraversalState typeMatch = new(typeof(DefaultUnmatchedString), unmatchedMatch, sourceText);
                var unmatchedTokenUnit = TokenUnit.InstantiateFromMatch(typeMatch, out var result);
                tokens.Add(unmatchedTokenUnit);
            }
        }

        unmatchedStartIndex = -1;
    }
}