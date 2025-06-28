namespace MTGCardParser.TokenCaptures;

public class LifeChangeQuantity : TokenCaptureBase<LifeChangeQuantity>
{
    public override RegexTemplate<LifeChangeQuantity> RegexTemplate => new(nameof(LifeVerb), nameof(Quantity), "life");

    public LifeVerb? LifeVerb { get; set; }
    public Quantity? Quantity { get; set; }
}

public enum LifeVerb
{
    Gain,
    Lose
}

