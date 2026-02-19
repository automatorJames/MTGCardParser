namespace MTGPlexer.TokenUnits;

public class EnchantPermanent : TokenUnit
{
    protected override Snippet[] Snippets => ["enchant", Prop(CardOrCreatureType), Prop(CardOutsideBattlefield)];

    public OneOf<CardType, CreatureType> CardOrCreatureType { get; set; }

    [Optional]
    public CardOutsideBattlefield CardOutsideBattlefield { get; set; }
}