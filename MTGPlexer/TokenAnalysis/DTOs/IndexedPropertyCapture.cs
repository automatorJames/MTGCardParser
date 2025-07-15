namespace MTGPlexer.TokenAnalysis.DTOs; // Or your preferred namespace

/// <summary>
/// Represents a property capture from a token, enriched with a stable index
/// for consistent processing (e.g., coloring) and ordered by position.
/// </summary>
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
    public bool IgnoreInAnalysis { get; }

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
        IgnoreInAnalysis = RegexPropInfo.Prop.DeclaringType.GetCustomAttribute<IgnoreInAnalysisAttribute>() != null;

        // todo: have to handle TokenUnitOneOfTypes with special logic
    }

    public override string ToString() => $"Prop: {RegexPropInfo.Name} | Position: {CapturePosition} | Capture: \"{Span.ToStringValue()}\"";
}