namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A state of the game a static ability can depend on: an object's aspect ("it's attacking", "this creature is equipped") or kind ("it's a human"), counters on it, cards in a zone, something having happened this turn, card types among cards, a life total, a hand size, soulbond pairing, or a player controlling something. Shared by WhileState and WhileConditionStatic.</summary>
/// <exampledoc>Kor Scythemaster</exampledoc>
/// <examplecapture>it's attacking</examplecapture>
[Dependent]
public class GameState : GlyphOneOf
{
    public ItsAspect ItsAspect { get; set; }
    public ObjectIsKind ObjectIsKind { get; set; }
    public HasCounters HasCounters { get; set; }
    public CardCountCondition CardCountCondition { get; set; }
    public HappenedThisTurn HappenedThisTurn { get; set; }
    public CardTypesCountCondition CardTypesCountCondition { get; set; }
    public LifeThreshold LifeThreshold { get; set; }
    public HandSizeCondition HandSizeCondition { get; set; }
    public IsPaired IsPaired { get; set; }
    public PermanentIs PermanentIs { get; set; }
    public PlayerControls PlayerControls { get; set; }
}
