namespace MTGCardParser.TokenCaptures;

public class LifeChangeQuantity : ITokenCapture
{
    public string RegexTemplate => $@"§{nameof(LifeVerb)}§ §{nameof(Quantity)}§ life";

    public LifeVerb LifeVerb { get; set; }
    public Quantity Quantity { get; set; }
}

public enum LifeVerb
{
    [RegPat("gains?")]
    Gain,

    [RegPat("loses?")]
    Lose
}

