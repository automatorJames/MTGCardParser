namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Withering Wisps, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Withering Wisps</exampledoc>
[TokenizationOrder(-1)]
public class WitheringWispsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["activate no more times each turn than the number of snow swamps you control"];
}
