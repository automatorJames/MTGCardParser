namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of The Chain Veil, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Chain Veil</exampledoc>
[Dependent]
public class TheChainVeil2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you didn't activate a loyalty ability of a planeswalker this turn, you lose 2 life"];
}
