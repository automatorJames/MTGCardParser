namespace MTGPlexer.TokenUnitComponents;

public record PolyItemCapture
{
    public ExtractedCapture Capture { get; }
    public TemplatePropInfo TemplatePropInfo { get; }
    public Type Type { get; }
    public object Value { get; }
    public CaptureTypeVariant CaptureTypeVariant { get; }
    public string DistinguishingName { get; }

    public PolyItemCapture(object value, ExtractedCapture capture, TemplatePropInfo propInfo, string distinguishingName = null)
    {
        Capture = capture;
        TemplatePropInfo = propInfo;
        this.Value = value;
        Type = value.GetType();
        CaptureTypeVariant = Type.ToCaptureTypeVariant();
        DistinguishingName = distinguishingName;
    }

    public override string ToString() => CaptureTypeVariant == CaptureTypeVariant.Enum ?
        Value.ToString()
        : Type.Name;
}