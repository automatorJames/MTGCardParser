namespace MTGRulesIR.Model;

/// <summary>An amount: a number, X, a count of something, or an amount the text names only by what it depends on.</summary>
public abstract record Quantity : IrNode;

public sealed record OpaqueQuantity(string Text, string Glyph) : Quantity, IOpaque;

public sealed record Fixed(int Value) : Quantity;

/// <summary>A variable named in the text: "x".</summary>
public sealed record Variable(string Name) : Quantity;

/// <summary>The number of objects a selector picks out ("the number of swamps you control", "for each creature you control").</summary>
public sealed record CountOf(Selector Of) : Quantity;

/// <summary>A characteristic of an object, as an amount: "its power", "that creature's toughness".</summary>
public sealed record CharacteristicOf(Selector Of, string Characteristic) : Quantity;

public sealed record Negated(Quantity Of) : Quantity;

public sealed record Product(Quantity Left, Quantity Right) : Quantity;

/// <summary>An amount card text names by a word rather than a number: "all", "any number", "one or more", "that many".</summary>
public sealed record Special(string Kind) : Quantity;

/// <summary>A power/toughness pair or change: "1/1", "+2/+2".</summary>
public sealed record StatsMod(Quantity Power, Quantity Toughness) : IrNode;

/// <summary>How long an effect lasts.</summary>
public abstract record Duration : IrNode;

public sealed record OpaqueDuration(string Text, string Glyph) : Duration, IOpaque;

/// <summary>Until a point in a turn: "until end of turn", "until your next upkeep".</summary>
public sealed record UntilPhase(PhaseRef Phase) : Duration;

/// <summary>For the rest of the current turn or combat.</summary>
public sealed record ThisTurn(string Period = "Turn") : Duration;

/// <summary>While a condition holds: "as long as you control a forest".</summary>
public sealed record While(Condition Condition) : Duration;

/// <summary>Until the card leaves the battlefield.</summary>
public sealed record UntilThisLeaves : Duration;

/// <summary>A step or phase, and whose: "your upkeep", "each player's upkeep", "end of turn", "your next end step".</summary>
public sealed record PhaseRef(string Phase, string Part = null, Selector Whose = null, bool Next = false) : IrNode;

/// <summary>A fact about the game that an effect or trigger depends on.</summary>
public abstract record Condition : IrNode;

public sealed record OpaqueCondition(string Text, string Glyph) : Condition, IOpaque;

/// <summary>A count of objects compared with a threshold: "there are seven or more cards in your graveyard".</summary>
public sealed record CountCondition(Selector Of, string Comparison, Quantity Threshold) : Condition;

/// <summary>A player controlling (or not) what a selector picks out: "you control a forest", "you don't control an ogre".</summary>
public sealed record Controls(Selector Player, Selector Objects, bool Negated = false, string Connective = null, string Tense = null) : Condition;

/// <summary>How an earlier choice, requirement or flip came out: "if you do", "if the player doesn't", "if you win the flip".</summary>
public sealed record OutcomeCondition(Selector Player, string Result) : Condition;

/// <summary>The card itself is on the battlefield.</summary>
public sealed record SelfOnBattlefield : Condition;

/// <summary>An object named earlier is (or isn't) of some kinds: "it's a creature card".</summary>
public sealed record IsKind(Selector Object, IReadOnlyList<string> Kinds, bool Negated = false, IReadOnlyList<IrNode> Qualified = null) : Condition;

public sealed record And(IReadOnlyList<Condition> Conditions) : Condition;

/// <summary>What a triggered ability waits for.</summary>
public abstract record Event : IrNode;

public sealed record OpaqueEvent(string Text, string Glyph) : Event, IOpaque;

/// <summary>Something happening to objects a selector picks out: they enter, die, attack, become tapped, …</summary>
public sealed record ObjectEvent(Selector Subject, IReadOnlyList<string> Events, Selector UnderControlOf = null, string Only = null, Condition While = null, IReadOnlyList<IrNode> Details = null) : Event;

/// <summary>A creature meeting another in combat: "blocks or becomes blocked by a non-wall creature".</summary>
public sealed record CombatEncounterEvent(Selector Subject, string Encounter, Selector Other) : Event;

/// <summary>A point in a turn arriving: "at the beginning of your upkeep".</summary>
public sealed record PhaseEvent(PhaseRef At) : Event;

/// <summary>A player casting a spell the filter admits.</summary>
public sealed record SpellCastEvent(Selector Caster, Selector Spell, bool OrCopy = false, Condition While = null) : Event;

