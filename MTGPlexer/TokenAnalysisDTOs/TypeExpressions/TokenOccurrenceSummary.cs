namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

public class TokenOccurrenceSummary
{
    public Type Type { get; }
    public string TypeNameFriendly { get; }
    public int OccurrenceCount { get; }
    public Dictionary<string, EnumCaptureTraceSummary> EnumCaptureSummaries { get; } = [];
    public RegexGraph RegexGraph { get; }

    public TokenOccurrenceSummary(IEnumerable<TokenUnit> rootTokenUnitsOfType)
    {
        if (rootTokenUnitsOfType == null || !rootTokenUnitsOfType.Any())
            throw new Exception($"Cannot construct {nameof(TokenOccurrenceSummary)} with null or empty collection");

        var distinctTypes = rootTokenUnitsOfType.Select(x => x.Type).Distinct();

        if (distinctTypes.Count() > 1)
            throw new Exception($"Expected exactly one distinct type among root {nameof(TokenUnit)} instances, but got {distinctTypes.Count()}");

        var representativeTokenUnit = rootTokenUnitsOfType.First();
        Type = representativeTokenUnit.Type;

        if (!TokenTypeRegistry.RegexGraphs.TryGetValue(Type, out var graph))
            throw new Exception($"No {nameof(RegexGraph)} registered for {nameof(TokenUnit)} type {Type.Name}");

        RegexGraph = graph;
        TypeNameFriendly = Type.Name.ToFriendlyCase(TitleDisplayOption.Sentence);
        OccurrenceCount = rootTokenUnitsOfType.Count();

        var enumNodes = RegexGraph.NamedGroupFlatGraph.Values.OfType<EnumNode>();

        foreach (var enumNode in enumNodes)
        {
            var enumNodeCaptures = rootTokenUnitsOfType.Select(x => x.CaptureContext.RootCaptureTrace[enumNode.FullyQualifiedName]);
            EnumCaptureSummaries[enumNode.FullyQualifiedName] = new(enumNodeCaptures);
        }
    }
}