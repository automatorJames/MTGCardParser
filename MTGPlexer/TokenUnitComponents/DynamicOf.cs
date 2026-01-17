namespace MTGPlexer.TokenUnitComponents;

public class DynamicOf<T> : DynamicOf
{
    public DynamicOf(PolyItemCapture item, ExtractedCapture capture)
    {
        Item = item;
        Capture = capture;
    }

    public override string ToString() => Capture.Value;
}

[Color("#696969")]
public class DynamicOf : XOf
{
    public PolyItemCapture Item { get; protected set; }
    public ExtractedCapture Capture { get; set; }
}