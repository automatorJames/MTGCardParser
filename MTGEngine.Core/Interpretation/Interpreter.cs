namespace MTGEngine.Core;

/// <summary>
/// Executes rules-IR effects against a game. Each effect kind the engine understands becomes events; one it doesn't
/// is skipped with an <see cref="EffectSkipped"/> event, so a game always proceeds and the skips say what to build
/// next (see <see cref="InterpretReport"/> for the same question asked of the whole corpus).
/// </summary>
public static class Interpreter
{
    public static EffectContext Execute(Effect effect, EffectContext ctx)
    {
        // Steps leave what they found under bindings ("the rest" after "put one of them into your hand"), so there's always somewhere to put it.
        if (ctx.Bindings is null)
            ctx = ctx with { Bindings = new Dictionary<string, ImmutableList<TargetRef>>() };
        if (EachPlayersOwn(effect, ctx) is { } each)
            return each;
        if (EachUnnamedPlayer(effect, ctx) is { } unnamed)
            return unnamed;
        var result = Run(effect, ctx);
        // "Sacrifice a creature. … damage equal to that creature's power": what a step chose (not a target, which is
        // known already) is what later references to its binding mean.
        if (effect is not (null or Sequence or Conditional or Optional) && ChosenBinding(effect) is { } binding && !result.Binds.ContainsKey(binding)
            && (result.Affected is { Count: > 0 } ? result.Affected : result.Last) is { Count: > 0 } chosen && !ReferenceEquals(chosen, ctx.Last) && !ReferenceEquals(chosen, ctx.Affected))
            result.Binds[binding] = chosen;
        // "Exile a nonland card from among them, then put the rest into your hand": the rest is what was among them and wasn't taken.
        if (result.Rest is null && effect?.GetType().GetProperty("Objects")?.GetValue(effect) is ObjectSelector { Filter.With: { } with }
            && with.OfType<Related>().FirstOrDefault(x => x.Relation == Relation.Among) is { } among)
        {
            var pool = Resolver.Targets(among.To, ctx) is { Count: > 0 } named ? named : ctx.Affected ?? ctx.Last ?? [];
            var taken = (result.Affected ?? result.Last ?? []).ToHashSet();
            if (pool.Count > 0 && !ReferenceEquals(result.Affected ?? result.Last, ctx.Affected ?? ctx.Last))
                result = result with { Rest = pool.Where(x => !taken.Contains(x)).ToImmutableList() };
        }
        return result;
    }

    /// <summary>
    /// "Each player loses 1 life for each creature they control", "each opponent discards a card for each …": done by
    /// each of the players in turn, with "they" meaning that player, when the amount counts something of theirs.
    /// </summary>
    static EffectContext EachPlayersOwn(Effect effect, EffectContext ctx)
    {
        if (effect is null or ForEachOf)
            return null;
        var type = effect.GetType();
        if (type.GetProperty("Player") is not { CanWrite: true } player || player.GetValue(effect) is not PlayerSelector { Kind: PlayerKind.EachPlayer or PlayerKind.EachOpponent or PlayerKind.YourOpponents or PlayerKind.Players } each)
            return null;
        if (!new[] { "Amount", "Count" }.Any(name => type.GetProperty(name)?.GetValue(effect) is Quantity amount && MentionsThatPlayer(amount)))
            return null;
        var single = (Effect)type.GetMethod("<Clone>$")!.Invoke(effect, null);
        player.SetValue(single, new PlayerSelector(PlayerKind.ThatPlayer));
        foreach (var one in Resolver.Players(each, ctx).ToList())
            Execute(single, ctx with { Last = [TargetRef.Of(one)] });
        return ctx;
    }

    /// <summary>
    /// "Each player returns a creature they control to its owner's hand", "each player exiles two cards from their
    /// hand" lowered without their "each player": objects of a "they" / "their" no one is named for are each player's,
    /// in turn.
    /// </summary>
    static EffectContext EachUnnamedPlayer(Effect effect, EffectContext ctx)
    {
        if (effect?.GetType().GetProperty("Objects")?.GetValue(effect) is not ObjectSelector { Filter: { } filter })
            return null;
        var owner = filter.Zone?.Owner;
        var controller = filter.Controller?.Player;
        if (!(owner is not null && MentionsThatPlayer(owner) || controller is not null && MentionsThatPlayer(controller)))
            return null;
        if (Resolver.Players(new PlayerSelector(PlayerKind.ThatPlayer), ctx).Count > 0)
            return null;
        List<TargetRef> affected = [];
        foreach (var player in ctx.State.TurnOrderFrom(ctx.Controller).ToList())
        {
            var result = Execute(effect, ctx with { Last = [TargetRef.Of(player)] });
            affected.AddRange(result.Affected ?? []);
        }
        return ctx with { Affected = affected.ToImmutableList(), Last = affected.ToImmutableList() };
    }

    /// <summary>Whether a quantity counts something of "that player" / "they" ("the number of lands they control").</summary>
    static bool MentionsThatPlayer(IrNode node) =>
        node is not null && MTGRulesIR.Lowering.NodeWalker.Nodes(node).Any(x => x is PlayerSelector { Kind: PlayerKind.ThatPlayer } or Reference { Binding: null, Text: "they" or "their" or "them" or "that player" });

    /// <summary>The binding of the objects an effect chooses itself ("sacrifice a creature", "exile a card from your hand"), if it has one.</summary>
    static string ChosenBinding(Effect effect) =>
        effect.GetType().GetProperty("Objects")?.GetValue(effect) is ObjectSelector { Binding: { } binding, Quantifier: not Quantifier.Target } ? binding : null;

