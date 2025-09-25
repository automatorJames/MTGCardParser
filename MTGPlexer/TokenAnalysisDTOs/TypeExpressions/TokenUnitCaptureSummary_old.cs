/*namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

/// <summary>
/// A summary of all property values captured for a given set of TokenUnits,
/// organized by Type, then by property path, with values ordered by frequency.
/// </summary>
public class TokenUnitCaptureSummary
{
    Dictionary<RegexCommentedAlternateLine, Dictionary<string, int>> _lineMatchVariantCounts = [];

    public List<TokenUnitCapture> TokenUnitCaptures { get; } = [];

    /// <summary>
    /// Private constructor to be used by the static factory method.
    /// Processes a given list of tokens into a summary.
    /// </summary>
    public TokenUnitCaptureSummary(List<TokenUnit> tokenUnits)
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
        foreach (var tokenUnit in tokenUnits)
        {
            if (!summaryCounts.TryGetValue(tokenUnit.Type, out var typeCounts))
            {
                typeCounts = [];
                summaryCounts[tokenUnit.Type] = typeCounts;
            }

            foreach (var indexedCapture in tokenUnit.IndexedPropertyCaptures)
            {
                FlattenAndCountRecursive(
                    [tokenUnit.Type.Name, indexedCapture.RegexPropInfo.Name],
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

    void FlattenAndCountRecursive(List<string> currentPropPath, string originalCaptureString, object currentValue, RegexPropInfo currentPropInfo, Dictionary<TerminalRegexPropPath, Dictionary<string, ValueCaptureVariantCollector>> propValCounts)
    {
        if (currentValue == null) 
            return;

        switch (currentValue)
        {
            case TokenUnitOneOf tokenUnitOneOf:
                var singleIndexedCapture = tokenUnitOneOf.GetIndexedPropertyCaptureSingle();
                currentPropPath.Add(singleIndexedCapture.RegexPropInfo.Name);
                FlattenAndCountRecursive(currentPropPath, originalCaptureString, singleIndexedCapture.Value, singleIndexedCapture.RegexPropInfo, propValCounts);
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

    ///// <summary>
    ///// Instance helper that safely increments the count for a given property path and value.
    ///// </summary>
    //static void IncrementValueCount(Dictionary<TerminalRegexPropPath, Dictionary<string, ValueCaptureVariantCollector>> propValCounts, List<string> propPath, RegexPropInfo terminalPropInfo, string originalCaptureString, string canonicalValueAsString)
    //{
    //    var terminalRegexPropPath = new TerminalRegexPropPath(terminalPropInfo, propPath);
    //
    //    if (!propValCounts.TryGetValue(terminalRegexPropPath, out var valueCaptureVariantCollectors))
    //    {
    //        valueCaptureVariantCollectors = [];
    //        propValCounts[terminalRegexPropPath] = valueCaptureVariantCollectors;
    //    }
    //
    //    if (!valueCaptureVariantCollectors.TryGetValue(canonicalValueAsString, out var valueCaptureVariantCollector))
    //        valueCaptureVariantCollectors[canonicalValueAsString] = new(canonicalValueAsString, originalCaptureString);
    //    else
    //        valueCaptureVariantCollector.IncrementVariant(originalCaptureString);
    //}

    void IncrementValueCount(List<string> propPath, string capture)
    {
        var rootType = TokenTypeRegistry.NameToType[propPath.First()];
        var propPathJoined = string.Join('.', propPath);
        var matchingAltLine = TokenTypeRegistry.Templates[rootType].FormattedRegex[propPathJoined, capture];

        if (matchingAltLine == null)
            return;

        if (!_lineMatchVariantCounts.TryGetValue(matchingAltLine, out var variantCounts)


        //var terminalRegexPropPath = new TerminalRegexPropPath(terminalPropInfo, propPath);
        //
        //if (!propValCounts.TryGetValue(terminalRegexPropPath, out var valueCaptureVariantCollectors))
        //{
        //    valueCaptureVariantCollectors = [];
        //    propValCounts[terminalRegexPropPath] = valueCaptureVariantCollectors;
        //}
        //
        //if (!valueCaptureVariantCollectors.TryGetValue(canonicalValueAsString, out var valueCaptureVariantCollector))
        //    valueCaptureVariantCollectors[canonicalValueAsString] = new(canonicalValueAsString, originalCaptureString);
        //else
        //    valueCaptureVariantCollector.IncrementVariant(originalCaptureString);
    }

}*/