namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a card can be, as in "isn't a creature": a card type, a creature type ("is a human"), a land type ("is a basic mountain"), a supertype ("is legendary", "was snow"), not being basic ("was nonbasic"), a color, tapped or untapped, a combat state ("isn't attacking", "attacking alone"), a status ("equipped", "enchanted", "monstrous"), or a token ("if it isn't a token").</summary>
/// <exampledoc>Animate Artifact</exampledoc>
/// <examplecapture>creature</examplecapture>
[Dependent]
public class CardAspect : GlyphOneOf
{
    public CardType? CardType { get; set; }
    public Supertype? Supertype { get; set; }
    [RegexPattern("nonbasic")]
    public bool Nonbasic { get; set; }
    public ManaColor? ManaColor { get; set; }
    public TapState? TapState { get; set; }
    public CombatState? CombatState { get; set; }
    public PermanentDesignation? Status { get; set; }
    public CreatureType? CreatureType { get; set; }
    public LandType? LandType { get; set; }
    [RegexPattern("token")]
    public bool Token { get; set; }
}
