namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Psychic Possession, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Psychic Possession</exampledoc>
[TokenizationOrder(-1)]
public class PsychicPossessionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever enchanted opponent draws a card, you may draw a card"];
}
