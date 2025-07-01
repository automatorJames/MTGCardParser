namespace MTGCardParser.TokenCaptures;

public class LifeQuantity : ITokenUnit
{
    public RegexTemplate<LifeQuantity> RegexTemplate => new(nameof(Quantity), "life");

    public Quantity? Quantity { get; set; }
}

