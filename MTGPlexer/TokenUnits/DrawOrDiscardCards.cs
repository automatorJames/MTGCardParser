namespace MTGPlexer.TokenUnits;

public class DrawOrDiscardCards : TokenUnit
{
    public DrawOrDiscardCards() : base(nameof(CardVerb), nameof(Quantity), "cards?") { }

    public CardVerb CardVerb { get; set; }
    public Quantity Quantity { get; set; }
}

[EnumOptions(OptionalPlural = true)]
public enum CardVerb
{
    Draw,
    Discard,
}