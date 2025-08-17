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

    public TokenUnitCaptureSummary(List<TokenUnit> hydratedTokenUnits)
    {
        // Structure: Type -> PropertyPath -> StringValue -> Count
        var summaryCounts = new Dictionary<Type, Dictionary<string, Dictionary<string, int>>>();

        // 1. AGGREGATE
        foreach (var unit in hydratedTokenUnits)
        {
            if (!summaryCounts.TryGetValue(unit.Type, out var typeCounts))
            {
                typeCounts = new Dictionary<string, Dictionary<string, int>>();
                summaryCounts[unit.Type] = typeCounts;
            }

            // Start the recursive process for the top-level token
            foreach (var indexedCapture in unit.IndexedPropertyCaptures)
            {
                FlattenAndCountRecursive(
                    indexedCapture.RegexPropInfo.FriendlyPropName,
                    indexedCapture.Value,
                    typeCounts
                );
            }
        }

        // 2. TRANSFORM
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
    /// Recursively traverses an object's properties, building a flattened property path
    /// and counting the final string values in the provided dictionary.
    /// </summary>
    private void FlattenAndCountRecursive(string currentPropPath, object currentValue, Dictionary<string, Dictionary<string, int>> typeCounts)
    {
        if (currentValue == null) return;

        switch (currentValue)
        {
            case TokenUnitOneOf tokenUnitOneOf:
                // This logic can be simplified: find the single non-null value and recurse on it.
                var oneOfProp = tokenUnitOneOf.GetType()
                    .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly)
                    .FirstOrDefault(p => p.GetValue(tokenUnitOneOf) != null);

                if (oneOfProp?.GetValue(tokenUnitOneOf) is { } childVal)
                {
                    // For a OneOf, we don't extend the path with its own property name,
                    // as it's just a container. We continue with the existing path.
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
    /// Helper method to safely increment the count for a given property path and value.
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