/// <summary>What is paid to activate an ability or cast a spell.</summary>
public abstract record Cost : IrNode;

public sealed record OpaqueCost(string Text, string Glyph) : Cost, IOpaque;

public sealed record PayMana(IReadOnlyList<string> Symbols) : Cost;

/// <summary>"{t}".</summary>
public sealed record TapSelf : Cost;

/// <summary>"{q}".</summary>
public sealed record UntapSelf : Cost;

public sealed record PayLife(Quantity Amount) : Cost;

/// <summary>A cost that is an action: sacrificing, discarding, exiling, tapping another permanent, removing counters.</summary>
public sealed record ActionCost(Effect Action) : Cost;

/// <summary>A zone, and whose when it's a player's: "your hand", "its owner's graveyard", "the battlefield".</summary>
public sealed record ZoneRef(string Zone, Selector Owner = null, string Position = null) : IrNode;

/// <summary>An amount at or beyond a bound: "two or more" (">="), "two or fewer" ("<=").</summary>
public sealed record Bound(Quantity Value, string Comparison) : Quantity;

/// <summary>A scaled amount with its rounding: "half the number of zombies you control, rounded down".</summary>
public sealed record Scaled(Quantity Base, string Factor, string Rounding = null) : Quantity;

/// <summary>Mana of a color chosen as it's added: "one mana of any color".</summary>
public sealed record ManaChoice(Quantity Count, IrNode Choice) : IrNode;

/// <summary>Throughout a step, phase or turn: "during your upkeep".</summary>
public sealed record DuringPhase(PhaseRef Phase) : Duration;

/// <summary>An object being, or not being, some things: "{this} is tapped", "it isn't a creature", "that creature was a human".</summary>
public sealed record ObjectIs(Selector Object, IReadOnlyList<IrNode> Aspects, bool Negated = false, string Tense = null) : Condition;

/// <summary>Something happened earlier in a span: "you attacked this turn", "a creature died this turn".</summary>
public sealed record Happened(IrNode Event, string Span) : Condition;

/// <summary>How a spell was cast: "{this} was kicked", "it wasn't kicked".</summary>
public sealed record WasCastWith(Selector Spell, string How, bool Negated = false, bool Twice = false) : Condition;

/// <summary>An object having (or not) counters on it: "it has a depletion counter on it", "there are no charge counters on {this}".</summary>
public sealed record HasCountersCondition(Selector Holder, CounterSpec Counters, bool Negated = false, string Tense = null, IrNode Other = null) : Condition;

/// <summary>A card being in a zone or state: "this card is in your graveyard", "this card is suspended".</summary>
public sealed record IsInZone(Selector Object, IrNode Place) : Condition;

public sealed record Not(Condition Condition) : Condition;

/// <summary>Something a player does that a trigger watches for: "you gain life", "an opponent draws their second card each turn".</summary>
public sealed record PlayerEvent(Selector Player, string Event, string Timing = null) : Event;

/// <summary>An event about to happen, as a replacement effect's condition: "you would draw a card".</summary>
public sealed record WouldHappen(Selector Subject, Effect What) : Event;

/// <summary>Two amounts compared: "your devotion to white is less than five".</summary>
public sealed record CompareCount(Quantity Count, string Comparison, Quantity Threshold) : Condition;

/// <summary>How many distinct kinds of an aspect appear among some objects: "card types among cards in your graveyard".</summary>
public sealed record DistinctCount(string Aspect, Selector Among) : Quantity;

/// <summary>An amount named by reference and adjusted: "twice that much life", "that much life plus 1".</summary>
public sealed record Adjusted(Quantity Base, string Factor = null, string Adjustment = null) : Quantity;

/// <summary>An amount computed from two others: "the number of cards in their hand minus 4".</summary>
public sealed record Arithmetic(Quantity Left, string Operator, Quantity Right) : Quantity;

/// <summary>A player's devotion to colors: the mana symbols of those colors among permanents they control.</summary>
public sealed record Devotion(IReadOnlyList<IrNode> Colors) : Quantity;

/// <summary>A player controlling more or fewer of some permanents than another: "an opponent controls more lands than you".</summary>
public sealed record CompareControl(Selector Player, Selector Objects, string Bound, Selector ComparedTo) : Condition;

/// <summary>A player's hand size: "you have no cards in hand", "that player has seven or more cards in hand".</summary>
public sealed record HandSize(Selector Player, IrNode Size, string Verb = null) : Condition;
