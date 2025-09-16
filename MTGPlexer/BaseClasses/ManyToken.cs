namespace MTGPlexer.BaseClasses;

public class ManyToken<T> : ManyToken
{
    public ManyItemCapture<T>[] Items { get; set; }
    

    public ManyToken(IEnumerable<ManyItemCapture<T>> items, Conjunction? conjunction, Capture conjunctionCapture)
    {
        Items = items.ToArray();
        ItemObjects = Items.Cast<ManyItemCapture>().ToList();
        Conjunction = conjunction;
        ConjunctionCapture = conjunctionCapture;

        ManyItemType = 
            typeof(T).IsAssignableTo(typeof(TokenUnit)) ? ManyItemType.TokenUnit
            : typeof(T).IsEnum ? ManyItemType.Enum
            : throw new Exception($"{nameof(ManyToken)} item type must either be TokenUnit or Enum");
    }
}

public enum Conjunction
{
    And,
    Or
}

public class ManyToken 
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