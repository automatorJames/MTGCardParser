namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Fire and Brimstone, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fire and Brimstone</exampledoc>
[TokenizationOrder(-1)]
public class FireAndBrimstoneRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals 4 damage to target player who attacked this turn and 4 damage to you"];
}
