namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Strength of the Tajuru, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Strength of the Tajuru</exampledoc>
[TokenizationOrder(-1)]
public class StrengthOfTheTajuruRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose target creature, then choose another target creature for each time this spell was kicked"];
}
