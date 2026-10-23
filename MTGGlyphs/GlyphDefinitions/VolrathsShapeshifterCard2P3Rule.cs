namespace MTGGlyphs.GlyphDefinitions;

[TokenizationOrder(-1)]
public class VolrathsShapeshifterCard2P3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["(this creature has that card's name, mana cost, color, types, abilities, power, and toughness"];
}
