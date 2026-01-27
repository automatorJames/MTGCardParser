namespace MTGPlexer.TokenUnitGraphComponents;

public class ManyOf<T> : ManyOf
{
    public ManyOf(IEnumerable<PolyItemCapture> items, Conjunction? conjunction, ExtractedCapture conjunctionCapture)
    {
        Items = items.ToList();
        Conjunction = conjunction;
        ConjunctionCapture = conjunctionCapture;
        ManyItemVariant = typeof(T).ToCaptureTypeVariant();
    }

    public override string ToString() => base.ToString();
}

[Color("#696969")]
public class ManyOf : XOf
{
    public List<PolyItemCapture> Items { get; set; }
    public CaptureTypeVariant ManyItemVariant { get; set; }
    public Conjunction? Conjunction { get; set; }
    public ExtractedCapture ConjunctionCapture { get; set; }

    public override string ToString()
    {
        var separator = Conjunction switch
        {
            TokenUnitGraphComponents.Conjunction.And => " & ",
            TokenUnitGraphComponents.Conjunction.Or => " | ",
            _ => " & ",
        };

        return string.Join(separator, Items.Select(x => x.ToString()));
    }
}

public enum ManyItemOrdinal
{
    First,
    SecondPlus,
    Last
}

public enum Conjunction
{
    And,
    Or
}