namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Psychic Battle, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Psychic Battle</exampledoc>
[TokenizationOrder(-1)]
public class PsychicBattleCard4Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["changing targets this way doesn't trigger abilities of permanents named", Nib.This];
}
