namespace MTGPlexer.TokenUnitGraphComponents;

public class OneOf<T1, T2> : OneOf
{
    public PolyItemCapture Item1 { get; set; }
    public PolyItemCapture Item2 { get; set; }

    public OneOf(PolyItemCapture capture, int capturePropOrdinal)
    {
        var capturedItemType = GetType().GetGenericArguments()[capturePropOrdinal];
        var propToSet = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)[capturePropOrdinal];
        propToSet.SetValue(this, capture);
        ItemType = propToSet.PropertyType;
        ManyItemVariant = capturedItemType.ToCaptureTypeVariant();
        Item = capture;
    }
}

public class OneOf<T1, T2, T3> : OneOf
{
    public PolyItemCapture Item1 { get; set; }
    public PolyItemCapture Item2 { get; set; }
    public PolyItemCapture Item3 { get; set; }

    public OneOf(PolyItemCapture capture, int capturedItemTypeOrdinal)
    {
        var capturedItemType = GetType().GetGenericArguments()[capturedItemTypeOrdinal];
        var propToSet = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)[capturedItemTypeOrdinal];
        propToSet.SetValue(this, capture);
        ItemType = propToSet.PropertyType;
        ManyItemVariant = capturedItemType.ToCaptureTypeVariant();
        Item = capture;
    }
}


[Color("#696969")]
public class OneOf : XOf
{
    public PolyItemCapture Item { get; set; }
    public CaptureTypeVariant ManyItemVariant { get; set; }
    public Type ItemType { get; set; }
}