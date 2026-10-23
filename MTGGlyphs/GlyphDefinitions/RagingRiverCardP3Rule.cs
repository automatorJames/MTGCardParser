namespace MTGGlyphs.GlyphDefinitions;

[TokenizationOrder(-1)]
public class RagingRiverCardP3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that creature can't be blocked this combat except by creatures with flying and creatures in a pile with the chosen label"];
}
