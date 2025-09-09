using System.Collections;

namespace MTGPlexer.BaseClasses;

public class ManyToken<T> : ManyToken
{
    public T[] Items { get; set; }
    

    public ManyToken(IEnumerable items, Conjunction conjunction)
    {
        Items = items.Cast<T>().ToArray();
        Conjunction = conjunction;
    }
}

public enum Conjunction
{
    And,
    Or
}

public class ManyToken 
{
    public Conjunction Conjunction { get; set; }
}