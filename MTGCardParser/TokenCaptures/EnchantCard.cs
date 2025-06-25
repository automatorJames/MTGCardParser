namespace MTGCardParser.TokenCaptures;

public class EnchantCard : ITokenCapture
{
    public string RegexTemplate => $@"enchant §{nameof(CardType)}§";

    public CardType? CardType { get; set; }
}