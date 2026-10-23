namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Scout's Warning, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Scout's Warning</exampledoc>
[TokenizationOrder(-1)]
public class ScoutsWarningRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the next creature card you play this turn can be played as though it had flash"];
}
