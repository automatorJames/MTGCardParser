namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Jandor's Ring, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jandor's Ring</exampledoc>
[TokenizationOrder(-1)]
public class JandorsRingCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["{2}, {t}, discard the last card you drew this turn: draw a card"];
}
