namespace Glyphotype.GlyphAnalysisDTOs.TypeExpressions;

public class GlyphOccurrenceSummary
{
    public Type Type { get; }
    public string TypeNameFriendly { get; }
    public int OccurrenceCount { get; }
    public Dictionary<string, EnumCaptureTraceSummary> EnumCaptureSummaries { get; } = [];
    public Dictionary<string, DynamicCaptureTraceSummary> DynamicCaptureSummaries { get; } = [];
    public RegexGraph RegexGraph { get; }

    private readonly IEnumerable<EnumNode> _enumNodes;
    private readonly IEnumerable<DynamicGlyphNode> _dynamicNodes;

    private GlyphOccurrenceSummary(Type type, int occurrenceCount)
    {
        Type = type;

        if (!GlyphTypeRegistry.RegexGraphs.TryGetValue(Type, out var graph))
            throw new Exception($"No {nameof(RegexGraph)} registered for {nameof(Glyph)} type {Type.Name}");

        RegexGraph = graph;
        TypeNameFriendly = Type.Name.ToFriendlyCase(TitleDisplayOption.Sentence);
        OccurrenceCount = occurrenceCount;

        _enumNodes = RegexGraph.NamedGroupFlatGraph.Values.OfType<EnumNode>();
        _dynamicNodes = RegexGraph.NamedGroupFlatGraph.Values.OfType<DynamicGlyphNode>();
    }

    /// <summary>
    /// Constructs a summary for a registered token type that had zero matches across the corpus,
    /// so it can still be listed (e.g. on TypeRegexPage) rather than silently disappearing.
    /// </summary>
    public GlyphOccurrenceSummary(Type zeroOccurrenceType) 
        : this(zeroOccurrenceType, 0)
    {
        foreach (var enumNode in _enumNodes)
            EnumCaptureSummaries[enumNode.FullyQualifiedName] = EnumCaptureTraceSummary.CreateEmpty(enumNode.FullyQualifiedName, enumNode.Navigation.UnderlyingType);

        foreach (var dynamicNode in _dynamicNodes)
            DynamicCaptureSummaries[dynamicNode.FullyQualifiedName] = DynamicCaptureTraceSummary.CreateEmpty(dynamicNode.FullyQualifiedName);
    }

    public GlyphOccurrenceSummary(Type type, IEnumerable<Glyph> rootGlyphsOfType)
        : this(type, ValidateAndCount(type, rootGlyphsOfType))
    {
        foreach (var enumNode in _enumNodes)
            EnumCaptureSummaries[enumNode.FullyQualifiedName] = new(enumNode.FullyQualifiedName, rootGlyphsOfType);

        foreach (var dynamicNode in _dynamicNodes)
            DynamicCaptureSummaries[dynamicNode.FullyQualifiedName] = new(dynamicNode.FullyQualifiedName, rootGlyphsOfType);
    }

    private static int ValidateAndCount(Type type, IEnumerable<Glyph> rootGlyphsOfType)
    {
        if (rootGlyphsOfType.Any(x => x.Type != type))
            throw new Exception($"All {nameof(rootGlyphsOfType)} must be of the specified type");

        return rootGlyphsOfType.Count();
    }
}