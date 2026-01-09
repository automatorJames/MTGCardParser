namespace MTGPlexer.CommonDTOs;

/// <summary>
/// Represents a property capture from a token, enriched with a stable index
/// for consistent processing (e.g., coloring) and ordered by position.
/// </summary>
public record PropertyCapture
{
    public RegexPropInfo RegexPropInfo { get; private set; }
    public Capture Capture { get; private set; }
    public object Value { get; private set; }
    public CaptureGroupPropPath CaptureGroupPropPath { get; private set; }

    public PropertyCapture()
    {
    }

    public PropertyCapture(RegexPropInfo regexPropInfo, Capture capture, object value, CaptureGroupPropPath parentTokenPath)
    {
        RegexPropInfo = regexPropInfo;
        Capture = capture;
        Value = value;

        CaptureGroupPropPath = regexPropInfo.IsTerminal
            ? new(parentTokenPath.PropPath
                .Dot(regexPropInfo.Name)
                .Dot(value.ToString()))
            : new(parentTokenPath.PropPath);
    }

    /// <summary>
    /// Used for synthesizing IndexedPropertyCaptures from ManyItemCaptures. This is necessary in flows that require
    /// an IndexedPropertyCapture but one does not exist because the capture was delegated to a ManyOf item, which performs a
    /// second-pass match to derive its items. The RegexPropInfo element doesn't represent a ManyOf item directly, but rather its parent property.
    /// </summary>
    public PropertyCapture DeriveForManyOfItem(ManyOf manyOf, ManyItemCapture capture)
    {
        var terminalValueOrTypeName = capture.ManyItemVariant == CaptureTypeVariant.Enum ? capture.ItemObject.ToString() : capture.ItemType.Name;
        var newPath = CaptureGroupPropPath.Append(RegexPropInfo.Name, capture.Oridinal.ToString(), terminalValueOrTypeName);

        return new PropertyCapture
        {
            RegexPropInfo = capture.RegexPropInfo,
            Capture = capture.Capture,
            Value = capture.ItemObject,
            CaptureGroupPropPath = newPath
        };
    }

    public PropertyCapture DeriveForManyOfConjunction(ManyOf manyOf)
    {
        if (manyOf.Conjunction == null)
            return null;

        return new PropertyCapture
        {
            RegexPropInfo = RegexPropInfo.DerviveForManyOfConjunction(),
            Capture = manyOf.ConjunctionCapture,
            Value = manyOf.Conjunction,
            CaptureGroupPropPath = CaptureGroupPropPath.Append(RegexPropInfo.Name, nameof(ManyOf.Conjunction), manyOf.Conjunction.ToString())
        };
    }

    public override string ToString() => $"Prop: {RegexPropInfo.Name} | Capture: \"{Capture.Value}\"";
}