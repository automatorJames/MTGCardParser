namespace MTGCardParser.TokenCaptures;

public class EnchantedCard : TokenCaptureBase<EnchantCard>
{
    public override RegexTemplate<EnchantCard> RegexTemplate => new("enchanted", nameof(CardType));

    public CardType? CardType { get; set; }
}