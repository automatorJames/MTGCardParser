namespace MTGPlexer.TokenUnitComponents;

public class OptionalOf<T> : OptionalOf
{
    public PolyItemCapture<T> Item { get; set; }

    public OptionalOf(PolyItemCapture<T> item)
    {
        Item = item;
        ItemObject = item;
    }

    public override string ToString() => base.ToString();
}

[Color("#696969")]
public class OptionalOf
{
    public PolyItemCapture ItemObject { get; set; }

    public override string ToString() => string.Join(" ", ItemObject.ToString());
}
