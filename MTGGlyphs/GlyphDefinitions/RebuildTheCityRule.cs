namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rebuild the City, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rebuild the City</exampledoc>
[TokenizationOrder(-1)]
public class RebuildTheCityRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create three tokens that are copies of it, except they're 3/3 creatures in addition to their other types and they have vigilance and menace"];
}
