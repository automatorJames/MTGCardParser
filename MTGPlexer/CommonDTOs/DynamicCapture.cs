namespace MTGPlexer.CommonDTOs;

public class DynamicCapture<T> : DynamicCapture
{
    public T Value {  get; }

    public DynamicCapture(T value, Capture capture) : base(value, capture)
    {
        Value = value;
    }

    public override string ToString() => Capture.Value;
}

public class DynamicCapture
{
    public object ValueObject { get; protected set; }
    public Capture Capture { get; set; }

    public DynamicCapture(object valueObject, Capture capture)
    {
        Capture = capture;
    }
}