namespace MTGPlexer.TokenUnits;

[IsolateForTesting]
public class WrapperClass : TokenUnit
//public class TestThing : TokenUnit
{
    //public TestThing() : base("the number of", nameof(CardKeyword), nameof(EnchantCard)) { }
    //public TestThing() : base("buh", nameof(GainedOrLostBuffs)) { }

    //public ManyToken<GainedOrLostBuff> GainedOrLostBuffs { get; set; }
    //public ManyToken<TestItemWrapper> TestItems { get; set; }
    //public CardKeyword CardKeyword { get; set; }
    //public EnchantCard EnchantCard { get; set; }

    public AlternatingClass AlternatingProp { get; set; }

    public Buffalo BuffaloProp { get; set; }
}

public enum Buffalo
{
    Buffalo,
    Wild,
    Wings
}
