namespace MTGPlexer.TokenUnitPrimitives;

[Dependent]
public class DynamicToken : TokenUnit
{
    public object Item { get; set; }

    public DynamicToken()
    {
    }

    public DynamicToken(object item)
    {
        Item = item;
    }
}
