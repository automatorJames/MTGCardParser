namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Winter Blast, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Winter Blast</exampledoc>
[TokenizationOrder(-1)]
public class WinterBlastRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals 2 damage to each of those creatures with flying"];
}
