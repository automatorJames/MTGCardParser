namespace MTGPlexer.TokenUnits;

public class LifeQuantity : TokenUnit
{
    protected override Snippet[] Snippets => [Prop(Quantity), "life"];

    public Quantity Quantity { get; set; }
}