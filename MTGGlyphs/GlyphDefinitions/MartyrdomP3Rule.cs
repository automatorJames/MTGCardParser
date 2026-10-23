namespace MTGGlyphs.GlyphDefinitions;

[TokenizationOrder(-1)]
public class MartyrdomP3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["only you may activate this ability"];
}
