namespace MTGGlyphs;

[Dependent]
public class CardAspect : GlyphOneOf
{
    public CardType? CardType { get; set; }
    public ManaColor? ManaColor { get; set; }
}
