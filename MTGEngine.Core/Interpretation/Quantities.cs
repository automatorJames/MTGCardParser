namespace MTGEngine.Core;

/// <summary>Quantities as numbers in the game now; null when the engine can't evaluate one.</summary>
public static class Quantities
{
    static readonly ImmutableHashSet<string> _basicLandTypes = ["Plains", "Island", "Swamp", "Mountain", "Forest"];
    static readonly ImmutableHashSet<string> _permanentTypes = ["Artifact", "Battle", "Creature", "Enchantment", "Land", "Planeswalker"];

    static readonly string[] _partyRoles = ["Cleric", "Rogue", "Warrior", "Wizard"];

    /// <summary>The most party roles the creatures can fill, each creature filling one.</summary>
    static int PartySize(List<Characteristics> creatures)
    {
        int Best(int role, HashSet<int> used)
        {
            if (role == _partyRoles.Length)
                return 0;
            var best = Best(role + 1, used);
            for (var i = 0; i < creatures.Count; i++)
                if (!used.Contains(i) && (creatures[i].Subtypes.Contains(_partyRoles[role]) || creatures[i].HasKeyword("Changeling")))
                {
                    used.Add(i);
                    best = Math.Max(best, 1 + Best(role + 1, used));
                    used.Remove(i);
                }
            return best;
        }
        return Best(0, []);
    }

