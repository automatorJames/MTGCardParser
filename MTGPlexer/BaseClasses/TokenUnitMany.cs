using System.Collections;

namespace MTGPlexer.BaseClasses;

public class TokenUnitMany<T> : ITokenUnitMany
{
    public T[] Items { get; set; }
    public Conjunction Conjunction { get; set; }

    public TokenUnitMany(IEnumerable items, Conjunction conjunction)
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

public interface ITokenUnitMany;