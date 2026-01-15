namespace MTGPlexer.TokenUnitComponents;

public record PolyItemCapture<T> : PolyItemCapture
{
    public T Item { get; set; }

    public PolyItemCapture(T item, Capture capture, TemplatePropInfo propInfo) : this(item, capture, propInfo, 0, null)
    {
    }

    public PolyItemCapture(T item, Capture capture, TemplatePropInfo propInfo, int ordinal) : this(item, capture, propInfo, ordinal, null)
    {
    }

    public PolyItemCapture(T item, Capture capture, TemplatePropInfo propInfo, int ordinal, string distinguishingName) 
        : base(typeof(T), item, capture, propInfo, ordinal, distinguishingName)
    {
        Item = item;
    }

    public override string ToString() => base.ToString();
}

public record PolyItemCapture
{
    public Capture Capture { get; }
    public TemplatePropInfo TemplatePropInfo { get; }
    public Type ItemType { get; }
    public object ItemObject { get; }
    public CaptureTypeVariant CaptureTypeVariant { get; }
    public string DistinguishingName { get; }

    public PolyItemCapture(Type type, object itemAsObject, Capture capture, TemplatePropInfo propInfo, int ordinal, string distinguishingName)
    {
        Capture = capture is Group group ? group.Captures[ordinal] : capture;
        TemplatePropInfo = propInfo;
        ItemType = type;
        ItemObject = itemAsObject;
        CaptureTypeVariant = type.ToCaptureTypeVariant();
        DistinguishingName = distinguishingName;
    }

    public override string ToString() => CaptureTypeVariant == CaptureTypeVariant.Enum ?
        ItemObject.ToString()
        : ItemType.Name;
}