namespace MTGPlexer.TokenUnits;

public class EnchantPermanent : TokenUnit
{
    protected override string[] Snippets => ["enchant", nameof(CardType), nameof(CardOutsideBattlefield)];

    public CardType CardType { get; set; }

    [OptionalComponent]
    public CardOutsideBattlefield CardOutsideBattlefield { get; set; }
}