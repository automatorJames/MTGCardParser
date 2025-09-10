namespace MTGPlexer.TokenUnits;

[IsolateForTesting]
//public class TestThing : TokenUnitOneOf
public class TestThing : TokenUnit
{
    //public TestThing() : base("the number of", nameof(CardKeyword), nameof(EnchantCard)) { }
    //public TestThing() : base("buh", nameof(GainedOrLostBuffs)) { }

    //public ManyToken<GainedOrLostBuff> GainedOrLostBuffs { get; set; }
    //public ManyToken<TestItemWrapper> TestItems { get; set; }
    //public CardKeyword CardKeyword { get; set; }
    //public EnchantCard EnchantCard { get; set; }

    public LandType LandType { get; set; }
    public WildCard WildCard { get; set; }
}

public enum WildCard
{
    Lands,
    UntappedLands,
    CardsIn
}

public enum TestItem
{
    A,
    B,
    C
}

public class TestItemWrapper : TokenUnit
{
    public TestItem TestItem { get; set; }
}
