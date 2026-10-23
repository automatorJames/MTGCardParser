namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mirror Gallery, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mirror Gallery</exampledoc>
[TokenizationOrder(-1)]
public class MirrorGalleryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the \"legend rule\" doesn't apply"];
}
