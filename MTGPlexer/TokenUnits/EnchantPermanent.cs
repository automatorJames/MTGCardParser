namespace MTGPlexer.TokenUnits;

[IsolateForTesting]
public class EnchantPermanent : TokenUnit
{
    protected override Snippet[] Snippets => ["enchant", Prop(CardOrCreatureType), Prop(CardOutsideBattlefield)];

    public OneOf<CardType, CreatureType> CardOrCreatureType { get; set; }

    [OptionalComponent]
    public CardOutsideBattlefield CardOutsideBattlefield { get; set; }
}