namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Clockspinning, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Clockspinning</exampledoc>
[TokenizationOrder(-1)]
public class ClockspinningCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose a counter on target permanent or suspended card"];
}
