namespace MTGPlexer.TokenUnitPrimitives;

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
