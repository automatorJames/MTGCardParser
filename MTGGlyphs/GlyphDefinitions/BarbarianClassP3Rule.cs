namespace MTGGlyphs.GlyphDefinitions;

[TokenizationOrder(-1)]
public class BarbarianClassP3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you would roll one or more dice, instead roll that many dice plus one and ignore the lowest roll"];
}
