namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Fiery Gambit, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fiery Gambit</exampledoc>
[TokenizationOrder(-1)]
public class FieryGambitCard5Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you lose a flip,", Nib.This, "has no effect"];
}
