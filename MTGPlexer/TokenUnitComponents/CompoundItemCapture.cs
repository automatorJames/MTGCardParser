namespace MTGPlexer.TokenUnitComponents;

public record CompoundItemCapture<T> : CompoundItemCapture
{
    public T Item { get; set; }

    public CompoundItemCapture(T item, Capture capture, int ordinal, RegexPropInfo propInfo) 
        : base(capture, typeof(T), item, ordinal, propInfo)
    {
        Item = item;
    }
}

public record CompoundItemCapture
{
    public Capture Capture { get; }
    public RegexPropInfo RegexPropInfo { get; }
    public Type ItemType { get; }
    public object ItemObject { get; }
    public CaptureTypeVariant CaptureTypeVariant { get; }

    public CompoundItemCapture(Capture capture, Type type, object itemAsObject, int ordinal, RegexPropInfo propInfo)
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