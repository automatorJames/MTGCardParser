namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Cultural Exchange, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cultural Exchange</exampledoc>
[TokenizationOrder(-1)]
public class CulturalExchangeRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["those players exchange control of those creatures"];
}
