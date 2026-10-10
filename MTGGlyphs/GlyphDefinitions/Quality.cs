namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a qualifier word can be: a card type, a color, tapped or untapped, a combat state ("attacking") or a creature type ("non-wall").</summary>
/// <exampledoc>Desert</exampledoc>
/// <examplecapture>attacking</examplecapture>
[Dependent]
public class Quality : GlyphOneOf
{
    public CardType? CardType { get; set; }
    public ManaColor? ManaColor { get; set; }
    public TapState? TapState { get; set; }
    public CombatState? CombatState { get; set; }
    public CreatureType? CreatureType { get; set; }
}
