namespace MTGPlexer.TokenAnalysis.DTOs;

public record NestedSpanLeaf
(
    IndexedPropertyCapture PropertyCapture,
    string Path,
    int NestedDepth
) 
: NestedSpanTerminal(Path, NestedDepth, PropertyCapture.Span.ToStringValue(), PropertyCapture.Palette)
{
    //public bool IsComplex { get; } = Token is TokenUnitComplex;

    public override string ToString() => PropertyCapture.Span.ToStringValue();
}
