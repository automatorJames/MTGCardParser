namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Game of Chaos, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Game of Chaos</exampledoc>
[TokenizationOrder(-1)]
public class GameOfChaosRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you lose the flip, you lose 1 life and that opponent gains 1 life, and that player decides whether to flip again"];
}
