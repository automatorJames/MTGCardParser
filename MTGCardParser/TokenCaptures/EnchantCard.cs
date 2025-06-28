namespace MTGCardParser.TokenCaptures;

public class EnchantCard : TokenCaptureBase<EnchantCard>
{
    public override RegexTemplate<EnchantCard> RegexTemplate => new("enchant", nameof(CardType));

    public CardType? CardType { get; set; }
}