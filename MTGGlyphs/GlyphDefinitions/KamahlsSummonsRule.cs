namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Kamahl's Summons, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kamahl's Summons</exampledoc>
[TokenizationOrder(-1)]
public class KamahlsSummonsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then each player creates a 2/2 green bear creature token for each card they revealed this way"];
}
