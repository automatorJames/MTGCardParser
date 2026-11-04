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

/// <summary>A source dealing damage, to one recipient or several, perhaps divided among them as the controller chooses, perhaps of a kind (combat, noncombat).</summary>
public sealed record DealDamage(Selector Source, IReadOnlyList<DamagePortion> Portions, string Divided = null, string Kind = null) : Effect;

/// <summary>A creature fighting another, or creatures fighting each other.</summary>
public sealed record Fight(Selector Fighter, Selector Opponent = null, bool EachOther = false) : Effect;

/// <summary>Spells that creatures tapped while casting help pay for: convoke, as an effect on other spells.</summary>
public sealed record Convoke(Selector Spells) : Effect;

/// <summary>Cards shuffled into a library.</summary>
public sealed record ShuffleInto(Selector Cards, ZoneRef Library) : Effect;

/// <summary>A player winning or losing the game, or being unable to: "you lose the game", "you can't lose the game this turn".</summary>
public sealed record GameOutcome(Selector Player, string Result, bool Cant = false, bool ThisTurn = false, bool ForZeroLife = false) : Effect;

/// <summary>Objects becoming, or entering as, a copy of another, with the exceptions the text makes.</summary>
public sealed record BecomeCopy(Selector Objects, Selector Of, string Verb, Duration Duration = null, IReadOnlyList<Effect> Exceptions = null) : Effect;

/// <summary>What creatures may block beyond the usual: only some creatures, an additional creature, any number.</summary>
public sealed record CanBlock(Selector Objects, Selector Only = null, string Count = null, string Period = null) : Effect;

/// <summary>A player barred from playing or casting: "can't cast spells", "can't cast more than one spell each turn", "can't play lands or activate abilities".</summary>
public sealed record CantPlay(Selector Player, string Verb, Selector What = null, Quantity MoreThan = null, Selector OrActivate = null, Duration Duration = null) : Effect;

/// <summary>A player getting an emblem with an ability.</summary>
public sealed record CreateEmblem(Selector Player, Ability Ability) : Effect;

public sealed record FlipCoin(Selector Player) : Effect;

/// <summary>Spells castable as though they had flash, perhaps for a period, perhaps for more mana.</summary>
public sealed record CastAsThoughFlash(Selector Spells, Duration Duration = null, PayMana Extra = null) : Effect;

/// <summary>Permanents their controller may choose not to untap during a step.</summary>
public sealed record MayNotUntap(Selector Objects, PhaseRef Step) : Effect;

public sealed record DamagePortion(Quantity Amount, Selector To) : IrNode;

public sealed record GainLife(Selector Player, Quantity Amount) : Effect;

public sealed record LoseLife(Selector Player, Quantity Amount) : Effect;

public sealed record Destroy(Selector Objects, string Manner = null) : Effect;

public sealed record Exile(Selector Objects, Duration Until = null, CounterSpec WithCounters = null, string Manner = null) : Effect;

/// <summary>
/// Objects getting, having or losing buffs: a power/toughness change, keywords, or both ("gets +1/+1 and has flying
/// until end of turn"), or base power and toughness set outright ("has base power and toughness 4/4").
/// </summary>
public sealed record Pump(Selector Objects, Quantity Power = null, Quantity Toughness = null, IReadOnlyList<KeywordInstance> Keywords = null, StatsMod BaseStats = null, Duration Duration = null, bool Remove = false, Quantity PerEach = null) : Effect;

public sealed record Tap(Selector Objects, IrNode During = null) : Effect;

public sealed record Untap(Selector Objects, IrNode During = null) : Effect;

public sealed record TapOrUntap(Selector Objects, IrNode During = null) : Effect;

public sealed record PutCounters(Selector Objects, CounterSpec Counters) : Effect;

public sealed record RemoveCounters(Selector Objects, CounterSpec Counters) : Effect;

/// <summary>Counters of a kind, counted: "three +1/+1 counters", "a doom counter", "two or more charge counters", "exactly three tide counters".</summary>
public sealed record CounterSpec(string Kind, Quantity Count, bool Additional = false, string Bound = null, string Prefix = null) : IrNode;

/// <summary>Creating tokens: the token, how many, and a per-each multiplier.</summary>
public sealed record CreateTokens(Token Token, Quantity Count = null, Quantity PerEach = null) : Effect;

/// <summary>What a created token is: described in full (<see cref="TokenSpec"/>), or something the lowering can't yet say (a copy of a permanent, for now).</summary>
public abstract record Token : IrNode;

public sealed record OpaqueToken(string Text, string Glyph) : Token, IOpaque;

/// <summary>A token that is a copy of another object, with the exceptions the text makes ("except it's a 1/1").</summary>
public sealed record TokenCopy(Selector Of, bool Tapped = false, StatsMod Stats = null, IReadOnlyList<Effect> Exceptions = null) : Token;

