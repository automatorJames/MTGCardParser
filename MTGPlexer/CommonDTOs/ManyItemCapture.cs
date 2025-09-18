namespace MTGPlexer.CommonDTOs;

public record ManyItemCapture<T> : ManyItemCapture
{
    public T Item { get; set; }

    public ManyItemCapture(T item, Capture capture, int ordinal, RegexPropInfo propInfo) : base(capture, typeof(T), item, ordinal, propInfo)
    {
        Item = item;
    }
}

public record ManyItemCapture
{
    public Capture Capture { get; }
    public int Oridinal { get; }
    public RegexPropInfo RegexPropInfo { get; }
    public Type ItemType { get; }
    public object ItemObject { get; }

    public ManyItemCapture(Capture capture, Type type, object itemAsObject, int ordinal, RegexPropInfo propInfo)
    {
        Capture = capture;
        Oridinal = ordinal;
        RegexPropInfo = propInfo;
        ItemType = type;
        ItemObject = itemAsObject;
    }
}
