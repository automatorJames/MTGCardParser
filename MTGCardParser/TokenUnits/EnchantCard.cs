using MTGCardParser.TokenUnits.Interfaces;

namespace MTGCardParser.TokenUnits;

public class EnchantCard : ITokenUnit
{
    public RegexTemplate<EnchantCard> RegexTemplate => new("enchant", nameof(CardType));

    public CardType? CardType { get; set; }
}