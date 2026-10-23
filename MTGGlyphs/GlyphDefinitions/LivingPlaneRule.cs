namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Living Plane, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Living Plane</exampledoc>
[TokenizationOrder(-1)]
public class LivingPlaneRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["all lands are 1/1 creatures that are still lands"];
}
