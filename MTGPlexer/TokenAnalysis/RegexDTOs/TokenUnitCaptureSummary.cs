using System.Reflection;

namespace MTGPlexer.TokenAnalysis.RegexDTOs;

/// <summary>
/// A summary of all property values captured for a given set of TokenUnits,
/// organized by Type, then by property path, with values ordered by frequency.
/// </summary>
public record TokenUnitCaptureSummary
{
    /// <summary>
    /// A dictionary where the key is the TokenUnit Type and the value is a list
    /// of all property paths and their captured values for that type.
    /// </summary>
    public Dictionary<Type, List<RegexPropValueSet>> TypePropCaptures { get; } = [];

    /// <summary>
    /// Private constructor to be used by the static factory method.
    /// Processes a given list of tokens into a summary.
    /// </summary>
    private TokenUnitCaptureSummary(List<TokenUnit> tokenUnits)
    {
        // Structure: Type -> PropertyPath -> StringValue -> Count
        var summaryCounts = new Dictionary<Type, Dictionary<string, Dictionary<string, int>>>();

        // 1. AGGREGATE COUNTS
        foreach (var unit in tokenUnits)
        {
            if (!summaryCounts.TryGetValue(unit.Type, out var typeCounts))
            {
                typeCounts = new Dictionary<string, Dictionary<string, int>>();
                summaryCounts[unit.Type] = typeCounts;
            }

            foreach (var indexedCapture in unit.IndexedPropertyCaptures)
            {
                FlattenAndCountRecursive(
                    indexedCapture.RegexPropInfo.FriendlyPropName,
                    indexedCapture.Value,
                    typeCounts
                );
            }
        }

        // 2. TRANSFORM AGGREGATES INTO FINAL DTO
        foreach (var type in TokenTypeRegistry.AppliedOrderTypes)
        {
            if (!summaryCounts.TryGetValue(type, out var typeCounts))
            {
                TypePropCaptures[type] = [];
                continue;
            }

            var propValueSets = new List<RegexPropValueSet>();
            foreach (var propPathPair in typeCounts)
            {
                var sortedValues = propPathPair.Value
                    .Select(valuePair => new StringValueCaptureCount(valuePair.Key, valuePair.Value))
                    .OrderByDescending(v => v.CaptureCount)
                    .ToList();

                if (sortedValues.Any())
                {
                    propValueSets.Add(new RegexPropValueSet(propPathPair.Key, sortedValues));
                }
            }
            TypePropCaptures[type] = propValueSets;
        }
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
        {
            CollectAllTokensRecursive(topLevelUnit, allTokens);
        }

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
        foreach (var childToken in currentUnit.ChildTokens)
        {
            CollectAllTokensRecursive(childToken, collection);
        }
    }

    /// <summary>
    /// Instance helper that recursively traverses an object's properties for the constructor.
    /// </summary>
    private void FlattenAndCountRecursive(string currentPropPath, object currentValue, Dictionary<string, Dictionary<string, int>> typeCounts)
    {
        if (currentValue == null) return;

        // CORRECTED SWITCH ORDER: Most-derived types must be checked first to prevent the base case from capturing them.
        switch (currentValue)
        {
            case TokenUnitOneOf tokenUnitOneOf:
                var oneOfProp = tokenUnitOneOf.GetType()
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .FirstOrDefault(p => p.GetValue(tokenUnitOneOf) != null);

                if (oneOfProp?.GetValue(tokenUnitOneOf) is { } childVal)
                {
                    // Continue with the existing path, as the OneOf is just a container.
                    FlattenAndCountRecursive(currentPropPath, childVal, typeCounts);
                }
                break;

            case TokenUnitDistilled tokenUnitDistilled:
                foreach (var placeholderPropItem in tokenUnitDistilled.DistilledValues)
                {
                    foreach (var distilledItem in placeholderPropItem.Value)
                    {
                        var childPropPath = $"{currentPropPath}:{distilledItem.Key.Name.ToFriendlyCase()}";
                        IncrementValueCount(typeCounts, childPropPath, distilledItem.Value.ToString());
                    }
                }
                break;

            case TokenUnit childTokenUnit:
                // When we encounter another TokenUnit, we must NOT reflect over its properties.
                // Instead, we continue the same safe pattern of iterating only its captures.
                foreach (var indexedCapture in childTokenUnit.IndexedPropertyCaptures)
                {
                    var childPropPath = $"{currentPropPath}:{indexedCapture.RegexPropInfo.FriendlyPropName}";
                    FlattenAndCountRecursive(childPropPath, indexedCapture.Value, typeCounts);
                }
                break;

            default:
                // Base case: The value is a primitive or string, so we count it.
                IncrementValueCount(typeCounts, currentPropPath, currentValue.ToString());
                break;
        }
    }

    /// <summary>
    /// Instance helper that safely increments the count for a given property path and value.
    /// </summary>
    private void IncrementValueCount(Dictionary<string, Dictionary<string, int>> typeCounts, string propPath, string stringValue)
    {
        if (!typeCounts.TryGetValue(propPath, out var valueCounts))
        {
            valueCounts = new Dictionary<string, int>();
            typeCounts[propPath] = valueCounts;
        }

        valueCounts.TryGetValue(stringValue, out int currentCount);
        valueCounts[stringValue] = currentCount + 1;
    }
}