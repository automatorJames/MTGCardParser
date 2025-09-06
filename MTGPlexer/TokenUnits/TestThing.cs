namespace MTGPlexer.TokenUnits;

[IsolateForTesting]
public class TestThing : TokenUnitOneOf
{
    public TestThing() : base("the number of", nameof(CardKeyword), nameof(EnchantCard)) { }

    public CardKeyword CardKeyword { get; set; }
    public EnchantCard EnchantCard { get; set; }

    //public LandType LandType { get; set; }
    //public WildCard WildCard { get; set; }
}

public enum WildCard
{
    Lands,
    UntappedLands,
    CardsIn
}