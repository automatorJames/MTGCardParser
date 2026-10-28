namespace MTGRulesIR.Model;

/// <summary>Something that happens to the game, one-shot or continuous: the payload of every ability frame.</summary>
public abstract record Effect : IrNode;

public sealed record OpaqueEffect(string Text, string Glyph) : Effect, IOpaque;

/// <summary>Effects done in order.</summary>
public sealed record Sequence(IReadOnlyList<Effect> Steps) : Effect;

/// <summary>An effect done only if a condition holds.</summary>
public sealed record Conditional(Condition If, Effect Then, Effect Else = null) : Effect;

/// <summary>An effect a player may choose to do ("you may draw a card").</summary>
public sealed record Optional(Effect Effect, Selector Chooser = null) : Effect;

/// <summary>An effect put off to a later point in the turn ("destroy it at the beginning of the next end step").</summary>
public sealed record Delayed(Event When, Effect Effect) : Effect;

public sealed record Draw(Selector Player, Quantity Count, bool UpTo = false, bool Additional = false) : Effect;

public sealed record Discard(Selector Player, Quantity Count, ObjectFilter Filter = null, bool AtRandom = false) : Effect;

/// <summary>A source dealing damage, to one recipient or several.</summary>
public sealed record DealDamage(Selector Source, IReadOnlyList<DamagePortion> Portions) : Effect;

public sealed record DamagePortion(Quantity Amount, Selector To) : IrNode;

public sealed record GainLife(Selector Player, Quantity Amount) : Effect;

public sealed record LoseLife(Selector Player, Quantity Amount) : Effect;

public sealed record Destroy(Selector Objects, string Manner = null) : Effect;

public sealed record Exile(Selector Objects, Duration Until = null, IrNode WithCounters = null, string Manner = null) : Effect;

/// <summary>
/// Objects getting, having or losing buffs: a power/toughness change, keywords, or both ("gets +1/+1 and has flying
/// until end of turn"). Buffs the lowering doesn't model sit in <paramref name="OtherBuffs"/>.
/// </summary>
public sealed record Pump(Selector Objects, Quantity Power = null, Quantity Toughness = null, IReadOnlyList<KeywordInstance> Keywords = null, IReadOnlyList<IrNode> OtherBuffs = null, Duration Duration = null, bool Remove = false, Quantity PerEach = null) : Effect;

public sealed record Tap(Selector Objects, IrNode During = null) : Effect;

public sealed record Untap(Selector Objects, IrNode During = null) : Effect;

public sealed record TapOrUntap(Selector Objects, IrNode During = null) : Effect;

public sealed record PutCounters(Selector Objects, CounterSpec Counters) : Effect;

public sealed record RemoveCounters(Selector Objects, CounterSpec Counters) : Effect;

/// <summary>Counters of a kind, counted: "three +1/+1 counters", "a doom counter", "two or more charge counters".</summary>
public sealed record CounterSpec(string Kind, Quantity Count, bool Additional = false, string Bound = null, IrNode Other = null) : IrNode;

/// <summary>Creating tokens: the token (a <see cref="TokenSpec"/> when described in full), how many, and a per-each multiplier.</summary>
public sealed record CreateTokens(IrNode Token, Quantity Count = null, Quantity PerEach = null) : Effect;

public sealed record TokenSpec(IReadOnlyList<string> Types = null, IReadOnlyList<string> Subtypes = null, IReadOnlyList<string> Colors = null, Quantity Power = null, Quantity Toughness = null, string Name = null, IReadOnlyList<KeywordInstance> Keywords = null, IReadOnlyList<IrNode> Other = null, bool Tapped = false, bool Attacking = false, string Predefined = null) : IrNode;

public sealed record Sacrifice(Selector Player, Selector Objects) : Effect;

/// <summary>Moving objects to a zone ("return … to its owner's hand", "put … onto the battlefield tapped under your control").</summary>
public sealed record ChangeZone(Selector Objects, ZoneRef To, bool Tapped = false, Selector UnderControlOf = null, IReadOnlyList<IrNode> Details = null) : Effect;

/// <summary>Adding mana: one option, or a choice among several ("add {b} or {r}").</summary>
public sealed record AddMana(IReadOnlyList<IrNode> Options, string Amount = null) : Effect;

