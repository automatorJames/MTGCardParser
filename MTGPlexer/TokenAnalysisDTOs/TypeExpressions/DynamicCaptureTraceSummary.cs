namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

public class DynamicCaptureTraceSummary : NamedGroupCaptureTraceSummary
{
    protected override CaptureNodeKind NodeKind => CaptureNodeKind.Dynamic;

    /// <summary>
    /// For each type resolved at runtime for this DynamicCaptureNode, the concrete hydrated <see cref="TokenUnit"/>
    /// instances captured as that type — enough to build a real <see cref="TokenOccurrenceSummary"/> for it and
    /// render its own full pretty regex (see <see cref="DynamicSectionBuilder"/>) instead of just a literal value.
    /// </summary>
    public Dictionary<Type, List<TokenUnit>> ResolvedTypeCaptureUnits { get; } = [];


    /// <summary>
    /// Constructs a summary for an enum group that never occurred at all (its owning TokenUnit type
    /// had zero matches across the corpus), so every member is recorded with a zero count.
    /// </summary>
    DynamicCaptureTraceSummary(string fullyQualifiedName)
        : base(fullyQualifiedName, [])
    {
    }

    public static DynamicCaptureTraceSummary CreateEmpty(string fullyQualifiedName) => new(fullyQualifiedName, []);

    public DynamicCaptureTraceSummary(string fullyQualifiedName, IEnumerable<TokenUnit> tokenUnits)
        : base(fullyQualifiedName, tokenUnits)
    {
        foreach (var captureTrace in CaptureTraces)
        {
            if (captureTrace.ClrValue is not DynamicToken dynamicToken)
                throw new Exception($"{captureTrace.FullyQualifiedName} is of type {captureTrace.ClrValue.GetType().Name}, but expected {nameof(DynamicToken)}");

            if (dynamicToken.Item is not TokenUnit resolvedTokenUnit)
                throw new Exception($"{captureTrace.FullyQualifiedName} resolved to a {dynamicToken.Item?.GetType().Name ?? "null"}, but expected a {nameof(TokenUnit)}");

            ResolvedTypeCaptureUnits.TryAdd(dynamicToken.ResolvedType, []);
            ResolvedTypeCaptureUnits[dynamicToken.ResolvedType].Add(resolvedTokenUnit);
        }
    }
}