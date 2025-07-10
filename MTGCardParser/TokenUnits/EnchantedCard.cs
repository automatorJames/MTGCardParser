namespace MTGCardParser.TokenUnits;

public class EnchantedCard : TokenUnit
{
    public EnchantedCard() : base("enchanted", nameof(CardType), nameof(PermanentVerb)) { }

    public CardType CardType { get; set; }
    public PermanentVerb? PermanentVerb { get; set; }
}