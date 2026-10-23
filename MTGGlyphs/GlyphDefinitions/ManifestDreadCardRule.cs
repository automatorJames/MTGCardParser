namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Manifest Dread, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Manifest Dread</exampledoc>
[TokenizationOrder(-1)]
public class ManifestDreadCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This];
}
