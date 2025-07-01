namespace MTGCardParser.TokenCaptures;

public class DrawOrDiscardCards : ITokenUnit
{
    public RegexTemplate<DrawOrDiscardCards> RegexTemplate => new(nameof(CardVerb), nameof(Quantity), "cards?");

    public CardVerb? CardVerb { get; set; }
    public Quantity? Quantity { get; set; }
}

[EnumOptions(OptionalPlural = true)]
public enum CardVerb
{
    Draw,
    Discard,
}