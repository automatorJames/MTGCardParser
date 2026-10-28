namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Winter's Chill, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Winter's Chill</exampledoc>
[TokenizationOrder(-1)]
public class WintersChillRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["x can't be greater than the number of snow lands you control"];
}