public sealed record ManaAmount(IReadOnlyList<string> Symbols) : IrNode;

/// <summary>Objects that can't be the target of some spells or abilities.</summary>
public sealed record CantBeTargeted(Selector Objects, IReadOnlyList<string> By = null, IrNode Controller = null, Duration Duration = null) : Effect;

/// <summary>A combat restriction: what the objects can't do ("attack", "block"), and the details that qualify it.</summary>
public sealed record CombatRestriction(Selector Objects, IReadOnlyList<string> Cant, IReadOnlyList<IrNode> Details = null, Duration Duration = null) : Effect;

/// <summary>Keyword actions done as an effect: "scry 2", "investigate", "bolster 1".</summary>
public sealed record KeywordAction(IReadOnlyList<KeywordInstance> Actions, IReadOnlyList<IrNode> Other = null) : Effect;

/// <summary>The head of a modal spell or ability: how many modes are chosen, and by whom when not the controller. The modes follow as <see cref="Mode"/> lines.</summary>
public sealed record ChooseModes(string Count, Selector Chooser = null) : Effect;

/// <summary>A choice among effects: riot's "with your choice of a +1/+1 counter or haste".</summary>
public sealed record ChoiceOf(IReadOnlyList<Effect> Options) : Effect;

/// <summary>How a permanent enters the battlefield: tapped, with counters, with an ability, under a condition.</summary>
public sealed record EntersBattlefield(Selector Objects, IReadOnlyList<string> States = null, IReadOnlyList<IrNode> With = null, Condition If = null) : Effect;

/// <summary>Searching zones for cards and putting them somewhere.</summary>
public sealed record SearchLibrary(Selector Searcher, string Zones, IReadOnlyList<IrNode> Wanted, bool Reveal = false, IrNode Placement = null, IrNode Ending = null, bool Optional = false) : Effect;

public sealed record Shuffle(Selector Player = null) : Effect;

/// <summary>An effect with a variable defined after it: "…, where x is the number of cards in your hand".</summary>
public sealed record LetVariable(Effect Effect, string Name, Quantity Definition) : Effect;

/// <summary>Paying a cost as an effect: "you may pay {g}", "unless you pay {u}".</summary>
public sealed record PayCost(Cost Cost, Selector Player = null, Quantity PerEach = null, IrNode Plus = null) : Effect;

/// <summary>An effect that happens unless a payment is made.</summary>
public sealed record UnlessPaid(Effect Effect, IrNode Payment) : Effect;

public sealed record PreventDamage(IrNode Amount, string Kind = null, Selector To = null, Selector Source = null, Duration Duration = null, bool ToAndBy = false) : Effect;

public sealed record Reveal(Selector Player, Selector What) : Effect;

public sealed record LookAt(Selector Player, Selector What) : Effect;

/// <summary>An effect that lasts for a duration stated apart from it: "until end of turn, you may play that card".</summary>
public sealed record Lasting(Effect Effect, Duration Duration) : Effect;

/// <summary>Objects granted a whole ability ("has '{t}: add one mana of any color'"), possibly with keywords alongside.</summary>
public sealed record GrantAbility(Selector Objects, Ability Ability, IReadOnlyList<KeywordInstance> Alongside = null) : Effect;

public sealed record Mill(Selector Player, Quantity Count) : Effect;

/// <summary>A change to what spells or abilities cost.</summary>
public sealed record CostModification(Selector Spells, PayMana Amount, string Change, string Action = null, Quantity PerEach = null, IrNode Condition = null) : Effect;

/// <summary>Something done as an event happens rather than after it: "as {this} enters, choose an opponent".</summary>
public sealed record AsEventHappens(Event When, Effect Effect) : Effect;

/// <summary>A reflexive trigger: "when you do, …".</summary>
public sealed record WhenDone(Selector Player, Effect Effect) : Effect;

public sealed record Regenerate(Selector Objects) : Effect;

