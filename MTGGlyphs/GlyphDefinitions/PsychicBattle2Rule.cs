namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Psychic Battle, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Psychic Battle</exampledoc>
[TokenizationOrder(-1)]
public class PsychicBattle2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the player who reveals the card with the greatest mana value may change the target or targets"];
}
