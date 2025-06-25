namespace MTGCardParser.TokenCaptures;

public class DrawOrDiscardCards : ITokenCapture
{
    public string RegexTemplate => $@"§{nameof(CardVerb)}§ §{nameof(Quantity)}§ cards?";

    public CardVerb? CardVerb { get; set; }
    public Quantity? Quantity { get; set; }
}

[RegOpt(OptionalPlural = true)]
public enum CardVerb
{
    Draw,
    Discard,
}