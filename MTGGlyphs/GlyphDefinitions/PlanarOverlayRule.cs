namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Planar Overlay, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Planar Overlay</exampledoc>
[TokenizationOrder(-1)]
public class PlanarOverlayRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player chooses a land they control of each basic land type"];
}
