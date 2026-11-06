namespace MTGEngine.Core;

/// <summary>
/// What one effect's execution knows: the game, who controls the effect and what it's on, the targets chosen for
/// each target selector, X, what references point at, and what the previous step affected.
/// </summary>
public sealed record EffectContext(
    Game Game,
    PlayerId Controller,
    ObjectId? Source,
    Dictionary<IrNode, ImmutableList<TargetRef>> TargetsBySelector,
    int X = 0,
    Dictionary<string, ImmutableList<TargetRef>> Bindings = null,
    ImmutableList<TargetRef> Last = null,
    TargetRef? Subject = null,
    TargetRef? TriggerSubject = null,
    int LastAmount = 0,
    ImmutableList<TargetRef> Affected = null,
    GameState Frozen = null,
    ImmutableList<TargetRef> Rest = null)
{
    public GameState State => Frozen ?? Game.State;

    /// <summary>A context over a state alone, for evaluating static conditions and filters where no effect is running.</summary>
    public static EffectContext Snapshot(GameState state, PlayerId controller, ObjectId? source) =>
        new(null, controller, source, new Dictionary<IrNode, ImmutableList<TargetRef>>(ReferenceEqualityComparer.Instance), Frozen: state);

    public Dictionary<string, ImmutableList<TargetRef>> Binds => Bindings ?? new();

    public static EffectContext For(Game game, StackItem item, TargetRef? triggerSubject = null)
    {
        var slots = TargetSlots.Of(item.Effects).ToList();
        var bySelector = TargetSlots.BySlot(slots, item.Targets, item.TargetCounts);

        var bindings = new Dictionary<string, ImmutableList<TargetRef>>();
        foreach (var (key, value) in item.Bindings ?? ImmutableDictionary<string, ImmutableList<TargetRef>>.Empty)
            bindings[key] = value;
        foreach (var slot in slots)
            if (slot.Binding is { } binding && bySelector.TryGetValue(slot.Selector, out var chosen))
                bindings[binding] = chosen;

        return new EffectContext(game, item.Controller, item.Source, bySelector, item.X, bindings, TriggerSubject: triggerSubject, Last: triggerSubject is { } subject ? [subject] : null);
    }
}

/// <summary>Selectors as objects and players in the game now.</summary>
public static class Resolver
{
    /// <summary>What was chosen for a target selector: its own slot's choice, or the choice for the slot bound to the same name (the same target named again).</summary>
    public static ImmutableList<TargetRef> Chosen(IrNode selector, string binding, EffectContext ctx) =>
        ctx.TargetsBySelector.TryGetValue(selector, out var chosen) ? chosen
        : binding is not null && ctx.Bindings?.TryGetValue(binding, out var named) == true ? named
        : [];

    public static ImmutableList<TargetRef> Targets(Selector selector, EffectContext ctx)
    {
        switch (selector)
        {
            case null:
                return [];
            case PlayerSelector:
                return Players(selector, ctx).Select(TargetRef.Of).ToImmutableList();
            case Union union:
                return union.Members.SelectMany(x => Targets(x, ctx)).Distinct().ToImmutableList();
            case Reference reference:
                return Referenced(reference, ctx);
            case ObjectSelector { Quantifier: Quantifier.Target } targeted:
                return Chosen(targeted, targeted.Binding, ctx);
            case ObjectSelector { Filter.Kinds: [{ Value: nameof(ObjectKind.Player) }] } players:
                return ctx.State.PlayersInGame.Where(p => PlayerFilterMatches(players.Filter, p, ctx)).Select(TargetRef.Of).ToImmutableList();
            default:
                return Objects(selector, ctx).Select(TargetRef.Of).ToImmutableList();
        }
    }

