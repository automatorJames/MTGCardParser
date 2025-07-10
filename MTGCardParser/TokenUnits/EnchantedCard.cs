namespace MTGCardParser.TokenUnits;

public class EnchantedCard : TokenUnit
{
    public RegexTemplate<EnchantedCard> RegexTemplate => new("enchanted", nameof(CardType), nameof(PermanentVerb));

    public CardType CardType { get; set; }
    public PermanentVerb? PermanentVerb { get; set; }
}