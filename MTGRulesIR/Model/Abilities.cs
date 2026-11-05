namespace MTGRulesIR.Model;

/// <summary>One ability of a card, as one line of its text states it: the frames an engine registers on a card object.</summary>
public abstract record Ability : IrNode;

public sealed record OpaqueAbility(string Text, string Glyph) : Ability, IOpaque;

/// <summary>A line of keyword abilities: "flying, trample".</summary>
/// <param name="Other">Members of the line that didn't lower to a keyword.</param>
public sealed record KeywordLine(IReadOnlyList<KeywordInstance> Keywords, IReadOnlyList<IrNode> Other = null) : Ability;

/// <summary>One keyword, with the parameter it takes, if any: "bushido 1" has an amount, "cycling {2}" a cost, "protection from red" a parameter.</summary>
public sealed record KeywordInstance(string Keyword, Quantity Amount = null, Cost Cost = null, string Parameter = null, StatsMod Stats = null) : IrNode;

/// <summary>A continuous effect the permanent has as long as it's on the battlefield, possibly only while a condition holds.</summary>
public sealed record StaticAbility(Effect Effect, Condition While = null) : Ability;

/// <summary>What an instant or sorcery does when it resolves.</summary>
public sealed record SpellAbility(Effect Effect) : Ability;

/// <summary>An effect-shaped line on a permanent that the lowering hasn't classified as static, triggered or a replacement.</summary>
public sealed record EffectLine(Effect Effect) : Ability;

public sealed record TriggeredAbility(Event Trigger, Effect Effect, Condition If = null) : Ability;

public sealed record ActivatedAbility(IReadOnlyList<Cost> Costs, Effect Effect) : Ability;

/// <summary>A planeswalker's loyalty ability: the loyalty change (negative for "[−3]") and what it does.</summary>
public sealed record LoyaltyAbility(Quantity Change, Effect Effect) : Ability;

/// <summary>A Saga's chapter ability: the chapter numbers it fires on, and what it does.</summary>
public sealed record ChapterAbility(IReadOnlyList<int> Chapters, Effect Effect) : Ability;

/// <summary>One mode of a modal spell or ability.</summary>
public sealed record Mode(Effect Effect, Cost Cost = null) : Ability;

/// <summary>A row of a die-roll table: the results it covers and what happens on them.</summary>
public sealed record DieResultRow(int Low, int High, Effect Effect) : Ability;

/// <summary>A level bracket of a leveler: "level 1-3", "level 4+". The abilities and stats that follow belong to it.</summary>
public sealed record LevelRange(int Low, int High = 0, bool OrMore = false) : Ability;

/// <summary>A power/toughness line of its own, as in a level bracket.</summary>
public sealed record StatsLine(Quantity Power, Quantity Toughness) : Ability;

/// <summary>A row of a station table: the charge counters needed, and the ability or stats gained at that many.</summary>
public sealed record StationThresholdRow(int Threshold, Ability Ability) : Ability;

/// <summary>An aura's "enchant …": what it can be attached to.</summary>
public sealed record EnchantAbility(Selector Target) : Ability;

/// <summary>Limits on when the card's ability may be activated: "activate only as a sorcery", "activate only once each turn".</summary>
public sealed record ActivationLimits(IReadOnlyList<Timing> Timings, bool ForCasting = false) : Ability;

/// <summary>One activation limit: its kind ("SorcerySpeed", "OnceEachTurn", "During", "If", …) and the phase or condition it names.</summary>
public sealed record Timing(string Kind, PhaseRef Phase = null, Condition If = null, string Keyword = null) : IrNode;

/// <summary>An ability under an ability word ("landfall —", "threshold —"); the word only labels it.</summary>
public sealed record LabeledAbility(string Word, Ability Ability) : Ability;

/// <summary>An ability under a flavor word - a name the card gives it, with no rules meaning: "Hero's Reward — …".</summary>
public sealed record FlavorAbility(string Name, Ability Ability) : Ability;

/// <summary>A replacement effect the card states as a line: how it enters the battlefield, or what happens as an event happens.</summary>
public sealed record ReplacementAbility(Effect Effect) : Ability;

/// <summary>A cost paid as well as casting: "as an additional cost to cast this spell, sacrifice a creature".</summary>
public sealed record AdditionalCost(Selector Spells, Cost Cost) : Ability;

/// <summary>A cost that may be paid rather than the usual one: "you may pay {1}{w} rather than pay this spell's mana cost".</summary>
public sealed record AlternativeCost(Cost Cost, string Replaces, Cost Additional = null, Condition If = null) : Ability;

/// <summary>Keywords of which one is chosen: "your choice of flying, trample, or haste".</summary>
public sealed record KeywordChoice(IReadOnlyList<KeywordInstance> Options) : IrNode;

/// <summary>The ability this text is part of, granted elsewhere: "except it has this ability".</summary>
public sealed record SameAbility : Ability;
