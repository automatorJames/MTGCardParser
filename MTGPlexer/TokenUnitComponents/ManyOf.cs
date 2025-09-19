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
        ItemType = typeof(T);

        ManyItemVariant = 
            typeof(T).IsAssignableTo(typeof(TokenUnit)) ? ManyItemVariant.TokenUnit
            : typeof(T).IsEnum ? ManyItemVariant.Enum
            : throw new Exception($"{nameof(ManyOf)} item type must either be TokenUnit or Enum");
    }
}

public enum Conjunction
{
    And,
    Or
}

[Color("#696969")]
public class ManyOf 
{
    public List<ManyItemCapture> ItemObjects { get; set; }
    public ManyItemVariant ManyItemVariant { get; set; }
    public Type ItemType { get; set; }
    public Conjunction? Conjunction { get; set; }
    public Capture ConjunctionCapture { get; set; }
}

public enum ManyItemVariant
{
    TokenUnit,
    Enum
}