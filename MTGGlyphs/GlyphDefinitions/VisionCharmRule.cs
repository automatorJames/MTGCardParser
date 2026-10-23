namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Vision Charm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vision Charm</exampledoc>
[TokenizationOrder(-1)]
public class VisionCharmRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each land of the first chosen type becomes the second chosen type until end of turn"];
}
