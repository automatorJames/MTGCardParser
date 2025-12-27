namespace MTGPlexer.Tokenizers;

using System.Text.RegularExpressions;

public class Tokenizer
{
    private readonly Dictionary<Type, Regex> _orderedTypeRegexes = [];
    private readonly Dictionary<Type, Regex> _orderedAnchoredTypeRegexes = [];

    // A dictionary where each pattern simply matches int (Key) number of "." (any) chars (built as different lengths encountered)
    private static readonly Dictionary<int, Regex> _unmatchedRegexCache = [];

    public Tokenizer(List<Type> orderedTypes)
    {
        foreach (var type in orderedTypes)
        {
            var regex = TokenTypeRegistry.Templates[type].Regex;
            _orderedTypeRegexes[type] = regex;
            _orderedAnchoredTypeRegexes[type] = new Regex($"\\G({regex})", RegexOptions.Singleline | RegexOptions.Compiled);
        }
    }

    public List<TokenUnit> Tokenize(SourceTextDTO sourceText, Group scopeToGroup = null, Type scopeToType = null)
    {
        if (string.IsNullOrEmpty(sourceText.FormattedText))
            throw new Exception("Source text may not be null or empty");

        var tokens = new List<TokenUnit>();
        int currentIndex = 0;
        int endIndex = sourceText.FormattedText.Length;
        int unmatchedStartIndex = -1;

        if (scopeToGroup != null)
        {
            currentIndex = scopeToGroup.Index;
            endIndex = scopeToGroup.Index + scopeToGroup.Length;
        }

        while (currentIndex < endIndex)
        {
            bool matched = false;

            var filteredTypeRegexes = _orderedAnchoredTypeRegexes;
            if (scopeToType != null && scopeToType != typeof(TokenUnit))
            {
                filteredTypeRegexes = _orderedAnchoredTypeRegexes
                    .Where(x => x.Key.IsAssignableTo(scopeToType))
                    .ToDictionary(x => x.Key, x => x.Value);
            }

            // **Step 1: Prioritize matching a known token.**
            foreach (var (type, regex) in filteredTypeRegexes)
            {
                var match = regex.Match(sourceText.FormattedText, currentIndex);

                // Provisional Match check
                if (match.Success && match.Length > 0 && (match.Index + match.Length <= endIndex))
                {
                    Dictionary<DynamicRegexProp, object> dynamicPrefilledValues = TokenTypeRegistry.Templates[type].RegexSegments
                            .OfType<DynamicRegexProp>()
                            .ToDictionary(x => x, x => (object)null);

                    if (dynamicPrefilledValues.Any())
                    {
                        if (dynamicPrefilledValues.Count > 1)
                            throw new NotImplementedException($"Type '{type.Name}' has {dynamicPrefilledValues.Count} dynamic properties, but the max supported is 1");

                        foreach (var dynamicPrefilledValue in dynamicPrefilledValues.Keys.ToList())
                        {
                            var dynamicGroup = match.Groups[dynamicPrefilledValue.Name];

                            if (!dynamicGroup.Success)
                                goto NextIteration;

                            var dynamicType = dynamicPrefilledValue.RegexPropInfo.BaseType.GenericTypeArguments[0];

                            // Recursive call to resolve the dynamic portion
                            var tokenSet = Tokenize(sourceText, dynamicGroup, dynamicType);

                            // Find the first "real" token (ignoring unmatched noise inside the dynamic portion)
                            var dynamicToken = tokenSet.FirstOrDefault(x => x is not DefaultUnmatchedString);

                            if (dynamicToken == null)
                            {
                                // Fail: The dynamic portion didn't resolve to a valid sub-token.
                                // We discard the entire provisional match.
                                goto NextIteration;
                            }
                            else
                            {
                                // Success: Store the resolved child token
                                dynamicPrefilledValues[dynamicPrefilledValue] = dynamicToken;
                            }
                        }
                    }

                    // --- COMMIT PHASE ---
                    // If we reach this point, the match is confirmed valid.

                    // 1. Flush "junk" text that preceded this match.
                    FlushUnmatched(sourceText, tokens, ref unmatchedStartIndex, match.Index);

                    // 2. Add the parent token.
                    TokenUnitMatch typeMatch = new(type, match, sourceText, new CaptureGroupPropPath(type.Name));
                    var token = TokenUnit.InstantiateFromMatch(typeMatch, dynamicPrefilledValues);
                    tokens.Add(token);

                    // 3. Advance state.
                    currentIndex = match.Index + match.Length;
                    unmatchedStartIndex = -1; // Reset junk tracker
                    matched = true;
                    break;
                }

            NextIteration:;
            }

            // **Step 2: Ratchet Logic **
            // If no token matched at this boundary, jump to the next possible boundary (after the next space).
            if (!matched)
            {
                if (unmatchedStartIndex == -1)
                {
                    unmatchedStartIndex = currentIndex;
                }

                // Find the next space within the bounds of the current text/scope
                int nextSpaceIndex = sourceText.FormattedText.IndexOf(' ', currentIndex);

                if (nextSpaceIndex == -1 || nextSpaceIndex >= endIndex)
                {
                    // No more spaces within scope; jump to the end
                    currentIndex = endIndex;
                }
                else
                {
                    // Move to the character immediately following the space
                    currentIndex = nextSpaceIndex + 1;
                }
            }
        }

        // **Step 3: Final flush for any remaining text at the end of the scope.**
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
                TokenUnitMatch typeMatch = new(typeof(DefaultUnmatchedString), unmatchedMatch, sourceText);
                var unmatchedTokenUnit = TokenUnit.InstantiateFromMatch(typeMatch);
                tokens.Add(unmatchedTokenUnit);
            }
        }

        unmatchedStartIndex = -1;
    }
}