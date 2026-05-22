namespace MTGPlexer.TokenUnitPrimitives;

/// <summary>
/// Represents a property on a TokenUnit type that should be treated as an optional match (i.e. a group with the "?" quantifier).
/// T is constrained to TokenUnit because Enum type properties can already be expressed as nullable with the "?" operator.
/// </summary>
public class OptionalOf<T> : TokenUnit where T : TokenUnit
{
    public T Item { get; set; }

    public OptionalOf()
    {
    }

    public OptionalOf(object item)
    {
        if (item.GetType() is not T)
            throw new Exception($"Expected type {typeof(T).Name}, but received object of type {item.GetType().Name}");

        Item = (T)item;
    }
}