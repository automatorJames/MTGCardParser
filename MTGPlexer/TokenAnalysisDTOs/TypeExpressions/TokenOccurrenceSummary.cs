namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

public class TokenOccurrenceSummary
{
    public Type Type { get; }
    public string TypeNameFriendly { get; }
    public int OccurrenceCount { get; }
    public Dictionary<string, EnumCaptureTraceSummary> EnumCaptureSummaries { get; } = [];
    public Dictionary<string, DynamicCaptureTraceSummary> DynamicCaptureSummaries { get; } = [];
    public RegexGraph RegexGraph { get; }

    private readonly IEnumerable<EnumNode> _enumNodes;
    private readonly IEnumerable<DynamicTokenNode> _dynamicNodes;

    private TokenOccurrenceSummary(Type type, int occurrenceCount)
    {
        Type = type;

        if (!TokenTypeRegistry.RegexGraphs.TryGetValue(Type, out var graph))
            throw new Exception($"No {nameof(RegexGraph)} registered for {nameof(TokenUnit)} type {Type.Name}");

        RegexGraph = graph;
        TypeNameFriendly = Type.Name.ToFriendlyCase(TitleDisplayOption.Sentence);
        OccurrenceCount = occurrenceCount;

        _enumNodes = RegexGraph.NamedGroupFlatGraph.Values.OfType<EnumNode>();
        _dynamicNodes = RegexGraph.NamedGroupFlatGraph.Values.OfType<DynamicTokenNode>();
    }

    /// <summary>
    /// Constructs a summary for a registered token type that had zero matches across the corpus,
    /// so it can still be listed (e.g. on TypeRegexPage) rather than silently disappearing.
    /// </summary>
    public TokenOccurrenceSummary(Type zeroOccurrenceType) 
        : this(zeroOccurrenceType, 0)
    {
        foreach (var enumNode in _enumNodes)
            EnumCaptureSummaries[enumNode.FullyQualifiedName] = EnumCaptureTraceSummary.CreateEmpty(enumNode.FullyQualifiedName, enumNode.Navigation.UnderlyingType);

        foreach (var dynamicNode in _dynamicNodes)
            DynamicCaptureSummaries[dynamicNode.FullyQualifiedName] = DynamicCaptureTraceSummary.CreateEmpty(dynamicNode.FullyQualifiedName);
    }

    public TokenOccurrenceSummary(Type type, IEnumerable<TokenUnit> rootTokenUnitsOfType)
        : this(type, ValidateAndCount(type, rootTokenUnitsOfType))
    {
        foreach (var enumNode in _enumNodes)
            EnumCaptureSummaries[enumNode.FullyQualifiedName] = new(enumNode.FullyQualifiedName, rootTokenUnitsOfType);

        foreach (var dynamicNode in _dynamicNodes)
            DynamicCaptureSummaries[dynamicNode.FullyQualifiedName] = new(dynamicNode.FullyQualifiedName, rootTokenUnitsOfType);
    }

    private static int ValidateAndCount(Type type, IEnumerable<TokenUnit> rootTokenUnitsOfType)
    {
        if (rootTokenUnitsOfType.Any(x => x.Type != type))
            throw new Exception($"All {nameof(rootTokenUnitsOfType)} must be of the specified type");

        return rootTokenUnitsOfType.Count();
    }
}