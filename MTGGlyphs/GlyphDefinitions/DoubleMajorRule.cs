namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Double Major, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Double Major</exampledoc>
[TokenizationOrder(-1)]
public class DoubleMajorRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["copy target creature spell you control, except it isn't legendary if the spell is legendary"];
}