    public static int? Evaluate(Quantity quantity, EffectContext ctx)
    {
        switch (quantity)
        {
            case null:
                return 1;
            case Fixed fixedValue:
                return fixedValue.Value;
            case Variable { Name: "X" or "x" }:
                return ctx.X;
            case CountOf { Of: ObjectSelector { Filter.Kinds: [{ Value: nameof(ObjectKind.Counter) }] } counters }:
            {
                var kind = counters.Filter.Qualifiers?.FirstOrDefault(q => q.Vocabulary == "CounterKind")?.Quality;
                var on = counters.Filter.With?.OfType<Related>().FirstOrDefault(r => r.Relation == Relation.On)?.To;
                var holders = on is null ? ctx.State.Battlefield.Select(x => x.Id).ToImmutableList() : Resolver.Objects(on, ctx);
                return holders.Select(ctx.State.Object).Where(x => x is not null)
                    .Sum(x => kind is null ? x.CounterMap.Where(c => c.Key != "deathtouch-damage" && c.Key != "regeneration-shield").Sum(c => c.Value) : x.CountersOf(kind));
            }
            // "The number of creatures in your party": up to one each of cleric, rogue, warrior and wizard.
            case CountOf { Of: ObjectSelector { Filter.With: { } with } party } when with.OfType<Related>().FirstOrDefault(r => r.Relation == Relation.InPartyOf) is { } inParty:
            {
                var players = Resolver.Players(inParty.To, ctx);
                return players.Sum(player => PartySize(ctx.State.Battlefield.Where(x => x.Controller == player).Select(x => Characteristics.Of(ctx.State, x)).Where(x => x.IsCreature).ToList()));
            }
            // Strive: "for each target beyond the first", of the targets chosen as the spell is cast.
            case CountOf { Of: ObjectSelector { Filter: { Kinds: [{ Value: nameof(ObjectKind.Target) }], Qualifiers: [{ Quality: nameof(Distinction.BeyondTheFirst) }] } } }:
                return Math.Max(0, ctx.TargetsBySelector.Values.Sum(x => x.Count) - 1);
            // "The number of cards you've drawn this turn", "other spells you've cast this turn", "permanents you've sacrificed this turn".
            case CountOf { Of: ObjectSelector { Filter.With: { } with } acted } when with.OfType<ActedOn>().FirstOrDefault() is { Span: "ThisTurn" or null } actedOn:
            {
                var players = Resolver.Players(actedOn.By, ctx).ToHashSet();
                var rest = acted.Filter with { Kinds = null, Zone = null, With = with.Where(x => x is not ActedOn).ToList() };
                bool Fits(ObjectId? id) =>
                    id is { } some && ctx.State.Object(some) is { } obj && !(acted.Other && some == ctx.Source)
                    && Filters.Matches(ctx.State, rest, obj with { Zone = Zone.Battlefield }, ctx.Controller, ctx.Source, ctx: ctx);
                var events = ctx.State.ThisTurnEventList;
                return actedOn.Action switch
                {
                    "Cast" => events.OfType<SpellCast>().Count(x => players.Contains(x.Item.Controller) && Fits(x.Item.Spell)),
                    "Drawn" => events.OfType<Drew>().Count(x => players.Contains(x.Player) && x.Card is not null),
                    "Discarded" or "CycledOrDiscarded" => events.OfType<Discarded>().Count(x => players.Contains(x.Player) && Fits(x.Card)),
                    "Sacrificed" => events.OfType<Sacrificed>().Count(x => players.Contains(x.Player) && Fits(x.Object)),
                    _ => null,
                };
            }
            case CountOf count:
                return Resolver.Targets(count.Of, ctx).Count;
            case CountersItHad had:
                return ctx.Source is { } holder && ctx.State.Object(holder) is { } held
                    ? (held.Zone == Zone.Battlefield ? held : ctx.State.LastKnownOf(holder) ?? held).CountersOf(had.Kind)
                    : 0;
            case BlockersBeyondFirst:
                return ctx.Source is { } blocked ? Math.Max(0, ctx.State.Battlefield.Count(x => x.Blocking == blocked) - 1) : 0;
            case ColorsSpentToCast:
                return ctx.Source is { } spent && ctx.State.Object(spent)?.SpentToCast is { } pool ? Colors.All.Count(c => pool.Of(c) > 0) : 0;
            // "You gain life equal to the life lost this way": what the step before did.
            case AmountThisWay { Kind: "LifeLost" or "DamageDealt" or "DamagePrevented" }:
                return ctx.LastAmount;
            case TotalOf total:
                return Resolver.Objects(total.Of, ctx).Select(ctx.State.Object).Where(x => x is not null)
                    .Sum(x => Filters.CharacteristicValue(total.Characteristic, x, Characteristics.Of(ctx.State, x)));
            case LifeTotal life:
                return Resolver.Players(life.Player, ctx) is [var player, ..] ? ctx.State.Player(player).Life : null;
            case Negated negated:
                return -Evaluate(negated.Of, ctx);
            case Product product:
                return Evaluate(product.Left, ctx) * Evaluate(product.Right, ctx);
            case Arithmetic arithmetic:
            {
                var left = Evaluate(arithmetic.Left, ctx);
                var right = Evaluate(arithmetic.Right, ctx);
                return arithmetic.Operator switch
                {
                    "Plus" or "+" => left + right,
                    "Minus" or "-" => left - right,
                    _ => null,
                };
            }
            case Adjusted adjusted:
            {
                var value = Evaluate(adjusted.Base, ctx);
                var adjustment = adjusted.Adjustment is null ? 0 : Evaluate(adjusted.Adjustment, ctx);
                return adjusted.Factor switch
                {
                    "Twice" or "Double" => value * 2 + (adjusted.Minus ? -adjustment : adjustment),
                    null => value + (adjusted.Minus ? -adjustment : adjustment),
                    _ => null,
                };
            }
            case Scaled scaled:
            {
                var value = Evaluate(scaled.Base, ctx);
                if (value is null)
                    return null;
                return scaled.Factor switch
                {
                    "Half" => scaled.Rounding == "Up" ? (value.Value + 1) / 2 : value.Value / 2,
                    "Twice" => value * 2,
                    "ThreeTimes" => value * 3,
                    "AThird" => scaled.Rounding == "Up" ? (value.Value + 2) / 3 : value.Value / 3,
                    _ => null,
                };
            }
            case CharacteristicOf characteristic:
            {
                var objects = Resolver.Objects(characteristic.Of, ctx);
                // "Gain life equal to that creature card's toughness" when no card was chosen: nothing.
                if (objects.Count == 0)
                    return characteristic.Of is ObjectSelector { Quantifier: Quantifier.Those } ? 0 : null;
                var obj = ctx.State.Object(objects[0]) ?? ctx.State.LastKnownOf(objects[0]);
                return obj is null ? null : Filters.CharacteristicValue(characteristic.Characteristic, obj, Characteristics.Of(ctx.State, obj));
            }
            case Special { Kind: "ThatMany" or "ThatMuch" }:
                return ctx.LastAmount;
            // "For each 1 damage prevented this way": what the shield just stopped.
            case DamageOrLifeTotal { Unit: "Damage", Change: "Prevented", When: "ThisWay" }:
                return ctx.LastAmount;
            case DamageOrLifeTotal { When: "ThisTurn", Player: { } whose } total when total.Unit == "Life" && total.Change is "Gained" or "Lost" || total.Unit == "Damage" && total.Change == "Dealt":
            {
                var players = Resolver.Players(whose, ctx).ToHashSet();
                var events = ctx.State.ThisTurnEventList;
                return total.Change switch
                {
                    "Gained" => events.OfType<LifeChanged>().Where(x => x.Delta > 0 && players.Contains(x.Player)).Sum(x => x.Delta),
                    "Lost" => events.OfType<LifeChanged>().Where(x => x.Delta < 0 && players.Contains(x.Player)).Sum(x => -x.Delta),
                    _ => events.OfType<DamageDealt>().Where(x => x.Target.IsPlayer && players.Contains(x.Target.Player.Value)).Sum(x => x.Amount),
                };
            }
            case Special { Kind: "All" }:
                return int.MaxValue;
            case Bound bound:
                return Evaluate(bound.Value, ctx);
            case Extreme extreme:
            {
                var values = Resolver.Objects(extreme.Among, ctx).Select(ctx.State.Object).Where(x => x is not null).Select(x => Filters.CharacteristicValue(extreme.Characteristic, x, Characteristics.Of(ctx.State, x))).ToList();
                if (values.Count == 0)
                    return 0;
                return extreme.Which == "Least" ? values.Min() : values.Max();
            }
            // Converge: "the number of colors of mana spent to cast this spell".
            case DistinctCount { Aspect: "Color" or "Colors", Among: ObjectSelector { Filter.Kinds: [{ Value: nameof(ObjectKind.ManaSpent) }] } }:
                return Evaluate(new ColorsSpentToCast(), ctx);
            case DistinctCount distinct:
            {
                var objects = Resolver.Objects(distinct.Among, ctx).Select(ctx.State.Object).Where(x => x is not null).ToList();
                return distinct.Aspect switch
                {
                    "Color" or "Colors" => objects.SelectMany(x => Characteristics.Of(ctx.State, x).Colors).Distinct().Count(),
                    "CardType" or "CardTypes" => objects.SelectMany(x => Characteristics.Of(ctx.State, x).Types).Distinct().Count(),
                    "CreatureType" or "CreatureTypes" => objects.SelectMany(x => Characteristics.Of(ctx.State, x).Subtypes).Distinct().Count(),
                    "Name" or "Names" => objects.Select(x => x.Card.Name).Distinct().Count(),
                    // Domain: Plains, Island, Swamp, Mountain and Forest among them.
                    "BasicLandTypes" or "BasicLandType" => objects.SelectMany(x => Characteristics.Of(ctx.State, x).Subtypes).Where(_basicLandTypes.Contains).Distinct().Count(),
                    "ManaValues" or "ManaValue" => objects.Select(x => x.Card.ManaCost.ManaValue).Distinct().Count(),
                    "PermanentTypes" => objects.SelectMany(x => Characteristics.Of(ctx.State, x).Types).Where(_permanentTypes.Contains).Distinct().Count(),
                    _ => null,
                };
            }
            case Devotion devotion:
            {
                var colors = devotion.Colors.Select(Colors.OfName).Where(x => x is not null).Select(x => x.Value).ToList();
                return ctx.State.Battlefield.Where(x => x.Controller == ctx.Controller).Sum(x => colors.Sum(c => x.Card.ManaCost.Colored.GetValueOrDefault(c)) + x.Card.ManaCost.Hybrid.Count(h => h.Any(colors.Contains)));
            }
            default:
                return null;
        }
    }
}