/// <summary>A token described in full: its types, subtypes, colors, stats, name, keywords, and the abilities it has beyond keywords.</summary>
public sealed record TokenSpec(IReadOnlyList<string> Types = null, IReadOnlyList<string> Subtypes = null, IReadOnlyList<string> Colors = null, Quantity Power = null, Quantity Toughness = null, string Name = null, IReadOnlyList<KeywordInstance> Keywords = null, IReadOnlyList<Ability> Abilities = null, bool Tapped = false, bool Attacking = false, string Predefined = null) : Token;

public sealed record Sacrifice(Selector Player, Selector Objects) : Effect;

/// <summary>Moving objects to a zone ("return … to its owner's hand", "put … onto the battlefield tapped under your control").</summary>
public sealed record ChangeZone(Selector Objects, ZoneRef To, bool Tapped = false, Selector UnderControlOf = null, IReadOnlyList<string> States = null, IReadOnlyList<Effect> With = null) : Effect;

/// <summary>Adding mana: one option, or a choice among several ("add {b} or {r}").</summary>
public sealed record AddMana(IReadOnlyList<ManaOption> Options, string Amount = null) : Effect;

/// <summary>One way mana can be added: a fixed amount of symbols, or a choice ("one mana of any color").</summary>
public abstract record ManaOption : IrNode;

public sealed record OpaqueManaOption(string Text, string Glyph) : ManaOption, IOpaque;

public sealed record ManaAmount(IReadOnlyList<string> Symbols) : ManaOption;

/// <summary>Objects that can't be the target of some spells or abilities.</summary>
public sealed record CantBeTargeted(Selector Objects, IReadOnlyList<string> By = null, Controlled Controller = null, Duration Duration = null) : Effect;

/// <summary>
/// A combat restriction: what the objects can't do ("attack", "block"), whom it concerns (<paramref name="By"/>: the
/// creatures that can't block them, or that they can't block), the player it concerns ("can't attack you"), modifiers
/// ("alone", "except by"), and a condition under which it holds.
/// </summary>
public sealed record CombatRestriction(Selector Objects, IReadOnlyList<string> Cant, Selector By = null, string Recipient = null, IReadOnlyList<string> Modifiers = null, Condition If = null, Duration Duration = null) : Effect;

/// <summary>Keyword actions done as an effect: "scry 2", "investigate", "bolster 1".</summary>
public sealed record KeywordAction(IReadOnlyList<KeywordInstance> Actions, IReadOnlyList<IrNode> Other = null) : Effect;

/// <summary>The head of a modal spell or ability: how many modes are chosen, and by whom when not the controller. The modes follow as <see cref="Mode"/> lines.</summary>
public sealed record ChooseModes(string Count, Selector Chooser = null) : Effect;

/// <summary>A choice among effects: riot's "with your choice of a +1/+1 counter or haste".</summary>
public sealed record ChoiceOf(IReadOnlyList<Effect> Options) : Effect;

/// <summary>How a permanent enters the battlefield: tapped, with counters, with an ability, under a condition.</summary>
public sealed record EntersBattlefield(Selector Objects, IReadOnlyList<string> States = null, IReadOnlyList<Effect> With = null, Condition If = null, Duration Duration = null) : Effect;

/// <summary>Searching zones for cards and putting them somewhere.</summary>
public sealed record SearchLibrary(Selector Searcher, string Zones, IReadOnlyList<Selector> Wanted, bool Reveal = false, Effect Placement = null, Effect Ending = null, bool Optional = false) : Effect;

public sealed record Shuffle(Selector Player = null) : Effect;

/// <summary>An effect with a variable defined after it: "…, where x is the number of cards in your hand".</summary>
public sealed record LetVariable(Effect Effect, string Name, Quantity Definition) : Effect;

/// <summary>Paying a cost as an effect: "you may pay {g}", "unless you pay {u}".</summary>
public sealed record PayCost(Cost Cost, Selector Player = null, Quantity PerEach = null, IrNode Plus = null) : Effect;

/// <summary>An effect that happens unless a payment is made.</summary>
public sealed record UnlessPaid(Effect Effect, PayCost Payment) : Effect;

public sealed record PreventDamage(Quantity Amount, string Kind = null, Selector To = null, Selector Source = null, Duration Duration = null, bool ToAndBy = false) : Effect;

public sealed record Reveal(Selector Player, Selector What) : Effect;

public sealed record LookAt(Selector Player, Selector What) : Effect;

/// <summary>An effect that lasts for a duration stated apart from it: "until end of turn, you may play that card".</summary>
public sealed record Lasting(Effect Effect, Duration Duration) : Effect;

/// <summary>Objects granted a whole ability ("has '{t}: add one mana of any color'"), possibly with keywords alongside.</summary>
public sealed record GrantAbility(Selector Objects, Ability Ability, IReadOnlyList<KeywordInstance> Alongside = null) : Effect;

public sealed record Mill(Selector Player, Quantity Count) : Effect;

/// <summary>A change to what spells or abilities cost.</summary>
public sealed record CostModification(Selector Spells, PayMana Amount, string Change, string Action = null, Quantity PerEach = null, Condition Condition = null) : Effect;