    /// <summary>
    /// The objects an effect acts on: as <see cref="Objects"/>, except that "a" or "an" in an effect ("return another
    /// creature you control to its owner's hand", "exile a card at random from your graveyard") is one of them, chosen
    /// by the effect's controller or at random. In a condition "a creature" means any, so this is for effects only.
    /// </summary>
    public static ImmutableList<ObjectId> ForEffect(Selector selector, EffectContext ctx)
    {
        var objects = Objects(selector, ctx);
        if (selector is not ObjectSelector { Quantifier: Quantifier.Any } any || objects.Count <= 1)
            return objects;
        var atRandom = any.Filter?.Qualifiers?.Any(q => q is { Vocabulary: "Manner", Quality: "AtRandom" }) == true;
        var picked = atRandom && ctx.Game is not null ? objects.OrderBy(_ => ctx.Game.Rng.Next()).Take(1).ToImmutableList() : Choose(objects, 1, any.UpTo, ctx);
        if (any.Binding is { } binding)
            ctx.Binds[binding] = picked.Select(TargetRef.Of).ToImmutableList();
        return picked;
    }

    public static ImmutableList<ObjectId> Objects(Selector selector, EffectContext ctx)
    {
        var state = ctx.State;
        switch (selector)
        {
            case null:
                return [];
            case Self:
                return ctx.Source is { } self && state.Object(self) is not null ? [self] : [];
            case Reference reference:
                return Referenced(reference, ctx).Where(x => x.IsObject).Select(x => x.Object.Value).ToImmutableList();
            case Union union:
                return union.Members.SelectMany(x => Objects(x, ctx)).Distinct().ToImmutableList();
            case ObjectSelector { Quantifier: Quantifier.Target } targeted:
                return Chosen(targeted, targeted.Binding, ctx).Where(x => x.IsObject).Select(x => x.Object.Value).ToImmutableList();
            case ObjectSelector { Quantifier: Quantifier.Attached }:
                return ctx.Source is { } attachment && state.Object(attachment)?.AttachedTo is { } host ? [host] : [];
            case ObjectSelector pooled when PoolOf(pooled, ctx) is { } pool:
            {
                var matching = Filters.DistinctBy(pool.Where(id => state.Object(id) is { } obj && PoolMatches(pooled.Filter, obj, ctx)), pooled.Filter, state, state.Object).ToImmutableList();
                var wanted = pooled.Quantifier switch
                {
                    Quantifier.Count => Quantities.Evaluate(pooled.Count, ctx),
                    Quantifier.Any => 1,
                    _ => null,
                };
                if (wanted is { } count && (count < matching.Count || pooled.UpTo))
                {
                    var atRandom = pooled.Filter?.Qualifiers?.Any(q => q is { Vocabulary: "Manner", Quality: "AtRandom" }) == true;
                    var picked = atRandom ? matching.OrderBy(_ => ctx.Game.Rng.Next()).Take(count).ToImmutableList() : Choose(matching, count, pooled.UpTo, ctx);
                    ctx.Binds["#rest"] = pool.Except(picked).Select(TargetRef.Of).ToImmutableList();
                    matching = picked;
                }
                if (pooled.Binding is { } poolBinding)
                    ctx.Binds[poolBinding] = matching.Select(TargetRef.Of).ToImmutableList();
                return matching;
            }
            case ObjectSelector { Quantifier: Quantifier.Those } those:
            {
                var pool = (ctx.Affected ?? ctx.Last ?? []).Where(x => x.IsObject).Select(x => x.Object.Value).ToImmutableList();
                var matched = pool.Where(id => state.Object(id) is { } obj && Filters.Matches(state, those.Filter with { Zone = null }, obj, ctx.Controller, ctx.Source, ctx: ctx) || those.Filter?.Kinds is null).ToImmutableList();
                // "When this leaves the battlefield, return the exiled card": nothing earlier in the ability was exiled, so it's what this exiled before.
                if (matched.Count == 0 && those.Filter?.With?.Any(x => x is Related { Relation: Relation.ExiledWith }) == true)
                    matched = state.Objects.Values.Where(obj => Filters.Matches(state, those.Filter with { Zone = null }, obj, ctx.Controller, ctx.Source, ctx: ctx)).Select(x => x.Id).ToImmutableList();
                return matched;
            }
            case ObjectSelector { Quantifier: Quantifier.Rest }:
                return Leftovers(ctx);
            case ObjectSelector selector2:
            {
                var position = selector2.Filter?.Qualifiers?.FirstOrDefault(q => q.Vocabulary == "Position")?.Quality;
                var matches = Filters.DistinctBy(Candidates(selector2.Filter, ctx).Where(obj => Filters.Matches(state, selector2.Filter, obj, ctx.Controller, ctx.Source, selector2.Other ? ctx.Source : null, ctx: ctx)), selector2.Filter, state, x => x).Select(x => x.Id).ToImmutableList();
                if (position is "Top" or "Bottom" && selector2.Filter?.Zone?.Zone == nameof(Zone.Library))
                {
                    var owners = selector2.Filter.Zone.Owner is null ? [ctx.Controller] : Players(selector2.Filter.Zone.Owner, ctx);
                    var ordered = owners.SelectMany(p => position == "Top" ? state.Player(p).Library : state.Player(p).Library.Reverse()).Where(matches.Contains).ToImmutableList();
                    var wanted = selector2.Quantifier == Quantifier.Count ? Quantities.Evaluate(selector2.Count, ctx) ?? 1 : 1;
                    return ordered.Take(wanted).ToImmutableList();
                }
                var result = selector2.Quantifier == Quantifier.Count && Quantities.Evaluate(selector2.Count, ctx) is { } count && selector2.Count is not Bound ? matches.Take(count).ToImmutableList() : matches;
                if (selector2.Binding is { } binding)
                    ctx.Binds[binding] = result.Select(TargetRef.Of).ToImmutableList();
                return result;
            }
            default:
                return [];
        }
    }

