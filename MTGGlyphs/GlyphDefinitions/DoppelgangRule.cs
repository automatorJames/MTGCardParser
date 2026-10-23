namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Doppelgang, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Doppelgang</exampledoc>
[TokenizationOrder(-1)]
public class DoppelgangRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each of x target permanents, create x tokens that are copies of that permanent"];
}
