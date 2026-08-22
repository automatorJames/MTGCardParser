namespace MTGGlyphs.GlyphDefinitions;

[MustMatchWholeLine]
public class CardAbilityLine : CompoundOf<Keyword>
{
    public override Joiner Joiner => Joiner.CommaSpace;
}