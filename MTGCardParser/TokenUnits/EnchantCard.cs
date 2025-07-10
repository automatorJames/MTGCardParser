namespace MTGCardParser.TokenUnits;

public class EnchantCard : TokenUnit
{
    public RegexTemplate RegexTemplate => new("enchant", nameof(CardType));

    public CardType CardType { get; set; }
}