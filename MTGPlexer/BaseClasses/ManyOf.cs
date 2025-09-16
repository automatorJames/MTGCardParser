namespace MTGPlexer.BaseClasses;

public class ManyOf<T> : ManyOf
{
    public ManyItemCapture<T>[] Items { get; set; }
    

    public ManyOf(IEnumerable<ManyItemCapture<T>> items, Conjunction? conjunction, Capture conjunctionCapture)
    {
        Items = items.ToArray();
        ItemObjects = Items.Cast<ManyItemCapture>().ToList();
        Conjunction = conjunction;
        ConjunctionCapture = conjunctionCapture;

        ManyItemType = 
            typeof(T).IsAssignableTo(typeof(TokenUnit)) ? ManyItemType.TokenUnit
            : typeof(T).IsEnum ? ManyItemType.Enum
            : throw new Exception($"{nameof(ManyOf)} item type must either be TokenUnit or Enum");
    }
}

public enum Conjunction
{
    And,
    Or
}

public class ManyOf 
{
    public List<ManyItemCapture> ItemObjects { get; set; }
    public ManyItemType ManyItemType { get; set; }
    public Conjunction? Conjunction { get; set; }
    public Capture ConjunctionCapture { get; set; }
}

public enum ManyItemType
{
    TokenUnit,
    Enum
}