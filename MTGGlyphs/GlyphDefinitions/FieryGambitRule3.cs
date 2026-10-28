namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Fiery Gambit, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fiery Gambit</exampledoc>
[TokenizationOrder(-1)]
public class FieryGambitRule3 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you win one or more flips,", Nib.This, "deals 3 damage to target creature"];
}
