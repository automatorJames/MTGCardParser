namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Cleansing Meditation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cleansing Meditation</exampledoc>
/// <examplecapture>threshold — if there are seven or more cards in your graveyard, instead destroy all enchantments, then return all cards in your graveyard destroyed this way to the battlefield</examplecapture>
[TokenizationOrder(-1)]
public class CleansingMeditationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["threshold — if there are seven or more cards in your graveyard, instead destroy all enchantments, then return all cards in your graveyard destroyed this way to the battlefield"];
}
