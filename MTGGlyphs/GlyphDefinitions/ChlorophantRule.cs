namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Chlorophant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Chlorophant</exampledoc>
/// <examplecapture>threshold — as long as there are seven or more cards in your graveyard, this creature has "at the beginning of your upkeep, you may put another +1/+1 counter on this creature</examplecapture>
[TokenizationOrder(-1)]
public class ChlorophantRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["threshold — as long as there are seven or more cards in your graveyard, this creature has \"at the beginning of your upkeep, you may put another +1/+1 counter on this creature"];
}
