namespace MTGGlyphs.GlyphDefinitions;

[TokenizationOrder(-1)]
public class FalseOrdersP3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures it was blocking that had become blocked by only that creature this combat become unblocked"];
}
