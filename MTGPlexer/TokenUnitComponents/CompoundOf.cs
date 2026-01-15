namespace MTGPlexer.TokenUnitComponents;

public class CompoundOf<T> : CompoundOf
{
    public PolyItemCapture<T>[] Items { get; set; }

    public CompoundOf(IEnumerable<PolyItemCapture<T>> items)
    {
        Items = items.ToArray();
        ItemObjects = Items.Cast<PolyItemCapture>().ToList();
        CaptureTypeVariant = typeof(T).ToCaptureTypeVariant();
    }

    public override string ToString() => base.ToString();
}

[Color("#696969")]
public class CompoundOf : XOf
{
    public List<PolyItemCapture> ItemObjects { get; set; }
    public CaptureTypeVariant CaptureTypeVariant { get; set; }

    public override string ToString() => string.Join(" ", ItemObjects.Select(x => x.ToString()));
}
