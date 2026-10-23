namespace MTGGlyphs.GlyphDefinitions;

[TokenizationOrder(-1)]
public class VolrathsShapeshifterCardP3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as the top card of your graveyard is a creature card, this creature has the full text of that card and has the text \"{2}: discard a card"];
}
