namespace MTGPlexer.TokenUnitPrimitives;

public class DynamicOf<T> : DynamicTokenUnit where T : TokenUnit
{
    public T Item { get; set; }

    public DynamicOf()
    {
    }

    public DynamicOf(object item)
    {
        if (item.GetType().IsAssignableTo(typeof(T)))
            throw new Exception($"Expected type derived from {typeof(T).Name}, but received object of type {item.GetType().Name}");

        Item = (T)item;
    }
}


public class DynamicTokenUnit : TokenUnit
{
    public Type ResolvedType { get; set; }

}