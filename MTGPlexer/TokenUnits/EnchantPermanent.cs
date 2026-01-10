namespace MTGPlexer.TokenUnits;

public class EnchantPermanent : TokenUnit
{
    protected override Snippet[] Snippets => ["enchant", Prop(CardType), Prop(CardOutsideBattlefield)];

    public CardType CardType { get; set; }

    [OptionalComponent]
    public CardOutsideBattlefield CardOutsideBattlefield { get; set; }
}