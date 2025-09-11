using MTGPlexer.RegexGeneration.RegexSegments;

namespace MTGPlexer.TokenAnalysisDTOs.CardAnalysis;

/// <summary>
/// Represents a property capture from a token, enriched with a stable index
/// for consistent processing (e.g., coloring) and ordered by position.
/// </summary>
public record IndexedPropertyCapture
{
    public RegexPropInfo RegexPropInfo { get; set; }
    //public TextSpan Span { get; }
    public Capture Capture { get; }
    public int Start { get; }
    public int End { get; }
    public int Length { get; }
    public bool IsChildToken { get; }
    public object Value { get; set; }
    public int CapturePosition { get; }
    public DeterministicPalette Palette { get; }
    public bool IgnoreInAnalysis { get; }

    //public IndexedPropertyCapture(RegexPropInfo regexPropInfo, TextSpan span, object value, int capturePosition)
    //{
    //    RegexPropInfo = regexPropInfo;
    //    Span = span;
    //    Start = Span.Position.Absolute;
    //    End = Span.Position.Absolute + Span.Length;
    //    Length = Span.Length;
    //    IsChildToken = regexPropInfo.RegexPropType == RegexPropType.TokenUnit || regexPropInfo.RegexPropType == RegexPropType.TokenUnitOneOf;
    //    Value = value;
    //    CapturePosition = capturePosition;
    //    Palette = DeterministicPalette.GetFixedRainbowPalette(CapturePosition);
    //    IgnoreInAnalysis = RegexPropInfo.Prop.DeclaringType.GetCustomAttribute<IgnoreInAnalysisAttribute>() != null;
    //}

    public IndexedPropertyCapture(RegexPropInfo regexPropInfo, Capture capture, object value, int capturePosition)
    {
        RegexPropInfo = regexPropInfo;
        Capture = capture ?? Match.Empty;
        Start = capture.Index;
        End = capture.Index + capture.Length;
        Length = capture.Length;
        IsChildToken = regexPropInfo.RegexPropType == RegexPropType.TokenUnit || regexPropInfo.RegexPropType == RegexPropType.TokenUnitOneOf;
        Value = value;
        CapturePosition = capturePosition;
        Palette = DeterministicPalette.GetFixedRainbowPalette(CapturePosition);
        IgnoreInAnalysis = RegexPropInfo.Prop.DeclaringType.GetCustomAttribute<IgnoreInAnalysisAttribute>() != null;
    }


    //public override string ToString() => $"Prop: {RegexPropInfo.Name} | Position: {CapturePosition} | Capture: \"{Span.ToStringValue()}\"";
    public override string ToString() => $"Prop: {RegexPropInfo.Name} | Position: {CapturePosition} | Capture: \"{Capture.Value}\"";
}