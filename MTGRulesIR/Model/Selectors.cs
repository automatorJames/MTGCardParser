namespace MTGRulesIR.Model;

/// <summary>What an effect acts on or who does it: objects picked out by a filter, a player, the card itself, or something named earlier.</summary>
public abstract record Selector : IrNode;

public sealed record OpaqueSelector(string Text, string Glyph) : Selector, IOpaque;

/// <summary>The card itself: "{this}", "this creature", "this spell".</summary>
public sealed record Self : Selector;

/// <summary>The ability this text is part of: "this ability".</summary>
public sealed record SelfAbility : Selector;

/// <summary>How objects are picked from those the filter admits.</summary>
public enum Quantifier
{
    /// <summary>Chosen as a target ("target creature", "two target artifacts").</summary>
    Target,
    /// <summary>Every one, one at a time ("each creature you control").</summary>
    Each,
    /// <summary>Every one, as a group ("all creatures", "creatures you control").</summary>
    All,
    /// <summary>Any one, chosen without targeting ("a creature", "a land card from your hand").</summary>
    Any,
    /// <summary>A number of them, chosen without targeting ("two islands").</summary>
    Count,
    /// <summary>Ones named earlier, by kind ("those creatures", "the exiled card").</summary>
    Those,
    /// <summary>The permanent this one is attached to ("enchanted creature").</summary>
    Attached,
    /// <summary>What was left over after a choice ("the rest").</summary>
    Rest,
}

/// <summary>
/// Objects picked by a filter: how they're picked (<see cref="Quantifier"/>), how many, and the filter. A selector that
/// introduces something later text refers back to carries a <paramref name="Binding"/>, which a <see cref="Reference"/> names.
/// </summary>
public sealed record ObjectSelector(Quantifier Quantifier, ObjectFilter Filter, Quantity Count = null, bool UpTo = false, bool Other = false, string Binding = null) : Selector;

/// <summary>
/// What objects must be: their kinds (card types, subtypes, "token", …), qualifier words ("tapped", "nonblack"),
/// who controls or owns them, further "with …" and relative clauses as <see cref="Predicate"/>s, and the zone
/// they're in when not the battlefield.
/// </summary>
public sealed record ObjectFilter(IReadOnlyList<Term> Kinds = null, IReadOnlyList<Qualifier> Qualifiers = null, Controlled Controller = null, IReadOnlyList<Predicate> With = null, ZoneRef Zone = null) : IrNode;

/// <summary>
/// One qualifier word, possibly negated: "tapped", "nonblack". <paramref name="Vocabulary"/> names the closed set the
/// word belongs to - a grammar vocabulary (a card type, a color, a creature type) or one the lowering defines (see
/// <see cref="Vocabularies"/>) - so the engine switches on the vocabulary first; null means the lowering hasn't
/// classified the word yet.
/// </summary>
public sealed record Qualifier(string Quality, bool Negated = false, string Vocabulary = null) : IrNode;

/// <summary>Who controls or owns something: "you control", "an opponent controls", "you don't control", "target player owns".</summary>
public sealed record Controlled(Selector Player, string Relation = "Control", bool Negated = false) : IrNode;

/// <summary>
/// A further condition on the objects a filter admits, beyond kinds, qualifier words, controller and zone: a "with …"
/// clause, a relative clause, a relation to another object. The family every narrowing of a filter belongs to, so the
/// engine can switch over it; what the lowering can't yet express is an <see cref="OpaquePredicate"/>.
/// </summary>
public abstract record Predicate : IrNode;

public sealed record OpaquePredicate(string Text, string Glyph) : Predicate, IOpaque;

/// <summary>A "with …" qualifier naming keywords the objects have, or lack.</summary>
public sealed record HasKeywords(IReadOnlyList<string> Keywords, bool Negated = false) : Predicate;

/// <summary>A "with …" qualifier naming a keyword with its parameter: "with protection from black", "with toxic 1".</summary>
public sealed record WithKeyword(KeywordInstance Keyword) : Predicate;

