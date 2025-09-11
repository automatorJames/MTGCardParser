using MTGPlexer.RegexGeneration.RegexSegments;

namespace MTGPlexer.TokenAnalysisDTOs.CardAnalysis;

/// <summary>
/// Represents a property capture from a token, enriched with a stable index
/// for consistent processing (e.g., coloring) and ordered by position.
/// </summary>
public record IndexedPropertyCapture
{
    public RegexPropInfo RegexPropInfo { get; set; }
    public StructuredMatch Match { get; set; }
    public int Start { get; }
    public int End { get; }
    public int Length { get; }
    public bool IsChildToken { get; }
    public object Value { get; set; }
    public int CapturePosition { get; }
    public DeterministicPalette Palette { get; }
    public bool IgnoreInAnalysis { get; }

    public IndexedPropertyCapture(RegexPropInfo regexPropInfo, StructuredMatch match, object value, int capturePosition)
    {
        RegexPropInfo = regexPropInfo;
        Match = match;
        Start = match.Index;
        End = match.End;
        Length = match.Length;
        IsChildToken = regexPropInfo.RegexPropType == RegexPropType.TokenUnit || regexPropInfo.RegexPropType == RegexPropType.TokenUnitOneOf;
        Value = value;
        CapturePosition = capturePosition;
        Palette = DeterministicPalette.GetFixedRainbowPalette(CapturePosition);
        IgnoreInAnalysis = RegexPropInfo.Prop.DeclaringType.GetCustomAttribute<IgnoreInAnalysisAttribute>() != null;
    }


    public override string ToString() => $"Prop: {RegexPropInfo.Name} | Position: {CapturePosition} | Capture: \"{Match.Value}\"";
}