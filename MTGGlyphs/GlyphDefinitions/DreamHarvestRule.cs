namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dream Harvest, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dream Harvest</exampledoc>
[TokenizationOrder(-1)]
public class DreamHarvestRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each opponent exiles cards from the top of their library until they have exiled cards with total mana value 5 or greater this way"];
}
