namespace MTGPlexer.TokenUnitGraphComponents;

/// <summary>
/// Represents a property on a TokenUnit type that should be treated as an optional match (i.e. a group with the "?" quantifier).
/// T is constrained to TokenUnit because Enum type properties can already be expressed as nullable with the "?" operator.
/// </summary>
public class OptionalOf<T> : OptionalOf where T : TokenUnit
{

    public OptionalOf(object item)
    {
        Item = item;
    }

    public override string ToString() => base.ToString();
}

[Color("#696969")]
public class OptionalOf : XOf
{
    public object Item { get; set; }

    public override string ToString() => string.Join(" ", Item.ToString());
}
