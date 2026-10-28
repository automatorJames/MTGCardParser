namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tide of War, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tide of War</exampledoc>
[TokenizationOrder(-1)]
public class TideOfWarRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you lose the flip, each blocked creature is sacrificed by its controller"];
}
