namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

public class DynamicCaptureTraceSummary : NamedGroupCaptureTraceSummary
{
    protected override CaptureNodeKind NodeKind => CaptureNodeKind.Dynamic;

    /// <summary>
    /// For each type resovled at runtime for this DynamicCaptureNode, the occurrence count per
    /// distinct capture trace value encountered.
    /// </summary>
    public Dictionary<Type, Dictionary<string, int>> ResolvedTypeCaptureValueOccurrenceCounts { get; } = [];


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

            ResolvedTypeCaptureValueOccurrenceCounts.TryAdd(dynamicToken.ResolvedType, []);
            ResolvedTypeCaptureValueOccurrenceCounts[dynamicToken.ResolvedType].TryAdd(captureTrace.CaptureValue, 0);
            ResolvedTypeCaptureValueOccurrenceCounts[dynamicToken.ResolvedType][captureTrace.CaptureValue]++;
        }
    }
}