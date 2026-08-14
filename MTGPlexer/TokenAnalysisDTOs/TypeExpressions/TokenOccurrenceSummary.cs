namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

public class TokenOccurrenceSummary
{
    public Type Type { get; }
    public string TypeNameFriendly { get; }
    public int OccurrenceCount { get; }
    public Dictionary<string, EnumCaptureTraceSummary> EnumCaptureSummaries { get; } = [];
    public Dictionary<string, DynamicCaptureTraceSummary> DynamicCaptureSummaries { get; } = [];
    public RegexGraph RegexGraph { get; }

    /// <summary>
    /// Constructs a summary for a registered token type that had zero matches across the corpus,
    /// so it can still be listed (e.g. on TypeRegexPage) rather than silently disappearing.
    /// </summary>
    public TokenOccurrenceSummary(Type zeroOccurrenceType)
    {
        Type = zeroOccurrenceType;

        if (!TokenTypeRegistry.RegexGraphs.TryGetValue(Type, out var graph))
            throw new Exception($"No {nameof(RegexGraph)} registered for {nameof(TokenUnit)} type {Type.Name}");

        RegexGraph = graph;
        TypeNameFriendly = Type.Name.ToFriendlyCase(TitleDisplayOption.Sentence);
        OccurrenceCount = 0;

        var enumNodes = RegexGraph.NamedGroupFlatGraph.Values.OfType<EnumNode>();

        foreach (var enumNode in enumNodes)
            EnumCaptureSummaries[enumNode.FullyQualifiedName] = EnumCaptureTraceSummary.CreateEmpty(enumNode.FullyQualifiedName, enumNode.Navigation.UnderlyingType);
    }

    public TokenOccurrenceSummary(Type type, IEnumerable<TokenUnit> rootTokenUnitsOfType)
    {
        if (rootTokenUnitsOfType.Any(x => x.Type != type))
            throw new Exception($"All {nameof(rootTokenUnitsOfType)} must be of the specified type");

        Type = type;

        if (!TokenTypeRegistry.RegexGraphs.TryGetValue(Type, out var graph))
            throw new Exception($"No {nameof(RegexGraph)} registered for {nameof(TokenUnit)} type {Type.Name}");

        RegexGraph = graph;
        TypeNameFriendly = Type.Name.ToFriendlyCase(TitleDisplayOption.Sentence);
        OccurrenceCount = rootTokenUnitsOfType.Count();

        var enumNodes = RegexGraph.NamedGroupFlatGraph.Values.OfType<EnumNode>();

        foreach (var enumNode in enumNodes)
            EnumCaptureSummaries[enumNode.FullyQualifiedName] = new(enumNode.FullyQualifiedName, rootTokenUnitsOfType);

        var dynamicNodes = RegexGraph.NamedGroupFlatGraph.Values.OfType<DynamicTokenNode>();

        foreach (var dynamicNode in dynamicNodes)
            DynamicCaptureSummaries[dynamicNode.FullyQualifiedName] = new(dynamicNode.FullyQualifiedName, rootTokenUnitsOfType);
    }
}