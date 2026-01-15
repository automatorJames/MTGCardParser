namespace MTGPlexer.TokenUnitComponents;

public class DynamicOf<T> : DynamicOf
{
    public T Value {  get; }

    public DynamicOf(T value, Capture capture) : base(value, capture, typeof(T))
    {
        Value = value;
    }

    public override string ToString() => Capture.Value;
}

[Color("#696969")]
public abstract class DynamicOf
{
    public object ValueObject { get; protected set; }
    public Capture Capture { get; set; }
    public RegexPropType RegexPropType { get; set; }

    public DynamicOf(object valueObject, Capture capture, Type itemType)
    {
        Capture = capture;
        ValueObject = valueObject;
        RegexPropType = RegexPropInfo.GetRegexPropType(itemType);
    }
}