namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Skophos Maze-Warden, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Skophos Maze-Warden</exampledoc>
/// <examplecapture>you may have this creature fight that creature</examplecapture>
[Dependent]
public class SkophosMazeWardenEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may have this creature fight that creature"];
}
