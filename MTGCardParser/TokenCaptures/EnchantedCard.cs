namespace MTGCardParser.TokenCaptures;

public class EnchantedCard : ITokenCapture
{
    public RegexTemplate<EnchantCard> RegexTemplate => new("enchanted", nameof(CardType));

    public CardType? CardType { get; set; }
}