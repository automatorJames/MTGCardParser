namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Enigma Jewel // Locus of Enlightenment, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Enigma Jewel // Locus of Enlightenment</exampledoc>
[TokenizationOrder(-1)]
public class TheEnigmaJewelRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the enigma jewel enters tapped"];
}
