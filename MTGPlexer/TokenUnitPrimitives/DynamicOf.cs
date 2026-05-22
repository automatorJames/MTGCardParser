namespace MTGPlexer.TokenUnitPrimitives;

public class DynamicOf<T> : TokenUnit
{
    public T Item { get; set; }

    public DynamicOf()
    {
    }

    public DynamicOf(object item)
    {
        if (item.GetType() is not T)
            throw new Exception($"Expected type {typeof(T).Name}, but received object of type {item.GetType().Name}");

        Item = (T)item;
    }
}