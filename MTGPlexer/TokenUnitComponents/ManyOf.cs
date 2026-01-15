namespace MTGPlexer.TokenUnitComponents;

public class ManyOf<T> : ManyOf
{
    public ManyItemCapture<T>[] Items { get; set; }

    public ManyOf(IEnumerable<ManyItemCapture<T>> items, Conjunction? conjunction, Capture conjunctionCapture)
    {
        Items = items.ToArray();
        ItemObjects = Items.Cast<ManyItemCapture>().ToList();
        Conjunction = conjunction;
        ConjunctionCapture = conjunctionCapture;
        ManyItemVariant = typeof(T).ToCaptureTypeVariant();
    }

    public override string ToString() => base.ToString();

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

[Color("#696969")]
public class ManyOf
{
    public List<ManyItemCapture> ItemObjects { get; set; }
    public CaptureTypeVariant ManyItemVariant { get; set; }
    public Conjunction? Conjunction { get; set; }
    public Capture ConjunctionCapture { get; set; }

    public override string ToString()
    {
        var separator = Conjunction switch
        {
            TokenUnitComponents.Conjunction.And => " & ",
            TokenUnitComponents.Conjunction.Or => " | ",
            _ => " & ",
        };

        return string.Join(separator, ItemObjects.Select(x => x.ToString()));
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