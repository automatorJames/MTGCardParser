namespace MTGPlexer.TokenUnits;

//public class _TestTokenUnitStuff : TokenUnit
//{
//    //public override Snippet[] Snippets => ["destroy all", Prop(CardType), NoSpace("s")];
//    public override Snippet[] Snippets => ["destroy all", Prop(CardType), Plural];
//
//    public CardType CardType { get; set; }
//}

//public class TestClass : TokenUnit
//{
//    public override Snippet[] Snippets => ["target", Prop(Letters)];
//
//    public ManyOf<Letter> Letters { get; set; }
//}

//[IsolateForTesting]
//public class TestClass : TokenUnit
//{
//    public override Snippet[] Snippets => ["target", Prop(Letters)];
//
//    public CompoundOf<Letter> Letters { get; set; }
//}

[IsolateForTesting]
public class DiscardACard : TokenUnit
{
    public override Snippet[] Snippets => ["discard a card"];
}

public enum Letter
{
    A,
    B,
    C,
    [RegexPattern("d", "deeznutz")] D
}