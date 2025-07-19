namespace MTGPlexer.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class TokenizationOrderAttribute : Attribute
{
    public int Order { get; set; }

    public TokenizationOrderAttribute(int order)
    {
        Order = order;
    }
}
