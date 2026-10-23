namespace MTGGlyphs.GlyphDefinitions;

[TokenizationOrder(-1)]
public class ContainmentPriestP3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a nontoken creature would enter and it wasn't cast, exile it instead"];
}
