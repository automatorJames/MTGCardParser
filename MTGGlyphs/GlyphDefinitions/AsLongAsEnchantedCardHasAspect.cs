namespace MTGGlyphs.GlyphDefinitions;

//public class AsLongAsEnchantedCardHasAspect : Glyph
//{
//    public override Nib[] Nibs => ["as long as enchanted", Prop(CardType), Prop(Assertion), Opt("an?"), Prop(CardAspect), ",", Prop(PermanentVerb), Prop(Buff)];
//
//    public CardType CardType { get; set; }
//    public Assertion Assertion { get; set; }
//    public CardAspect CardAspect { get; set; }
//    public PermanentVerb PermanentVerb { get; set; }
//    public Buff Buff { get; set; }
//}

public class AsLongAsThing : Glyph
{
    public override Nib[] Nibs => ["as long as", Prop(EnchantedCardHasAspect), Prop(ItGetsOrLosesBuff)];
    public EnchantedCardHasAspect EnchantedCardHasAspect { get; set; }
    public ItGetsOrLosesBuff ItGetsOrLosesBuff { get; set; }
}

[Dependent]
public class EnchantedCardHasAspect : Glyph
{
    public override Nib[] Nibs => ["enchanted", Prop(CardType), Prop(Assertion), Opt("an?"), Prop(CardAspect)];

    public CardType CardType { get; set; }
    public Assertion Assertion { get; set; }
    public CardAspect CardAspect { get; set; }
}

[Dependent]
public class ItGetsOrLosesBuff : Glyph
{
    public PermanentVerb PermanentVerb { get; set; }
    public Buff Buff { get; set; }
}