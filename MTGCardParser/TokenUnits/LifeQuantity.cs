namespace MTGCardParser.TokenUnits;

public class LifeQuantity : TokenUnitBase
{
    public RegexTemplate<LifeQuantity> RegexTemplate => new(nameof(Quantity), "life");

    public Quantity? Quantity { get; set; }
}

