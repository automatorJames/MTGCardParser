namespace MTGCardParser.TokenCaptures;

public class EnchantCard : ITokenCapture
{
    public static string RegexTemplate => $@"enchant §{nameof(CardType)}§";

    public CardType? CardType { get; set; }
}