namespace Glyphotype.GlyphPrimitives;

/// <summary>
/// Represents a property on a Glyph type that should be treated as an optional match (i.e. a group with the "?" quantifier).
/// T is constrained to Glyph because Enum type properties can already be expressed as nullable with the "?" operator.
/// </summary>
public class OptionalOf<T> : Glyph where T : Glyph
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