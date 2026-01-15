namespace MTGPlexer.TokenUnitComponents;

public record PolyItemCapture<T> : PolyItemCapture
{
    public T Item { get; set; }

    public PolyItemCapture(T item, Capture capture, int ordinal, TemplatePropInfo propInfo) 
        : base(capture, typeof(T), item, ordinal, propInfo)
    {
        Item = item;
    }

    public override string ToString() => base.ToString();
}

public record PolyItemCapture
{
    public Capture Capture { get; }
    public TemplatePropInfo RegexPropInfo { get; }
    public Type ItemType { get; }
    public object ItemObject { get; }
    public CaptureTypeVariant CaptureTypeVariant { get; }

    public PolyItemCapture(Capture capture, Type type, object itemAsObject, int ordinal, TemplatePropInfo propInfo)
    {
        Capture = capture is Group group ? group.Captures[ordinal] : capture;
        RegexPropInfo = propInfo;
        ItemType = type;
        ItemObject = itemAsObject;
        CaptureTypeVariant = type.ToCaptureTypeVariant();
    }

    public override string ToString() => CaptureTypeVariant == CaptureTypeVariant.Enum ?
        ItemObject.ToString()
        : ItemType.Name;
}