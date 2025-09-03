namespace MTGPlexer.TokenAnalysis.CardCaptureDTOs;

public record SpanLeaf
(
    IndexedPropertyCapture PropertyCapture,
    string Path,
    int NestedDepth,
    string OriginalLineText,
    string CardName
) 
: SpanTerminal(
    Path, 
    NestedDepth, 
    OriginalLineText.Substring(PropertyCapture.Start, PropertyCapture.Length).Replace(Card.ThisToken, CardName), 
    PropertyCapture.Palette, 
    PropertyCapture.IgnoreInAnalysis
)
{
    public override string ToString() => PropertyCapture.Span.ToStringValue();
}
