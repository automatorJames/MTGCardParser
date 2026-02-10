namespace MTGPlexer.TokenUnitGraphComponents;

public class ManyOf<T> : ManyOf
{
    public ManyOf(IEnumerable<object> items, Conjunction? conjunction)
    {
        Items = items.ToList();
        Conjunction = conjunction;
    }

    public override string ToString() => base.ToString();
}

[Color("#696969")]
public class ManyOf : XOf
{
    public List<object> Items { get; set; }
    public Conjunction? Conjunction { get; set; }

    public override string ToString()
    {
        var separator = Conjunction switch
        {
            MTGPlexer.Conjunction.And => " & ",
            MTGPlexer.Conjunction.Or => " | ",
            _ => " & ",
        };

        return string.Join(separator, Items.Select(x => x.ToString()));
    }
}