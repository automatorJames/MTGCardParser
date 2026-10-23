namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Wayward Angel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wayward Angel</exampledoc>
/// <examplecapture>threshold — as long as there are seven or more cards in your graveyard, this creature gets +3/+3, is black, has trample, and has "at the beginning of your upkeep, sacrifice a creature</examplecapture>
[TokenizationOrder(-1)]
public class WaywardAngelRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["threshold — as long as there are seven or more cards in your graveyard, this creature gets +3/+3, is black, has trample, and has \"at the beginning of your upkeep, sacrifice a creature"];
}
