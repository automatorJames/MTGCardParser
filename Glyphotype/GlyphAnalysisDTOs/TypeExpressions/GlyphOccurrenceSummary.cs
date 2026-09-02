namespace Glyphotype.GlyphAnalysisDTOs.TypeExpressions;

public class GlyphOccurrenceSummary
{
    public Type Type { get; }
    public string TypeNameFriendly { get; }
    public int OccurrenceCount { get; }

    /// <summary>
    /// Every matched occurrence of <see cref="Type"/> across the corpus, in encounter order - the source
    /// for TypeRegexPage's "Matches" footer tray view (see <see cref="Presentation.MatchContentRenderer"/>).
    /// Empty for a zero-occurrence or dynamically-embedded (document-agnostic) summary.
    /// </summary>
    public List<MatchOccurrence> MatchOccurrences { get; } = [];

    public Dictionary<string, EnumCaptureTraceSummary> EnumCaptureSummaries { get; } = [];
    public Dictionary<string, DynamicCaptureTraceSummary> DynamicCaptureSummaries { get; } = [];
    public RegexGraph RegexGraph { get; }

    private readonly IEnumerable<EnumNode> _enumNodes;
    private readonly IEnumerable<DynamicGlyphNode> _dynamicNodes;

    private GlyphOccurrenceSummary(Type type, int occurrenceCount)
    {
        Type = type;

        if (!GlyphTypeRegistry.RegexGraphIncludingDependents.TryGetValue(Type, out var graph))
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

    public GlyphOccurrenceSummary(Type type, IEnumerable<MatchOccurrence> occurrences)
        : this(type, ValidateAndCount(type, occurrences))
    {
        MatchOccurrences = occurrences.ToList();
        var glyphs = MatchOccurrences.Select(x => x.Glyph);

        foreach (var enumNode in _enumNodes)
            EnumCaptureSummaries[enumNode.FullyQualifiedName] = new(enumNode.FullyQualifiedName, glyphs);

        foreach (var dynamicNode in _dynamicNodes)
            DynamicCaptureSummaries[dynamicNode.FullyQualifiedName] = new(dynamicNode.FullyQualifiedName, glyphs);
    }

    private static int ValidateAndCount(Type type, IEnumerable<MatchOccurrence> occurrences)
    {
        if (occurrences.Any(x => x.Glyph.Type != type))
            throw new Exception($"All {nameof(occurrences)} must be of the specified type");

        return occurrences.Count();
    }
}