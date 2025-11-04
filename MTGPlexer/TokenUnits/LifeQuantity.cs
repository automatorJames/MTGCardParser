namespace MTGPlexer.TokenUnits;

public class LifeQuantity : TokenUnit
{
    protected override string[] Snippets => [nameof(Quantity), "life"];

    public Quantity Quantity { get; set; }
}