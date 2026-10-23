namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Baron Helmut Zemo, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Baron Helmut Zemo</exampledoc>
/// <examplecapture>boast — exile any number of black cards from your graveyard with fifteen or more black mana symbols among their mana costs: copy those exiled cards</examplecapture>
[TokenizationOrder(-1)]
public class BaronHelmutZemoRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["boast — exile any number of black cards from your graveyard with fifteen or more black mana symbols among their mana costs: copy those exiled cards"];
}
