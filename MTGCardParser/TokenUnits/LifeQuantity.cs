namespace MTGCardParser.TokenUnits;

public class LifeQuantity : TokenUnit
{
    public RegexTemplate<LifeQuantity> RegexTemplate => new(nameof(Quantity), "life");

    public Quantity Quantity { get; set; }
}

