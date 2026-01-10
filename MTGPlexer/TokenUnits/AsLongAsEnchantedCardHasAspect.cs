namespace MTGPlexer.TokenUnits;

public class AsLongAsEnchantedCardHasAspect() : TokenUnit
{
    protected override string[] Snippets => ["as long as enchanted", nameof(CardType), nameof(Assertion), "(an? )?", nameof(CardAspect), ",", nameof(PermanentVerb), nameof(Buff), nameof(WithPowerToughnessEqual)];

    public CardType CardType { get; set; }
    public Assertion Assertion { get; set; }
    public CardAspect CardAspect { get; set; }
    public PermanentVerb PermanentVerb { get; set; }
    public Buff Buff { get; set; }
    public WithPowerToughnessEqual WithPowerToughnessEqual { get; set; }
}