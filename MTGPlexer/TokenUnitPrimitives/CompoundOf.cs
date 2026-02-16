namespace MTGPlexer.TokenUnitPrimitives;

public class CompoundOf<T> : CompoundOf
{
    public CompoundOf(IEnumerable<object> items)
    {
        Items = items.ToList();
    }

    public override string ToString() => base.ToString();
}

[Color("#696969")]
public class CompoundOf : XOf
{
    public List<object> Items { get; set; }

    public override string ToString() => string.Join(" ", Items.Select(x => x.ToString()));
}