    /// <summary>What "the rest" means: what the last choice left, from the context or from a choice the resolver made.</summary>
    static ImmutableList<ObjectId> Leftovers(EffectContext ctx) =>
        (ctx.Rest ?? (ctx.Bindings?.TryGetValue("#rest", out var rest) == true ? rest : null) ?? []).Where(x => x.IsObject).Select(x => x.Object.Value).ToImmutableList();

    /// <summary>
    /// The objects a selector chooses among, when it names a pool rather than the game: "one of them" (among what a
    /// reference means), "the rest", "cards revealed this way" (what the last step affected). Null for none of those.
    /// </summary>
    static ImmutableList<ObjectId> PoolOf(ObjectSelector selector, EffectContext ctx)
    {
        var filter = selector.Filter;
        if (filter is null)
            return null;
        if (filter.Qualifiers?.Any(q => q.Vocabulary == "LeftoverCards") == true)
            return Leftovers(ctx);
        // "The sacrificed creature", "the discarded card": what the costs took, else what the step before affected.
        if (filter.Qualifiers?.Any(q => q.Vocabulary == "ThisWayAction") == true)
            return ((ctx.Bindings?.TryGetValue("#cost", out var paid) == true && paid.Count > 0 ? paid : null) ?? ctx.Affected ?? ctx.Last ?? []).Where(x => x.IsObject).Select(x => x.Object.Value).ToImmutableList();
        // "The copy", "the copies": what the step before made.
        if (filter.Kinds?.Any(k => k.Vocabulary is "RetargetedObject" or "CopiedSpell") == true)
            return (ctx.Last ?? ctx.Affected ?? []).Where(x => x.IsObject).Select(x => x.Object.Value).ToImmutableList();
        // "Each creature dealt damage this way": what the damage step before hit.
        if (filter.Qualifiers?.Any(q => q.Vocabulary == "DamageHistory" && q.Quality == "DealtDamageThisWay") == true)
            return (ctx.Last ?? []).Where(x => x.IsObject).Select(x => x.Object.Value).ToImmutableList();
        if (filter.Qualifiers?.Any(q => q.Vocabulary == "AffectedThisWay") == true)
            return (ctx.Affected ?? ctx.Last ?? []).Where(x => x.IsObject).Select(x => x.Object.Value).ToImmutableList();
        if (filter.With?.OfType<Related>().FirstOrDefault(x => x.Relation == Relation.Among) is { } among)
        {
            var named = Objects(among.To, ctx);
            return named.Count > 0 ? named : (ctx.Affected ?? ctx.Last ?? []).Where(x => x.IsObject).Select(x => x.Object.Value).ToImmutableList();
        }
        return null;
    }

