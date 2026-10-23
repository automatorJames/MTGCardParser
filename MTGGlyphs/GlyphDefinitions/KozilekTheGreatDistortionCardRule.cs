namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Kozilek, the Great Distortion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kozilek, the Great Distortion</exampledoc>
[TokenizationOrder(-1)]
public class KozilekTheGreatDistortionCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["discard a card with mana value x: counter target spell with mana value x"];
}
