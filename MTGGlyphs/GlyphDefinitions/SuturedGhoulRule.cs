namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sutured Ghoul, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sutured Ghoul</exampledoc>
[TokenizationOrder(-1)]
public class SuturedGhoulRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "'s power is equal to the total power of the exiled cards and its toughness is equal to their total toughness"];
}
