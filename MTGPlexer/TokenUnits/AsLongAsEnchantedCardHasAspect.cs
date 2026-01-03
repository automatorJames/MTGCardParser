namespace MTGPlexer.TokenUnits;

[IsolateForTesting]
public class AsLongAsEnchantedCardHasAspect() : TokenUnit
{
    protected override string[] Snippets => ["as long as enchanted", nameof(CardType), nameof(Assertion), "(an? )?", nameof(CardAspect), ",", nameof(PermanentVerb), nameof(Buff)];

    public CardType CardType { get; set; }
    public Assertion Assertion { get; set; }
    public CardAspect CardAspect { get; set; }
    public PermanentVerb PermanentVerb { get; set; }
    public Buff Buff { get; set; }
}

[Dependent]
public class CardAspect() : TokenUnitOneOf
{
    public CardType CardType { get; set; }
    public ManaColor ManaColor { get; set; }
}

public enum Assertion
{
    Is,

    [RegexPattern("isn't")]
    Isnt
}