/// <summary>Objects that share a controller, or don't: "two target creatures controlled by the same player", "controlled by different players".</summary>
public sealed record SharedController(string Which) : Predicate;

/// <summary>A "with …" qualifier bounding a characteristic: "with power 2 or less".</summary>
public sealed record CharacteristicBound(IReadOnlyList<string> Characteristics, Quantity Value, Comparison Comparison, bool Total = false) : Predicate;

/// <summary>Objects with exactly these stats: "target 1/1 creature".</summary>
public sealed record HasStats(Quantity Power, Quantity Toughness) : Predicate;

/// <summary>How objects stand to another object or group.</summary>
public enum Relation
{
    /// <summary>Taken from the group: "a creature card from among them".</summary>
    Among,
    /// <summary>Placed on the object, said of counters: "charge counters on this artifact".</summary>
    On,
    /// <summary>The object itself, named by a selector that isn't a filter: "discard that card".</summary>
    Is,
    /// <summary>Not the object: "another creature", "other than this creature".</summary>
    OtherThan,
    /// <summary>The source an ability belongs to: "an activated ability of an artifact".</summary>
    AbilityOf,
    /// <summary>Dealt damage by the object: "a creature dealt damage by this creature this turn".</summary>
    DealtDamageBy,
    /// <summary>Attached to the object: "an aura attached to it".</summary>
    AttachedTo,
    /// <summary>Sharing a creature type with the object.</summary>
    SharesCreatureTypeWith,
    /// <summary>Sharing a color with the object.</summary>
    SharesColorWith,
    /// <summary>Sharing a name with the object.</summary>
    SharesNameWith,
    /// <summary>Exiled by the object's effect: "cards exiled with this creature".</summary>
    ExiledWith,
    /// <summary>In the player's party: "creatures in your party".</summary>
    InPartyOf,
    /// <summary>Haunted by the object (haunt): "the creature it haunts".</summary>
    HauntedBy,
}

/// <summary>Objects in a <see cref="Relation"/> to another object or group.</summary>
public sealed record Related(Relation Relation, Selector To) : Predicate;

/// <summary>Objects of a characteristic chosen or named earlier: "of the chosen type", "of that color".</summary>
public sealed record OfChosen(string Characteristic) : Predicate;

/// <summary>Spells cast from a zone: "a spell cast from your graveyard".</summary>
public sealed record CastFrom(ZoneRef Zone) : Predicate;

/// <summary>Spells cast during a phase or turn: "a spell during an opponent's turn".</summary>
public sealed record CastDuring(PhaseRef Phase) : Predicate;

/// <summary>Objects a condition holds of: "creatures that didn't attack this turn".</summary>
public sealed record Satisfying(Condition Condition) : Predicate;

/// <summary>Objects an earlier effect acted on: "each creature put into a graveyard this way".</summary>
public sealed record AffectedBy(Effect Effect) : Predicate;

/// <summary>Cards with a name: "a card named Scion of Darkness".</summary>
public sealed record Named(string Name) : Predicate;

/// <summary>Objects differing from one another in a characteristic: "with different names".</summary>
public sealed record Distinct(string Characteristic) : Predicate;

/// <summary>Objects sharing a characteristic with another: "with the same name as that card".</summary>
public sealed record SharesCharacteristic(string Characteristic, Selector With) : Predicate;

/// <summary>Objects with the greatest or least of a characteristic, among a group: "with the greatest power among creatures you control".</summary>
public sealed record HasExtreme(string Characteristic, string Which, Selector Among = null) : Predicate;

/// <summary>Objects whose characteristic compares to another object's: "with power less than this creature's power".</summary>
public sealed record CharacteristicCompared(string Characteristic, Comparison Comparison, Selector Other, string OtherCharacteristic) : Predicate;

/// <summary>Objects whose characteristic compares to that of the object the clause is about - the ability's source, or what the trigger is about: "with lesser power", "with greater mana value".</summary>
public sealed record RelativeCharacteristic(string Characteristic, Comparison Comparison) : Predicate;

