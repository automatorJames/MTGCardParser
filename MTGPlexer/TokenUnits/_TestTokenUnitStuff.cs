namespace MTGPlexer.TokenUnits;

//public class _TestTokenUnitStuff : TokenUnit
//{
//    //protected override Snippet[] Snippets => ["destroy all", Prop(CardType), NoSpace("s")];
//    protected override Snippet[] Snippets => ["destroy all", Prop(CardType), Plural];
//
//    public CardType CardType { get; set; }
//}

//[IsolateForTesting]
//public class TestClass : TokenUnit
//{
//    protected override Snippet[] Snippets => ["target", Prop(Letters)];
//
//    public ManyOf<Letter> Letters { get; set; }
//}

[IsolateForTesting]
public class TestClass : TokenUnit
{
    protected override Snippet[] Snippets => ["target", Prop(Letters)];

    public CompoundOf<Letter> Letters { get; set; }
}

public enum Letter
{
    A,
    B,
    C,
    D
}