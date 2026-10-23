namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A state of the game a static ability can depend on: an object's aspect ("it's attacking", "this creature is equipped"), kind ("it's a human"), keywords ("it has defender", "it doesn't have suspend") or a characteristic shared with another ("it has the same name as that card"), counters on it (or "there are exactly three tide counters on this creature"), a card being in a zone ("this card is in your graveyard"), cards in a zone (or a player's count of them, "an opponent has eight or more cards in their graveyard", or a card of a kind being there, "there is a desert card in your graveyard"), cards having left a zone this turn ("a card left your graveyard this turn"), permanents on the battlefield ("there are five or more islands on the battlefield", "no creatures are on the battlefield"), creatures attacking ("three or more creatures are attacking") or others attacking or blocking alongside ("at least two other creatures attack"), something having happened this turn, a player or a permanent having done something this turn ("you didn't cast a spell this turn", "an opponent was dealt damage this turn", "a creature died under your control this turn", "this creature was dealt damage this turn", "an opponent had a land enter the battlefield under their control this turn") - players first, so a player's event isn't read as a permanent's - a number of objects having done something this turn, card types among cards, a life total (against a threshold, exactly, against another player's, against a starting life total: "your life total is less than your starting life total", or some margin above it: "you have at least 7 life more than your starting life total"), devotion below a threshold, a hand size, soulbond pairing, a group's total power, an object's power or toughness bound ("its mana value is 2 or less", "it has mana value 3 or less"), whose turn it is, a player controlling something (exactly a number of them, or else having a card of a kind in a zone), nothing of a kind ("you control no untapped lands") or more of something than another player, two players each controlling something, a player's designation ("you have the city's blessing"), a player being poisoned ("defending player is poisoned") or having a number of poison counters ("an opponent has three or more poison counters"), a completed dungeon ("you've completed a dungeon"), how this permanent was cast ("you cast it from your hand") or when this spell was ("you cast this spell during your main phase"), the dragon-reveal cost ("you revealed a dragon card or controlled a dragon as you cast this spell"), the most common color among permanents ("red is the most common color among all permanents or is tied for most common", "it shares a color with the most common color …"), how often this ability has been activated this turn, or which time it has resolved this turn ("this is the second time this ability has resolved this turn"), the mana spent on a spell ("at least three black mana was spent to cast this spell", "mana from a treasure was spent to cast it"), or the value of X ("x is 5 or more"). Shared by WhileState, WhileConditionStatic and activation restrictions.</summary>
/// <exampledoc>Kor Scythemaster</exampledoc>
/// <examplecapture>it's attacking</examplecapture>
[Dependent]
public class GameState : GlyphOneOf
{
    public ItsAspect ItsAspect { get; set; }
    public ObjectIsKind ObjectIsKind { get; set; }
    public ObjectHasKeyword ObjectHasKeyword { get; set; }
    public SharesCharacteristicWith SharesCharacteristicWith { get; set; }
    public HasCounters HasCounters { get; set; }
    public CountersThere CountersThere { get; set; }
    public CardIsInZone CardIsInZone { get; set; }
    public CardCountCondition CardCountCondition { get; set; }
    public PlayerCardCountCondition PlayerCardCountCondition { get; set; }
    public CardInZoneCondition CardInZoneCondition { get; set; }
    public CardsLeftZoneThisTurn CardsLeftZoneThisTurn { get; set; }
    public PermanentsOnBattlefield PermanentsOnBattlefield { get; set; }
    public HappenedThisTurn HappenedThisTurn { get; set; }
    public PlayerDidThisTurn PlayerDidThisTurn { get; set; }
    public PermanentDidThisTurn PermanentDidThisTurn { get; set; }
    public ObjectsEventCount ObjectsEventCount { get; set; }
    public CardTypesCountCondition CardTypesCountCondition { get; set; }
    public LifeThreshold LifeThreshold { get; set; }
    public LifeExactly LifeExactly { get; set; }
    public LifeComparedTo LifeComparedTo { get; set; }
    public LifeTotalCondition LifeTotalCondition { get; set; }
    public LifeAboveStarting LifeAboveStarting { get; set; }
    public DevotionThreshold DevotionThreshold { get; set; }
    public HandSizeCondition HandSizeCondition { get; set; }
    public IsPaired IsPaired { get; set; }
    public TotalCharacteristicCondition TotalCharacteristic { get; set; }
    public CharacteristicBoundCondition CharacteristicBound { get; set; }
    public HasCharacteristicBound HasCharacteristicBound { get; set; }
    public WhoseTurnCondition WhoseTurn { get; set; }
    public PermanentIs PermanentIs { get; set; }
    public ControlsMoreThan ControlsMoreThan { get; set; }
    public BothPlayersControl BothPlayersControl { get; set; }
    public PlayerControlsNone PlayerControlsNone { get; set; }
    public ControlsOrCardInZone ControlsOrCardInZone { get; set; }
    public ControlsExactly ControlsExactly { get; set; }
    public PlayerControls PlayerControls { get; set; }
    public PlayerHasDesignation PlayerHasDesignation { get; set; }
    public PlayerIsPoisoned PlayerIsPoisoned { get; set; }
    public PlayerPoisonCount PlayerPoisonCount { get; set; }
    public CompletedDungeon CompletedDungeon { get; set; }
    public CastByPlayerFrom CastByPlayerFrom { get; set; }
    public CastThisSpellDuring CastThisSpellDuring { get; set; }
    public RevealedOrControlledAsCast RevealedOrControlledAsCast { get; set; }
    public ColorIsMostCommon ColorIsMostCommon { get; set; }
    public SharesMostCommonColor SharesMostCommonColor { get; set; }
    public AbilityActivatedTimes AbilityActivatedTimes { get; set; }
    public AbilityResolvedNthTime AbilityResolvedNthTime { get; set; }
    public ManaSpentCondition ManaSpentCondition { get; set; }
    public ManaFromSourceSpent ManaFromSourceSpent { get; set; }
    public PermanentsAreOnBattlefield PermanentsAreOnBattlefield { get; set; }
    public PermanentsAreAttacking PermanentsAreAttacking { get; set; }
    public OthersAlsoInCombat OthersAlsoInCombat { get; set; }
    public PlayerHadPermanentsEnter PlayerHadPermanentsEnter { get; set; }
    public XThreshold XThreshold { get; set; }
}
