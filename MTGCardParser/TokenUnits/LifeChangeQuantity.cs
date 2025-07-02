namespace MTGCardParser.TokenUnits;

public class LifeChangeQuantity : ITokenUnit
{
    public RegexTemplate<LifeChangeQuantity> RegexTemplate => new(nameof(WhichPlayer), nameof(LifeVerb), nameof(Quantity), "life");

    public WhichPlayer? WhichPlayer { get; set; }
    public LifeVerb? LifeVerb { get; set; }
    public Quantity? Quantity { get; set; }
}

public enum LifeVerb
{
    Gain,
    Lose
}