    static EffectContext Run(Effect effect, EffectContext ctx)
    {
        var game = ctx.Game;
        var state = ctx.State;

        switch (effect)
        {
            case null:
                return ctx;

            // "Target player draws x cards, {this} deals x damage to any target, and you gain x life, where x is …": the
            // "where x is" lowered around the last step defines x for the steps before it too.
            case Sequence sequence when sequence.Steps.Select((step, i) => (step, i)).FirstOrDefault(x => x.i > 0 && x.step is LetVariable { Name: "X" or "x" }) is ({ } found, var at)
                && sequence.Steps.Take(at).Any(step => MTGRulesIR.Lowering.NodeWalker.Nodes(step).Any(n => n is Variable { Name: "X" or "x" })):
            {
                var let = (LetVariable)found;
                if (Quantities.Evaluate(let.Definition, ctx) is not { } x)
                    return Skipped(game, let, "variable definition", ctx);
                var withX = ctx with { X = x };
                foreach (var step in sequence.Steps.Select(s => ReferenceEquals(s, found) ? let.Effect : s))
                    withX = Execute(step, withX);
                return withX with { X = ctx.X };
            }

            case Sequence sequence:
                foreach (var step in sequence.Steps)
                    ctx = Execute(step, ctx);
                return ctx;

            // "Each player may put an artifact, creature, enchantment, or land card from their hand onto the battlefield": each in turn, for themselves.
            case Optional { Chooser: PlayerSelector { Kind: PlayerKind.EachPlayer or PlayerKind.EachOpponent } } eachMay when Resolver.Players(eachMay.Chooser, ctx) is { Count: > 1 } mayPlayers:
            {
                foreach (var player in mayPlayers)
                {
                    var yes = game.Decide(new YesNo(player, $"You may: {Describe(eachMay.Effect)}")).Yes;
                    game.Emit(new ChoiceMade(player, "may", yes ? "yes" : "no"));
                    if (yes)
                        Execute(eachMay.Effect, ctx with { Last = [TargetRef.Of(player)] });
                }
                return ctx;
            }

            case Optional optional:
            {
                var chooser = optional.Chooser is null ? ctx.Controller : Resolver.Players(optional.Chooser, ctx).FirstOrDefault(ctx.Controller);
                var yes = game.Decide(new YesNo(chooser, $"You may: {Describe(optional.Effect)}")).Yes;
                game.LastOptionalTaken = yes;
                game.Emit(new ChoiceMade(chooser, "may", yes ? "yes" : "no"));
                return yes ? Execute(optional.Effect, ctx) : ctx;
            }

            case Conditional conditional:
            {
                var holds = Conditions.Evaluate(conditional.If, ctx);
                if (holds is null)
                {
                    game.Skip(conditional.If, "condition not evaluable");
                    return ctx;
                }
                game.LastConditionHeld = holds.Value;
                return holds.Value ? Execute(conditional.Then, ctx) : Execute(conditional.Else, ctx);
            }

            case Otherwise otherwise:
                return game.LastConditionHeld ? ctx : Execute(otherwise.Effect, ctx);

            // "It's still a land" is folded into the animation before it (see CardDefinition); alone it changes nothing.
            case StillOfKind:
                return ctx;

            case Lasting lasting:
                return Execute(WithDuration(lasting.Effect, lasting.Duration), ctx);

            case Instead instead:
                return Execute(instead.Effect, ctx);

            case WhenDone done:
            {
                var players = Resolver.Players(done.Player, ctx);
                return game.LastOptionalTaken ? Execute(done.Effect, ctx) : ctx;
            }

            case DealDamage damage:
            {
                var sources = damage.Source is null ? (ctx.Source is { } s ? [s] : []) : Resolver.Objects(damage.Source, ctx);
                var total = 0;
                List<TargetRef> hit = [];
                // "Each creature deals damage to itself equal to its power", "up to two target creatures each deal damage
                // equal to their power": each source deals its own, with "its" and "itself" meaning that source.
                var each = damage.Source is not (null or Self) && sources.Count > 0;
                // "Up to two target creatures each deal damage …" with none chosen deals none.
                if (damage.Source is not (null or Self) && sources.Count == 0)
                    return ctx;
                foreach (var source in each ? sources.Select(x => (ObjectId?)x) : [sources.Count > 0 ? sources[0] : ctx.Source])
                foreach (var portion in damage.Portions)
                {
                    var own = ctx;
                    if (each && source is { } dealer)
                    {
                        var bindings = new Dictionary<string, ImmutableList<TargetRef>>(ctx.Bindings ?? []);
                        if (damage.Source is ObjectSelector { Binding: { } binding })
                            bindings[binding] = [TargetRef.Of(dealer)];
                        if (damage.Source is Reference { Binding: { } referenced })
                            bindings[referenced] = [TargetRef.Of(dealer)];
                        own = ctx with { Last = [TargetRef.Of(dealer)], Bindings = bindings };
                    }
                    var targets = portion.To is null ? (ctx.TriggerSubject is { } subject ? [subject] : ctx.Last ?? [])
                        : portion.To is Reference { Text: "itself" or "themselves" or "himself" or "herself" } ? Resolver.Targets(portion.To, own)
                        : Resolver.Targets(portion.To, ctx);
                    // "Deals damage to each player equal to the number of lands they control": each player's own amount.
                    if (damage.Divided is null && targets.Count >= 1 && targets.All(x => x.IsPlayer) && MentionsThatPlayer(portion.Amount))
                    {
                        foreach (var player in targets)
                            if (Quantities.Evaluate(portion.Amount, own with { Last = [player] }) is > 0 and var share)
                            {
                                game.DealDamage(source, player, share, combat: false);
                                total += share;
                                hit.Add(player);
                            }
                        continue;
                    }
                    var amount = Quantities.Evaluate(portion.Amount, own);
                    if (amount is null)
                    {
                        game.Skip(portion.Amount, "amount not evaluable");
                        continue;
                    }
                    // "Divided as you choose among one, two, or three targets": as evenly as it goes, the first ones getting the remainder.
                    var shares = damage.Divided is not null && targets.Count > 0
                        ? targets.Select((_, n) => amount.Value / targets.Count + (n < amount.Value % targets.Count ? 1 : 0)).ToList()
                        : targets.Select(_ => amount.Value).ToList();
                    for (var n = 0; n < targets.Count; n++)
                    {
                        if (shares[n] <= 0)
                            continue;
                        game.DealDamage(source, targets[n], shares[n], combat: false);
                        total += shares[n];
                        hit.Add(targets[n]);
                    }
                }
                return ctx with { LastAmount = total, Last = hit.Count > 0 ? hit.ToImmutableList() : ctx.Last };
            }

            case Draw draw:
            {
                var count = Quantities.Evaluate(draw.Count, ctx) ?? 1;
                var before = game.Log.Count;
                foreach (var player in Resolver.Players(draw.Player, ctx))
                    game.Draw(player, count);
                // "Draw a card and reveal it. If it isn't a land card, discard it": "it" is what was drawn.
                var drawn = game.Log.Skip(before).OfType<Drew>().Where(x => x.Card is not null).Select(x => TargetRef.Of(x.Card.Value)).ToImmutableList();
                return drawn.Count > 0 ? ctx with { LastAmount = count, Last = drawn, Affected = drawn } : ctx with { LastAmount = count };
            }

            case GainLife gain:
            {
                var amount = Quantities.Evaluate(gain.Amount, ctx);
                if (amount is null)
                {
                    game.Skip(gain.Amount, "amount not evaluable");
                    return ctx;
                }
                foreach (var player in Resolver.Players(gain.Player, ctx))
                    game.ChangeLife(player, amount.Value, "gain");
                return ctx with { LastAmount = amount.Value };
            }

            case LoseLife lose:
            {
                var amount = Quantities.Evaluate(lose.Amount, ctx);
                if (amount is null)
                {
                    game.Skip(lose.Amount, "amount not evaluable");
                    return ctx;
                }
                var lost = 0;
                foreach (var player in Resolver.Players(lose.Player, ctx))
                {
                    var before = game.State.Player(player).Life;
                    game.ChangeLife(player, -amount.Value, "lose");
                    lost += Math.Max(0, before - game.State.Player(player).Life);
                }
                return ctx with { LastAmount = lost };
            }

            case SetLifeTotal set:
            {
                var amount = Quantities.Evaluate(set.Amount, ctx);
                if (amount is null)
                    return Skipped(game, set, "amount not evaluable", ctx);
                foreach (var player in Resolver.Players(set.Player, ctx))
                    game.ChangeLife(player, amount.Value - state.Player(player).Life, "set");
                return ctx;
            }

            case Destroy destroy:
            {
                var objects = Resolver.ForEffect(destroy.Objects, ctx);
                List<TargetRef> affected = [];
                foreach (var id in objects)
                    if (game.Destroy(id, destroy.Manner == "CantBeRegenerated"))
                        affected.Add(TargetRef.Of(id));
                return ctx with { Affected = affected.ToImmutableList(), Last = objects.Select(TargetRef.Of).ToImmutableList() };
            }

            case Exile exile:
            {
                var objects = Resolver.ForEffect(exile.Objects, ctx);
                foreach (var id in objects)
                    game.MoveToZone(id, Zone.Exile);
                if (ctx.Source is { } exiler && game.State.Object(exiler) is { } by)
                    foreach (var card in objects.Select(game.State.Object).Where(x => x is { Zone: Zone.Exile }))
                        game.Emit(new ExiledWith(new ExileLink(exiler, by.ZoneChangeCount, card.Id, card.ZoneChangeCount)));
                if (exile.Until is UntilThisLeaves && ctx.Source is { } keeper && game.State.Object(keeper) is { Zone: Zone.Battlefield } holder)
                {
                    var exiled = objects.Select(game.State.Object).Where(x => x is { Zone: Zone.Exile }).Select(x => (x.Id, x.ZoneChangeCount)).ToImmutableList();
                    if (exiled.Count > 0)
                        game.Emit(new LinkedExileCreated(new LinkedExile(keeper, holder.ZoneChangeCount, exiled)));
                }
                return ctx with { Affected = objects.Select(TargetRef.Of).ToImmutableList(), Last = objects.Select(TargetRef.Of).ToImmutableList() };
            }

            case Sacrifice sacrifice:
            {
                var players = Resolver.Players(sacrifice.Player, ctx);
                if (players.Count == 0)
                    players = [ctx.Controller];
                List<TargetRef> affected = [];
                foreach (var player in players)
                {
                    var chosen = ChooseFromSelector(sacrifice.Objects, player, ctx, "Sacrifice", mustControl: true);
                    foreach (var id in chosen)
                        if (game.Sacrifice(player, id))
                            affected.Add(TargetRef.Of(id));
                }
                return ctx with { Affected = affected.ToImmutableList(), Last = affected.ToImmutableList() };
            }

            case Tap tap:
            {
                var objects = Resolver.ForEffect(tap.Objects, ctx);
                foreach (var id in objects)
                    game.Tap(id);
                return ctx with { Last = objects.Select(TargetRef.Of).ToImmutableList() };
            }

            case Untap untap:
            {
                var objects = Resolver.ForEffect(untap.Objects, ctx);
                foreach (var id in objects)
                    game.Untap(id);
                return ctx with { Last = objects.Select(TargetRef.Of).ToImmutableList() };
            }

            case TapOrUntap either:
            {
                var objects = Resolver.Objects(either.Objects, ctx);
                foreach (var id in objects)
                {
                    var obj = game.State.Object(id);
                    if (obj is null)
                        continue;
                    var options = ImmutableList.Create("Tap", "Untap");
                    if (game.Decide(new ChooseOne(ctx.Controller, $"Tap or untap {obj.Card.Name}?", options)).Index == 0)
                        game.Tap(id);
                    else
                        game.Untap(id);
                }
                return ctx with { Last = objects.Select(TargetRef.Of).ToImmutableList() };
            }

            case PutCounters put:
            {
                var count = Quantities.Evaluate(put.Counters.Count, ctx) ?? 1;
                var kind = put.Counters.Kind;
                if (kind is null)
                    return Skipped(game, put, "counter kind unknown", ctx);
                // "Defending player gets a poison counter": a player's counters.
                if (put.Objects is PlayerSelector or Reference && Resolver.Targets(put.Objects, ctx).Where(x => x.IsPlayer).ToList() is { Count: > 0 } players)
                {
                    foreach (var player in players)
                        if (kind == "poison")
                            game.Emit(new PoisonChanged(player.Player.Value, count));
                        else if (kind is "energy" or "e")
                            game.Emit(new EnergyChanged(player.Player.Value, count));
                    return ctx with { Last = players.ToImmutableList() };
                }
                var objects = put.Objects is null ? (ctx.Source is { } s ? [s] : []) : Resolver.ForEffect(put.Objects, ctx);
                foreach (var id in objects)
                    // Sunburst's counters are +1/+1 on a creature, charge counters on anything else.
                    game.Emit(new CountersChanged(id, kind != "sunburst" ? kind : state.Object(id) is { } burst && burst.Card.IsCreature ? "+1/+1" : "charge", count));
                return ctx with { Last = objects.Select(TargetRef.Of).ToImmutableList() };
            }

            case RemoveCounters { Counters.Kind: null } removeAll:
            {
                // "Remove all counters from …", "remove a counter from …": every kind, or one of any kind.
                var objects = removeAll.Objects is null ? (ctx.Source is { } s ? [s] : []) : Resolver.Objects(removeAll.Objects, ctx);
                var all = removeAll.Counters.Count is Special { Kind: "All" } or null;
                var removed = 0;
                foreach (var id in objects)
                    foreach (var (kind, have) in state.Object(id)?.CounterMap.Where(x => x.Key is not ("regeneration-shield" or "deathtouch-damage" or "monstrous" or "echo-paid")).ToList() ?? [])
                    {
                        var take = all ? have : Math.Min(have, (Quantities.Evaluate(removeAll.Counters.Count, ctx) ?? 1) - removed);
                        if (take <= 0)
                            continue;
                        game.Emit(new CountersChanged(id, kind, -take));
                        removed += take;
                    }
                return ctx with { LastAmount = removed };
            }

            case RemoveCounters remove:
            {
                var kind = remove.Counters.Kind;
                var objects = remove.Objects is null ? (ctx.Source is { } s ? [s] : []) : Resolver.Objects(remove.Objects, ctx);
                var removed = 0;
                foreach (var id in objects)
                {
                    var have = state.Object(id)?.CountersOf(kind) ?? 0;
                    var count = remove.Counters.Count is Special { Kind: "All" } ? have : Math.Min(have, Quantities.Evaluate(remove.Counters.Count, ctx) ?? 1);
                    if (count > 0)
                    {
                        game.Emit(new CountersChanged(id, kind, -count));
                        removed += count;
                    }
                }
                return ctx with { LastAmount = removed };
            }

            case CreateTokens create:
            {
                var count = (Quantities.Evaluate(create.Count, ctx) ?? 1) * (create.PerEach is null ? 1 : Quantities.Evaluate(create.PerEach, ctx) ?? 1);
                if (create.Token is TokenCopy copy)
                {
                    var originals = copy.Of is Self ? (ctx.Source is { } own ? [own] : []) : Resolver.Objects(copy.Of, ctx);
                    List<TargetRef> copies = [];
                    foreach (var original in originals.Select(state.Object).Where(x => x is not null))
                        for (var i = 0; i < count; i++)
                        {
                            var token = game.CreateCopyToken(original.Card, ctx.Controller);
                            if (copy.Tapped)
                                game.Tap(token);
                            if (copy.Stats is { } stats)
                                game.Emit(new TemporaryEffectAdded(Hold(new TemporaryEffect(token, 0, 0, [], null, stats), effect, ctx)));
                            foreach (var exception in copy.Exceptions ?? [])
                                Execute(exception, ctx with { Source = token, Last = [TargetRef.Of(token)] });
                            copies.Add(TargetRef.Of(token));
                        }
                    return ctx with { Affected = copies.ToImmutableList(), Last = copies.ToImmutableList() };
                }
                if (create.Token is not TokenSpec spec)
                    return Skipped(game, create, "token not described", ctx);
                // "Create an x/x green elemental creature token, where x is …": its size as of now.
                if (spec.Power is not (null or Fixed) && Quantities.Evaluate(spec.Power, ctx) is { } tokenPower)
                    spec = spec with { Power = new Fixed(tokenPower) };
                if (spec.Toughness is not (null or Fixed) && Quantities.Evaluate(spec.Toughness, ctx) is { } tokenToughness)
                    spec = spec with { Toughness = new Fixed(tokenToughness) };
                List<TargetRef> made = [];
                for (var i = 0; i < count; i++)
                    made.Add(TargetRef.Of(game.CreateToken(spec, ctx.Controller)));
                return ctx with { Affected = made.ToImmutableList(), Last = made.ToImmutableList() };
            }

            case Pump pump:
            {
                var objects = pump.Objects is null ? (ctx.Source is { } s ? [s] : []) : Resolver.Objects(pump.Objects, ctx);
                var power = Quantities.Evaluate(pump.Power, ctx) ?? 0;
                var toughness = Quantities.Evaluate(pump.Toughness, ctx) ?? 0;
                if (pump.PerEach is not null)
                {
                    var factor = Quantities.Evaluate(pump.PerEach, ctx) ?? 0;
                    power *= factor;
                    toughness *= factor;
                }
                if (pump.Remove)
                {
                    power = -power;
                    toughness = -toughness;
                }
                var keywords = (pump.Keywords ?? []).SelectMany(KeywordAbilities.NamesOf).ToImmutableList();
                if (pump.ChoiceOf is { Count: > 0 } options)
                {
                    var choice = game.Decide(new ChooseOne(ctx.Controller, "Choose a keyword", options.Select(x => x.Keyword).ToImmutableList()));
                    keywords = keywords.AddRange(KeywordAbilities.NamesOf(options[Math.Clamp(choice.Index, 0, options.Count - 1)]));
                }
                var until = UntilTurn(pump.Duration, state);
                if (until is null && pump.Duration is not null)
                    return Skipped(game, pump, $"duration {pump.Duration.GetType().Name}", ctx);
                foreach (var id in objects)
                    game.Emit(new TemporaryEffectAdded(Hold(new TemporaryEffect(id, power, toughness, pump.Remove ? [] : keywords, until == -1 ? null : until, pump.BaseStats), effect, ctx)));
                return ctx with { Last = objects.Select(TargetRef.Of).ToImmutableList() };
            }

            // "Double the number of +1/+1 counters on it": as many again of that kind (each kind, for "each kind of counter").
            // Each target gets one counter, and what's left goes one at a time around them again.
            case DistributeCounters { Targeted: true, Among: ObjectSelector among } distribute:
            {
                var total = Quantities.Evaluate(distribute.Counters.Count, ctx) ?? 0;
                var targets = Resolver.Chosen(among, among.Binding, ctx).Where(x => x.IsObject && state.Object(x.Object.Value) is { Zone: Zone.Battlefield }).Select(x => x.Object.Value).ToList();
                if (targets.Count == 0 || total <= 0 || distribute.Counters.Kind is not { } kind)
                    return ctx;
                var each = targets.ToDictionary(x => x, _ => 0);
                for (var i = 0; i < total; i++)
                    each[targets[i % targets.Count]]++;
                foreach (var (id, count) in each.Where(x => x.Value > 0))
                    game.Emit(new CountersChanged(id, kind, count));
                return ctx with { Affected = each.Where(x => x.Value > 0).Select(x => TargetRef.Of(x.Key)).ToImmutableList(), LastAmount = total };
            }

            // "Move a +1/+1 counter from target creature onto a second target creature", "move all counters from this onto target creature".
            case AssignsNoCombatDamage noDamage when noDamage.Duration is ThisTurn or UntilPhase { Phase.Phase: "EndOfTurn" }:
            {
                foreach (var id in noDamage.Objects is null or Self ? (ctx.Source is { } own ? [own] : []) : Resolver.Objects(noDamage.Objects, ctx))
                    game.Emit(new TemporaryEffectAdded(new TemporaryEffect(id, 0, 0, [], state.Turn, Restrictions: ["AssignCombatDamage"])));
                return ctx;
            }

            // "Exchange control of target land you control and target land an opponent controls": each goes to the other's controller.
            case ExchangeControl exchange:
            {
                var objects = exchange.Objects.SelectMany(x => x is Self ? (ctx.Source is { } self ? [self] : []) : Resolver.Objects(x, ctx))
                    .Select(x => game.State.Object(x)).Where(x => x is { Zone: Zone.Battlefield }).Distinct().ToList();
                if (objects is not [var first, var second] || first.Controller == second.Controller)
                    return ctx;
                game.GainControl([first.Id], second.Controller, null, ctx);
                game.GainControl([second.Id], first.Controller, null, ctx);
                return ctx with { Affected = [TargetRef.Of(first.Id), TargetRef.Of(second.Id)] };
            }

            // "Target creature phases out", "… phases out until this enchantment leaves the battlefield".
            case Phasing phasing when phasing.Duration is null or UntilThisLeaves:
            {
                var objects = phasing.Objects is null or Self ? (ctx.Source is { } own ? [own] : []) : Resolver.Objects(phasing.Objects, ctx);
                foreach (var id in objects)
                    if (phasing.Direction == "In")
                        game.PhaseIn(id);
                    else
                        game.PhaseOut(id, phasing.Duration is UntilThisLeaves ? ctx.Source : null);
                return ctx with { Affected = objects.Select(TargetRef.Of).ToImmutableList() };
            }

            case MoveCounters move:
            {
                var from = move.From is null or Self ? (ctx.Source is { } own ? [own] : []) : Resolver.Objects(move.From, ctx);
                var to = (move.To is Self ? (ctx.Source is { } self ? [self] : []) : Resolver.Objects(move.To, ctx)).Where(x => state.Object(x) is { Zone: Zone.Battlefield }).ToList();
                if (to.Count == 0)
                    return ctx;
                var count = move.Counters.Count is Special { Kind: "All" or "AnyNumber" } ? int.MaxValue : Quantities.Evaluate(move.Counters.Count, ctx) ?? 1;
                var moved = 0;
                foreach (var holder in from.Where(x => !to.Contains(x) || from.Count > 1).Select(x => game.State.Object(x)).Where(x => x is { Zone: Zone.Battlefield }).ToList())
                    foreach (var (kind, have) in holder.CounterMap.Where(x => x.Value > 0 && x.Key is not ("deathtouch-damage" or "regeneration-shield") && (move.Counters.Kind is null || x.Key == move.Counters.Kind)).ToList())
                    {
                        var amount = Math.Min(have, count - moved);
                        if (amount <= 0 || to[0] == holder.Id)
                            continue;
                        game.Emit(new CountersChanged(holder.Id, kind, -amount));
                        game.Emit(new CountersChanged(to[0], kind, amount));
                        moved += amount;
                    }
                return ctx with { LastAmount = moved };
            }

            case DoubleCounters doubled:
            {
                var objects = doubled.On is Self ? (ctx.Source is { } own ? [own] : []) : Resolver.Objects(doubled.On, ctx);
                foreach (var obj in objects.Select(state.Object).Where(x => x is not null).ToList())
                    foreach (var (kind, count) in obj.CounterMap.Where(c => (doubled.EachKind || c.Key == doubled.Kind) && c.Key is not ("deathtouch-damage" or "regeneration-shield") && c.Value > 0).ToList())
                        game.Emit(new CountersChanged(obj.Id, kind, count));
                return ctx;
            }

            // "Double its power until end of turn" is +X/+0 where X is its power; "switch its power and toughness" is the
            // difference, each way (both as of now).
            case DoubleStats or SwitchStats:
            {
                var (selector, duration) = effect is DoubleStats d ? (d.Objects, d.Duration) : (((SwitchStats)effect).Objects, ((SwitchStats)effect).Duration);
                var until = UntilTurn(duration, state);
                if (until is null && duration is not null)
                    return Skipped(game, effect, $"duration {duration.GetType().Name}", ctx);
                var objects = selector is Self ? (ctx.Source is { } own ? [own] : []) : Resolver.Objects(selector, ctx);
                foreach (var obj in objects.Select(state.Object).Where(x => x is { Zone: Zone.Battlefield }).ToList())
                {
                    var traits = Characteristics.Of(state, obj);
                    var (power, toughness) = effect is DoubleStats { Which: var which }
                        ? (which is "Power" or "PowerAndToughness" ? Math.Max(0, traits.Power) : 0, which is "Toughness" or "PowerAndToughness" ? Math.Max(0, traits.Toughness) : 0)
                        : (traits.Toughness - traits.Power, traits.Power - traits.Toughness);
                    game.Emit(new TemporaryEffectAdded(Hold(new TemporaryEffect(obj.Id, power, toughness, [], until == -1 ? null : until), effect, ctx)));
                }
                return ctx with { Last = objects.Select(TargetRef.Of).ToImmutableList() };
            }

            case LoseAllAbilities lose:
            {
                var objects = Resolver.Objects(lose.Objects, ctx);
                foreach (var id in objects)
                    game.Emit(new TemporaryEffectAdded(Hold(new TemporaryEffect(id, 0, 0, [], null, LosesAbilities: true), effect, ctx)));
                return ctx;
            }

            case Animate animate:
            {
                var until = UntilTurn(animate.Duration, state);
                if (until is null && animate.Duration is not null)
                    return Skipped(game, animate, $"duration {animate.Duration.GetType().Name}", ctx);
                if (animate.Transforms)
                    return Skipped(game, animate, "transforms", ctx);
                var objects = animate.Objects is null ? (ctx.Source is { } s ? [s] : []) : Resolver.Objects(animate.Objects, ctx);
                var stats = animate.BaseStats ?? (animate.Power is not null || animate.Toughness is not null ? new StatsMod(Evaluated(animate.Power, ctx), Evaluated(animate.Toughness, ctx)) : null);
                var change = TypeChange.OfAnimate(animate);
                foreach (var id in objects)
                    game.Emit(new TemporaryEffectAdded(Hold(new TemporaryEffect(id, 0, 0, (animate.Keywords ?? []).Select(x => x.Keyword).ToImmutableList(), until == -1 ? null : until, stats, TypeChange: change), effect, ctx)));
                return ctx with { Last = objects.Select(TargetRef.Of).ToImmutableList() };
            }

            case BecomesAspect { Aspects: [{ Vocabulary: "PermanentDesignation", Value: "Saddled" }], Negated: false } saddled:
                return Execute(new BecomeSaddled(saddled.Objects), ctx);

            case BecomeSaddled saddle:
            {
                foreach (var id in saddle.Objects is null or Self ? (ctx.Source is { } own ? [own] : []) : Resolver.Objects(saddle.Objects, ctx))
                    game.Emit(new ChoiceRecorded(id, "SaddledTurn", state.Turn.ToString()));
                return ctx;
            }

            case EnterAttacking:
                if (ctx.Source is { } sneaking && game.State.Object(sneaking) is { Zone: Zone.Battlefield })
                    game.Emit(new EnteredAttacking(sneaking));
                return ctx;

            case BecomesAspect aspect:
            {
                var until = UntilTurn(aspect.Duration, state);
                if (until is null)
                    return Skipped(game, aspect, "aspect duration", ctx);
                if (aspect.Negated)
                {
                    var removes = new TypeChange(Removes: aspect.Aspects.Where(x => x.Vocabulary == "CardType").Select(x => x.Value).ToImmutableList());
                    foreach (var id in aspect.Objects is null ? (ctx.Source is { } own ? [own] : []) : Resolver.Objects(aspect.Objects, ctx))
                        game.Emit(new TemporaryEffectAdded(Hold(new TemporaryEffect(id, 0, 0, [], until == -1 ? null : until, TypeChange: removes), effect, ctx)));
                    return ctx;
                }
                var source = ctx.Source is { } sourceId ? state.Object(sourceId) : null;
                var aspects = aspect.Aspects.Select(term => ChosenNow(term, ctx)).ToList();
                if (TypeChange.OfAspects(aspects, aspect.InAddition, source) is not { } change)
                    return Skipped(game, aspect, "aspect", ctx);
                var objects = aspect.Objects is null ? (ctx.Source is { } s ? [s] : []) : Resolver.Objects(aspect.Objects, ctx);
                foreach (var id in objects)
                    game.Emit(new TemporaryEffectAdded(Hold(new TemporaryEffect(id, 0, 0, [], until == -1 ? null : until, TypeChange: change), effect, ctx)));
                return ctx with { Last = objects.Select(TargetRef.Of).ToImmutableList() };
            }

            case ChooseEffect choose:
                return Choose(choose, ctx);

            case Replacement replacement:
                if (!game.CreateReplacement(replacement, ctx))
                    return Skipped(game, replacement, "replacement", ctx);
                return ctx;

            case RedirectDamage redirect:
                if (!game.CreateRedirect(redirect, ctx))
                    return Skipped(game, redirect, "redirection amount", ctx);
                return ctx;

            case PreventDamage prevent:
                if (prevent.Amount is Special { Kind: "ThatMuch" })
                    return Skipped(game, prevent, "prevent that damage", ctx);
                game.CreateShields(prevent, ctx);
                return ctx;

            case ChangeZone change:
            {
                if (!Enum.TryParse<Zone>(change.To?.Zone, out var zone))
                    return Skipped(game, change, $"zone {change.To?.Zone}", ctx);
                var objects = Resolver.ForEffect(change.Objects, ctx);
                var controller = change.UnderControlOf is null ? (PlayerId?)null : Resolver.Players(change.UnderControlOf, ctx).FirstOrDefault(ctx.Controller);
                var position = change.To.Position == nameof(Position.Top) ? 0 : change.To.Position == nameof(Position.Bottom) ? int.MaxValue : -1;
                foreach (var id in objects)
                {
                    var obj = state.Object(id);
                    if (obj is null)
                        continue;
                    if (zone == Zone.Battlefield && !obj.Card.IsPermanentCard)
                        continue;
                    // "On their choice of the top or bottom of their library": the owner chooses.
                    var at = change.To.Position == "TopOrBottom" ? (game.Decide(new YesNo(obj.Owner, $"Put {obj.Card.Name} on top of your library?")).Yes ? 0 : int.MaxValue) : position;
                    game.MoveToZone(id, zone, controller ?? (zone == Zone.Battlefield ? ctx.Controller : null), change.Tapped, at);
                }
                var moved = objects.Select(TargetRef.Of).ToImmutableList();
                return ctx with { Affected = moved, Last = moved };
            }

            case ShuffleInto shuffle:
            {
                var objects = Resolver.ForEffect(shuffle.Cards, ctx);
                foreach (var id in objects)
                    game.MoveToZone(id, Zone.Library);
                foreach (var owner in objects.Select(id => state.Object(id)?.Owner).Where(x => x is not null).Distinct())
                    game.Shuffle(owner.Value);
                return ctx;
            }

            case Shuffle shuffle:
            {
                var players = shuffle.Player is null ? [ctx.Controller] : Resolver.Players(shuffle.Player, ctx);
                foreach (var player in players)
                    game.Shuffle(player);
                return ctx;
            }

            case Discard discard:
            {
                var players = Resolver.Players(discard.Player, ctx);
                var all = discard.Count is Special { Kind: "All" };
                var count = all ? int.MaxValue : Quantities.Evaluate(discard.Count, ctx) ?? 1;
                List<TargetRef> affected = [];
                foreach (var player in players)
                {
                    var hand = state.InZone(Zone.Hand, player).Where(x => discard.Filter is null || Resolver.PoolMatches(discard.Filter, x, ctx)).Select(x => x.Id).ToList();
                    var take = Math.Min(count, hand.Count);
                    IEnumerable<ObjectId> chosen;
                    if (take >= hand.Count)
                        chosen = hand;
                    else if (discard.AtRandom)
                        chosen = hand.OrderBy(_ => game.Rng.Next()).Take(take);
                    else
                        chosen = game.Decide(new ChooseTargets(player, "Discard", hand.Select(TargetRef.Of).ToImmutableList(), take, take)).Targets?.Where(x => x.IsObject).Select(x => x.Object.Value).Take(take) ?? hand.Take(take);
                    foreach (var id in chosen.ToList())
                    {
                        game.Discard(player, id);
                        affected.Add(TargetRef.Of(id));
                    }
                }
                return ctx with { Affected = affected.ToImmutableList(), LastAmount = affected.Count };
            }

            case Mill mill:
            {
                var count = Quantities.Evaluate(mill.Count, ctx) ?? 1;
                List<TargetRef> affected = [];
                foreach (var player in Resolver.Players(mill.Player, ctx))
                    foreach (var id in game.State.Player(player).Library.Take(count).ToList())
                    {
                        game.Emit(new Milled(player, id));
                        game.MoveToZone(id, Zone.Graveyard);
                        affected.Add(TargetRef.Of(id));
                    }
                return ctx with { Affected = affected.ToImmutableList() };
            }

            case CounterSpell counter:
            {
                var objects = Resolver.Objects(counter.Objects, ctx);
                foreach (var id in objects)
                    game.CounterSpell(id);
                // "If that spell's mana value was 3 or less": the spell countered.
                return objects.Count > 0 ? ctx with { Last = objects.Select(TargetRef.Of).ToImmutableList() } : ctx;
            }

            case AddMana add:
            {
                foreach (var option in add.Options)
                    switch (option)
                    {
                        case ManaAmount amount:
                        {
                            var times = amount.Count is null ? 1 : Quantities.Evaluate(amount.Count, ctx) ?? 1;
                            for (var i = 0; i < times; i++)
                                foreach (var symbol in amount.Symbols)
                                    game.AddManaSymbol(ctx.Controller, symbol);
                            break;
                        }
                        case ManaChoice { Choice: "AnyColor" or "AnyOneColor" } choice:
                        {
                            var count = Quantities.Evaluate(choice.Count, ctx) ?? 1;
                            var pick = game.Decide(new ChooseOne(ctx.Controller, "Choose a color", Colors.All.Select(x => x.ToString()).ToImmutableList()));
                            var color = Colors.All[Math.Clamp(pick.Index, 0, 4)];
                            game.Emit(new ManaAdded(ctx.Controller, color, count));
                            break;
                        }
                        // "Add one mana of the chosen color": the color chosen as the source entered.
                        case ManaChoice { Choice: "TheChosenColor" } chosen:
                        {
                            var count = Quantities.Evaluate(chosen.Count, ctx) ?? 1;
                            if (ctx.Source is { } chooser && Colors.OfName(state.Object(chooser)?.Chosen("Color")) is { } color)
                                game.Emit(new ManaAdded(ctx.Controller, color, count));
                            else
                                game.Skip(option, "no chosen color");
                            break;
                        }
                        // "That player adds one mana of any type that land produced": a kind it could produce.
                        case ManaChoice { Choice: "AnyTypeProduced", Producer: { } producer } produced:
                        {
                            var count = Quantities.Evaluate(produced.Count, ctx) ?? 1;
                            var producers = Resolver.Objects(producer, ctx).Select(x => state.Object(x)).Where(x => x is not null).ToList();
                            var kinds = producers.SelectMany(x => ManaPayment.Produces(state, x)).Distinct().ToList();
                            if (kinds.Count == 0)
                                break;
                            // The mana goes to whoever tapped the permanent for it ("that player adds …").
                            var who = producers[0].Controller;
                            var kind = kinds.Count == 1 ? kinds[0] : kinds[Math.Clamp(game.Decide(new ChooseOne(who, "Choose a kind of mana", kinds.Select(x => x?.ToString() ?? "Colorless").ToImmutableList())).Index, 0, kinds.Count - 1)];
                            if (kind is { } color)
                                game.Emit(new ManaAdded(who, color, count));
                            else
                                game.Emit(new ManaAdded(who, null, count));
                            break;
                        }
                        // "Add two mana in any combination of colors": a color for each.
                        case ManaChoice { Choice: "AnyCombination" } combination:
                        {
                            var count = Quantities.Evaluate(combination.Count, ctx) ?? 1;
                            for (var n = 0; n < count; n++)
                            {
                                var pick = game.Decide(new ChooseOne(ctx.Controller, "Choose a color", Colors.All.Select(x => x.ToString()).ToImmutableList()));
                                game.Emit(new ManaAdded(ctx.Controller, Colors.All[Math.Clamp(pick.Index, 0, 4)], 1));
                            }
                            break;
                        }
                        default:
                            game.Skip(option, "mana option");
                            break;
                    }
                return ctx;
            }

            case ExtraLandPlays { EachTurn: false } extra when extra.Duration is null or ThisTurn && extra.Count is Fixed:
                foreach (var player in Resolver.Players(extra.Player ?? new PlayerSelector(PlayerKind.You), ctx))
                    game.Emit(new ExtraLandsGranted(player, ((Fixed)extra.Count).Value));
                return ctx;

            case ManaRestriction restriction:
                if (ctx.Source is { } maker)
                    game.RestrictRecentMana(ctx.Controller, restriction, maker);
                return ctx;

            case GainControl control:
            {
                var players = Resolver.Players(control.Player, ctx);
                var player = players.Count > 0 ? players[0] : ctx.Controller;
                var objects = Resolver.Objects(control.Objects, ctx);
                if (!game.GainControl(objects, player, control.Duration, ctx))
                    return Skipped(game, control, "control change duration", ctx);
                return ctx with { Last = objects.Select(TargetRef.Of).ToImmutableList() };
            }

            case Fight fight:
            {
                var fighters = Resolver.Objects(fight.Fighter, ctx);
                var opponents = fight.Opponent is null ? [] : Resolver.Objects(fight.Opponent, ctx);
                if (fight.EachOther && fighters.Count >= 2)
                    opponents = [fighters[1]];
                if (fighters.Count == 0 || opponents.Count == 0)
                    return ctx;
                var a = state.Object(fighters[0]);
                var b = state.Object(opponents[0]);
                if (a is null || b is null)
                    return ctx;
                var aTraits = Characteristics.Of(state, a);
                var bTraits = Characteristics.Of(state, b);
                game.DealDamage(a.Id, TargetRef.Of(b.Id), aTraits.Power, combat: false);
                game.DealDamage(b.Id, TargetRef.Of(a.Id), bTraits.Power, combat: false);
                return ctx;
            }

            case Attach attach:
            {
                var attachments = Resolver.Objects(attach.Attachment, ctx);
                // "Unattach all equipment from target creature": no host.
                if (attach.To is null)
                {
                    foreach (var id in attachments)
                        game.Emit(new Attached(id, null));
                    return ctx;
                }
                var hosts = Resolver.Objects(attach.To, ctx);
                if (attachments.Count > 0 && hosts.Count > 0)
                    game.Emit(new Attached(attachments[0], hosts[0]));
                return ctx;
            }

            case GameOutcome outcome:
            {
                foreach (var player in Resolver.Players(outcome.Player, ctx))
                    if (outcome.Result == "Win" && !outcome.Cant)
                        game.Win(player);
                    else if (outcome.Result == "Lose" && !outcome.Cant)
                        game.Emit(new PlayerLost(player, "effect"));
                    else
                        game.Skip(outcome, "outcome");
                return ctx;
            }

            case ExtraTurn extra:
            {
                foreach (var player in Resolver.Players(extra.Player, ctx))
                    game.ExtraTurns.Enqueue(player);
                return ctx;
            }

            case LookAt look:
            {
                var seen = Resolver.Objects(look.What, ctx).Select(TargetRef.Of).ToImmutableList();
                return seen.Count == 0 ? ctx : ctx with { Last = seen, Affected = seen };
            }

            // "Each player reveals the top five cards of their library": each from their own.
            case Reveal { Player: PlayerSelector { Kind: PlayerKind.EachPlayer or PlayerKind.EachOpponent } } reveal when Resolver.Players(reveal.Player, ctx) is { Count: > 1 } revealers:
            {
                List<TargetRef> all = [];
                foreach (var player in revealers)
                    all.AddRange(Resolver.Objects(reveal.What, ctx with { Last = [TargetRef.Of(player)] }).Select(TargetRef.Of));
                return all.Count == 0 ? ctx : ctx with { Last = all.ToImmutableList(), Affected = all.ToImmutableList() };
            }

            case Reveal reveal:
            {
                var shown = Resolver.Objects(reveal.What, ctx).Select(TargetRef.Of).ToImmutableList();
                return shown.Count == 0 ? ctx : ctx with { Last = shown, Affected = shown };
            }

            case RevealUntil until when until.Until is PlayerAction { What: ObjectSelector wanted }:
            {
                var players = until.Player is null ? [ctx.Controller] : Resolver.Players(until.Player, ctx);
                List<TargetRef> revealed = [];
                List<TargetRef> found = [];
                foreach (var player in players)
                    foreach (var id in game.State.Player(player).Library.ToList())
                    {
                        revealed.Add(TargetRef.Of(id));
                        if (until.Verb == "Exile")
                            game.MoveToZone(id, Zone.Exile);
                        if (game.State.Object(id) is { } card && Resolver.PoolMatches(wanted.Filter ?? new ObjectFilter(), card, ctx))
                        {
                            found.Add(TargetRef.Of(id));
                            break;
                        }
                    }
                return ctx with { Last = found.ToImmutableList(), Affected = revealed.ToImmutableList(), Rest = revealed.Except(found).ToImmutableList() };
            }

            case Skip { Phase.Phase: "Turn" } skipTurn:
            {
                var turns = Quantities.Evaluate(skipTurn.Turns, ctx) ?? 1;
                foreach (var player in skipTurn.Player is null ? [ctx.Controller] : Resolver.Players(skipTurn.Player, ctx))
                    game.TurnsToSkip[player] = game.TurnsToSkip.GetValueOrDefault(player) + turns;
                game.Emit(new ChoiceMade(ctx.Controller, "skip turn", turns.ToString()));
                return ctx;
            }

            case Skip skip when Game.SkippableStep(skip.Phase?.Phase) is { } skipped:
            {
                foreach (var player in skip.Player is null ? [ctx.Controller] : Resolver.Players(skip.Player, ctx))
                    game.Emit(new StepSkipScheduled(player, skipped));
                return ctx;
            }

            // "Target creature blocks this creature this turn if able"; "it must be blocked this turn if able".
            case MustBlock must when must.Attacker is not null && must.Duration is ThisTurn or UntilPhase { Phase.Phase: "EndOfTurn" }:
            {
                var attackers = must.Attacker is Self ? (ctx.Source is { } own ? [own] : []) : Resolver.Objects(must.Attacker, ctx);
                foreach (var blocker in Resolver.Objects(must.Blockers, ctx))
                    foreach (var attacker in attackers)
                        game.Emit(new BlockRequired(new BlockRequirement(blocker, attacker, state.Turn)));
                return ctx;
            }
            case MustBlock mustBlock:
            {
                var until = UntilTurn(mustBlock.Duration, state) ?? state.Turn;
                foreach (var id in Resolver.Objects(mustBlock.Blockers, ctx))
                    game.Emit(new TemporaryEffectAdded(Hold(new TemporaryEffect(id, 0, 0, [], until == -1 ? null : until, Restrictions: ["MustBlock"]), effect, ctx)));
                return ctx;
            }

            case CastAsThoughFlash flash:
            {
                var until = UntilTurn(flash.Duration, state);
                if (until is null)
                    return Skipped(game, flash, "duration", ctx);
                game.Emit(new PlayRestricted(new PlayRestriction(ctx.Controller, "Flash", flash.Spells, ctx.Source, ctx.Controller, until == -1 ? null : until)));
                return ctx;
            }

            case CantPlay cantPlay:
            {
                var until = cantPlay.Duration is DuringPhase ? state.Turn + 1 : UntilTurn(cantPlay.Duration, state);
                if (until is null)
                    return Skipped(game, cantPlay, "duration", ctx);
                foreach (var player in Resolver.Players(cantPlay.Player, ctx))
                    game.Emit(new PlayRestricted(new PlayRestriction(player, cantPlay.Verb, cantPlay.What, ctx.Source, ctx.Controller, until == -1 ? null : until, cantPlay.OrActivate is not null, cantPlay.MoreThan)));
                return ctx;
            }

            case GetEnergy energy:
            {
                var amount = Quantities.Evaluate(energy.Amount, ctx) ?? 1;
                foreach (var player in energy.Player is null ? [ctx.Controller] : Resolver.Players(energy.Player, ctx))
                    game.Emit(new EnergyChanged(player, amount));
                return ctx with { LastAmount = amount };
            }

            case CanAsThough asThough:
            {
                var objects = asThough.Objects is null ? (ctx.Source is { } own ? [own] : []) : Resolver.Objects(asThough.Objects, ctx);
                var until = UntilTurn(asThough.Duration, state);
                if (Characteristics.AsThough(asThough) is not { } allowance || until is null)
                    return Skipped(game, asThough, "as though", ctx);
                foreach (var id in objects)
                    game.Emit(new TemporaryEffectAdded(Hold(new TemporaryEffect(id, 0, 0, [], until == -1 ? null : until, Restrictions: [allowance]), effect, ctx)));
                return ctx;
            }

            case CantBeActivated cantActivate:
            {
                var holders = Characteristics.AbilityHolders(cantActivate.Abilities) switch
                {
                    ObjectSelector { Quantifier: Quantifier.Attached } => ctx.Last?.Where(x => x.IsObject).Select(x => x.Object.Value).ToImmutableList() ?? [],
                    var selector => Resolver.Objects(selector, ctx),
                };
                foreach (var id in holders)
                    game.Emit(new TemporaryEffectAdded(Hold(new TemporaryEffect(id, 0, 0, [], state.Turn, Restrictions: [cantActivate.ExceptMana ? "ActivateNonMana" : "Activate"]), effect, ctx)));
                return ctx;
            }

            case CreateEmblem emblem:
            {
                foreach (var player in emblem.Player is null ? [ctx.Controller] : Resolver.Players(emblem.Player, ctx))
                {
                    var id = new ObjectId(game.State.NextObjectId);
                    var card = new CardDefinition("Emblem", ManaCost.Free, [], ["Emblem"], [], null, null, null, [CardDefinition.Unwrap(emblem.Ability)], [], [], IsToken: false);
                    game.Emit(new ObjectCreated(new GameObject(id, card, player, player, Zone.Command)));
                }
                return ctx;
            }

            case CopySpell copy:
            {
                var spells = copy.Spell is Self ? (ctx.Source is { } own ? [own] : []) : Resolver.Objects(copy.Spell, ctx);
                if (spells.Count == 0 && ctx.TriggerSubject is { IsObject: true } subject)
                    spells = [subject.Object.Value];
                var times = copy.Times is null ? 1 : Quantities.Evaluate(copy.Times, ctx) ?? 1;
                List<TargetRef> copies = [];
                foreach (var spell in spells.Where(id => state.Object(id)?.Zone == Zone.Stack))
                    copies.AddRange(game.CopySpell(spell, ctx.Controller, times, copy.NewTargets).Select(TargetRef.Of));
                return ctx with { Last = copies.ToImmutableList(), Affected = copies.ToImmutableList() };
            }

            case ChangeTargets change:
            {
                var spells = Resolver.Objects(change.Spell, ctx);
                foreach (var spell in spells)
                    game.ChangeTargets(spell, ctx.Controller, ctx.Source, change.To);
                return ctx;
            }

            case BecomeCopy copy:
            {
                var until = UntilTurn(copy.Duration, state);
                if (until is null)
                    return Skipped(game, copy, "copy duration", ctx);
                var copiers = copy.Objects is null or Self ? (ctx.Source is { } own ? [own] : []) : Resolver.Objects(copy.Objects, ctx);
                // "Any creature on the battlefield" is chosen now; a target or reference is what it names.
                var originals = copy.Of switch
                {
                    ObjectSelector { Quantifier: Quantifier.Any } any => Resolver.Objects(any with { Quantifier = Quantifier.All }, ctx).Where(id => !copiers.Contains(id)).ToImmutableList(),
                    var named => Resolver.Objects(named, ctx),
                };
                if (originals.Count == 0)
                    return ctx;
                var options = originals.Select(TargetRef.Of).ToImmutableList();
                var picked = options.Count == 1 ? options[0] : game.Decide(new ChooseTargets(ctx.Controller, "Choose what to copy", options, 1, 1)).Targets?.FirstOrDefault(options.Contains) ?? options[0];
                if (state.Object(picked.Object.Value) is not { } original)
                    return ctx;
                foreach (var id in copiers)
                {
                    game.Emit(new BecameCopy(id, original.Card, until == -1 ? null : until));
                    foreach (var exception in copy.Exceptions ?? [])
                        Execute(exception, ctx with { Source = id, Last = [TargetRef.Of(id)] });
                }
                return ctx;
            }

            case GainClassLevel gain:
                if (ctx.Source is { } classCard && state.Object(classCard) is { } leveled && leveled.CountersOf("class-level") < gain.Level - 1)
                    game.Emit(new CountersChanged(classCard, "class-level", gain.Level - 1 - leveled.CountersOf("class-level")));
                return ctx;

            case TurnFace turn:
            {
                foreach (var id in Resolver.Objects(turn.Objects, ctx))
                    if (turn.Direction is "Up" or "FaceUp")
                        game.TurnUp(id, pay: false);
                    else
                        game.Emit(new TurnedFace(id, FaceDown: true));
                return ctx;
            }

            case FlipCoin flip:
            {
                var flipper = flip.Player is null ? ctx.Controller : Resolver.Players(flip.Player, ctx).FirstOrDefault(ctx.Controller);
                game.WonLastFlip = game.Rng.Next(2) == 0;
                game.LastOptionalTaken = game.WonLastFlip;
                game.Emit(new ChoiceMade(flipper, "flip", game.WonLastFlip ? "won" : "lost"));
                return ctx;
            }

            case RollDice roll:
            {
                var count = Quantities.Evaluate(roll.Count, ctx) ?? 1;
                var total = 0;
                for (var i = 0; i < count; i++)
                    total += game.Rng.Next(1, Math.Max(2, roll.Faces) + 1);
                game.LastDieRoll = total;
                game.Emit(new ChoiceMade(ctx.Controller, $"d{roll.Faces}", total.ToString()));
                return ctx with { LastAmount = total };
            }

            case PlayCard play:
            {
                var player = play.Player is null ? ctx.Controller : Resolver.Players(play.Player, ctx).FirstOrDefault(ctx.Controller);
                var cards = play.Card is Self ? (ctx.Source is { } own ? [own] : []) : Resolver.Objects(play.Card, ctx);
                if (UntilTurn(play.Duration, state, player) is null)
                    return Skipped(game, play, "play duration", ctx);
                game.GrantPlay(play, cards, player, ctx);
                return ctx;
            }

            case CantBeRegenerated cantRegenerate:
            {
                var objects = cantRegenerate.Objects is null ? (ctx.Affected ?? ctx.Last ?? []).Where(x => x.IsObject).Select(x => x.Object.Value).ToImmutableList() : Resolver.Objects(cantRegenerate.Objects, ctx);
                foreach (var id in objects)
                    game.Emit(new TemporaryEffectAdded(Hold(new TemporaryEffect(id, 0, 0, [], state.Turn, Restrictions: ["BeRegenerated"]), effect, ctx)));
                return ctx;
            }

            case CantBeTargeted cantBe:
            {
                var by = cantBe.By ?? [];
                if (by.Contains("Countered") && cantBe.Objects is Self)
                    return ctx;
                var restriction = by.Contains("Countered") ? "BeCountered" : cantBe.Controller is not null ? "BeTargetedByOpponents" : "BeTargeted";
                var until = UntilTurn(cantBe.Duration, state);
                if (until is null)
                    return Skipped(game, cantBe, "duration", ctx);
                foreach (var id in Resolver.Objects(cantBe.Objects, ctx))
                    game.Emit(new TemporaryEffectAdded(Hold(new TemporaryEffect(id, 0, 0, [], until == -1 ? null : until, Restrictions: [restriction]), effect, ctx)));
                return ctx;
            }

            case MustCombat must:
            {
                var objects = must.Objects is null ? (ctx.Source is { } s ? [s] : []) : Resolver.Objects(must.Objects, ctx);
                var until = must.Period is "ThisTurn" ? state.Turn : (int?)null;
                foreach (var id in objects)
                    game.Emit(new TemporaryEffectAdded(Hold(new TemporaryEffect(id, 0, 0, [], until, Restrictions: must.Actions.Select(x => "Must" + x).ToImmutableList()), effect, ctx)));
                return ctx;
            }

            case SetStats set:
            {
                var amount = Quantities.Evaluate(set.Amount, ctx);
                if (amount is null)
                    return Skipped(game, set, "amount", ctx);
                var objects = set.Objects is null ? (ctx.Source is { } s ? [s] : []) : Resolver.Objects(set.Objects, ctx);
                foreach (var id in objects)
                {
                    var traits = state.Object(id) is { } obj ? Characteristics.Of(state, obj) : null;
                    var stats = new StatsMod(set.Which is "Toughness" ? new Fixed(traits?.Power ?? 0) : new Fixed(amount.Value), set.Which is "Power" ? new Fixed(traits?.Toughness ?? 0) : new Fixed(amount.Value));
                    game.Emit(new TemporaryEffectAdded(Hold(new TemporaryEffect(id, 0, 0, [], null, stats), effect, ctx)));
                }
                return ctx;
            }

            case LetVariable let:
            {
                var value = Quantities.Evaluate(let.Definition, ctx);
                if (value is null)
                    return Skipped(game, let, "variable definition", ctx);
                return Execute(let.Effect, ctx with { X = value.Value }) with { X = ctx.X };
            }

            // "For each creature target player controls, create a token that's a copy of that creature": once for each, about it.
            case ForEachOf { Count: CountOf { Of: ObjectSelector each } } forEach when MTGRulesIR.Lowering.NodeWalker.Nodes(forEach.Effect).OfType<Reference>().Any(r => r.Binding is null || r.Binding == each.Binding):
            {
                foreach (var id in Resolver.Objects(each, ctx).ToList())
                {
                    var bindings = new Dictionary<string, ImmutableList<TargetRef>>(ctx.Bindings ?? []);
                    if (each.Binding is { } binding)
                        bindings[binding] = [TargetRef.Of(id)];
                    ctx = Execute(forEach.Effect, ctx with { Last = [TargetRef.Of(id)], TriggerSubject = TargetRef.Of(id), Bindings = bindings }) with { TriggerSubject = ctx.TriggerSubject, Bindings = ctx.Bindings };
                }
                return ctx;
            }

            // "Each player draws a card for each creature card in their graveyard": each player as many times as their own count says.
            case ForEachOf forEach when MentionsThatPlayer(forEach.Count) && forEach.Effect.GetType().GetProperty("Player") is { CanWrite: true } playerProperty
                && playerProperty.GetValue(forEach.Effect) is PlayerSelector { Kind: PlayerKind.EachPlayer or PlayerKind.EachOpponent } each:
            {
                var single = (Effect)forEach.Effect.GetType().GetMethod("<Clone>$")!.Invoke(forEach.Effect, null);
                playerProperty.SetValue(single, new PlayerSelector(PlayerKind.ThatPlayer));
                foreach (var player in Resolver.Players(each, ctx).ToList())
                {
                    var own = ctx with { Last = [TargetRef.Of(player)] };
                    var times = Quantities.Evaluate(forEach.Count, own) ?? 0;
                    for (var i = 0; i < times; i++)
                        Execute(single, own);
                }
                return ctx;
            }

            case ForEachOf forEach:
            {
                var times = Quantities.Evaluate(forEach.Count, ctx) ?? 0;
                for (var i = 0; i < times; i++)
                    ctx = Execute(forEach.Effect, ctx);
                return ctx;
            }

            case ChoiceOf choice:
            {
                var pick = game.Decide(new ChooseOne(ctx.Controller, "Choose one", choice.Options.Select(Describe).ToImmutableList()));
                return Execute(choice.Options[Math.Clamp(pick.Index, 0, choice.Options.Count - 1)], ctx);
            }

            case RemoveFromCombat remove:
            {
                foreach (var id in Resolver.Objects(remove.Objects, ctx))
                    game.Emit(new RemovedFromCombat(id));
                return ctx;
            }

            case KeywordAction action:
                return KeywordActions.Execute(action, ctx);

            case UnlessPaid unless:
            {
                var payer = Payer(unless, ctx);
                var paid = false;
                if (payer is { } who && !state.Player(who).Lost)
                    switch (KeywordAbilities.CostOf(unless.Payment.Cost))
                    {
                        case PayMana mana:
                        {
                            var cost = ManaCost.Parse(string.Concat(mana.Symbols));
                            if (unless.Payment.PerEach is { } perEach)
                                cost = Enumerable.Repeat(cost, Math.Max(0, Quantities.Evaluate(perEach, ctx) ?? 1)).Aggregate(ManaCost.Free, (sum, one) => sum.Plus(one));
                            if (ManaPayment.CanPay(state, who, cost) && game.Decide(new YesNo(who, $"Pay {cost} to prevent: {Describe(unless.Effect)}?")).Yes)
                                paid = game.PayMana(who, cost);
                            break;
                        }
                        case PayLife life:
                        {
                            var amount = Quantities.Evaluate(life.Amount, ctx) ?? 0;
                            if (state.Player(who).Life >= amount && game.Decide(new YesNo(who, $"Pay {amount} life to prevent: {Describe(unless.Effect)}?")).Yes)
                            {
                                game.ChangeLife(who, -amount, "cost");
                                paid = true;
                            }
                            break;
                        }
                        case ActionCost { Action: var action }:
                            if (game.ActionPayable(action, who, ctx.Source, ctx.X) && game.Decide(new YesNo(who, $"{Describe(action)} to prevent: {Describe(unless.Effect)}?")).Yes)
                            {
                                Execute(action, ctx with { Controller = who });
                                paid = true;
                            }
                            break;
                        default:
                            game.Skip(unless.Payment.Cost, "unless-cost kind");
                            break;
                    }
                game.LastOptionalTaken = paid;
                return paid ? ctx : Execute(unless.Effect, ctx);
            }

            case CombatRestriction restriction:
            {
                var objects = restriction.Objects is null ? (ctx.Source is { } s ? [s] : []) : Resolver.Objects(restriction.Objects, ctx);
                var until = UntilTurn(restriction.Duration, state);
                if (until is null)
                    return Skipped(game, restriction, $"duration {restriction.Duration?.GetType().Name}", ctx);
                if (restriction.If is not null || restriction.By is not null || restriction.Modifiers is { Count: > 0 } || restriction.Recipient is not null)
                {
                    var rules = restriction.Cant.Select(cant => new CombatRule(cant, restriction.By, restriction.If, restriction.Modifiers, restriction.Recipient, ctx.Source ?? default, ctx.Controller)).ToImmutableList();
                    foreach (var id in objects)
                        game.Emit(new TemporaryEffectAdded(Hold(new TemporaryEffect(id, 0, 0, [], until == -1 ? null : until, CombatRules: rules), effect, ctx)));
                    return ctx;
                }
                foreach (var id in objects)
                    game.Emit(new TemporaryEffectAdded(Hold(new TemporaryEffect(id, 0, 0, [], until == -1 ? null : until, Restrictions: restriction.Cant.ToImmutableList()), effect, ctx)));
                return ctx;
            }

            // Collect evidence: exile cards from your graveyard, the greatest mana values first, until their total is enough.
            case CollectEvidence evidence:
            {
                var total = 0;
                List<TargetRef> exiled = [];
                foreach (var card in state.Player(ctx.Controller).Graveyard.Select(state.Object).Where(x => x is not null && x.Id != ctx.Source).OrderByDescending(x => x.Card.ManaCost.ManaValue).ToList())
                {
                    if (total >= evidence.Total)
                        break;
                    game.MoveToZone(card.Id, Zone.Exile);
                    total += card.Card.ManaCost.ManaValue;
                    exiled.Add(TargetRef.Of(card.Id));
                }
                return ctx with { Affected = exiled.ToImmutableList() };
            }

            case MustBeBlocked must when must.Duration is ThisTurn or UntilPhase { Phase.Phase: "EndOfTurn" }:
            {
                foreach (var attacker in must.Objects is Self ? (ctx.Source is { } own ? [own] : []) : Resolver.Objects(must.Objects, ctx))
                    game.Emit(new BlockRequired(new BlockRequirement(null, attacker, state.Turn)));
                return ctx;
            }

            case NinjutsuEnter when ctx.Source is { } ninja && state.Object(ninja) is { Zone: Zone.Hand }:
                game.MoveToZone(ninja, Zone.Battlefield, ctx.Controller, tapped: true);
                if (game.State.Object(ninja) is { Zone: Zone.Battlefield })
                    game.Emit(new EnteredAttacking(ninja));
                return ctx;

            case ZombieCopyOfThis zombie when ctx.Source is { } embalmed && state.Object(embalmed) is { } card:
            {
                var copy = card.Card with
                {
                    ManaCost = ManaCost.Free,
                    Colors = [MTGEngine.Core.Colors.OfName(zombie.Color).Value],
                    Subtypes = card.Card.Subtypes.Add("Zombie"),
                    Power = zombie.Size ?? card.Card.Power,
                    Toughness = zombie.Size ?? card.Card.Toughness,
                };
                var token = game.CreateCopyToken(copy, ctx.Controller);
                return ctx with { Affected = [TargetRef.Of(token)], Last = [TargetRef.Of(token)] };
            }

            // "This turn, each creature you control enters with an additional +1/+1 counter on it": an entry replacement its source has for that long.
            case EntersBattlefield { Duration: ThisTurn or UntilPhase { Phase.Phase: "EndOfTurn" } } lasting when ctx.Source is { } replacer && state.Object(replacer) is { Zone: Zone.Battlefield }:
                game.Emit(new TemporaryEffectAdded(new TemporaryEffect(replacer, 0, 0, [], state.Turn, Abilities: [new ReplacementAbility(lasting with { Duration = null })])));
                return ctx;

            // "If a creature enters this way, it enters with an additional +1/+1 counter on it": a step after the move, so the
            // counters go on the permanent that just entered.
            case EntersBattlefield { Duration: null, States: null or { Count: 0 }, With: { Count: > 0 } with } entered when entered.Objects is not (null or Self):
            {
                foreach (var id in Resolver.Objects(entered.Objects, ctx).Where(x => state.Object(x) is { Zone: Zone.Battlefield, EnteredThisTurn: true }).ToList())
                    foreach (var step in with)
                        Execute(step, ctx with { Source = id, Last = [TargetRef.Of(id)] });
                return ctx;
            }

            case ExileForLater:
                if (ctx.Source is { } warped)
                    game.ExileForLater(warped);
                return ctx;

            case GrantUntilEndOfTurn granted:
            {
                foreach (var id in Resolver.Objects(granted.Objects, ctx).Where(id => id != ctx.Source))
                    game.Emit(new TemporaryEffectAdded(Hold(new TemporaryEffect(id, 0, 0, [], state.Turn, Abilities: granted.Abilities.ToImmutableList()), effect, ctx)));
                return ctx;
            }

            case GrantAbility grant:
            {
                var objects = grant.Objects is null ? (ctx.Source is { } s ? [s] : []) : Resolver.Objects(grant.Objects, ctx);
                foreach (var id in objects)
                    game.Emit(new TemporaryEffectAdded(Hold(new TemporaryEffect(id, 0, 0, (grant.Alongside ?? []).Select(x => x.Keyword).ToImmutableList(), null, Abilities: [grant.Ability]), effect, ctx)));
                return ctx;
            }

            case Delayed delayed:
            {
                var expires = delayed.When is ObjectEvent { Only: "ThisTurn" or "ThisCombat" } || delayed.When is PhaseEvent { At.Next: false } ? state.Turn : (int?)null;
                var subject = ctx.TriggerSubject ?? ctx.Last?.FirstOrDefault();
                game.Emit(new DelayedTriggerCreated(new DelayedTrigger(state.NextDelayedId, delayed.When, delayed.Effect, ctx.Controller, ctx.Source, subject, expires)));
                return ctx;
            }

            case SearchLibrary search:
            {
                var searchers = search.Searcher is null ? [ctx.Controller] : Resolver.Players(search.Searcher, ctx);
                game.SearchedThisWay = false;
                if (search.Optional && !game.Decide(new YesNo(ctx.Controller, "Search your library?")).Yes)
                    return ctx;
                game.SearchedThisWay = true;
                List<TargetRef> found = [];
                foreach (var searcher in searchers)
                    foreach (var wanted in search.Wanted ?? [])
                    {
                        if (wanted is not ObjectSelector selector)
                            continue;
                        var all = selector.Quantifier == Quantifier.All || selector.Count is Special { Kind: "All" };
                        var count = selector.Quantifier == Quantifier.Count ? Quantities.Evaluate(selector.Count, ctx) ?? 1 : selector.Count is Special { Kind: "AnyNumber" } || all ? int.MaxValue : 1;
                        // The zones are the searcher's; "search target player's library", "search its controller's graveyard, hand, and library" are searched by you.
                        var searched = search.Zones switch
                        {
                            "LibraryAndOrGraveyard" or "LibraryAndGraveyard" => state.InZone(Zone.Library, searcher).Concat(state.InZone(Zone.Graveyard, searcher)),
                            "GraveyardHandAndLibrary" or "GraveyardHandAndOrLibrary" => state.InZone(Zone.Graveyard, searcher).Concat(state.InZone(Zone.Hand, searcher)).Concat(state.InZone(Zone.Library, searcher)),
                            _ => state.InZone(Zone.Library, searcher),
                        };
                        var chooser = search.Searcher is PlayerSelector { Kind: PlayerKind.TargetPlayer or PlayerKind.TargetOpponent } || search.Zones.Contains("Hand") ? ctx.Controller : searcher;
                        var library = Filters.DistinctBy(searched.Where(x => Filters.Matches(state, (selector.Filter ?? new ObjectFilter()) with { Zone = null }, x with { Zone = Zone.Battlefield }, searcher, ctx.Source, ctx: ctx) || Filters.Matches(state, selector.Filter, x, searcher, ctx.Source, ctx: ctx)), selector.Filter, state, x => x).Select(x => TargetRef.Of(x.Id)).ToImmutableList();
                        var take = Math.Min(count, library.Count);
                        if (take == 0)
                            continue;
                        // "For all cards with that name": every one found.
                        var chosen = all ? library.ToList() : game.Decide(new ChooseTargets(chooser, "Search: choose cards", library, 0, take)).Targets?.Where(library.Contains).Distinct().Take(take).ToList() ?? library.Take(take).ToList();
                        found.AddRange(chosen);
                    }
                ctx = ctx with { Affected = found.ToImmutableList(), Last = found.Count > 0 ? found.ToImmutableList() : ctx.Last };
                if (search.Placement is not null)
                    ctx = Execute(search.Placement, ctx);
                else
                    foreach (var card in found)
                        game.MoveToZone(card.Object.Value, Zone.Hand);
                if (search.Ending is not null)
                    ctx = Execute(search.Ending, ctx);
                else
                    foreach (var searcher in searchers)
                        game.Shuffle(searcher);
                return ctx;
            }

            case DoesntUntap doesnt:
            {
                var objects = Resolver.Objects(doesnt.Objects, ctx);
                var source = ctx.Source is { } held ? state.Object(held) : null;
                foreach (var id in objects)
                {
                    // "For as long as this artifact remains tapped / on the battlefield", "for as long as you control this creature".
                    var lasting = doesnt.If switch
                    {
                        Did { Effect: Remains { State: "Tapped" } } => new TemporaryEffect(id, 0, 0, [], null, DoesntUntap: true, While: new SourceTapped(), WhileSource: ctx.Source, WhileController: ctx.Controller, WhileSourceZoneChanges: source?.ZoneChangeCount ?? -1),
                        Did { Effect: Remains } => new TemporaryEffect(id, 0, 0, [], null, DoesntUntap: true, WhileSource: ctx.Source, WhileController: ctx.Controller, WhileSourceZoneChanges: source?.ZoneChangeCount ?? -1),
                        Controls control => new TemporaryEffect(id, 0, 0, [], null, DoesntUntap: true, While: control, WhileSource: ctx.Source, WhileController: ctx.Controller),
                        _ => null,
                    };
                    game.Emit(new TemporaryEffectAdded(lasting ?? Hold(new TemporaryEffect(id, 0, 0, [], state.Turn + 1, DoesntUntap: true), effect, ctx)));
                }
                return ctx;
            }

            case ChooseCards choose:
            {
                var choosers = Resolver.Players(choose.Chooser, ctx);
                var chooser = choosers.Count > 0 ? choosers[0] : ctx.Controller;
                var from = Resolver.Objects(choose.From, ctx);
                var pool = from.Count > 0 ? from : (ctx.Affected ?? ctx.Last ?? []).Where(x => x.IsObject).Select(x => x.Object.Value).ToImmutableList();
                var candidates = pool.Where(id => state.Object(id) is { } obj && (choose.Filter is null || Resolver.PoolMatches(choose.Filter with { Zone = null }, obj, ctx))).Select(TargetRef.Of).ToImmutableList();
                var count = Math.Min(Quantities.Evaluate(choose.Count, ctx) ?? 1, candidates.Count);
                var picked = candidates.Count == 0 ? [] : game.Decide(new ChooseTargets(chooser, "Choose cards", candidates, choose.UpTo ? 0 : count, count)).Targets?.Where(candidates.Contains).Distinct().Take(count).ToImmutableList() ?? candidates.Take(count).ToImmutableList();
                var rest = pool.Select(TargetRef.Of).Where(x => !picked.Contains(x)).ToImmutableList();
                return ctx with { Affected = picked, Last = picked, Rest = rest };
            }

            case ChooseModes:
                return ctx;

            // A cost change is a rule of casting (see Game.CostToCast), not something that happens on resolution.
            case CostModification:
                return ctx;

            case PayCost pay:
            {
                var payer = pay.Player is null ? ctx.Controller : Resolver.Players(pay.Player, ctx).FirstOrDefault(ctx.Controller);
                var paid = false;
                switch (KeywordAbilities.CostOf(pay.Cost))
                {
                    case PayMana energy when energy.Symbols.All(x => x.Equals("{e}", StringComparison.OrdinalIgnoreCase)):
                    {
                        var needed = energy.Symbols.Count * (pay.PerEach is null ? 1 : Quantities.Evaluate(pay.PerEach, ctx) ?? 1);
                        if (state.Player(payer).Energy >= needed)
                        {
                            game.Emit(new EnergyChanged(payer, -needed));
                            paid = true;
                        }
                        break;
                    }
                    case PayMana mana when mana.Symbols.All(x => !x.Contains('e', StringComparison.OrdinalIgnoreCase)):
                    {
                        var cost = ManaCost.Parse(string.Concat(mana.Symbols));
                        if (pay.PerEach is not null)
                            cost = cost with { Generic = cost.Generic * (Quantities.Evaluate(pay.PerEach, ctx) ?? 1) };
                        paid = ManaPayment.CanPay(state, payer, cost) && game.PayMana(payer, cost);
                        break;
                    }
                    case PayLife life when Quantities.Evaluate(life.Amount, ctx) is { } amount:
                        if (state.Player(payer).Life >= amount)
                        {
                            game.ChangeLife(payer, -amount, "cost");
                            paid = true;
                        }
                        break;
                    case ActionCost { Action: var action } when game.ActionPayable(action, payer, ctx.Source, ctx.X):
                        ctx = Execute(action, ctx with { Controller = payer }) with { Controller = ctx.Controller };
                        paid = true;
                        break;
                    case ActionCost:
                        break;
                    default:
                        game.Skip(pay.Cost, "payment kind");
                        break;
                }
                game.LastOptionalTaken = paid;
                return ctx;
            }

            case Regenerate regenerate:
            {
                foreach (var id in Resolver.Objects(regenerate.Objects, ctx))
                    game.Emit(new CountersChanged(id, "regeneration-shield", 1));
                return ctx;
            }

            default:
                return Skipped(game, effect, "effect not interpreted", ctx);
        }
    }

