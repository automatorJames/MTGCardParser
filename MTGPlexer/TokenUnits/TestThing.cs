namespace MTGPlexer.TokenUnits;

//public class AlternatingClass : TokenUnitOneOf
//[IsolateForTesting]
//public class TestThing : TokenUnit
//{
//    //public TestThing() : base("the number of", nameof(CardKeyword), nameof(EnchantCard)) { }
//    //public TestThing() : base("buh", nameof(GainedOrLostBuffs)) { }
//
//    //public ManyToken<SimpleAlphabet> Alphabets { get; set; }
//    public ManyOf<Alphabet> Alphabets { get; set; }
//
//    //public ManyToken<GainedOrLostBuff> GainedOrLostBuffs { get; set; }
//    //public ManyToken<OneOfEnum> Letters { get; set; }
//    //public CardKeyword CardKeyword { get; set; }
//    //public EnchantCard EnchantCard { get; set; }
//
//    //public LandType LandTypeProp { get; set; }
//    //public WildCard WildCardProp { get; set; }
//}
////
////public enum WildCard
////{
////    Lands,
////    UntappedLands,
////    CardsIn
////}
////
////
////
////public class AlphabetWrapper : TokenUnit
////{
////    public Alphabet Alphabet { get; set; }
////}
////
////
////public class TestLevelA : TokenUnit
////{
////    public Alphabet Alphabet { get; set; }
////    public TestLevelB MuhB { get; set; }
////}
////
////public class TestLevelB : TokenUnit
////{
////    public TestLevelB() : base("the", nameof(Alphabet))
////    {
////        
////    }
////
////    public Alphabet Alphabet { get; set; }
////}
////
////public class Parent : TokenUnit
////{
////    public Alphabet Alphabet { get; set; }
////    public Child Child { get; set; }
////}
////
////public class Child : TokenUnit
////{
////    public Numbers Numbers { get; set; }
////}
////
////public class TokenUnitWithBool : TokenUnit
////{
////    public bool YouMay { get; set; }
////    public Numbers Numbers { get; set; }
////}
////
////public class OneOfEnum : TokenUnitOneOf
////{
////    public Alphabet Alphabet { get; set; }
////    public Numbers Numbers { get; set; }
////}
////
////

//[IsolateForTesting]
//public class McGuffin : TokenUnit
//{
//    public McGuffin() : base("creature has", nameof(KeywordLite_Many)) { }
//
//    //[OptionalMany]
//    //public ManyOf<Mini> Mini { get; set; }
//
//    //[OptionalMany]
//    //public FeckMe FeckMe { get; set; }
//
//    public ManyOf<KeywordLite> KeywordLite_Many { get; set; }
//}

[IsolateForTesting]
public class McGuffin : TokenUnit
{
    public McGuffin() : base("creature has", nameof(McBuffins)) { }

    //[OptionalMany]
    //public ManyOf<Mini> Mini { get; set; }

    //[OptionalMany]
    //public FeckMe FeckMe { get; set; }

    public ManyOf<Buffin> McBuffins { get; set; }
}

[TokenUnitProperty]
public class McStuffin : TokenUnit
{
    public KeywordLite KeywordLite { get; set; }
}

////
public enum Alphabet
{
    ABC,
    DEF,
    GHI
}


public enum KeywordLite
{
    Flying,
    Reach,
    Haste,
    Trample
}

public enum DogStuffe
{
    PoopFling,
    ButtPoop
}

[TokenUnitProperty]
public class Buffin : TokenUnitOneOf
{
    public KeywordLite KeywordLite { get; set; }
    public DogShite DogShite { get; set; }
}

public class DogShite : TokenUnit
{
    public DogStuffe DogStuffe { get; set; }
}

//
//public enum Numbers
//{
//    One,
//    Two,
//    Three
//}

//[IsolateForTesting]
//public class WeepBeep : TokenUnit
//{
//    public WeepyBeepy WeepyBeepy { get; set; }
//}

//[IsolateForTesting]
//public class A : TokenUnit
//{
//    public A() : base("all", nameof(B)) { }
//    public B B { get; set; }
//}
//
//public class B : TokenUnit
//{
//    public B() : base("combat", nameof(C)) { }
//    public C C { get; set; }
//    
//}
//
//public class C : TokenUnit
//{
//    public C() : base("damage that", nameof(D)) { }
//    public D D { get; set; }
//}
//
//public class D : TokenUnit
//{
//    public D() : base("would be dealt this", nameof(DummyEnum)) { }
//    public DummyEnum DummyEnum { get; set; }
//}
//
//public enum DummyEnum
//{
//    Turn,
//}

public enum WeepyBeepy
{
    [RegexPattern("wee", "weep", "weepy")]
    Weep,

    [RegexPattern("beepy", "beep", "bee")]
    Beep,

    Turn,
}


