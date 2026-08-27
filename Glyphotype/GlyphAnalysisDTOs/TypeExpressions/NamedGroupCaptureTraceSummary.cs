namespace Glyphotype.GlyphAnalysisDTOs.TypeExpressions;

public abstract class NamedGroupCaptureTraceSummary
{
    protected CaptureTrace _representativeCaptureTrace;

    /// <summary>The concrete graph node type this summary expects its <see cref="CaptureTrace"/>s to come from - checked in the constructor as a sanity check against a mismatched summary/trace pairing.</summary>
    protected abstract Type ExpectedSourceNodeType { get; }

    /// <summary>
    /// The fully qualified name that represents the path from the RegexGraph root node
    /// to the enum property being summarized herein.
    /// </summary>
    public string FullyQualifiedName { get; }

    protected List<CaptureTrace> CaptureTraces { get; }

    protected NamedGroupCaptureTraceSummary(string fullyQualifiedName, IEnumerable<Glyph> glyphs)
    {
        FullyQualifiedName = fullyQualifiedName;

        // RootCaptureTrace[fullyQualifiedName] returns one representative trace per glyph - but a group
        // nested inside a repeated ("*"-quantified) ancestor list can capture more than once within a
        // single glyph's match (e.g. two keywords in one ManyOf<Buff> list), with every occurrence past
        // the first hanging off that representative's own Siblings rather than getting a separate entry
        // here. Expanding each representative via its own enumeration (self + Siblings - see
        // CaptureTrace.GetEnumerator) counts every real occurrence, not just one per glyph.
        CaptureTraces = glyphs
            .Select(x => x.CaptureContext.RootCaptureTrace[fullyQualifiedName])
            .Where(x => x != null)
            .SelectMany(x => x)
            .ToList();

        if (CaptureTraces.Count == 0)
            return;

        _representativeCaptureTrace = CaptureTraces.First();

        if (!ExpectedSourceNodeType.IsInstanceOfType(_representativeCaptureTrace.SourceNode))
            throw new Exception($"Expected {nameof(CaptureTrace.SourceNode)} of type {ExpectedSourceNodeType.Name}, but got {_representativeCaptureTrace.SourceNode.GetType().Name}");
    }
}