    /// <summary>Whether a pooled object fits the rest of its selector's filter, wherever it is (a card looked at in the library is still "a creature card").</summary>
    public static bool PoolMatches(ObjectFilter filter, GameObject obj, EffectContext ctx)
    {
        var rest = filter with
        {
            Zone = null,
            Kinds = filter.Kinds?.Any(k => k.Vocabulary is "RetargetedObject" or "CopiedSpell") == true ? null : filter.Kinds,
            Qualifiers = filter.Qualifiers?.Where(q => q.Vocabulary is not ("LeftoverCards" or "AffectedThisWay" or "DamageHistory" or "ThisWayAction" or "Position")).ToList(),
            With = filter.With?.Where(p => p is not Related { Relation: Relation.Among }).ToList(),
        };
        var namesCards = rest.Kinds?.Any(x => x.Value is nameof(ObjectKind.Card) or nameof(ObjectKind.Spell)) == true;
        return Filters.Matches(ctx.State, rest, namesCards || obj.Zone == Zone.Battlefield ? obj : obj with { Zone = Zone.Battlefield }, ctx.Controller, ctx.Source, ctx: ctx);
    }

    /// <summary><paramref name="count"/> of <paramref name="candidates"/>, chosen by the effect's controller (the first ones when there's no game to ask).</summary>
    static ImmutableList<ObjectId> Choose(ImmutableList<ObjectId> candidates, int count, bool upTo, EffectContext ctx)
    {
        count = Math.Min(count, candidates.Count);
        if (ctx.Game is null || count == 0)
            return candidates.Take(count).ToImmutableList();
        var options = candidates.Select(TargetRef.Of).ToImmutableList();
        var picked = ctx.Game.Decide(new ChooseTargets(ctx.Controller, "Choose", options, upTo ? 0 : count, count)).Targets?.Where(options.Contains).Distinct().Take(count).Select(x => x.Object.Value).ToList() ?? [];
        if (!upTo && picked.Count < count)
            picked.AddRange(candidates.Except(picked).Take(count - picked.Count));
        return picked.ToImmutableList();
    }

    /// <summary>The objects a filter could mean: those in its zone, or on the battlefield when it names none, or cards and spells where it names those.</summary>
    public static IEnumerable<GameObject> Candidates(ObjectFilter filter, EffectContext ctx)
    {
        var state = ctx.State;
        if (filter?.Zone is { } zone && Enum.TryParse<Zone>(zone.Zone, out var wanted))
            return state.InZone(wanted);
        if (filter?.Kinds?.Any(x => x.Value == nameof(ObjectKind.Spell)) == true)
            return state.InZone(Zone.Stack);
        if (filter?.Kinds?.Any(x => x.Value == nameof(ObjectKind.Card)) == true)
            return state.Objects.Values.Where(x => x.Zone != Zone.Battlefield && x.Zone != Zone.Stack);
        return state.Battlefield;
    }

