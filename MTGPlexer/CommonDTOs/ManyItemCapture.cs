namespace MTGPlexer.CommonDTOs;

public record ManyItemCapture<T> : ManyItemCapture
{
    public T Item { get; set; }

    public ManyItemCapture(T item, Capture capture) : base(capture, typeof(T), item)
    {
        Item = item;
    }
}

public record ManyItemCapture
{
    public Capture Capture { get; }
    public Type Type { get; }
    public object ItemAsObject { get; }

    public ManyItemCapture(Capture capture, Type type, object itemAsObject)
    {
        Capture = capture;
        Type = type;
        ItemAsObject = itemAsObject;
    }
}
