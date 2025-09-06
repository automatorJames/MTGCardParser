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

    public override bool ValidateStructure()
    {
        bool typeIsValidManyType =
            typeof(T).IsAssignableTo(typeof(TokenUnit))
            || typeof(T).IsEnum;

        return typeIsValidManyType;
    }
}

public enum Conjunction
{
    And,
    Or
}

public class ManyToken : TokenUnit 
{
    public Conjunction Conjunction { get; set; }
}