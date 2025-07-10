namespace MTGCardParser.TokenUnits;

public class LifeChangeQuantity : TokenUnit
{
    public LifeChangeQuantity() : base(nameof(WhichPlayer), nameof(LifeVerb), nameof(Quantity), "life") { }

    public WhichPlayer WhichPlayer { get; set; }
    public LifeVerb LifeVerb { get; set; }
    public Quantity Quantity { get; set; }
}

public enum LifeVerb
{
    Gain,
    Lose
}

