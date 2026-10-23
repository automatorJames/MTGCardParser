namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Well-Laid Plans, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Well-Laid Plans</exampledoc>
[TokenizationOrder(-1)]
public class WellLaidPlansRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["prevent all damage that would be dealt to a creature by another creature if they share a color"];
}
