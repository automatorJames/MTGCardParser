namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tamiyo, Field Researcher, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tamiyo, Field Researcher</exampledoc>
[TokenizationOrder(-1)]
public class TamiyoFieldResearcherRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until your next turn, whenever either of those creatures deals combat damage, you draw a card"];
}
