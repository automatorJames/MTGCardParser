namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

public record TokenUnitCapture
{
    int _orphanCaptureCount;

    public Type Type { get; }
    public string TypeName { get; }
    public string TypeNameFriendly { get; }
    public int OccurrenceCount { get; }
    public FormattedRegex FormattedRegex { get; }
    public Palette Palette { get; }
    public HashSet<RegexCommentedAlternateLine> LinesWithMatches { get; } = [];

    /// <summary>
    /// Maps capture group prop path --> set of capture value variant counts (the string key is the canonical value)
    /// </summary>
    public Dictionary<CaptureGroupPropPath, Dictionary<object, CaptureValueVariantSet>> PropPathVariantSets { get; } = [];

    public TokenUnitCapture(Type type, List<TokenUnit> rootTokensUnitsOfType)
    {
        Type = type;
        TypeName = type.Name;
        TypeNameFriendly = TypeName.ToFriendlyCase(TitleDisplayOption.Sentence);
        Palette = TokenTypeRegistry.Palettes[type];
        FormattedRegex = TokenTypeRegistry.Templates[type].FormattedRegex;
        OccurrenceCount = rootTokensUnitsOfType.Count;

        foreach (var tokenUnit in rootTokensUnitsOfType)
            foreach (var indexedCapture in tokenUnit.IndexedPropertyCaptures)
                FlattenAndCountRecursive([tokenUnit.Type.Name, indexedCapture.RegexPropInfo.Name], indexedCapture.Value);
    }

    void FlattenAndCountRecursive(List<string> currentPropPath, object currentValue)
    {
        if (currentValue == null)
            return;

        // todo: handle dynamic token prop types in the switch below

        switch (currentValue)
        {
            case TokenUnitOneOf tokenUnitOneOf:
                var singleIndexedCapture = tokenUnitOneOf.GetIndexedPropertyCaptureSingle();
                currentPropPath.Add(singleIndexedCapture.RegexPropInfo.Name);
                FlattenAndCountRecursive(currentPropPath, singleIndexedCapture.Value);
                break;

            case TokenUnit childTokenUnit:
                foreach (var indexedCapture in childTokenUnit.IndexedPropertyCaptures)
                {
                    var childPropPath = currentPropPath.Concat([indexedCapture.RegexPropInfo.Name]).ToList();
                    FlattenAndCountRecursive(childPropPath, indexedCapture.Value);
                }
                break;

            default:
                // Base case: The value is a primitive or string, so we count it.
                IncrementValueCount(currentPropPath, currentValue);
                break;
        }
    }

    void IncrementValueCount(List<string> propPath, object terminalValue)
    {
        CaptureGroupPropPath groupPropPath = new(propPath);
        var matchingAltLine = FormattedRegex[groupPropPath.PropPath, terminalValue];

        if (matchingAltLine == null)
        {
            _orphanCaptureCount++;
            return;
        }

        LinesWithMatches.Add(matchingAltLine);
        
        var terminalCaptureAsFriendlyString = terminalValue.ToString().ToFriendlyCase(TitleDisplayOption.Lower);

        // If no variantSetDict for this capture group path exists already, make one
        if (!PropPathVariantSets.TryGetValue(groupPropPath, out var variantSetDict))
        {
            variantSetDict = new();
            PropPathVariantSets[groupPropPath] = variantSetDict;
        }

        // If the variantSetDict already contains the line's canonical value, increment it,
        // otherwise create a new CaptureValueVariantSet for the canonical value, and add it as an entry to the parent dict
        if (variantSetDict.TryGetValue(matchingAltLine.CanonicalValue, out var variantSet))
            variantSet.IncrementVariant(terminalCaptureAsFriendlyString);
        else
        {
            variantSet = new(matchingAltLine, terminalCaptureAsFriendlyString);
            variantSetDict[terminalValue] = variantSet;
        }
    }
}