/// <summary>Conditions as true or false in the game now; null when the engine can't evaluate one.</summary>
public static class Conditions
{
    public static bool? Evaluate(Condition condition, EffectContext ctx)
    {
        var state = ctx.State;
        switch (condition)
        {
            case null:
                return true;
            case SourceTapped:
                return ctx.Source is { } tappedSource && ctx.State.Object(tappedSource) is { Tapped: true };
            case AtMaxSpeed:
                return ctx.State.Player(ctx.Controller).Speed >= 4;
            case Evolves:
            {
                if (ctx.Source is not { } evolving || state.Object(evolving) is not { } self || ctx.TriggerSubject is not { IsObject: true } entered || state.Object(entered.Object.Value) is not { } other)
                    return null;
                var mine = Characteristics.Of(state, self);
                var theirs = Characteristics.Of(state, other);
                return theirs.Power > mine.Power || theirs.Toughness > mine.Toughness;
            }
            case And and:
            {
                bool? result = true;
                foreach (var part in and.Conditions)
                {
                    var value = Evaluate(part, ctx);
                    if (value is null)
                        return null;
                    result &= value;
                }
                return result;
            }
            case Not not:
                return !Evaluate(not.Condition, ctx);
            case WasCast wasCast:
            {
                var objects = Resolver.Objects(wasCast.Spell ?? new Self(), ctx).Select(state.Object).Where(x => x is not null).ToList();
                if (objects.Count == 0)
                    return wasCast.Negated;
                var holds = objects.Any(x => x.WasCastWith("Cast") && (wasCast.From is not { Zone: var from } || x.WasCastWith("From" + from)));
                return holds != wasCast.Negated;
            }
            case ManaSpent spent:
            {
                var objects = Resolver.Objects(spent.For ?? new Self(), ctx).Select(state.Object).Where(x => x?.SpentToCast is not null).ToList();
                if (objects.Count == 0)
                    return false;
                var pool = objects[0].SpentToCast;
                var amount = spent.Color is { } colorName ? Colors.OfName(colorName) is { } color ? pool.Of(color) : colorName == "Colorless" ? pool.Colorless : 0 : pool.Total;
                return spent.Amount switch
                {
                    Bound bound when Quantities.Evaluate(bound.Value, ctx) is { } threshold => Filters.Compare(amount, bound.Comparison, threshold),
                    Fixed exactly => spent.Exactly ? amount == exactly.Value : amount >= exactly.Value,
                    null => amount > 0,
                    _ => null,
                };
            }
            case WasCastWith { Twice: false } cast:
            {
                // A permanent that has left the battlefield ("when it dies, if it was cast for its blitz cost") as it last was there.
                var objects = Resolver.Objects(cast.Spell, ctx).Select(state.Object).Where(x => x is not null)
                    .Select(x => x.Zone is Zone.Graveyard or Zone.Exile && state.LastKnownOf(x.Id) is { } known && known.ZoneChangeCount == x.ZoneChangeCount - 1 ? known : x).ToList();
                return objects.Count == 0 ? null : objects.Any(x => x.WasCastWith(cast.How)) != cast.Negated;
            }
            case WasAffectedThisWay affected:
            {
                var pool = (ctx.Affected ?? ctx.Last ?? []).Where(x => x.IsObject).Select(x => state.Object(x.Object.Value)).Where(x => x is not null).ToList();
                return affected.Object switch
                {
                    ObjectSelector { Filter: var filter } => pool.Any(x => Resolver.PoolMatches(filter ?? new ObjectFilter(), x, ctx)),
                    var named => Resolver.Objects(named, ctx).Any(id => pool.Any(x => x.Id == id)),
                };
            }
            case HandSize hand:
            {
                var players = Resolver.Players(hand.Player, ctx);
                if (players.Count == 0)
                    return null;
                return players.Any(p =>
                {
                    var size = state.Player(p).Hand.Count;
                    if (hand.ComparedTo is { } compared)
                    {
                        var comparison = compared.Comparison switch { "More" or "Greater" => Comparison.Greater, "Fewer" or "Less" => Comparison.Less, _ => Comparison.Equal };
                        return Resolver.Players(compared.Than, ctx).Where(o => o != p).All(o => Filters.Compare(size, comparison, state.Player(o).Hand.Count));
                    }
                    return hand.Size switch
                    {
                        Fixed exactly => size == exactly.Value,
                        Bound bound when Quantities.Evaluate(bound.Value, ctx) is { } threshold => Filters.Compare(size, bound.Comparison, threshold),
                        null => size > 0,
                        _ => false,
                    };
                });
            }
            // "This spell costs {1} less to cast if it targets …": "it" is the spell.
            case Targets targets when targets.Spell is Self or Reference { Text: "it" }:
            {
                var chosen = ctx.TargetsBySelector.Values.SelectMany(x => x).Distinct().ToList();
                if (targets.Target is ObjectSelector { Filter: var filter })
                    return chosen.Any(t => t.IsObject && state.Object(t.Object.Value) is { } obj && Filters.Matches(state, filter, obj, ctx.Controller, ctx.Source, ctx: ctx));
                if (targets.Target is PlayerSelector)
                    return chosen.Any(t => t.IsPlayer && Resolver.Players(targets.Target, ctx).Contains(t.Player.Value));
                return null;
            }
            // "If an opponent controls more lands than you", "unless you control more creatures than defending player".
            case CompareControl compare when compare.Objects is ObjectSelector { Filter: var filter }:
            {
                int Count(PlayerId player) => state.Battlefield.Count(x => x.Controller == player && Filters.Matches(state, filter, x, ctx.Controller, ctx.Source, ctx: ctx));
                var subjects = compare.Player is null ? [ctx.Controller] : Resolver.Players(compare.Player, ctx);
                bool Beats(int mine, int theirs) => compare.Bound == "AtMost" ? mine < theirs : mine > theirs;
                return subjects.Any(subject =>
                {
                    var others = compare.ComparedTo is PlayerSelector { Kind: PlayerKind.EachOtherPlayer }
                        ? state.PlayersInGame.Where(p => p != subject).ToList()
                        : Resolver.Players(compare.ComparedTo, ctx).Where(p => p != subject).ToList();
                    if (others.Count == 0)
                        return false;
                    var any = compare.ComparedTo is PlayerSelector { Kind: PlayerKind.AnyOpponent or PlayerKind.AnyPlayer };
                    return any ? others.Any(o => Beats(Count(subject), Count(o))) : others.All(o => Beats(Count(subject), Count(o)));
                });
            }
            case Controls controls:
            {
                var players = Resolver.Players(controls.Player, ctx);
                var objects = Resolver.Objects(controls.Objects, ctx).Select(state.Object).Where(x => x is { Zone: Zone.Battlefield }).ToList();
                // "Three or more creatures with different powers": distinct among the player's own.
                if (controls.Objects is ObjectSelector { Filter: { } distinct } && distinct.With?.Any(x => x is Distinct) == true)
                    objects = Filters.DistinctBy(state.Battlefield.Where(x => players.Contains(x.Controller) && Filters.Matches(state, distinct, x, ctx.Controller, ctx.Source, ctx: ctx)), distinct, state, x => x).ToList();
                var count = objects.Count(x => players.Contains(x.Controller));
                var holds = controls.Objects is ObjectSelector { Count: Bound bound } && Quantities.Evaluate(bound.Value, ctx) is { } threshold
                    ? Filters.Compare(count, bound.Comparison, threshold)
                    : controls.Objects is ObjectSelector { Count: Fixed { Value: 0 } } ? count == 0 : count > 0;
                return holds != controls.Negated;
            }
            case CountCondition counted:
            {
                var count = Resolver.Targets(counted.Of, ctx).Count;
                var threshold = Quantities.Evaluate(counted.Threshold, ctx);
                return threshold is null ? null : Filters.Compare(count, counted.Comparison, threshold.Value);
            }
            case CompareCount compare:
            {
                var count = Quantities.Evaluate(compare.Count, ctx);
                var threshold = Quantities.Evaluate(compare.Threshold, ctx);
                return count is null || threshold is null ? null : Filters.Compare(count.Value, compare.Comparison, threshold.Value);
            }
            case ThereAre exist:
            {
                var objects = Resolver.Objects(exist.Objects, ctx);
                if (exist.Objects is ObjectSelector { Count: Fixed { Value: 0 } })
                    return objects.Count == 0;
                if (exist.Objects is ObjectSelector { Count: Bound bound } && Quantities.Evaluate(bound.Value, ctx) is { } threshold)
                    return Filters.Compare(objects.Count, bound.Comparison, threshold);
                return objects.Count > 0;
            }
            case IsKind kind:
            {
                var objects = Resolver.Objects(kind.Object, ctx).Select(state.Object).Where(x => x is not null).ToList();
                // "If it's a land card" about what was revealed, when nothing was: it isn't.
                if (objects.Count == 0)
                    return kind.Object is ObjectSelector { Quantifier: Quantifier.Those or Quantifier.Attached } or Reference ? kind.Negated : null;
                var holds = objects.All(obj =>
                {
                    var traits = Characteristics.Of(state, obj);
                    return kind.Kinds.Any(k => Filters.KindMatches(k, obj, traits, state)) && (kind.Qualified is null || Filters.Matches(state, kind.Qualified with { Zone = null }, obj, ctx.Controller, ctx.Source, ctx: ctx));
                });
                return holds != kind.Negated;
            }
            case ObjectIs aspect:
            {
                var objects = Resolver.Objects(aspect.Object, ctx).Select(state.Object).Where(x => x is not null).ToList();
                // "If it's white" about the equipped creature when nothing is equipped: it isn't.
                if (objects.Count == 0)
                    return aspect.Object is ObjectSelector { Quantifier: Quantifier.Those or Quantifier.Attached } or Reference ? aspect.Negated : null;
                var holds = objects.All(obj =>
                {
                    var traits = Characteristics.Of(state, obj);
                    return aspect.Aspects.All(term => term.Vocabulary switch
                    {
                        "TapState" => term.Value == "Tapped" ? obj.Tapped : !obj.Tapped,
                        "CombatState" => term.Value == "Attacking" ? obj.Attacking : term.Value == "Blocking" ? obj.Blocking is not null : false,
                        _ => Filters.KindMatches(term, obj, traits, state) || Filters.QualifierMatches(new Qualifier(term.Value, Vocabulary: term.Vocabulary), obj, traits, ctx.Source, state),
                    });
                });
                return holds != aspect.Negated;
            }
            case HasKeywordsCondition keywords:
            {
                var objects = Resolver.Objects(keywords.Object, ctx).Select(state.Object).Where(x => x is not null).ToList();
                if (objects.Count == 0)
                    return null;
                return objects.All(obj => keywords.Keywords.All(Characteristics.Of(state, obj).HasKeyword)) != keywords.Negated;
            }
            case HasCountersCondition counters:
            {
                var holders = Resolver.Targets(counters.Holder, ctx);
                if (holders.Count == 0)
                    return null;
                var holds = holders.All(holder =>
                {
                    // "If it had a death counter on it": a permanent that has left is judged as it last was on the battlefield.
                    var obj = holder.IsObject ? state.Object(holder.Object.Value) : null;
                    if (obj is not null && counters.Tense == "Had" && obj.Zone != Zone.Battlefield && state.LastKnownOf(obj.Id) is { } known)
                        obj = known;
                    var amount = counters.Counters.Kind is not { } kind
                        ? holder.IsObject ? obj?.CounterMap.Values.Sum() ?? 0 : state.Player(holder.Player.Value).Poison
                        : holder.IsObject ? obj?.CountersOf(kind) ?? 0 : kind == "poison" ? state.Player(holder.Player.Value).Poison : 0;
                    return counters.Counters.Count switch
                    {
                        Bound bound when Quantities.Evaluate(bound.Value, ctx) is { } threshold => Filters.Compare(amount, bound.Comparison, threshold),
                        Fixed fixedValue => amount >= fixedValue.Value,
                        _ => amount > 0,
                    };
                });
                return holds != counters.Negated;
            }
            case IsInZone inZone:
            {
                var objects = Resolver.Objects(inZone.Object, ctx).Select(state.Object).Where(x => x is not null).ToList();
                return objects.Count == 0 ? null : Enum.TryParse<Zone>(inZone.Place.Zone, out var zone) && objects.All(x => x.Zone == zone);
            }
            case ItsPhase phase:
            {
                var whose = Resolver.Players(phase.Phase.Whose, ctx);
                var holds = (whose.Count == 0 || whose.Contains(state.ActivePlayer)) && (phase.Phase.Phase is null or "Turn" || StepOf(phase.Phase.Phase) == state.Step);
                return holds != phase.Negated;
            }
            case Happened history when History.Supported(history):
                return History.Evaluate(history, ctx);
            case Happened { Event: PlayerAction action } happened when action.Action is "GainedLife" or "LostLife":
            {
                var players = Resolver.Players(action.Player, ctx);
                return players.Any(p => action.Action == "GainedLife" ? state.Player(p).LifeGainedThisTurn > 0 : state.Player(p).LifeLostThisTurn > 0);
            }
            case Happened { Event: NamedEvent { Kind: "YouAttacked" }, Span: "ThisTurn" or null }:
                return (state.AttackedThisTurn ?? []).Any(id => state.Object(id)?.Controller == ctx.Controller);
            case Happened { Event: PlayerAction { Action: "Cast", What: null or ObjectSelector { Filter: null or { Qualifiers: null, With: null } } } cast, Span: "ThisTurn" or null } happened:
            {
                var players = Resolver.Players(cast.Player, ctx);
                if (players.Count == 0)
                    players = state.PlayersInGame.ToImmutableList();
                var spells = players.Sum(p => state.Player(p).SpellsCastThisTurn);
                return cast.Count is Bound bound && Quantities.Evaluate(bound.Value, ctx) is { } threshold ? Filters.Compare(spells, bound.Comparison, threshold) : spells > 0;
            }
            case Did { Effect: Remains remains }:
            {
                var objects = Resolver.Objects(remains.Objects, ctx).Select(state.Object).ToList();
                if (objects.Count == 0)
                    return false;
                return objects.All(x => x is not null && remains.State switch
                {
                    "Tapped" => x.Zone == Zone.Battlefield && x.Tapped,
                    "Untapped" => x.Zone == Zone.Battlefield && !x.Tapped,
                    "OnTheBattlefield" => x.Zone == Zone.Battlefield,
                    "Exiled" => x.Zone == Zone.Exile,
                    _ => false,
                });
            }
            // "If this permanent is an enchantment", "if it's a land card", "if all lands on the battlefield are islands": what something is now, not something done.
            case Did { Effect: BecomesAspect { Aspects.Count: > 0 } aspect }:
            {
                var objects = Resolver.Objects(aspect.Objects ?? new Self(), ctx).Select(state.Object).Where(x => x is not null).ToList();
                if (objects.Count == 0)
                    return false;
                var holds = objects.All(x => aspect.Aspects.All(term => Filters.KindMatches(term, x, Characteristics.Of(state, x), state)));
                return holds != aspect.Negated;
            }
            case Did:
                return ctx.Game?.LastOptionalTaken;
            case OutcomeCondition { Result: "Did" }:
                return ctx.Game?.LastOptionalTaken;
            case OutcomeCondition { Result: "DidNot" or "Cannot" }:
                return ctx.Game is null ? null : !ctx.Game.LastOptionalTaken;
            case OutcomeCondition { Result: "WonFlip" or "Won" }:
                return ctx.Game?.WonLastFlip;
            case OutcomeCondition { Result: "LostFlip" or "Lost" }:
                return ctx.Game is null ? null : !ctx.Game.WonLastFlip;
            case DieResult roll:
                return ctx.Game is { } rolled ? rolled.LastDieRoll >= roll.Low && rolled.LastDieRoll <= roll.High : null;
            case OutcomeCondition:
                return null;
            default:
                return null;
        }
    }

    public static Step? StepOf(string phase) =>
        phase switch
        {
            "Upkeep" => Step.Upkeep,
            "Draw" or "DrawStep" => Step.Draw,
            "EndStep" or "End" or "EndOfTurn" => Step.End,
            "Combat" or "BeginningOfCombat" or "CombatPhase" or "BeginningOfCombatStep" => Step.BeginCombat,
            "DeclareAttackers" => Step.DeclareAttackers,
            "DeclareBlockers" => Step.DeclareBlockers,
            "CombatDamage" => Step.CombatDamage,
            "EndOfCombat" or "EndOfCombatStep" or "CombatEnd" => Step.EndCombat,
            "NextEndStep" or "TheNextEndStep" => Step.End,
            "Untap" => Step.Untap,
            "Cleanup" => Step.Cleanup,
            "MainPhase" or "PrecombatMainPhase" or "FirstMainPhase" => Step.Main1,
            "PostcombatMainPhase" or "SecondMainPhase" => Step.Main2,
            _ => null,
        };
}
