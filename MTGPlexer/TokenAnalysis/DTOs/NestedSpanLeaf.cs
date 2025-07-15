namespace MTGPlexer.TokenAnalysis.DTOs;

public record NestedSpanLeaf
(
    IndexedPropertyCapture PropertyCapture,
    string Path,
    int NestedDepth
) 
: NestedSpanTerminal(Path, NestedDepth, PropertyCapture.Span.ToStringValue().Trim(), PropertyCapture.Palette)
{
    public override string ToString() => PropertyCapture.Span.ToStringValue();
}
