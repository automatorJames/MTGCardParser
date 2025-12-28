namespace MTGPlexer.Tokenizers;

using System.Text.RegularExpressions;

public class Tokenizer
{
    const string _tokenTypeGroupPrefixName = "TYPE_";
    List<Type> _orderedTypes;
    Dictionary<Type, Regex> _megaRegexes = [];

    // A dictionary where each pattern simply matches int (Key) number of "." (any) chars (built as different lengths encountered)
    private static readonly Dictionary<int, Regex> _unmatchedRegexCache = [];

    public Tokenizer(List<Type> orderedTypes)
    {
        _orderedTypes = orderedTypes;
        SetMegaRegexForType(typeof(TokenUnit));
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

            // The TokenUnit pattern is the default regex to apply
            Regex applicableMegaRegex = _megaRegexes[typeof(TokenUnit)];

            if (scopeToType != null)
                if (!_megaRegexes.TryGetValue(scopeToType, out applicableMegaRegex))
                    applicableMegaRegex = SetMegaRegexForType(scopeToType);

            var megaMatch = applicableMegaRegex.Match(sourceText.FormattedText, currentIndex);

            if (megaMatch.Success && megaMatch.Index == currentIndex && (megaMatch.Index + megaMatch.Length <= endIndex))
            {
                // Find which group matched (constrained to top-level ordered types)
                var matchedTypeName = megaMatch.GetGroupNames()
                    .FirstOrDefault(x => x.StartsWith(_tokenTypeGroupPrefixName) && megaMatch.Groups[x].Success)
                    .Replace(_tokenTypeGroupPrefixName, "");

                var matchedType = TokenTypeRegistry.NameToType[matchedTypeName];

                Dictionary<DynamicRegexProp, object> dynamicPrefilledValues = TokenTypeRegistry.Templates[matchedType].RegexSegments
                        .OfType<DynamicRegexProp>()
                        .ToDictionary(x => x, x => (object)null);

                //// Get a match for the type-specific pattern
                //// This may not actually be necessary (i.e. we could pass the megaMatch), but I don't that know yet
                //var isolatedTypeMatch = TokenTypeRegistry.TypeRegexes[matchedType].Match(sourceText.FormattedText, currentIndex);

                TokenUnitMatch typeMatch = new(matchedType, megaMatch, sourceText, new CaptureGroupPropPath(matchedTypeName));

                if (dynamicPrefilledValues.Any())
                {
                    if (dynamicPrefilledValues.Count > 1)
                        throw new NotImplementedException($"Type '{matchedType.Name}' has {dynamicPrefilledValues.Count} dynamic properties, but the max supported is 1");

                    foreach (var dynamicPrefilledValue in dynamicPrefilledValues.Keys.ToList())
                    {
                        var dynamicGroup = megaMatch.Groups[dynamicPrefilledValue.Name];

                        if (!dynamicGroup.Success)
                            goto FailProvisionalDynamicCheck;

                        var dynamicType = dynamicPrefilledValue.RegexPropInfo.BaseType.GenericTypeArguments[0];

                        // Recursive call to resolve the dynamic portion
                        var tokenSet = Tokenize(sourceText, dynamicGroup, dynamicType);

                        // Find the first "real" token (ignoring unmatched noise inside the dynamic portion)
                        var dynamicToken = tokenSet.FirstOrDefault(x => x is not DefaultUnmatchedString);

                        if (dynamicToken == null)
                        {
                            // Fail: The dynamic portion didn't resolve to a valid sub-token.
                            // We discard the entire provisional match.
                            goto FailProvisionalDynamicCheck;
                        }
                        else
                        {
                            // Success: Store the resolved child token
                            dynamicPrefilledValues[dynamicPrefilledValue] = dynamicToken;
                        }
                    }
                }

                matched = true;

                // --- COMMIT PHASE ---
                // If we reach this point, the match is confirmed valid.

                // 1. Flush "junk" text that preceded this match.
                FlushUnmatched(sourceText, tokens, ref unmatchedStartIndex, megaMatch.Index);

                // 2. Add the parent token.
                var token = TokenUnit.InstantiateFromMatch(typeMatch, dynamicPrefilledValues);
                tokens.Add(token);

                // 3. Advance state.
                currentIndex = megaMatch.Index + megaMatch.Length;
                unmatchedStartIndex = -1; // Reset junk tracker
                matched = true;
                break;
            }

        FailProvisionalDynamicCheck:;

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

    Regex SetMegaRegexForType(Type scopeToType)
    {
        var typeRegexes = TokenTypeRegistry.TypeRegexes
            .Where(x => x.Key.IsAssignableTo(scopeToType))
            .OrderBy(x => _orderedTypes.IndexOf(x.Key));

        var combinedPattern = string.Join("|", typeRegexes.Select(kvp => $"(?<{_tokenTypeGroupPrefixName}{kvp.Key.Name}>{kvp.Value})"));

        var megaRegex = new Regex(combinedPattern,
            RegexOptions.Compiled |
            RegexOptions.ExplicitCapture |
            RegexOptions.Singleline);

        _megaRegexes[scopeToType] = megaRegex;

        return megaRegex; // return in case the caller wants to use the regex immediately
    }
}