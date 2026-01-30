namespace MTGPlexer.CommonDTOs;

public record PolyItemCapture
{
    public ExtractedCapture Capture { get; }
    public Type Type { get; }
    public object Value { get; }
    public CaptureTypeVariant CaptureTypeVariant { get; }
    public object DistinguishingValue { get; }

    public PolyItemCapture(
        object value, 
        ExtractedCapture capture, 
        object distinguishingValue = null)
    {
        Capture = capture;
        this.Value = value;
        Type = value.GetType();
        CaptureTypeVariant = Type.ToCaptureTypeVariant();
        DistinguishingValue = distinguishingValue;
    }

    public override string ToString() => CaptureTypeVariant == CaptureTypeVariant.Enum ?
        Value.ToString()
        : Type.Name;
}