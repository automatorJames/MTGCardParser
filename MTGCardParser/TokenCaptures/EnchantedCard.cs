namespace MTGCardParser.TokenCaptures;

public class EnchantedCard : ITokenUnit
{
    public RegexTemplate<EnchantedCard> RegexTemplate => new("enchanted", nameof(CardType), nameof(PermanentVerb));

    public CardType? CardType { get; set; }
    public PermanentVerb? PermanentVerb { get; set; }
}