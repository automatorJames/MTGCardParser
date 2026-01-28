namespace MTGPlexer.CommonDTOs;

public record PolyItemCapture
{
    public ExtractedCapture Capture { get; }
    public TemplatePropInfo TemplatePropInfo { get; }
    public Type Type { get; }
    public object Value { get; }
    public CaptureTypeVariant CaptureTypeVariant { get; }
    public object DistinguishingValue { get; }

    public PolyItemCapture(
        object value, 
        ExtractedCapture capture, 
        TemplatePropInfo propInfo, 
        object distinguishingValue = null)
    {
        Capture = capture;
        TemplatePropInfo = propInfo;
        this.Value = value;
        Type = value.GetType();
        CaptureTypeVariant = Type.ToCaptureTypeVariant();
        DistinguishingValue = distinguishingValue;
    }

    public override string ToString() => CaptureTypeVariant == CaptureTypeVariant.Enum ?
        Value.ToString()
        : Type.Name;
}