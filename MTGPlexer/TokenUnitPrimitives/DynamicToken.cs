namespace MTGPlexer.TokenUnitPrimitives;

[Dependent]
public class DynamicToken : TokenUnit
{
    public override Joiner Joiner => Joiner.Pipe;

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
