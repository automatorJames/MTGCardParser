namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Memories Returning, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Memories Returning</exampledoc>
[TokenizationOrder(-1)]
public class MemoriesReturningRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then they put one on the bottom of your library"];
}