public enum PlayerKind
{
    You,
    EachOpponent,
    EachPlayer,
    EachOtherPlayer,
    AnyOpponent,
    AnyPlayer,
    YourOpponents,
    Players,
    NoPlayer,
    TargetPlayer,
    TargetOpponent,
    DefendingPlayer,
    AttackingPlayer,
    EnchantedPlayer,
    /// <summary>A player named earlier whose binding the lowering couldn't recover ("their", "that player's" as a possessive word).</summary>
    ThatPlayer,
    /// <summary>The controller of <see cref="PlayerSelector.Of"/>.</summary>
    ControllerOf,
    /// <summary>The owner of <see cref="PlayerSelector.Of"/>.</summary>
    OwnerOf,
    /// <summary>A player chosen earlier, or of someone's choice.</summary>
    Chosen,
}

public sealed record PlayerSelector(PlayerKind Kind, Selector Of = null, string Binding = null) : Selector;

/// <summary>
/// Something named earlier in the line - "it", "that creature", "that player" - as the lowering resolved it: the
/// <paramref name="Binding"/> of the selector that introduced it, when that selector was lowered, else the text of
/// the antecedent the grammar found.
/// </summary>
public sealed record Reference(string Binding, string Text, string Antecedent = null) : Selector;

/// <summary>Several selectors together: "you and each creature you control", "target player or planeswalker".</summary>
public sealed record Union(IReadOnlyList<Selector> Members, Connective Conjunction = Connective.And) : Selector;

/// <summary>Who chooses which object an effect applies to: "of their choice".</summary>
public sealed record ChosenBy(Selector Player) : Predicate;

/// <summary>An alternative kind with qualifiers of its own, beside a filter's kinds: "red planeswalker" in "target red creature or red planeswalker".</summary>
public sealed record QualifiedKind(Term Kind, IReadOnlyList<Qualifier> Qualifiers) : Predicate;

/// <summary>Qualifiers any of which may hold: "blue or red", "instant and/or sorcery".</summary>
public sealed record QualifierChoice(IReadOnlyList<Qualifier> Options, Connective Conjunction = Connective.Or) : Predicate;

/// <summary>Several vocabulary members where one is expected: "artifacts, legendaries, and/or sagas".</summary>
public sealed record TermSet(IReadOnlyList<Term> Members) : IrNode;

/// <summary>What a spell targets, as a qualifier on the spell: "that targets this creature".</summary>
public sealed record Targeting(Selector Target, string Count = null) : Predicate;

/// <summary>Who casts (or controls) the spells meant, as a qualifier: "you cast", "your opponents cast during your turn".</summary>
public sealed record CastBy(Selector Player, string Relation, string From = null, bool ThisTurn = false, PhaseRef During = null) : Predicate;

/// <summary>A qualifier on objects carrying counters: "with a +1/+1 counter on it".</summary>
public sealed record WithCounters(CounterSpec Counters, IrNode Other = null) : Predicate;

/// <summary>Something a player did: "cast a spell", "attacked", as an event a condition looks back on.</summary>
public sealed record PlayerAction(Selector Player, string Action, Selector What = null, bool FromHand = false, Quantity Count = null) : Event;

/// <summary>One kind of card a player may play or cast: "play lands", "cast creature spells".</summary>
public sealed record Playable(string Verb, Selector What) : IrNode;

/// <summary>A player compared with another by a measure: "who controls more creatures than you", "who has more life than you".</summary>
public sealed record Compared(string Comparison, Selector Than, Selector Measure = null) : Predicate;

/// <summary>A qualifier on creatures by their combat relation to another: "blocking or blocked by it".</summary>
public sealed record InCombatWith(Selector Other, string Relation = "BlockingOrBlockedBy") : Predicate;

/// <summary>Cards a player did something with over a span: "cards you've drawn this turn".</summary>
public sealed record ActedOn(Selector By, string Action, string Span = "ThisTurn") : Predicate;
