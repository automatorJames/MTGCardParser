namespace Glyphotype.GlyphAnalysisDTOs.TypeExpressions;

public class DynamicCaptureTraceSummary : NamedGroupCaptureTraceSummary
{
    protected override CaptureNodeKind NodeKind => CaptureNodeKind.Dynamic;

    /// <summary>
    /// For each type resolved at runtime for this DynamicCaptureNode, the concrete hydrated <see cref="Glyph"/>
    /// instances captured as that type — enough to build a real <see cref="GlyphOccurrenceSummary"/> for it and
    /// render its own full pretty regex (see <see cref="DynamicSectionBuilder"/>) instead of just a literal value.
    /// </summary>
    public Dictionary<Type, List<Glyph>> ResolvedTypeCaptureUnits { get; } = [];


    /// <summary>
    /// Constructs a summary for an enum group that never occurred at all (its owning Glyph type
    /// had zero matches across the corpus), so every member is recorded with a zero count.
    /// </summary>
    DynamicCaptureTraceSummary(string fullyQualifiedName)
        : base(fullyQualifiedName, [])
    {
    }

    public static DynamicCaptureTraceSummary CreateEmpty(string fullyQualifiedName) => new(fullyQualifiedName, []);

    public DynamicCaptureTraceSummary(string fullyQualifiedName, IEnumerable<Glyph> glyphs)
        : base(fullyQualifiedName, glyphs)
    {
        foreach (var captureTrace in CaptureTraces)
        {
            if (captureTrace.ClrValue is not DynamicGlyph dynamicGlyph)
                throw new Exception($"{captureTrace.FullyQualifiedName} is of type {captureTrace.ClrValue.GetType().Name}, but expected {nameof(DynamicGlyph)}");

            if (dynamicGlyph.Item is not Glyph resolvedGlyph)
                throw new Exception($"{captureTrace.FullyQualifiedName} resolved to a {dynamicGlyph.Item?.GetType().Name ?? "null"}, but expected a {nameof(Glyph)}");

            ResolvedTypeCaptureUnits.TryAdd(dynamicGlyph.ResolvedType, []);
            ResolvedTypeCaptureUnits[dynamicGlyph.ResolvedType].Add(resolvedGlyph);
        }
    }
}