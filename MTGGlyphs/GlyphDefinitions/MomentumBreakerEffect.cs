namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Momentum Breaker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Momentum Breaker</exampledoc>
[Dependent]
public class MomentumBreakerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you gain life equal to your speed"];
}
