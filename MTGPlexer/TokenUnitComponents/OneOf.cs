namespace MTGPlexer.TokenUnitComponents;

public class OneOf<T1, T2> : OneOf
{
    public PolyItemCapture<T1> Item1 { get; set; }
    public PolyItemCapture<T2> Item2 { get; set; }

    public OneOf(PolyItemCapture<T1> capture)
    {
        Item1 = capture;
        ItemType = typeof(T1);
        ManyItemVariant = typeof(T1).ToCaptureTypeVariant();
        ItemObject = capture;
    }

    public OneOf(PolyItemCapture<T2> capture)
    {
        Item2 = capture;
        ItemType = typeof(T2);
        ManyItemVariant = typeof(T2).ToCaptureTypeVariant();
        ItemObject = capture;
    }
}

public class OneOf<T1, T2, T3> : OneOf
{
    public PolyItemCapture<T1> Item1 { get; set; }
    public PolyItemCapture<T2> Item2 { get; set; }
    public PolyItemCapture<T3> Item3 { get; set; }

    public OneOf(PolyItemCapture<T1> capture)
    {
        Item1 = capture;
        ItemType = typeof(T1);
        ManyItemVariant = typeof(T1).ToCaptureTypeVariant();
    }

    public OneOf(PolyItemCapture<T2> capture)
    {
        Item2 = capture;
        ItemType = typeof(T2);
        ManyItemVariant = typeof(T2).ToCaptureTypeVariant();
    }

    public OneOf(PolyItemCapture<T3> capture)
    {
        Item3 = capture;
        ItemType = typeof(T3);
        ManyItemVariant = typeof(T3).ToCaptureTypeVariant();
    }
}


[Color("#696969")]
public class OneOf : XOf
{
    public PolyItemCapture ItemObject { get; set; }
    public CaptureTypeVariant ManyItemVariant { get; set; }
    public Type ItemType { get; set; }
}