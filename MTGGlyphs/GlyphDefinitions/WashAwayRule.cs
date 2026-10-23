namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Wash Away, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wash Away</exampledoc>
[TokenizationOrder(-1)]
public class WashAwayRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter target spell [that wasn't cast from its owner's hand]"];
}
