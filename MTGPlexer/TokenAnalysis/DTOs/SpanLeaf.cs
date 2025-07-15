namespace MTGPlexer.TokenAnalysis.DTOs;

public record SpanLeaf
(
    IndexedPropertyCapture PropertyCapture,
    string Path,
    int NestedDepth
) 
: SpanTerminal(Path, NestedDepth, PropertyCapture.Span.ToStringValue().Trim(), PropertyCapture.Palette, PropertyCapture.IgnoreInAnalysis)
{
    public override string ToString() => PropertyCapture.Span.ToStringValue();
}
