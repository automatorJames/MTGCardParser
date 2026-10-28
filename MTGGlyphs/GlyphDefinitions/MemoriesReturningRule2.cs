namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Memories Returning, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Memories Returning</exampledoc>
[TokenizationOrder(-1)]
public class MemoriesReturningRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then you put one into your hand"];
}
