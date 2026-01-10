namespace MTGPlexer.TokenUnits;

public class EnchantedCard : TokenUnit
{
    protected override Snippet[] Snippets => ["enchanted", Prop(CardType), Prop(PermanentVerb)];

    public CardType CardType { get; set; }
    public PermanentVerb? PermanentVerb { get; set; }
}