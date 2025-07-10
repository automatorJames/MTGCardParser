namespace MTGCardParser.TokenTesting.DTOs; // Or your preferred namespace

/// <summary>
/// Represents a property capture from a token, enriched with a stable index
/// for consistent processing (e.g., coloring) and ordered by position.
/// </summary>
/// <param name="Property">The metadata for the captured property.</param>
/// <param name="Span">The text span of the capture.</param>
/// <param name="OriginalIndex">A stable, zero-based index of this capture within its parent token's original list of properties.</param>
public record IndexedPropertyCapture
{
    public RegexPropInfo RegexPropInfo { get; init; }
    public TextSpan Span { get; init; }
    public int OriginalIndex { get; init; }
    public int SpanStart { get; init; }
    public int SpanEnd { get; init; }

    public IndexedPropertyCapture(RegexPropInfo property, TextSpan span, int originalIndex)
    {
        RegexPropInfo = property;
        Span = span;
        OriginalIndex = originalIndex;
        SpanStart = Span.Position.Absolute;
        SpanEnd = Span.Position.Absolute + Span.Length;
    }

    public override string ToString() => $"Prop: {RegexPropInfo.Name} | Prop Index: {OriginalIndex} | Capture: \"{Span.ToStringValue()}\"";
}