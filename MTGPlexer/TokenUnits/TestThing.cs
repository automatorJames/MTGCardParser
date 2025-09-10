namespace MTGPlexer.TokenUnits;

[IsolateForTesting]
//public class AlternatingClass : TokenUnitOneOf
public class TestThing : TokenUnit
{
    //public TestThing() : base("the number of", nameof(CardKeyword), nameof(EnchantCard)) { }
    //public TestThing() : base("buh", nameof(GainedOrLostBuffs)) { }

    //public ManyToken<GainedOrLostBuff> GainedOrLostBuffs { get; set; }
    public ManyToken<AlphaBetWrapper> Letters { get; set; }
    //public CardKeyword CardKeyword { get; set; }
    //public EnchantCard EnchantCard { get; set; }

    //public LandType LandTypeProp { get; set; }
    //public WildCard WildCardProp { get; set; }
}

public enum WildCard
{
    Lands,
    UntappedLands,
    CardsIn
}

public enum Alphabet
{
    A,
    B,
    C
}

public class AlphaBetWrapper : TokenUnit
{
    public Alphabet Alphabet { get; set; }
}


[IsolateForTesting]
public class TestLevelA : TokenUnit
{
    public Alphabet Alphabet { get; set; }
    public TestLevelB MuhB { get; set; }
}

public class TestLevelB : TokenUnit
{
    public TestLevelB() : base("the", nameof(Alphabet))
    {
        
    }

    public Alphabet Alphabet { get; set; }
}