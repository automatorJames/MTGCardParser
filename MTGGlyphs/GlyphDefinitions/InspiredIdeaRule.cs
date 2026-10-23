namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Inspired Idea, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Inspired Idea</exampledoc>
[TokenizationOrder(-1)]
public class InspiredIdeaRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["[your maximum hand size is reduced by three for the rest of the game"];
}