    /// <summary>Who an "unless … pays" asks: the named player, else the controller of what the effect targets, else that player, else an opponent.</summary>
    static PlayerId? Payer(UnlessPaid unless, EffectContext ctx)
    {
        if (unless.Payment.Player is not null && Resolver.Players(unless.Payment.Player, ctx) is [var named, ..])
            return named;
        // "Unless you discard two cards": the action names who does it.
        if (unless.Payment.Cost is ActionCost { Action: var action } && action.GetType().GetProperty("Player")?.GetValue(action) is Selector actor && Resolver.Players(actor, ctx) is [var doer, ..])
            return doer;
        // "Sacrifice it unless you exile a creature you control": what the cost uses is yours, or what it saves is.
        if (unless.Payment.Cost is ActionCost { Action: var owned } && MTGRulesIR.Lowering.NodeWalker.Nodes(owned).OfType<ObjectFilter>().Any(x => x.Controller is { Player: PlayerSelector { Kind: PlayerKind.You }, Negated: false }))
            return ctx.Controller;
        if (unless.Effect is Sacrifice { Objects: Self } or Sacrifice { Player: PlayerSelector { Kind: PlayerKind.You } })
            return ctx.Controller;
        foreach (var target in ctx.TargetsBySelector.Values.SelectMany(x => x))
        {
            if (target.IsPlayer)
                return target.Player;
            if (ctx.State.Object(target.Object.Value) is { } obj)
                return obj.Controller;
        }
        if (ctx.Last?.FirstOrDefault(x => x.IsPlayer) is { IsPlayer: true } last)
            return last.Player;
        return ctx.State.OpponentsOf(ctx.Controller).FirstOrDefault();
    }

