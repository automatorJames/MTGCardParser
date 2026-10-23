namespace MTGGlyphs.GlyphDefinitions;

[TokenizationOrder(-1)]
public class RagingRiverCard2P3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then, for each attacking creature you control, choose \"left\" or \"right"];
}
