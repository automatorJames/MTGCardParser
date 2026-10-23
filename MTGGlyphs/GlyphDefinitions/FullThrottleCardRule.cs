namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Full Throttle, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Full Throttle</exampledoc>
[TokenizationOrder(-1)]
public class FullThrottleCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["at the beginning of each combat this turn, untap all creatures that attacked this turn"];
}
