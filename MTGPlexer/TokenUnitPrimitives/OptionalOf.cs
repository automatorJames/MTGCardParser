namespace MTGPlexer.TokenUnitPrimitives;

/// <summary>
/// Represents a property on a TokenUnit type that should be treated as an optional match (i.e. a group with the "?" quantifier).
/// T is constrained to TokenUnit because Enum type properties can already be expressed as nullable with the "?" operator.
/// </summary>
public class OptionalOf<T> : TokenUnit where T : TokenUnit
{
    public object Item { get; set; }

    public OptionalOf(object item)
    {
        Item = item;
    }
}