/// <summary>A permanent becoming a creature or another kind of permanent, for a time.</summary>
public sealed record Animate(Selector Objects, Quantity Power = null, Quantity Toughness = null, IReadOnlyList<string> Colors = null, IReadOnlyList<string> Subtypes = null, IReadOnlyList<string> Types = null, IrNode BaseStats = null, IReadOnlyList<KeywordInstance> Keywords = null, bool AllCreatureTypes = false, string Still = null, bool InAddition = false, Duration Duration = null) : Effect;

public sealed record DoesntUntap(Selector Objects, PhaseRef Step, Condition If = null) : Effect;

public sealed record CounterSpell(Selector Objects) : Effect;

public sealed record ChangeTargets(Selector Spell) : Effect;

public sealed record ChooseCards(Selector Chooser, Quantity Count, ObjectFilter Filter, Selector From, bool UpTo = false) : Effect;

public sealed record Attach(Selector Attachment, Selector To) : Effect;

/// <summary>An animated permanent keeping a type it had: "it's still a land".</summary>
public sealed record StillOfKind(IReadOnlyList<string> Kinds) : Effect;

public sealed record Transform(Selector Objects) : Effect;

/// <summary>A choice made as part of an effect: "choose an opponent", "choose a color".</summary>
public sealed record ChooseEffect(Selector Chooser, IReadOnlyList<IrNode> Choices, IrNode Except = null) : Effect;

/// <summary>Playing or casting a card: "cast it without paying its mana cost", "play that card this turn".</summary>
public sealed record PlayCard(Selector Player, Selector Card, string From = null, bool Free = false, IrNode ByPaying = null, Duration Duration = null) : Effect;

/// <summary>Objects becoming, or being, some aspects: a color, a type, a designation; "is a black zombie in addition to its other colors and types", "becomes white until end of turn".</summary>
public sealed record BecomesAspect(Selector Objects, IReadOnlyList<IrNode> Aspects, bool InAddition = false, bool Negated = false, Duration Duration = null, string Verb = null) : Effect;

public sealed record GainControl(Selector Player, Selector Objects, Duration Duration = null) : Effect;

/// <summary>A restriction on how mana just added may be spent.</summary>
public sealed record ManaRestriction(string Rule, IReadOnlyList<IrNode> Uses) : Effect;

/// <summary>An effect that replaces the usual one: "… instead".</summary>
public sealed record Instead(Effect Effect) : Effect;

public sealed record CopySpell(Selector Spell, IrNode Times = null, bool NewTargets = false, IrNode Except = null) : Effect;

public sealed record GetEnergy(Selector Player, Quantity Amount) : Effect;

/// <summary>A forced attack or block: "attacks each combat if able".</summary>
public sealed record MustCombat(Selector Objects, IReadOnlyList<string> Actions, string Period = null) : Effect;

/// <summary>A replacement effect: what would happen, and what happens instead.</summary>
public sealed record Replacement(Event Would, Effect Instead, Duration Duration = null) : Effect;

/// <summary>An effect done once per counted thing: "draw a card for each creature you control".</summary>
public sealed record ForEachOf(Effect Effect, Quantity Count) : Effect;

/// <summary>A combat permission that ignores a keyword: "can attack as though it didn't have defender".</summary>
public sealed record CanAsThough(Selector Objects, string Action, string Keyword, bool Had, IReadOnlyList<IrNode> Details = null, Duration Duration = null) : Effect;

public sealed record CantBeRegenerated(Selector Objects, Duration Duration = null) : Effect;

/// <summary>Revealing cards from the top of a library until one of some kind turns up.</summary>
public sealed record RevealUntil(Selector Player, string Verb, ZoneRef Library, IrNode Until) : Effect;

/// <summary>A standing permission to play cards from somewhere other than the hand.</summary>
public sealed record MayPlayFrom(Selector Player, IReadOnlyList<IrNode> Plays, IrNode From, bool Free = false, IrNode Using = null, Duration Duration = null) : Effect;

/// <summary>The alternative to a preceding condition or choice: "otherwise, …".</summary>
public sealed record Otherwise(Effect Effect) : Effect;

/// <summary>Setting power and/or toughness to an amount: "{this}'s power and toughness are each equal to the number of swamps you control".</summary>
public sealed record SetStats(Selector Objects, string Which, Quantity Amount) : Effect;
