namespace MTGRulesIR.Model;

/// <summary>What an effect acts on or who does it: objects picked out by a filter, a player, the card itself, or something named earlier.</summary>
public abstract record Selector : IrNode;

public sealed record OpaqueSelector(string Text, string Glyph) : Selector, IOpaque;

/// <summary>The card itself: "{this}", "this creature", "this spell".</summary>
public sealed record Self : Selector;

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
/// who controls or owns them, further "with …" and relative clauses (<paramref name="With"/>, lowered where a rule
/// exists), and the zone they're in when not the battlefield.
/// </summary>
public sealed record ObjectFilter(IReadOnlyList<string> Kinds = null, IReadOnlyList<Qualifier> Qualifiers = null, Controlled Controller = null, IReadOnlyList<IrNode> With = null, ZoneRef Zone = null) : IrNode;

/// <summary>One qualifier word, possibly negated: "tapped", "nonblack".</summary>
public sealed record Qualifier(string Quality, bool Negated = false) : IrNode;

/// <summary>Who controls or owns something: "you control", "an opponent controls", "you don't control", "target player owns".</summary>
public sealed record Controlled(Selector Player, string Relation = "Control", bool Negated = false) : IrNode;

/// <summary>A "with …" qualifier naming keywords the objects have, or lack.</summary>
public sealed record HasKeywords(IReadOnlyList<string> Keywords, bool Negated = false) : IrNode;

/// <summary>A "with …" qualifier bounding a characteristic: "with power 2 or less".</summary>
public sealed record CharacteristicBound(IReadOnlyList<string> Characteristics, Quantity Value, string Comparison, bool Total = false) : IrNode;

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
public sealed record Union(IReadOnlyList<Selector> Members, string Conjunction = "and") : Selector;

/// <summary>Who chooses which object an effect applies to: "of their choice".</summary>
public sealed record ChosenBy(Selector Player) : IrNode;

/// <summary>A kind with qualifiers of its own, in a list of kinds: "red planeswalker" in "target red creature or red planeswalker".</summary>
public sealed record QualifiedKind(string Kind, IReadOnlyList<Qualifier> Qualifiers) : IrNode;

/// <summary>Qualifiers any of which may hold: "blue or red", "instant and/or sorcery".</summary>
public sealed record QualifierChoice(IReadOnlyList<Qualifier> Options, string Conjunction = "or") : IrNode;

/// <summary>Several vocabulary members where one is expected: "artifacts, legendaries, and/or sagas".</summary>
public sealed record TermSet(IReadOnlyList<string> Values, string Vocabulary) : IrNode;

/// <summary>What a spell targets, as a qualifier on the spell: "that targets this creature".</summary>
public sealed record Targeting(Selector Target, string Count = null) : IrNode;