    static EffectContext Skipped(Game game, IrNode node, string why, EffectContext ctx)
    {
        game.Skip(node, why);
        return ctx;
    }

    /// <summary>The turn after which a duration ends: this turn for "until end of turn", -1 for no end, null for one the engine doesn't keep.</summary>
    public static int? UntilTurn(Duration duration, GameState state, PlayerId? you = null) =>
        duration switch
        {
            null => -1,
            // "Until the end of your next turn": the next turn that's yours (the lowering marks it as the end of your turn).
            UntilPhase { Phase: { Phase: "Turn", Part: "End", Whose: PlayerSelector { Kind: PlayerKind.You } } } => NextTurnOf(state, you ?? state.ActivePlayer),
            ThisTurn => state.Turn,
            UntilPhase { Phase.Phase: "EndOfTurn" or "EndStep" or "Cleanup" or "End" } => state.Turn,
            UntilPhase { Phase.Phase: "Turn", Phase.Next: true } => state.Turn + 1,
            UntilPhase { Phase.Phase: "Untap" or "Upkeep" or "YourNextTurn" } => state.Turn + 1,
            While or UntilThisLeaves => -1,
            _ => null,
        };

    /// <summary>The number of <paramref name="player"/>'s next turn after this one, in turn order.</summary>
    static int NextTurnOf(GameState state, PlayerId player)
    {
        var order = state.TurnOrderFrom(state.ActivePlayer).ToList();
        var at = order.IndexOf(player);
        return state.Turn + (at <= 0 ? order.Count : at);
    }

