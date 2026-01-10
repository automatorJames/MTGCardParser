namespace MTGPlexer.TokenUnits;

[IsolateForTesting]
public class DrawOrDiscardCards : TokenUnit
{
    protected override string[] Snippets => [nameof(CardVerb), nameof(Quantity), "cards?"];

    public CardVerb CardVerb { get; set; }
    public Quantity Quantity { get; set; }
}

[OptionalPlural]
public enum CardVerb
{
    Draw,
    Discard
}