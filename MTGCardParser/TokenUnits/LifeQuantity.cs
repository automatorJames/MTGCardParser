namespace MTGCardParser.TokenUnits;

public class LifeQuantity : TokenUnit
{
    public RegexTemplate RegexTemplate => new(nameof(Quantity), "life");

    public Quantity Quantity { get; set; }
}

