namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Skophos Maze-Warden, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Skophos Maze-Warden</exampledoc>
[TokenizationOrder(-1)]
public class SkophosMazeWardenCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever another creature becomes the target of an ability of a land you control named labyrinth of skophos, you may have this creature fight that creature"];
}