    /// <summary>An effect with a duration given by the words around it ("until end of turn, …", "… for as long as …"): set on any effect that has an unset one.</summary>
    static Effect WithDuration(Effect effect, Duration duration)
    {
        switch (effect)
        {
            case Sequence sequence:
                return new Sequence(sequence.Steps.Select(x => WithDuration(x, duration)).ToList());
            case Optional optional:
                return optional with { Effect = WithDuration(optional.Effect, duration) };
        }
        var property = effect.GetType().GetProperty("Duration");
        if (property is null || !property.CanWrite || property.GetValue(effect) is not null)
            return effect;
        var copy = (Effect)effect.GetType().GetMethod("<Clone>$")!.Invoke(effect, null);
        property.SetValue(copy, duration);
        return copy;
    }

    /// <summary>Objects for a selector where a player chooses them ("sacrifice a creature"): the filter's matches narrowed to the player's own, chosen down to the count.</summary>
    static ImmutableList<ObjectId> ChooseFromSelector(Selector selector, PlayerId player, EffectContext ctx, string prompt, bool mustControl)
    {
        if (selector is ObjectSelector { Quantifier: Quantifier.Any or Quantifier.Count } objects)
        {
            var count = objects.Quantifier == Quantifier.Any ? 1 : Quantities.Evaluate(objects.Count, ctx) ?? 1;
            var candidates = Resolver.Candidates(objects.Filter, ctx)
                .Where(obj => (!mustControl || obj.Controller == player) && Filters.Matches(ctx.State, objects.Filter, obj, ctx.Controller, ctx.Source, objects.Other ? ctx.Source : null, ctx: ctx))
                .Select(x => TargetRef.Of(x.Id)).ToImmutableList();
            if (candidates.Count == 0)
                return [];
            var take = Math.Min(count, candidates.Count);
            var chosen = candidates.Count <= take ? candidates : ctx.Game.Decide(new ChooseTargets(player, prompt, candidates, take, take)).Targets ?? candidates.Take(take).ToImmutableList();
            return chosen.Where(x => x.IsObject).Select(x => x.Object.Value).Take(take).ToImmutableList();
        }

        return Resolver.Objects(selector, ctx).Where(id => !mustControl || ctx.State.Object(id)?.Controller == player).ToImmutableList();
    }

