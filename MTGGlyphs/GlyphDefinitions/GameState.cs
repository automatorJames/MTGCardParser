namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A state of the game a static ability can depend on: an object's aspect ("it's attacking", "this creature is equipped") or kind ("it's a human"), counters on it (or "there are exactly three tide counters on this creature"), a card being in a zone ("this card is in your graveyard"), cards in a zone (or a player's count of them, "an opponent has eight or more cards in their graveyard", or a card of a kind being there, "there is a desert card in your graveyard"), permanents on the battlefield ("there are five or more islands on the battlefield"), something having happened this turn, a permanent or a player having done something this turn ("a creature died under your control this turn", "you didn't cast a spell this turn"), a number of objects having done something this turn, card types among cards, a life total (against a threshold, exactly, or against another player's), devotion below a threshold, a hand size, soulbond pairing, a group's total power, an object's power or toughness bound, whose turn it is, a player controlling something (exactly a number of them, or else having a card of a kind in a zone), nothing of a kind ("you control no untapped lands") or more of something than another player, two players each controlling something, a player's designation ("you have the city's blessing"), or how this permanent was cast ("you cast it from your hand"). Shared by WhileState, WhileConditionStatic and activation restrictions.</summary>
/// <exampledoc>Kor Scythemaster</exampledoc>
/// <examplecapture>it's attacking</examplecapture>
[Dependent]
public class GameState : GlyphOneOf
{
    public ItsAspect ItsAspect { get; set; }
    public ObjectIsKind ObjectIsKind { get; set; }
    public HasCounters HasCounters { get; set; }
    public CountersThere CountersThere { get; set; }
    public CardIsInZone CardIsInZone { get; set; }
    public CardCountCondition CardCountCondition { get; set; }
    public PlayerCardCountCondition PlayerCardCountCondition { get; set; }
    public CardInZoneCondition CardInZoneCondition { get; set; }
    public PermanentsOnBattlefield PermanentsOnBattlefield { get; set; }
    public HappenedThisTurn HappenedThisTurn { get; set; }
    public PermanentDidThisTurn PermanentDidThisTurn { get; set; }
    public PlayerDidThisTurn PlayerDidThisTurn { get; set; }
    public ObjectsEventCount ObjectsEventCount { get; set; }
    public CardTypesCountCondition CardTypesCountCondition { get; set; }
    public LifeThreshold LifeThreshold { get; set; }
    public LifeExactly LifeExactly { get; set; }
    public LifeComparedTo LifeComparedTo { get; set; }
    public DevotionThreshold DevotionThreshold { get; set; }
    public HandSizeCondition HandSizeCondition { get; set; }
    public IsPaired IsPaired { get; set; }
    public TotalCharacteristicCondition TotalCharacteristic { get; set; }
    public CharacteristicBoundCondition CharacteristicBound { get; set; }
    public WhoseTurnCondition WhoseTurn { get; set; }
    public PermanentIs PermanentIs { get; set; }
    public ControlsMoreThan ControlsMoreThan { get; set; }
    public BothPlayersControl BothPlayersControl { get; set; }
    public PlayerControlsNone PlayerControlsNone { get; set; }
    public ControlsOrCardInZone ControlsOrCardInZone { get; set; }
    public ControlsExactly ControlsExactly { get; set; }
    public PlayerControls PlayerControls { get; set; }
    public PlayerHasDesignation PlayerHasDesignation { get; set; }
    public CastByPlayerFrom CastByPlayerFrom { get; set; }
}
