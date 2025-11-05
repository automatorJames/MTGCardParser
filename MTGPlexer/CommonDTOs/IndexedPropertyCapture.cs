namespace MTGPlexer.CommonDTOs;

/// <summary>
/// Represents a property capture from a token, enriched with a stable index
/// for consistent processing (e.g., coloring) and ordered by position.
/// </summary>
public record IndexedPropertyCapture
{
    public RegexPropInfo RegexPropInfo { get; private set; }
    public Capture Capture { get; private set; }
    public int Start { get; private set; }
    public int End { get; private set; }
    public int Length { get; private set; }
    public bool IsChildToken { get; private set; }
    public string Text { get; private set; }
    public object Value { get; private set; }
    public object ParentValue { get; private set; }
    public int Ordinal { get; private set; }
    public Palette Palette { get; private set; }
    public bool IgnoreInAnalysis { get; private set; }
    public bool IsDerivedFromManyItem { get; private set; }
    public string Path { get; private set; }
    public CaptureGroupPropPath CaptureGroupPropPath { get; private set; }

    public IndexedPropertyCapture()
    {
    }

    public IndexedPropertyCapture(RegexPropInfo regexPropInfo, Capture capture, object value, int capturePosition, CaptureGroupPropPath parentTokenPath, string distinguishingAppendix = null)
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
        Path = parentTokenPath.PropPath.Dot(regexPropInfo.Name);

        CaptureGroupPropPath = regexPropInfo.IsTerminal 
            ? distinguishingAppendix is not null 
                ? new(parentTokenPath + "." + regexPropInfo.Name + distinguishingAppendix + "." + value.ToString())
                : new(parentTokenPath.PropPath.Dot(regexPropInfo.Name).Dot(value.ToString()))
            : new(parentTokenPath.PropPath);

        if (CaptureGroupPropPath.PropPath.Contains("TargetGainsOrLosesBuff_Many.GainedOrLostBuff_Many.GainedOrLostBuff_Many_last.Buff_last.PowerToughnessModification_last.PowerToughnessModification_last.ToughnessValue_last")) Debugger.Break();
    }

    /// <summary>
    /// Used for synthesizing IndexedPropertyCaptures from ManyItemCaptures. This is necessary in flows that require
    /// an IndexedPropertyCapture but one does not exist because the capture was delegated to a ManyOf item, which performs a
    /// second-pass match to derive its items. The RegexPropInfo element doesn't represent a ManyOf item directly, but rather its parent property.
    /// </summary>
    public IndexedPropertyCapture DeriveForManyOfItem(ManyOf manyOf, ManyItemCapture capture)
    {
        var newCaptureGroupPropPath =
            Path
            + "."
            + RegexPropInfo.Name
            + capture.Oridinal.Description()
            + "."
            + (capture.ManyItemVariant == ManyItemVariant.Enum ? capture.ItemObject.ToString() : "");

        return new IndexedPropertyCapture
        {
            IsDerivedFromManyItem = true,
            RegexPropInfo = capture.RegexPropInfo,
            Capture = capture.Capture,
            Start = capture.Capture.Index,
            Length = capture.Capture.Length,
            End = capture.Capture.Index + capture.Capture.Length,
            IsChildToken = IsChildToken,
            Text = capture.Capture.Value,
            Value = capture.ItemObject,
            ParentValue = manyOf,
            Ordinal = (int)capture.Oridinal,
            Palette = DeterministicPalette.GetFixedRainbowPalette((int)capture.Oridinal),
            IgnoreInAnalysis = false,
            Path = Path.Dot(capture.Oridinal.ToString()),
            CaptureGroupPropPath = new(newCaptureGroupPropPath)
        };
    }

    public IndexedPropertyCapture DeriveForManyOfConjunction(ManyOf manyOf)
    {
        if (manyOf.Conjunction == null)
            return null;

        return new IndexedPropertyCapture
        {
            IsDerivedFromManyItem = true,
            RegexPropInfo = RegexPropInfo.DerviveForManyOfConjunction(),
            Capture = manyOf.ConjunctionCapture,
            Start = manyOf.ConjunctionCapture.Index,
            Length = manyOf.ConjunctionCapture.Length,
            End = manyOf.ConjunctionCapture.Index + manyOf.ConjunctionCapture.Length,
            IsChildToken = RegexPropInfo.RegexPropType == RegexPropType.TokenUnit || RegexPropInfo.RegexPropType == RegexPropType.TokenUnitOneOf,
            Text = manyOf.ConjunctionCapture.Value,
            Value = manyOf.Conjunction,
            ParentValue = manyOf,
            Ordinal = 0,
            Palette = DeterministicPalette.GetFixedRainbowPalette(0),
            IgnoreInAnalysis = false,
            //Path = Path.Dot(capture.Oridinal.ToString()),
            CaptureGroupPropPath = new(Path.Dot(nameof(ManyOf.Conjunction)).Dot(manyOf.Conjunction.ToString()))
        };
    }

    public override string ToString() => $"Prop: {RegexPropInfo.Name} | Position: {Ordinal} | Capture: \"{Capture.Value}\"";
}