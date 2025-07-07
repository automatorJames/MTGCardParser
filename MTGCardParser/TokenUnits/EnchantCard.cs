namespace MTGCardParser.TokenUnits;

public class EnchantCard : TokenUnit
{
    public RegexTemplate<EnchantCard> RegexTemplate => new("enchant", nameof(CardType));

    public CardType? CardType { get; set; }
}