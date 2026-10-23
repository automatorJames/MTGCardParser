namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Game of Chaos, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Game of Chaos</exampledoc>
[TokenizationOrder(-1)]
public class GameOfChaosCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["double the life stakes with each flip"];
}
