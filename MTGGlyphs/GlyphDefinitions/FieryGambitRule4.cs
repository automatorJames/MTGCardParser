namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Fiery Gambit, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fiery Gambit</exampledoc>
[TokenizationOrder(-1)]
public class FieryGambitRule4 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you win two or more flips,", Nib.This, "deals 6 damage to each opponent"];
}
