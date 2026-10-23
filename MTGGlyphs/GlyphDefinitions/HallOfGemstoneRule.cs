namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hall of Gemstone, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hall of Gemstone</exampledoc>
[TokenizationOrder(-1)]
public class HallOfGemstoneRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until end of turn, lands tapped for mana produce mana of the chosen color instead of any other color"];
}
