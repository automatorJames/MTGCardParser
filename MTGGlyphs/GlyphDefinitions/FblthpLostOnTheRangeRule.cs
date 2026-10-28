namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Fblthp, Lost on the Range, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fblthp, Lost on the Range</exampledoc>
[TokenizationOrder(-1)]
public class FblthpLostOnTheRangeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the top card of your library has plot"];
}