    public static string Describe(IrNode node) => node?.GetType().Name ?? "nothing";

    /// <summary>A temporary effect with the lasting its effect states: "for as long as …" judged as it goes, "until this leaves the battlefield" ended when the source does.</summary>
    static TemporaryEffect Hold(TemporaryEffect temporary, Effect effect, EffectContext ctx)
    {
        var duration = effect.GetType().GetProperty("Duration")?.GetValue(effect) as Duration;
        return duration switch
        {
            While lasting => temporary with { UntilEndOfTurn = null, While = lasting.Condition, WhileSource = ctx.Source, WhileController = ctx.Controller },
            UntilThisLeaves when ctx.Source is { } source && ctx.State.Object(source) is { } holder => temporary with { UntilEndOfTurn = null, WhileSource = source, WhileSourceZoneChanges = holder.ZoneChangeCount },
            _ => temporary,
        };
    }

    /// <summary>A quantity as a fixed number now, for effects that keep it ("becomes an X/X creature").</summary>
    static Quantity Evaluated(Quantity quantity, EffectContext ctx) =>
        quantity is null or Fixed ? quantity : Quantities.Evaluate(quantity, ctx) is { } value ? new Fixed(value) : quantity;

    /// <summary>"Becomes the color of your choice": the choice made now, as the aspect it names.</summary>
    static Term ChosenNow(Term aspect, EffectContext ctx)
    {
        if (aspect.Vocabulary != nameof(ChosenCharacteristic))
            return aspect;
        var (kind, vocabulary) = aspect.Value switch
        {
            "ColorOfYourChoice" or "ColorOrColorsOfYourChoice" => ("Color", "ManaColor"),
            "BasicLandTypeOfYourChoice" => ("BasicLandType", "LandType"),
            "CreatureTypeOfYourChoice" => ("CreatureType", "CreatureType"),
            _ => (null, null),
        };
        if (kind is null)
            return aspect;
        var options = Choices.Options(kind, ctx.Controller, ctx, null);
        var pick = ctx.Game.Decide(new ChooseOne(ctx.Controller, $"Choose a {kind}", options)).Index;
        return new Term(options[Math.Clamp(pick, 0, options.Count - 1)], vocabulary);
    }

