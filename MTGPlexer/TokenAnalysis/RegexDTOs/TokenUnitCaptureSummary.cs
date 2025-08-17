namespace MTGPlexer.TokenAnalysis.RegexDTOs;

public record TokenUnitCaptureSummary
{
    public Dictionary<Type, List<RegexPropValueSet>> TypePropCaptures { get; } = [];

    public TokenUnitCaptureSummary(List<TokenUnit> hydratedTokenUnits)
    {
        Dictionary<Type, Dictionary<PropPathValAsString, int>> typePropCaptureStringValues = [];
    
        foreach (var hydratedTokenUnit in hydratedTokenUnits)
        {
            Dictionary<PropPathValAsString, int> typePropDict;
    
            if (!typePropCaptureStringValues.TryGetValue(hydratedTokenUnit.Type, out typePropDict))
            {
                typePropDict = [];
                typePropCaptureStringValues[hydratedTokenUnit.Type] = typePropDict;
            }

            TokenCaptureValuesFlattened tokenCaptureValuesFlattened = new(hydratedTokenUnit);

            foreach (var propPathValAsString in tokenCaptureValuesFlattened.PropPathValAsStrings)
            {
                if (typePropDict.TryGetValue(propPathValAsString, out _))
                    typePropDict[propPathValAsString]++;
                else
                    typePropDict[propPathValAsString] = 1;
            }
        }
    
        foreach (var type in TokenTypeRegistry.AppliedOrderTypes)
        {
            if (!typePropCaptureStringValues.TryGetValue(type, out Dictionary<PropPathValAsString, int> propDict))
            {
                // This type regex has no captures
                TypePropCaptures[type] = [];
                continue;
            }

            List<RegexPropValueSet> regexPropValueSets = [];
            var propPathGroups = propDict.GroupBy(x => x.Key.PropPath);

            foreach (var propPathGroup in propPathGroups)
            {
                var valueCaptureCounts = propPathGroup.Select(x => new StringValueCaptureCount(x.Key.ValAsString, x.Value)).ToList();
                valueCaptureCounts = valueCaptureCounts.OrderByDescending(x => x.CaptureCount).ToList();
                RegexPropValueSet set = new(propPathGroup.Key, valueCaptureCounts);
                regexPropValueSets.Add(set);
            }

            TypePropCaptures[type] = regexPropValueSets;
        }
    }
}
