namespace MTGPlexer.TokenUnits;

public class LifeQuantity : TokenUnit
{
    public override Snippet[] Snippets => [Prop(Quantity), "life"];

    public Quantity Quantity { get; set; }
}