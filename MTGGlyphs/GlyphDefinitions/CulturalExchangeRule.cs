namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Cultural Exchange, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cultural Exchange</exampledoc>
[TokenizationOrder(-1)]
public class CulturalExchangeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose the same number of creatures another target player controls"];
}
