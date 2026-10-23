namespace MTGGlyphs.GlyphDefinitions;

[TokenizationOrder(-1)]
public class HeadGamesP3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target opponent puts the cards from their hand on top of their library"];
}
