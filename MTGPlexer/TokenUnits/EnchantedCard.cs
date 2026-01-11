namespace MTGPlexer.TokenUnits;

public class EnchantedCard : TokenUnit
{
    protected override Snippet[] Snippets => ["enchanted", Prop(CardType), Prop(PermanentVerb), Prop(Buff)];

    public CardType CardType { get; set; }
    public PermanentVerb? PermanentVerb { get; set; }
    public Buff Buff { get; set; }
}