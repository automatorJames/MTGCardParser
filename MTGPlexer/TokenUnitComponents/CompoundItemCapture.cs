namespace MTGPlexer.TokenUnitComponents;

public record CompoundItemCapture<T> : CompoundItemCapture
{
    public T Item { get; set; }

    public CompoundItemCapture(T item, Capture capture, int captureIndex, int ordinal, RegexPropInfo propInfo) 
        : base(capture, captureIndex, typeof(T), item, ordinal, propInfo)
    {
        Item = item;
    }
}

public record CompoundItemCapture
{
    public Capture Capture { get; }
    public int Oridinal { get; }
    public RegexPropInfo RegexPropInfo { get; }
    public Type ItemType { get; }
    public object ItemObject { get; }
    public CaptureTypeVariant CaptureTypeVariant { get; }

    public CompoundItemCapture(Capture capture, int captureIndex, Type type, object itemAsObject, int ordinal, RegexPropInfo propInfo)
    {
        Capture = capture is Group group ? group.Captures[captureIndex] : capture;
        Oridinal = ordinal;
        RegexPropInfo = propInfo;
        ItemType = type;
        ItemObject = itemAsObject;
        CaptureTypeVariant = type.ToCaptureTypeVariant();
    }

    public override string ToString() => CaptureTypeVariant == CaptureTypeVariant.Enum ?
        ItemObject.ToString()
        : ItemType.Name;
}