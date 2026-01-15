namespace MTGPlexer.TokenUnitComponents;

public class CompoundOf<T> : CompoundOf
{
    public CompoundOf(IEnumerable<PolyItemCapture> items)
    {
        Items = items.ToList();
        CaptureTypeVariant = typeof(T).ToCaptureTypeVariant();
    }

    public override string ToString() => base.ToString();
}

[Color("#696969")]
public class CompoundOf : XOf
{
    public List<PolyItemCapture> Items { get; set; }
    public CaptureTypeVariant CaptureTypeVariant { get; set; }

    public override string ToString() => string.Join(" ", Items.Select(x => x.ToString()));
}
