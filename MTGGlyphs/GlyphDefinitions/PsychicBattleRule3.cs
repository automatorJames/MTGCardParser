namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Psychic Battle, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Psychic Battle</exampledoc>
[TokenizationOrder(-1)]
public class PsychicBattleRule3 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if two or more cards are tied for greatest, the target or targets remain unchanged"];
}
