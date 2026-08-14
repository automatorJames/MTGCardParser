namespace MTGPlexer.TokenUnitPrimitives;

[Dependent]
public class DynamicToken : TokenUnit
{
    public object Item { get; }
    public Type ResolvedType { get; }

    public DynamicToken()
    {
    }

    public DynamicToken(object item)
    {
        Item = item;
        ResolvedType = item.GetType();
    }
}
