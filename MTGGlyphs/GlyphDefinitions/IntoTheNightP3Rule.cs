namespace MTGGlyphs.GlyphDefinitions;

[TokenizationOrder(-1)]
public class IntoTheNightP3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["discard any number of cards, then draw that many cards plus one"];
}
