namespace MTGPlexer.TokenUnits;

public class EnchantCard : TokenUnit
{
    protected override string[] Snippets => ["enchant", nameof(CardType)];

    public CardType CardType { get; set; }
}