using System.Diagnostics;

namespace MTGPlexer.CommonDTOs;

/// <summary>
/// Represents a property capture from a token, enriched with a stable index
/// for consistent processing (e.g., coloring) and ordered by position.
/// </summary>
public record IndexedPropertyCapture
{
    public RegexPropInfo RegexPropInfo { get; set; }
    public Capture Capture { get; set; }
    public int Start { get; }
    public int End { get; }
    public int Length { get; }
    public bool IsChildToken { get; }
    public string Text { get; set; }
    public object Value { get; set; }
    public int Ordinal { get; }
    public Palette Palette { get; }
    public bool IgnoreInAnalysis { get; }
    public bool IsDerivedFromManyItem { get; }
    public string Path { get; set; }
    public CaptureGroupPropPath CaptureGroupPropPath { get; set; }

    public IndexedPropertyCapture(RegexPropInfo regexPropInfo, Capture capture, object value, int capturePosition, string parentTokenPath)
    {
        RegexPropInfo = regexPropInfo;
        Capture = capture;
        Start = capture.Index;
        Length = capture.Length;
        End = Start + Length;
        IsChildToken = regexPropInfo.RegexPropType == RegexPropType.TokenUnit || regexPropInfo.RegexPropType == RegexPropType.TokenUnitOneOf;
        Text = capture.Value;
        Value = value;
        Ordinal = capturePosition;
        Palette = DeterministicPalette.GetFixedRainbowPalette(Ordinal);
        IgnoreInAnalysis = RegexPropInfo.Prop.DeclaringType.GetCustomAttribute<IgnoreInAnalysisAttribute>() != null;
        Path = parentTokenPath.Dot(regexPropInfo.Name);
        CaptureGroupPropPath = regexPropInfo.IsTerminal ? new(parentTokenPath.Dot(regexPropInfo.Name).Dot(value.ToString())) : new(parentTokenPath);
    }

    /// <summary>
    /// Constructor used for synthesizing IndexedPropertyCaptures from ManyItemCaptures. This is necessary in flows that require
    /// an IndexedPropertyCapture but one does not exist because the capture was delegated to a ManyOf item, which performs a
    /// second-pass match to derive its items. The RegexPropInfoelement doesn't represent a ManyOf item directly, but rather its parent property.
    /// </summary>
    public IndexedPropertyCapture(ManyItemCapture capture, string fullPathToManyOfItem)
    {
        IsDerivedFromManyItem = true;
        RegexPropInfo = capture.RegexPropInfo;
        Capture = capture.Capture;
        Start = capture.Capture.Index;
        Length = capture.Capture.Length;
        End = Start + Length;
        IsChildToken = RegexPropInfo.RegexPropType == RegexPropType.TokenUnit || RegexPropInfo.RegexPropType == RegexPropType.TokenUnitOneOf;
        Text = capture.Capture.Value;
        Value = capture.ItemObject;
        Ordinal = capture.Oridinal;
        Palette = DeterministicPalette.GetFixedRainbowPalette(Ordinal);
        IgnoreInAnalysis = RegexPropInfo.Prop.DeclaringType.GetCustomAttribute<IgnoreInAnalysisAttribute>() != null;
        Path = fullPathToManyOfItem;
    }

    public override string ToString() => $"Prop: {RegexPropInfo.Name} | Position: {Ordinal} | Capture: \"{Capture.Value}\"";
}