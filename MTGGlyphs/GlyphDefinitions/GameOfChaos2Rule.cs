namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Game of Chaos, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Game of Chaos</exampledoc>
[TokenizationOrder(-1)]
public class GameOfChaos2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you win the flip, you gain 1 life and target opponent loses 1 life, and you decide whether to flip again"];
}
