namespace MTGGlyphs.GlyphDefinitions;

[TokenizationOrder(-1)]
public class PoulticeSliverP3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["(the next time that sliver would be destroyed this turn, instead tap it, remove it from combat, and heal all damage on it"];
}
