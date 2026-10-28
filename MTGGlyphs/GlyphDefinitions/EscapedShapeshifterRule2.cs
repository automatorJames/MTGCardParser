namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Escaped Shapeshifter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Escaped Shapeshifter</exampledoc>
[TokenizationOrder(-1)]
public class EscapedShapeshifterRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the same is true for first strike, trample, and protection from any color"];
}
