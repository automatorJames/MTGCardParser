namespace MTGPlexer.CommonDTOs;

public class DynamicCapture<T> : DynamicCapture
{
    public T Value {  get; }

    public DynamicCapture(T value, Capture capture) : base(value, capture, typeof(T))
    {
        Value = value;
    }

    public override string ToString() => Capture.Value;
}

[Color("#696969")]
public class DynamicCapture
{
    public object ValueObject { get; protected set; }
    public Capture Capture { get; set; }
    public RegexPropType RegexPropType { get; set; }

    public DynamicCapture(object valueObject, Capture capture, Type itemType)
    {
        Capture = capture;
        ValueObject = valueObject;
        RegexPropType = itemType.GetRegexPropType();
    }
}