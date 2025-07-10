namespace MTGCardParser.TokenUnits;

public class EnchantCard : TokenUnit
{
    public EnchantCard() : base("enchant", nameof(CardType)) { }

    public CardType CardType { get; set; }
}