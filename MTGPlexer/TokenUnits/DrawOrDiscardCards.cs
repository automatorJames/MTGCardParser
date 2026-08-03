namespace MTGPlexer.TokenUnits;

[IsolateForTesting]
public class DrawOrDiscardCards : TokenUnit
{
    public override Snippet[] Snippets => [Prop(CardVerb), Prop(Quantity), "cards?"];

    public CardVerb CardVerb { get; set; }
    public Quantity Quantity { get; set; }
}

[OptionalPlural]
public enum CardVerb
{
    Draw,

    [RegexPattern("discard", "discard angrily")]
    Discard
}