/// <summary>Something done as an event happens rather than after it: "as {this} enters, choose an opponent".</summary>
public sealed record AsEventHappens(Event When, Effect Effect) : Effect;

/// <summary>A reflexive trigger: "when you do, …".</summary>
public sealed record WhenDone(Selector Player, Effect Effect) : Effect;

public sealed record Regenerate(Selector Objects) : Effect;

/// <summary>A permanent becoming a creature or another kind of permanent, for a time.</summary>
public sealed record Animate(Selector Objects, Quantity Power = null, Quantity Toughness = null, IReadOnlyList<string> Colors = null, IReadOnlyList<string> Subtypes = null, IReadOnlyList<string> Types = null, StatsMod BaseStats = null, IReadOnlyList<KeywordInstance> Keywords = null, bool AllCreatureTypes = false, string Still = null, bool InAddition = false, Duration Duration = null) : Effect;

public sealed record DoesntUntap(Selector Objects, PhaseRef Step, Condition If = null) : Effect;

public sealed record CounterSpell(Selector Objects) : Effect;

public sealed record ChangeTargets(Selector Spell) : Effect;

public sealed record ChooseCards(Selector Chooser, Quantity Count, ObjectFilter Filter, Selector From, bool UpTo = false) : Effect;

public sealed record Attach(Selector Attachment, Selector To) : Effect;

/// <summary>An animated permanent keeping a type it had: "it's still a land".</summary>
public sealed record StillOfKind(IReadOnlyList<string> Kinds) : Effect;

public sealed record Transform(Selector Objects) : Effect;

/// <summary>A choice made as part of an effect: "choose an opponent", "choose a color".</summary>
/// <summary>A player choosing something: a kind of thing ("choose a color", "choose a creature type"), or an object or player ("choose a creature"), possibly with exceptions ("other than wall").</summary>
public sealed record ChooseEffect(Selector Chooser, IReadOnlyList<string> Kinds = null, IReadOnlyList<Selector> From = null, IReadOnlyList<string> Except = null) : Effect;

/// <summary>Playing or casting a card: "cast it without paying its mana cost", "play that card this turn".</summary>
public sealed record PlayCard(Selector Player, Selector Card, string From = null, bool Free = false, IrNode ByPaying = null, Duration Duration = null) : Effect;

/// <summary>Objects becoming, or being, some aspects: a color, a type, a designation; "is a black zombie in addition to its other colors and types", "becomes white until end of turn".</summary>
public sealed record BecomesAspect(Selector Objects, IReadOnlyList<Term> Aspects, bool InAddition = false, bool Negated = false, Duration Duration = null, string Verb = null) : Effect;

public sealed record GainControl(Selector Player, Selector Objects, Duration Duration = null) : Effect;

/// <summary>A restriction on how mana just added may be spent.</summary>
public sealed record ManaRestriction(string Rule, IReadOnlyList<Playable> Uses) : Effect;

/// <summary>An effect that replaces the usual one: "… instead".</summary>
public sealed record Instead(Effect Effect) : Effect;

public sealed record CopySpell(Selector Spell, Quantity Times = null, bool NewTargets = false, IrNode Except = null) : Effect;

public sealed record GetEnergy(Selector Player, Quantity Amount) : Effect;

/// <summary>A forced attack or block: "attacks each combat if able".</summary>
public sealed record MustCombat(Selector Objects, IReadOnlyList<string> Actions, string Period = null) : Effect;

/// <summary>A replacement effect: what would happen, and what happens instead.</summary>
public sealed record Replacement(Event Would, Effect Instead, Duration Duration = null, bool NextTime = false) : Effect;

/// <summary>An effect done once per counted thing: "draw a card for each creature you control".</summary>
public sealed record ForEachOf(Effect Effect, Quantity Count) : Effect;

/// <summary>A combat permission that ignores a keyword: "can attack as though it didn't have defender".</summary>
public sealed record CanAsThough(Selector Objects, string Action, string Keyword, bool Had, Selector Blocked = null, Duration During = null, Duration Duration = null) : Effect;

public sealed record CantBeRegenerated(Selector Objects, Duration Duration = null) : Effect;

/// <summary>Revealing cards from the top of a library until one of some kind turns up.</summary>
public sealed record RevealUntil(Selector Player, string Verb, ZoneRef Library, Event Until) : Effect;

/// <summary>A standing permission to play cards from somewhere other than the hand.</summary>
public sealed record MayPlayFrom(Selector Player, IReadOnlyList<Playable> Plays, ZoneRef From, bool Free = false, IrNode Using = null, Duration Duration = null) : Effect;

/// <summary>The alternative to a preceding condition or choice: "otherwise, …".</summary>
public sealed record Otherwise(Effect Effect) : Effect;

/// <summary>Setting power and/or toughness to an amount: "{this}'s power and toughness are each equal to the number of swamps you control".</summary>
public sealed record SetStats(Selector Objects, string Which, Quantity Amount) : Effect;
