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
                    bool endsAtBoundary = matchEndIndex == endIndex ||
                                         (matchEndIndex < endIndex && _boundaryChars.Contains(sourceText.FormattedText[matchEndIndex]));

                    if (!endsAtBoundary)
                        goto NextIteration;

                    Dictionary<DynamicOfSegment, object> dynamicPrefilledValues = TokenTypeRegistry.Templates[type].RegexSegments
                            .OfType<DynamicOfSegment>()
                            .ToDictionary(x => x, x => (object)null);

                    // Dynamic capture handling
                    if (dynamicPrefilledValues.Any())
                    {
                        // todo: I see no reason why we couldn't support any number of dynamics; just need to handle the spacing and ordering correctly/
                        if (dynamicPrefilledValues.Count > 1)
                            throw new NotImplementedException($"Type '{type.Name}' has {dynamicPrefilledValues.Count} dynamic properties, but the max supported is 1");

                        var dynamicProp = dynamicPrefilledValues.First().Key;

                        var dynamicGroup = match.Groups[dynamicProp.Name];

                        if (!dynamicGroup.Success)
                            goto NextIteration;

                        var dynamicType = dynamicProp.TemplatePropInfo.UnderlyingType.GenericTypeArguments[0];

                        // Recursive call to resolve the dynamic portion
                        var tokenSet = Tokenize(sourceText, dynamicGroup, dynamicType);

                        // Dynamic match tokens must not begin with DefaultUnmatchedString, and must contain at least one non-DefaultUnmatchedString
                        if (tokenSet.First() is DefaultUnmatchedString || tokenSet.FirstOrDefault(x => x is not DefaultUnmatchedString) is not TokenUnit dynamicMatchToken)
                            goto NextIteration;

                        // Although the dynamic match must begin exactly where the parent match left off, it's allowed to be shorter than the remaining space in the parent.
                        // When this occurs, shorten the parent match so that it ends where its dynamic child stops, allowing following tokens to be matched by something else.
                        if (dynamicMatchToken.Match.AbsoluteEnd != match.Index + match.Length)
                            match = regex.Match(sourceText.FormattedText, currentIndex, dynamicMatchToken.Match.AbsoluteEnd - match.Index);

                        dynamicPrefilledValues[dynamicProp] = dynamicMatchToken;
                    }

                    // --- COMMIT PHASE ---
                    FlushUnmatched(sourceText, tokens, ref unmatchedStartIndex, match.Index);

                    TokenUnitMatch typeMatch = new(type, match, sourceText, new CaptureGroupPropPath(type.Name));
                    var token = TokenUnit.InstantiateFromMatch(typeMatch, dynamicPrefilledValues);
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
                TokenUnitMatch typeMatch = new(typeof(DefaultUnmatchedString), unmatchedMatch, sourceText);
                var unmatchedTokenUnit = TokenUnit.InstantiateFromMatch(typeMatch);
                tokens.Add(unmatchedTokenUnit);
            }
        }

        unmatchedStartIndex = -1;
    }
}