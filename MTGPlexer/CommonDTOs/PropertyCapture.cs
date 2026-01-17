namespace MTGPlexer.CommonDTOs;

/// <summary>
/// Represents a captured value for property on a TokenUnitType.
/// </summary>
public record PropertyCapture
{
    public TemplatePropInfo TemplatePropInfo { get; private set; }
    public ExtractedCapture Capture { get; private set; }
    public object Value { get; private set; }
    public CaptureGroupPropPath CaptureGroupPropPath { get; private set; }

    public PropertyCapture()
    {
    }

    public PropertyCapture(TemplatePropInfo templatePropInfo, ExtractedCapture capture, object value, CaptureGroupPropPath parentTokenPath)
    {
        TemplatePropInfo = templatePropInfo;
        Capture = capture;
        Value = value;

        CaptureGroupPropPath = templatePropInfo.IsTerminal
            ? new(parentTokenPath.PropPath
                .Dot(templatePropInfo.Name)
                .Dot(value.ToString()))
            : new(parentTokenPath.PropPath);
    }

    /// <summary>
    /// Used for synthesizing IndexedPropertyCaptures from ManyItemCaptures. This is necessary in flows that require
    /// an IndexedPropertyCapture but one does not exist because the capture was delegated to a ManyOf item, which performs a
    /// second-pass match to derive its items. The TemplatePropInfo element doesn't represent a ManyOf item directly, but rather its parent property.
    /// </summary>
    public PropertyCapture DeriveForManyOfItem(ManyOf manyOf, PolyItemCapture capture)
    {
        var terminalValueOrTypeName = capture.CaptureTypeVariant == CaptureTypeVariant.Enum ? capture.Value.ToString() : capture.Type.Name;
        var newPath = CaptureGroupPropPath.Append(TemplatePropInfo.Name, capture.DistinguishingName, terminalValueOrTypeName);

        return new PropertyCapture
        {
            TemplatePropInfo = capture.TemplatePropInfo,
            Capture = capture.Capture,
            Value = capture.Value,
            CaptureGroupPropPath = newPath
        };
    }

    public PropertyCapture DeriveForManyOfConjunction(ManyOf manyOf)
    {
        if (manyOf.Conjunction == null)
            return null;

        return new PropertyCapture
        {
            TemplatePropInfo = TemplatePropInfo.DeriveForManyOfConjunction(),
            Capture = manyOf.ConjunctionCapture,
            Value = manyOf.Conjunction,
            CaptureGroupPropPath = CaptureGroupPropPath.Append(TemplatePropInfo.Name, nameof(ManyOf.Conjunction), manyOf.Conjunction.ToString())
        };
    }

    public PropertyCapture DeriveForCompoundOfItem(CompoundOf compoundOf, PolyItemCapture capture)
    {
        var terminalValueOrTypeName = capture.CaptureTypeVariant == CaptureTypeVariant.Enum ? capture.Value.ToString() : capture.Type.Name;
        var newPath = CaptureGroupPropPath.Append(TemplatePropInfo.Name, terminalValueOrTypeName);

        return new PropertyCapture
        {
            TemplatePropInfo = capture.TemplatePropInfo,
            Capture = capture.Capture,
            Value = capture.Value,
            CaptureGroupPropPath = newPath
        };
    }

    public PropertyCapture DeriveForOneOfItem(OneOf compoundOf, PolyItemCapture capture)
    {
        var terminalValueOrTypeName = capture.CaptureTypeVariant == CaptureTypeVariant.Enum ? capture.Value.ToString() : capture.Type.Name;
        var newPath = CaptureGroupPropPath.Append(TemplatePropInfo.Name, capture.Type.Name, terminalValueOrTypeName);

        return new PropertyCapture
        {
            TemplatePropInfo = capture.TemplatePropInfo,
            Capture = capture.Capture,
            Value = capture.Value,
            CaptureGroupPropPath = newPath
        };
    }

    public override string ToString() => $"Prop: {TemplatePropInfo.Name} | Capture: \"{Capture.Value}\"";
}