    public static ImmutableList<PlayerId> Players(Selector selector, EffectContext ctx)
    {
        var state = ctx.State;
        switch (selector)
        {
            case null:
                return [];
            case PlayerSelector player:
            {
                var result = PlayersOf(state, player, ctx.Controller, ctx.Source, ctx);
                if (player.Binding is { } binding)
                    ctx.Binds[binding] = result.Select(TargetRef.Of).ToImmutableList();
                return result;
            }
            case Reference reference:
                return Referenced(reference, ctx).Where(x => x.IsPlayer).Select(x => x.Player.Value).ToImmutableList();
            case Union union:
                return union.Members.SelectMany(x => Players(x, ctx)).Distinct().ToImmutableList();
            case ObjectSelector { Filter.Kinds: [{ Value: nameof(ObjectKind.Player) }] } players:
                return state.PlayersInGame.Where(p => PlayerFilterMatches(players.Filter, p, ctx)).ToImmutableList();
            case ObjectSelector { Quantifier: Quantifier.Target } targeted:
                return Chosen(targeted, targeted.Binding, ctx).Where(x => x.IsPlayer).Select(x => x.Player.Value).ToImmutableList();
            default:
                return [];
        }
    }

    /// <summary>Players of a selector outside an effect: for static filters ("creatures you control").</summary>
    public static ImmutableList<PlayerId> Players(GameState state, Selector selector, PlayerId you, ObjectId? source) =>
        selector is PlayerSelector player ? PlayersOf(state, player, you, source, null) : [];

    static ImmutableList<PlayerId> PlayersOf(GameState state, PlayerSelector player, PlayerId you, ObjectId? source, EffectContext ctx)
    {
        switch (player.Kind)
        {
            case PlayerKind.You:
                return [you];
            case PlayerKind.EachOpponent or PlayerKind.YourOpponents or PlayerKind.AnyOpponent:
                return state.OpponentsOf(you).ToImmutableList();
            case PlayerKind.EachPlayer or PlayerKind.Players or PlayerKind.AnyPlayer:
                return state.PlayersInGame.ToImmutableList();
            case PlayerKind.EachOtherPlayer:
                return state.OpponentsOf(you).ToImmutableList();
            case PlayerKind.TargetPlayer or PlayerKind.TargetOpponent:
                return ctx is null ? [] : Chosen(player, player.Binding, ctx).Where(x => x.IsPlayer).Select(x => x.Player.Value).ToImmutableList();
            case PlayerKind.ControllerOf or PlayerKind.OwnerOf:
            {
                if (ctx is null)
                    return [];
                var objects = Objects(player.Of, ctx);
                return objects.Select(id => state.Object(id)).Where(x => x is not null).Select(x => player.Kind == PlayerKind.OwnerOf ? x.Owner : x.Controller).Distinct().ToImmutableList();
            }
            case PlayerKind.DefendingPlayer:
                return state.PlayersInGame.Where(p => p != state.ActivePlayer).Take(1).ToImmutableList();
            case PlayerKind.AttackingPlayer:
                return [state.ActivePlayer];
            case PlayerKind.ThatPlayer:
                return ThatPlayer(ctx);
            case PlayerKind.EnchantedPlayer:
                return [];
            case PlayerKind.Chosen:
                return [];
            case PlayerKind.NoPlayer:
                return [];
            default:
                return [];
        }
    }

    static bool PlayerFilterMatches(ObjectFilter filter, PlayerId player, EffectContext ctx)
    {
        foreach (var qualifier in filter.Qualifiers ?? [])
            if (qualifier.Quality == "Opponent" && (player == ctx.Controller) != qualifier.Negated)
                return false;
        foreach (var predicate in filter.With ?? [])
            if (predicate is Satisfying satisfying)
            {
                if (Conditions.Evaluate(satisfying.Condition, ctx with { Subject = TargetRef.Of(player), Last = [TargetRef.Of(player)] }) != true)
                    return false;
            }
            else
                return false;
        return true;
    }

