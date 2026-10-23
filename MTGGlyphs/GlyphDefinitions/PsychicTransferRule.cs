namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Psychic Transfer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Psychic Transfer</exampledoc>
[TokenizationOrder(-1)]
public class PsychicTransferRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the difference between your life total and target player's life total is 5 or less, exchange life totals with that player"];
}
