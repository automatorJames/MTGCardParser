namespace MTGPlexer.TokenAnalysis.DTOs; // Or your preferred namespace

/// <summary>
/// Represents a property capture from a token, enriched with a stable index
/// for consistent processing (e.g., coloring) and ordered by position.
/// </summary>
/// <param name="Property">The metadata for the captured property.</param>
/// <param name="Span">The text span of the capture.</param>
/// <param name="Position">A stable, zero-based index of this capture within its parent token's original list of properties.</param>
public record IndexedPropertyCapture
{
    public RegexPropInfo RegexPropInfo { get; }
    public TextSpan Span { get; }
    public int Index { get; }
    public int Start { get; }
    public int End { get; }
    public int Length { get; }
    public bool IsChildToken { get; }
    public bool IsComplex { get; }
    public object Value { get; }
    public int CapturePosition { get; }
    public DeterministicPalette Palette { get; }

    public IndexedPropertyCapture(RegexPropInfo regexPropInfo, TextSpan span, object value, int capturePosition)
    {
        RegexPropInfo = regexPropInfo;
        Span = span;
        Start = Span.Position.Absolute;
        End = Span.Position.Absolute + Span.Length;
        Length = Span.Length;
        IsChildToken = regexPropInfo.RegexPropType == RegexPropType.TokenUnit;
        IsComplex = value is TokenUnitComplex;
        Value = value;
        CapturePosition = capturePosition;
        Palette = new(CapturePosition);
        // todo: have to handle TokenUnitOneOfTypes with special logic
    }

    public override string ToString() => $"Prop: {RegexPropInfo.Name} | Position: {CapturePosition} | Capture: \"{Span.ToStringValue()}\"";
}