    /// <summary>
    /// "That player", "their": the players the step before acted on; else the one player the
    /// effect targets ("target opponent loses half their life"), or the player the trigger is about.
    /// </summary>
    static ImmutableList<PlayerId> ThatPlayer(EffectContext ctx)
    {
        if (ctx is null)
            return [];
        if (ctx.Last?.Where(x => x.IsPlayer).Select(x => x.Player.Value).ToImmutableList() is { Count: > 0 } last)
            return last;
        var targeted = ctx.TargetsBySelector.Values.SelectMany(x => x).Where(x => x.IsPlayer).Select(x => x.Player.Value).Distinct().ToImmutableList();
        if (targeted.Count == 1)
            return targeted;
        // "Target creature an opponent controls deals damage to each other creature that player controls": its controller.
        var controllers = ctx.TargetsBySelector.Values.SelectMany(x => x).Where(x => x.IsObject).Select(x => ctx.State.Object(x.Object.Value)?.Controller).Where(x => x is not null).Select(x => x.Value).Distinct().ToImmutableList();
        if (targeted.Count == 0 && controllers.Count == 1)
            return controllers;
        return ctx.TriggerSubject is { IsPlayer: true } subject ? [subject.Player.Value] : [];
    }

    static ImmutableList<TargetRef> Referenced(Reference reference, EffectContext ctx)
    {
        if (reference.Binding is { } binding && ctx.Binds.TryGetValue(binding, out var bound))
            return bound;
        if (reference.Text is "who")
            return ctx.Subject is { } who ? [who] : [];
        if (ctx.TriggerSubject is { } subject && (reference.Antecedent is null || reference.Text.StartsWith("that") || reference.Text is "it" or "its" or "they" or "them" or "their"))
            return [subject];
        // "Target player … permanents they control": a player word with nothing before it is that player.
        if (ctx.Last is null or { Count: 0 } && reference.Text is "they" or "their" or "them" or "that player")
            return ThatPlayer(ctx).Select(TargetRef.Of).ToImmutableList();
        return ctx.Last ?? [];
    }
}

/// <summary>A target the player chooses when casting or activating: the selector that asks for it, where it appears, and the binding it gives.</summary>
public sealed record TargetSlot(Selector Selector, string Binding, bool UpTo, int Count);

public static class TargetSlots
{
    /// <summary>
    /// Every target selector among the effects, in walk order; players and objects alike. A selector bound to a
    /// name another one already has is the same target again ("target creature gets +1/+1 and gains flying"), not
    /// another slot.
    /// </summary>
    public static IEnumerable<TargetSlot> Of(IEnumerable<IrNode> roots)
    {
        var seen = new HashSet<IrNode>(ReferenceEqualityComparer.Instance);
        var bound = new HashSet<string>();
        bool Fresh(IrNode node, string binding) => seen.Add(node) && (binding is null || bound.Add(binding));
        foreach (var root in roots)
            // An ability the effect grants or a token has ("gains '{t}: this creature fights another target creature'") chooses its targets when it's used.
            foreach (var node in MTGRulesIR.Lowering.NodeWalker.Nodes(root, x => x is not Ability))
                switch (node)
                {
                    case ObjectSelector { Quantifier: Quantifier.Target } objects when Fresh(node, objects.Binding):
                        // "Each of one or two target creatures": up to two, at least one.
                        yield return new TargetSlot(objects, objects.Binding, objects.UpTo || objects.Count is Special { Kind: "AnyNumber" },
                            objects.Count switch { Special { Kind: "AnyNumber" } => int.MaxValue, Special { Kind: "OneOrTwo" } => 2, Special { Kind: "OneTwoOrThree" } => 3, Fixed { Value: var n } => n, _ => 1 });
                        break;
                    // "Distribute three +1/+1 counters among one, two, or three target creatures": one slot for up to that many.
                    case DistributeCounters { Targeted: true, Among: ObjectSelector among } distribute when Fresh(among, among.Binding):
                        yield return new TargetSlot(among, among.Binding, false, distribute.Range switch { "OneOrTwo" => 2, "OneTwoOrThree" => 3, _ => int.MaxValue });
                        break;
                    case PlayerSelector { Kind: PlayerKind.TargetPlayer or PlayerKind.TargetOpponent } player when Fresh(node, player.Binding):
                        yield return new TargetSlot(player, player.Binding, false, 1);
                        break;
                }
    }

