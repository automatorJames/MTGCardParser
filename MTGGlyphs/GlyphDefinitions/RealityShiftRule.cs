namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Reality Shift, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Reality Shift</exampledoc>
[TokenizationOrder(-1)]
public class RealityShiftRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["its controller manifests the top card of their library"];
}
