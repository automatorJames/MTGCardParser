namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

public abstract class NamedGroupCaptureTraceSummary
{
    protected CaptureTrace _representativeCaptureTrace;

    protected abstract CaptureNodeKind NodeKind { get; }

    /// <summary>
    /// The fully qualified name that represents the path from the RegexGraph root node
    /// to the enum property being summarized herein.
    /// </summary>
    public string FullyQualifiedName { get; }

    protected List<CaptureTrace> CaptureTraces { get; }

    protected NamedGroupCaptureTraceSummary(string fullyQualifiedName, IEnumerable<TokenUnit> tokenUnits)
    {
        FullyQualifiedName = fullyQualifiedName;

        CaptureTraces = tokenUnits
            .Select(x => x.CaptureContext.RootCaptureTrace[fullyQualifiedName])
            .Where(x => x != null)
            .ToList();

        if (CaptureTraces.Count == 0)
            return;

        _representativeCaptureTrace = CaptureTraces.First();

        if (_representativeCaptureTrace.NodeKind != NodeKind)
            throw new Exception($"Expected {nameof(CaptureTrace.NodeKind)} {NodeKind}, but got {_representativeCaptureTrace.NodeKind}");
    }
}