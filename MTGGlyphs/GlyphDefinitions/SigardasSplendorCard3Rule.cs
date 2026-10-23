namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sigarda's Splendor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sigarda's Splendor</exampledoc>
[TokenizationOrder(-1)]
public class SigardasSplendorCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then note your life total"];
}
