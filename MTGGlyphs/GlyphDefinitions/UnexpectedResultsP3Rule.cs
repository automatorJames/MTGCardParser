namespace MTGGlyphs.GlyphDefinitions;

[TokenizationOrder(-1)]
public class UnexpectedResultsP3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["shuffle your library, then reveal the top card"];
}
