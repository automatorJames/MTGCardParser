namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Living Lands, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Living Lands</exampledoc>
[TokenizationOrder(-1)]
public class LivingLandsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["all forests are 1/1 creatures that are still lands"];
}
