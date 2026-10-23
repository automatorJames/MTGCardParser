namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tomik, Distinguished Advokist, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tomik, Distinguished Advokist</exampledoc>
[TokenizationOrder(-1)]
public class TomikDistinguishedAdvokistCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["your opponents can't play land cards from graveyards"];
}