    /// <summary>
    /// The chosen targets by the slot they were chosen for: <paramref name="counts"/> says how many each slot took
    /// ("up to two target creatures" may take two, an "up to one" none); without it, one each in order.
    /// </summary>
    public static Dictionary<IrNode, ImmutableList<TargetRef>> BySlot(IReadOnlyList<TargetSlot> slots, IReadOnlyList<TargetRef> targets, IReadOnlyList<int> counts)
    {
        var bySelector = new Dictionary<IrNode, ImmutableList<TargetRef>>(ReferenceEqualityComparer.Instance);
        if (counts is null || counts.Count != slots.Count)
        {
            for (var i = 0; i < slots.Count && i < targets.Count; i++)
                bySelector[slots[i].Selector] = [targets[i]];
            return bySelector;
        }
        var at = 0;
        for (var i = 0; i < slots.Count; i++)
        {
            var taken = targets.Skip(at).Take(counts[i]).ToImmutableList();
            at += counts[i];
            if (taken.Count > 0)
                bySelector[slots[i].Selector] = taken;
        }
        return bySelector;
    }

    /// <summary>What may be chosen for a slot: objects matching its filter in its zone, players for player targets, both for "any target".</summary>
    public static ImmutableList<TargetRef> Candidates(TargetSlot slot, Game game, PlayerId controller, ObjectId? source)
    {
        var state = game.State;
        var ctx = new EffectContext(game, controller, source, new Dictionary<IrNode, ImmutableList<TargetRef>>(ReferenceEqualityComparer.Instance));

        switch (slot.Selector)
        {
            case PlayerSelector { Kind: PlayerKind.TargetOpponent }:
                return state.OpponentsOf(controller).Select(TargetRef.Of).ToImmutableList();
            case PlayerSelector:
                return state.PlayersInGame.Select(TargetRef.Of).ToImmutableList();
            case ObjectSelector objects:
            {
                var filter = objects.Filter;
                List<TargetRef> candidates = [];
                var anyTarget = filter?.Kinds?.Any(x => x.Value == nameof(ObjectKind.Any)) == true;
                bool IsPlayerKind(Term x) => x.Value == nameof(ObjectKind.Player) || x.Vocabulary == "PlayerIdentity";
                var playersToo = anyTarget || filter?.Kinds?.Any(IsPlayerKind) == true;
                var opponentsOnly = filter?.Qualifiers?.Any(q => q.Quality == "Opponent") == true || filter?.Kinds?.Any(x => x.Vocabulary == "PlayerIdentity" && x.Value == "Opponent") == true;

                if (playersToo)
                    candidates.AddRange(state.PlayersInGame.Where(p => !opponentsOnly || p != controller).Select(TargetRef.Of));

                var objectKinds = filter?.Kinds?.Where(x => !IsPlayerKind(x)).ToList();
                var objectFilter = anyTarget
                    ? new ObjectFilter(Kinds: [new Term("Creature", "CardType"), new Term("Planeswalker", "CardType"), new Term("Battle", "CardType")])
                    : playersToo ? filter with { Kinds = objectKinds } : filter;

                if (!playersToo || anyTarget || objectKinds is { Count: > 0 })
                    foreach (var obj in Resolver.Candidates(objectFilter, ctx))
                        if (Filters.Matches(state, objectFilter, obj, controller, source, objects.Other ? source : null, ctx: ctx) && !Protected(state, obj, controller, source))
                            candidates.Add(TargetRef.Of(obj.Id));

                return candidates.ToImmutableList();
            }
            default:
                return [];
        }
    }

    static bool Protected(GameState state, GameObject obj, PlayerId by, ObjectId? source)
    {
        var traits = Characteristics.Of(state, obj);
        if (source is { } targeting && state.Object(targeting) is { } spell && Keywords.ProtectedFrom(state, obj, traits, spell))
            return true;
        return traits.HasKeyword("Shroud") || traits.Cant("BeTargeted") || (traits.HasKeyword("Hexproof") || traits.Cant("BeTargetedByOpponents")) && obj.Controller != by;
    }
}
