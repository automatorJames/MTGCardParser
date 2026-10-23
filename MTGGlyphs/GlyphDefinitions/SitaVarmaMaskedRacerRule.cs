namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sita Varma, Masked Racer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sita Varma, Masked Racer</exampledoc>
[TokenizationOrder(-1)]
public class SitaVarmaMaskedRacerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then you may have the base power and toughness of each other creature you control become equal to sita varma's power until end of turn"];
}
