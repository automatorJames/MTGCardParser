namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mirror Box, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mirror Box</exampledoc>
[TokenizationOrder(-1)]
public class MirrorBoxRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the \"legend rule\" doesn't apply to permanents you control"];
}
