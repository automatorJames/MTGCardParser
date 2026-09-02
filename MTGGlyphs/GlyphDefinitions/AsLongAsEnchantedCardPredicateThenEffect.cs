namespace MTGGlyphs.GlyphDefinitions;

[IsolateForTesting]
public class AsLongAsEnchantedCardPredicateThenEffect : Glyph
{
    public override Nib[] Nibs => ["as long as enchanted", Prop(CardType), Prop(Assertion), Opt("an?"), Prop(CardAspect), ",", Prop(Effect)];

    public CardType CardType{ get; set; }
    public Assertion Assertion { get; set; }
    public CardAspect CardAspect { get; set; }
    public DynamicGlyph Effect { get; set; }
}