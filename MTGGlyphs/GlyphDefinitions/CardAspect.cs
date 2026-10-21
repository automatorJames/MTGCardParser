namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a card can be, as in "isn't a creature": a card type, a creature type ("is a human"), a color, tapped or untapped, a combat state ("isn't attacking", "attacking alone"), or a status ("equipped", "enchanted", "monstrous").</summary>
/// <exampledoc>Animate Artifact</exampledoc>
/// <examplecapture>creature</examplecapture>
[Dependent]
public class CardAspect : GlyphOneOf
{
    public CardType? CardType { get; set; }
    public ManaColor? ManaColor { get; set; }
    public TapState? TapState { get; set; }
    public CombatState? CombatState { get; set; }
    public PermanentStatus? Status { get; set; }
    public CreatureType? CreatureType { get; set; }
}
