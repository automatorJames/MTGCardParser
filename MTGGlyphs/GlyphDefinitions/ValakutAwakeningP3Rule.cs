namespace MTGGlyphs.GlyphDefinitions;

[TokenizationOrder(-1)]
public class ValakutAwakeningP3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put any number of cards from your hand on the bottom of your library, then draw that many cards plus one"];
}
