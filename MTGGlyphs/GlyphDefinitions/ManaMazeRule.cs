namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mana Maze, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mana Maze</exampledoc>
[TokenizationOrder(-1)]
public class ManaMazeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["players can't cast spells that share a color with the spell most recently cast this turn"];
}
