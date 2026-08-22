namespace Glyphotype.RegexGeneration.Graph;

/// <summary>
/// One piece of <see cref="CaptureTraceWalker.GetSegments"/>'s output: either a run of plain, uncaptured
/// source text falling between (or after) a trace's children, or one child <see cref="CaptureTrace"/>
/// itself for the caller to recurse into. Exactly one of <see cref="Text"/> / <see cref="Child"/> is set.
/// </summary>
public readonly record struct CaptureTraceSegment(string Text, CaptureTrace Child)
{
    public static CaptureTraceSegment ForText(string text) => new(text, null);
    public static CaptureTraceSegment ForChild(CaptureTrace child) => new(null, child);
}

/// <summary>
/// Walks a <see cref="CaptureTrace"/>'s immediate children in position order, yielding the gaps between
/// them (plain source text this node matched but no child captured) interleaved with the children
/// themselves. This cursor/substring math is the one shared traversal every "render a CaptureTrace's
/// source text, colored per capture" consumer builds its own output from - <c>SpanView</c> recurses into
/// each child to build nested, per-level underline spans, while <see cref="Presentation.MatchContentRenderer"/>
/// recurses the same way but flattens to a single color per leaf run - so it exists in exactly one place.
/// </summary>
public static class CaptureTraceWalker
{
    public static IEnumerable<CaptureTraceSegment> GetSegments(CaptureTrace trace)
    {
        var cursor = 0;

        foreach (var child in trace.Children.OrderBy(c => c.Index))
        {
            var relativeChildStart = child.Index - trace.Index;

            if (relativeChildStart > cursor)
                yield return CaptureTraceSegment.ForText(trace.CaptureContext.SourceText.Substring(trace.Index + cursor, relativeChildStart - cursor));

            yield return CaptureTraceSegment.ForChild(child);
            cursor = child.End - trace.Index;
        }

        if (cursor < trace.Length)
            yield return CaptureTraceSegment.ForText(trace.CaptureContext.SourceText.Substring(trace.Index + cursor, trace.Length - cursor));
    }
}
