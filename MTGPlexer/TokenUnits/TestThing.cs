namespace MTGPlexer.TokenUnits;

//public class AlternatingClass : TokenUnitOneOf
//[IsolateForTesting]
//public class TestThing : TokenUnit
//{
//    //public TestThing() : base("the number of", nameof(CardKeyword), nameof(EnchantCard)) { }
//    //public TestThing() : base("buh", nameof(GainedOrLostBuffs)) { }
//
//    //public ManyToken<SimpleAlphabet> Alphabets { get; set; }
//    public ManyToken<Alphabet> Alphabets { get; set; }
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
//public class SimpleAlphabet : TokenUnit
//{
//    public Alphabet Alphabet { get; set; }
//}
////
//public enum Alphabet
//{
//    ABC,
//    DEF,
//    GHI
//}
//
//public enum Numbers
//{
//    One,
//    Two,
//    Three
//}