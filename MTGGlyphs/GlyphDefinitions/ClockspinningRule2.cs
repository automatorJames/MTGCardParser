namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Clockspinning, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Clockspinning</exampledoc>
[TokenizationOrder(-1)]
public class ClockspinningRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["remove that counter from that permanent or card or put another of those counters on it"];
}
