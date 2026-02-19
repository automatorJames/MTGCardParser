namespace MTGPlexer.TokenUnitPrimitives;

public class DynamicOf<T> : TokenUnit
{
    public object Item { get; set; }

    public DynamicOf(object item)
    {
        Item = item;
    }
}