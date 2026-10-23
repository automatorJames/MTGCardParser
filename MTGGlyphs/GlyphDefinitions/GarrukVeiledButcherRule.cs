namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Garruk, Veiled Butcher, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Garruk, Veiled Butcher</exampledoc>
[TokenizationOrder(-1)]
public class GarrukVeiledButcherRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each opponent who didn't discard two nonland cards this way, you draw a card"];
}
