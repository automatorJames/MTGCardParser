namespace MTGPlexer.TokenUnitComponents;

public record ManyItemCapture<T> : ManyItemCapture
{
    public T Item { get; set; }

    public ManyItemCapture(T item, Capture capture, int captureIndex, ManyItemOrdinal ordinal, RegexPropInfo propInfo) 
        : base(capture, captureIndex, typeof(T), item, ordinal, propInfo)
    {
        Item = item;
    }
}

public record ManyItemCapture
{
    public Capture Capture { get; }
    public ManyItemOrdinal Oridinal { get; }
    public RegexPropInfo RegexPropInfo { get; }
    public Type ItemType { get; }
    public object ItemObject { get; }
    public CaptureTypeVariant CaptureItemVariant { get; }

    public ManyItemCapture(Capture capture, int captureIndex, Type type, object itemAsObject, ManyItemOrdinal ordinal, RegexPropInfo propInfo)
    {
        Capture = capture is Group group ? group.Captures[captureIndex] : capture;
        Oridinal = ordinal;
        RegexPropInfo = propInfo;
        ItemType = type;
        ItemObject = itemAsObject;
        CaptureItemVariant = type.ToCaptureTypeVariant();
    }

    public override string ToString() => CaptureItemVariant == CaptureTypeVariant.Enum ?
        ItemObject.ToString()
        : ItemType.Name;
}