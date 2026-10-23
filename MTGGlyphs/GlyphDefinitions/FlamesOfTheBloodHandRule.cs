namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Flames of the Blood Hand, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Flames of the Blood Hand</exampledoc>
[TokenizationOrder(-1)]
public class FlamesOfTheBloodHandRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if that player or that planeswalker's controller would gain life this turn, that player gains no life instead"];
}
