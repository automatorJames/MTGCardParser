namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

/// <summary>
/// A summary of all property values captured for a given set of TokenUnits,
/// organized by Type, then by property path, with values ordered by frequency.
/// </summary>
public record TokenUnitCaptureSummary
{
    public List<TokenUnitCapture> TokenUnitCaptures { get; } = [];

    /// <summary>
    /// Private constructor to be used by the static factory method.
    /// Processes a given list of tokens into a summary.
    /// </summary>
    private TokenUnitCaptureSummary(List<TokenUnit> tokenUnits)
    {
        // Count all token occurrences
        var tokenOccurrenceCounts = tokenUnits
            .GroupBy(x => x.Type)
            .OrderByDescending(x => x.Count())
            .ToDictionary(x => x.Key, x => x
            .Count());

        // Add zero-count types, if any, so they're represented even though they didn't match anything
        TokenTypeRegistry.AppliedOrderTypes
            .Where(x => !tokenOccurrenceCounts.ContainsKey(x))
            .ToList()
            .ForEach(x => tokenOccurrenceCounts[x] = 0);

        // Structure: Type -> PropertyPath -> ValueCaptureVariantSet -> Canonical string (as key) -> ValueCaptureVariantCollector
        var summaryCounts = new Dictionary<Type, Dictionary<TerminalRegexPropPath, Dictionary<string, ValueCaptureVariantCollector>>>();

        // 1. AGGREGATE COUNTS
        foreach (var unit in tokenUnits)
        {
            if (!summaryCounts.TryGetValue(unit.Type, out var typeCounts))
            {
                typeCounts = [];
                summaryCounts[unit.Type] = typeCounts;
            }

            foreach (var indexedCapture in unit.IndexedPropertyCaptures)
            {
                FlattenAndCountRecursive(
                    [indexedCapture.RegexPropInfo.Name],
                    indexedCapture.Capture.Value,
                    indexedCapture.Value,
                    indexedCapture.RegexPropInfo,
                    typeCounts
                );
            }
        }

        // 2. TRANSFORM AGGREGATES INTO FINAL DTOS
        foreach (var type in TokenTypeRegistry.AppliedOrderTypes)
        {
            if (!summaryCounts.TryGetValue(type, out var typePropValCounts))
                TokenUnitCaptures.Add(new(type, 0, null));
            else
                TokenUnitCaptures.Add(new(type, tokenOccurrenceCounts[type], typePropValCounts));
        }

        // Order by applied order
        TokenUnitCaptures = TokenUnitCaptures
            .OrderBy(x => TokenTypeRegistry.AppliedOrderTypes.IndexOf(x.Type))
            .ToList();
    }

    /// <summary>
    /// Efficiently creates both a top-level and a global summary by traversing the token hierarchy only once.
    /// </summary>
    /// <param name="topLevelTokenUnits">The initial list of top-level hydrated tokens.</param>
    /// <returns>A tuple containing the top-level and global summaries.</returns>
    public static (TokenUnitCaptureSummary topLevel, TokenUnitCaptureSummary global) CreateSummaries(List<TokenUnit> topLevelTokenUnits)
    {
        // 1. Perform a single traversal to collect all descendant tokens into a flat list.
        var allTokens = new List<TokenUnit>();
        foreach (var topLevelUnit in topLevelTokenUnits)
            CollectAllTokensRecursive(topLevelUnit, allTokens);

        // 2. Create the two distinct summaries using their respective token lists.
        var topLevelSummary = new TokenUnitCaptureSummary(topLevelTokenUnits);
        var globalSummary = new TokenUnitCaptureSummary(allTokens);

        return (topLevelSummary, globalSummary);
    }

    /// <summary>
    /// A private static helper to recursively walk the TokenUnit child hierarchy and populate a flat list.
    /// </summary>
    private static void CollectAllTokensRecursive(TokenUnit currentUnit, List<TokenUnit> collection)
    {
        collection.Add(currentUnit);

        foreach (var childToken in currentUnit.GetChildTokens())
            CollectAllTokensRecursive(childToken, collection);
    }

    private void FlattenAndCountRecursive(List<string> currentPropPath, string originalCaptureString, object currentValue, RegexPropInfo currentPropInfo, Dictionary<TerminalRegexPropPath, Dictionary<string, ValueCaptureVariantCollector>> propValCounts)
    {
        if (currentValue == null) return;

        switch (currentValue)
        {
            case TokenUnitOneOf tokenUnitOneOf:
                var tokenUnitOneOfOnlyChild = tokenUnitOneOf.GetSingleNonNullChildToken();
                currentPropPath.Add(tokenUnitOneOfOnlyChild.ParentTokenProp.Name);
                FlattenAndCountRecursive(currentPropPath, originalCaptureString, tokenUnitOneOfOnlyChild, tokenUnitOneOfOnlyChild.ParentTokenProp, propValCounts);
                break;

            case TokenUnit childTokenUnit:
                foreach (var indexedCapture in childTokenUnit.IndexedPropertyCaptures)
                {
                    var childPropPath = currentPropPath.Concat([indexedCapture.RegexPropInfo.Name]).ToList();
                    FlattenAndCountRecursive(childPropPath, originalCaptureString, indexedCapture.Value, indexedCapture.RegexPropInfo, propValCounts);
                }
                break;

            default:
                // Base case: The value is a primitive or string, so we count it.
                IncrementValueCount(propValCounts, currentPropPath, currentPropInfo, originalCaptureString, currentValue.ToString().ToFriendlyCase());
                break;
        }
    }

    /// <summary>
    /// Instance helper that safely increments the count for a given property path and value.
    /// </summary>
    private void IncrementValueCount(Dictionary<TerminalRegexPropPath, Dictionary<string, ValueCaptureVariantCollector>> propValCounts, List<string> propPath, RegexPropInfo terminalPropInfo, string originalCaptureString, string canonicalValueAsString)
    {
        var terminalRegexPropPath = new TerminalRegexPropPath(terminalPropInfo, propPath);

        if (!propValCounts.TryGetValue(terminalRegexPropPath, out var valueCaptureVariantCollectors))
        {
            valueCaptureVariantCollectors = [];
            propValCounts[terminalRegexPropPath] = valueCaptureVariantCollectors;
        }

        if (!valueCaptureVariantCollectors.TryGetValue(canonicalValueAsString, out var valueCaptureVariantCollector))
            valueCaptureVariantCollectors[canonicalValueAsString] = new(canonicalValueAsString, originalCaptureString);
        else
            valueCaptureVariantCollector.IncrementVariant(originalCaptureString);
    }
}