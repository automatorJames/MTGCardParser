namespace MTGGlyphs.GlyphDefinitions;

//[IsolateForTesting]
//public class AsLongAsEnchantedCardPredicateThenEffect : Glyph
//{
//    public override Nib[] Nibs => ["as long as enchanted", Prop(CardType), Prop(Assertion), Opt("an?"), Prop(CardAspect), ",", Prop(Effect), Prop(TestThing)];
//
//    public CardType CardType{ get; set; }
//    public Assertion Assertion { get; set; }
//    public CardAspect CardAspect { get; set; }
//    public DynamicGlyph Effect { get; set; }
//    public TestThing TestThing { get; set; }
//}

public class AsLongAsEnchantedCardPredicateThenEffect : Glyph
{
    public override Nib[] Nibs => ["as long as enchanted", Prop(CardType), Prop(Assertion), Opt(Pattern("an?")), Prop(CardAspect), ",", Prop(Effect)];

    public CardType CardType{ get; set; }
    public Assertion Assertion { get; set; }
    public CardAspect CardAspect { get; set; }
    public DynamicGlyph Effect { get; set; }
}


//[Dependent]
public class TestThing : Glyph
{
    public override Nib[] Nibs => ["it's", Pattern("an?"), Prop(CardType), "with power and toughness each equal to its mana value"];

    [JoinedBy(Joiner.Space)]
    public CompoundOf<CardType> CardType { get; set; }

}