namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Regenerate, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Regenerate</exampledoc>
[TokenizationOrder(-1)]
public class RegenerateRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "target creature"];
}
