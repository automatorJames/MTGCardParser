namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a card can be, as in "isn't a creature": a card type, a color, tapped or untapped, or a combat state ("isn't attacking").</summary>
/// <exampledoc>Animate Artifact</exampledoc>
/// <examplecapture>creature</examplecapture>
[Dependent]
public class CardAspect : GlyphOneOf
{
    public CardType? CardType { get; set; }
    public ManaColor? ManaColor { get; set; }
    public TapState? TapState { get; set; }
    public CombatState? CombatState { get; set; }
}
