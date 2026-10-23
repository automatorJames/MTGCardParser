namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Full Throttle, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Full Throttle</exampledoc>
[Dependent]
public class FullThrottleEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["untap all creatures that attacked this turn"];
}
