namespace MTGPlexer.TokenUnits;

public class AsLongAsEnchantedCardHasAspect : TokenUnit
{
    protected override Snippet[] Snippets => ["as long as enchanted", Prop(CardType), Prop(Assertion), "(an? )?", Prop(CardAspect), ",", Prop(PermanentVerb), Prop(Buff)];

    public CardType CardType { get; set; }
    public Assertion Assertion { get; set; }
    public CardAspect CardAspect { get; set; }
    public PermanentVerb PermanentVerb { get; set; }
    public Buff Buff { get; set; }
}