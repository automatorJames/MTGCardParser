namespace MTGCardParser.TokenCaptures;

public class EnchantedCard : ITokenCapture
{
    public string RegexTemplate => $@"enchanted §{nameof(CardType)}§";

    public CardType? CardType { get; set; }
}