namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Thassa's Oracle, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Thassa's Oracle</exampledoc>
[TokenizationOrder(-1)]
public class ThassasOracleRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if x is greater than or equal to the number of cards in your library, you win the game"];
}
