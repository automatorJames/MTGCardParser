namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Volrath's Dungeon, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Volrath's Dungeon</exampledoc>
[TokenizationOrder(-1)]
public class VolrathsDungeonRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["any player may activate this ability but only during their turn"];
}