    /// <summary>
    /// "Choose a color / creature type / card name", recorded on the effect's source so "the chosen type" reads it;
    /// "choose one of them" picks among what the previous step found; "choose target opponent" names who was targeted.
    /// </summary>
    static EffectContext Choose(ChooseEffect choose, EffectContext ctx)
    {
        var game = ctx.Game;
        var state = ctx.State;
        var chooser = Resolver.Players(choose.Chooser, ctx).FirstOrDefault(ctx.Controller);

        if (choose.Kinds is [var kind, ..])
        {
            if (kind == "Opponent")
            {
                var opponents = state.OpponentsOf(chooser).ToImmutableList();
                if (opponents.Count == 0)
                    return ctx;
                var picked = opponents[Math.Clamp(game.Decide(new ChooseOne(chooser, "Choose an opponent", opponents.Select(x => state.Player(x).Name).ToImmutableList())).Index, 0, opponents.Count - 1)];
                if (ctx.Source is { } holder)
                    game.Emit(new ChoiceRecorded(holder, "Player", picked.Value.ToString()));
                return ctx with { Last = [TargetRef.Of(picked)] };
            }
            var options = Choices.Options(kind, chooser, ctx, choose.Except);
            if (options is not { Count: > 0 })
                return Skipped(game, choose, $"choice of {kind}", ctx);
            var value = options[Math.Clamp(game.Decide(new ChooseOne(chooser, $"Choose a {kind}", options)).Index, 0, options.Count - 1)];
            game.Emit(new ChoiceMade(chooser, kind, value));
            if (ctx.Source is { } source)
                game.Emit(new ChoiceRecorded(source, Choices.Category(kind), value));
            return ctx;
        }

        if (choose.From is [PlayerSelector players, ..])
            return ctx with { Last = Resolver.Players(players, ctx).Select(TargetRef.Of).ToImmutableList() };

        if (choose.From is [ObjectSelector objects, ..])
        {
            var among = objects.Filter?.With?.OfType<Related>().FirstOrDefault(x => x.Relation == Relation.Among);
            var pool = among is not null
                ? (Resolver.Objects(among.To, ctx) is { Count: > 0 } named ? named : (ctx.Affected ?? ctx.Last ?? []).Where(x => x.IsObject).Select(x => x.Object.Value).ToImmutableList())
                : Resolver.Objects(objects with { Quantifier = Quantifier.All }, ctx);
            var rest = objects.Filter with { With = objects.Filter.With?.Where(x => x != among).ToList() };
            var candidates = pool.Where(id => state.Object(id) is { } obj && Resolver.PoolMatches(rest with { Zone = null }, obj, ctx)).Select(TargetRef.Of).ToImmutableList();
            var count = Math.Min(objects.Quantifier == Quantifier.Count ? Quantities.Evaluate(objects.Count, ctx) ?? 1 : 1, candidates.Count);
            var picked = candidates.Count == 0 ? [] : game.Decide(new ChooseTargets(chooser, "Choose", candidates, objects.UpTo ? 0 : count, count)).Targets?.Where(candidates.Contains).Distinct().Take(count).ToImmutableList() ?? candidates.Take(count).ToImmutableList();
            if (picked.Count < count && !objects.UpTo)
                picked = picked.AddRange(candidates.Except(picked).Take(count - picked.Count));
            return ctx with { Affected = picked, Last = picked, Rest = pool.Select(TargetRef.Of).Where(x => !picked.Contains(x)).ToImmutableList() };
        }

        return Skipped(game, choose, "choice", ctx);
    }
}
