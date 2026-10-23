namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Fractalize, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fractalize</exampledoc>
[TokenizationOrder(-1)]
public class FractalizeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until end of turn, target creature becomes a green and blue fractal with base power and toughness each equal to x plus 1"];
}
