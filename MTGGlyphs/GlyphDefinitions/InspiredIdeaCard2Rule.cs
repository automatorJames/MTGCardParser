namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Inspired Idea, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Inspired Idea</exampledoc>
[TokenizationOrder(-1)]
public class InspiredIdeaCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["]"];
}
