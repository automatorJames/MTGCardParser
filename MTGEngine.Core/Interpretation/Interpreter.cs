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
        var game = ctx.Game;
        var state = ctx.State;

        switch (effect)
        {
            case null:
                return ctx;

            case Sequence sequence:
                foreach (var step in sequence.Steps)
                    ctx = Execute(step, ctx);
                return ctx;

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
                return holds.Value ? Execute(conditional.Then, ctx) : Execute(conditional.Else, ctx);
            }

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
                var source = sources.Count > 0 ? sources[0] : ctx.Source;
                var total = 0;
                List<TargetRef> hit = [];
                foreach (var portion in damage.Portions)
                {
                    var amount = Quantities.Evaluate(portion.Amount, ctx);
                    if (amount is null)
                    {
                        game.Skip(portion.Amount, "amount not evaluable");
                        continue;
                    }
                    var targets = portion.To is null ? (ctx.TriggerSubject is { } subject ? [subject] : ctx.Last ?? []) : Resolver.Targets(portion.To, ctx);
                    foreach (var target in targets)
                    {
                        game.DealDamage(source, target, amount.Value, combat: false);
                        total += amount.Value;
                        hit.Add(target);
                    }
                }
                return ctx with { LastAmount = total, Last = hit.Count > 0 ? hit.ToImmutableList() : ctx.Last };
            }

            case Draw draw:
            {
                var count = Quantities.Evaluate(draw.Count, ctx) ?? 1;
                foreach (var player in Resolver.Players(draw.Player, ctx))
                    game.Draw(player, count);
                return ctx with { LastAmount = count };
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
                foreach (var player in Resolver.Players(lose.Player, ctx))
                    game.ChangeLife(player, -amount.Value, "lose");
                return ctx with { LastAmount = amount.Value };
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
                var objects = Resolver.Objects(destroy.Objects, ctx);
                List<TargetRef> affected = [];
                foreach (var id in objects)
                    if (game.Destroy(id, destroy.Manner == "CantBeRegenerated"))
                        affected.Add(TargetRef.Of(id));
                return ctx with { Affected = affected.ToImmutableList(), Last = objects.Select(TargetRef.Of).ToImmutableList() };
            }

            case Exile exile:
            {
                var objects = Resolver.Objects(exile.Objects, ctx);
                foreach (var id in objects)
                    game.MoveToZone(id, Zone.Exile);
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
                var objects = Resolver.Objects(tap.Objects, ctx);
                foreach (var id in objects)
                    game.Tap(id);
                return ctx with { Last = objects.Select(TargetRef.Of).ToImmutableList() };
            }

            case Untap untap:
            {
                var objects = Resolver.Objects(untap.Objects, ctx);
                foreach (var id in objects)
                    game.Untap(id);
                return ctx with { Last = objects.Select(TargetRef.Of).ToImmutableList() };
            }

            case PutCounters put:
            {
                var count = Quantities.Evaluate(put.Counters.Count, ctx) ?? 1;
                var kind = put.Counters.Kind;
                if (kind is null)
                    return Skipped(game, put, "counter kind unknown", ctx);
                var objects = put.Objects is null ? (ctx.Source is { } s ? [s] : []) : Resolver.Objects(put.Objects, ctx);
                foreach (var id in objects)
                    game.Emit(new CountersChanged(id, kind, count));
                return ctx with { Last = objects.Select(TargetRef.Of).ToImmutableList() };
            }

            case RemoveCounters remove:
            {
                var kind = remove.Counters.Kind;
                if (kind is null)
                    return Skipped(game, remove, "counter kind unknown", ctx);
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
                if (create.Token is not TokenSpec spec)
                    return Skipped(game, create, "token not described", ctx);
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
                var keywords = (pump.Keywords ?? []).Select(x => x.Keyword).ToImmutableList();
                if (pump.ChoiceOf is { Count: > 0 } options)
                {
                    var choice = game.Decide(new ChooseOne(ctx.Controller, "Choose a keyword", options.Select(x => x.Keyword).ToImmutableList()));
                    keywords = keywords.Add(options[Math.Clamp(choice.Index, 0, options.Count - 1)].Keyword);
                }
                var until = UntilTurn(pump.Duration, state);
                if (until is null && pump.Duration is not null)
                    return Skipped(game, pump, $"duration {pump.Duration.GetType().Name}", ctx);
                foreach (var id in objects)
                    game.Emit(new TemporaryEffectAdded(new TemporaryEffect(id, power, toughness, pump.Remove ? [] : keywords, until == -1 ? null : until, pump.BaseStats)));
                return ctx with { Last = objects.Select(TargetRef.Of).ToImmutableList() };
            }

            case LoseAllAbilities lose:
            {
                var objects = Resolver.Objects(lose.Objects, ctx);
                foreach (var id in objects)
                    game.Emit(new TemporaryEffectAdded(new TemporaryEffect(id, 0, 0, [], null, LosesAbilities: true)));
                return ctx;
            }

            case Animate animate:
            {
                var until = UntilTurn(animate.Duration, state);
                if (until is null && animate.Duration is not null)
                    return Skipped(game, animate, $"duration {animate.Duration.GetType().Name}", ctx);
                if (animate.Types is { Count: > 0 } || animate.Transforms)
                    return Skipped(game, animate, "type change", ctx);
                var objects = Resolver.Objects(animate.Objects, ctx);
                var stats = animate.BaseStats ?? (animate.Power is not null || animate.Toughness is not null ? new StatsMod(animate.Power, animate.Toughness) : null);
                foreach (var id in objects)
                    game.Emit(new TemporaryEffectAdded(new TemporaryEffect(id, 0, 0, (animate.Keywords ?? []).Select(x => x.Keyword).ToImmutableList(), until == -1 ? null : until, stats)));
                return ctx;
            }

            case ChangeZone change:
            {
                if (!Enum.TryParse<Zone>(change.To?.Zone, out var zone))
                    return Skipped(game, change, $"zone {change.To?.Zone}", ctx);
                var objects = Resolver.Objects(change.Objects, ctx);
                var controller = change.UnderControlOf is null ? (PlayerId?)null : Resolver.Players(change.UnderControlOf, ctx).FirstOrDefault(ctx.Controller);
                var position = change.To.Position == nameof(Position.Top) ? 0 : change.To.Position == nameof(Position.Bottom) ? int.MaxValue : -1;
                foreach (var id in objects)
                {
                    var obj = state.Object(id);
                    if (obj is null)
                        continue;
                    if (zone == Zone.Battlefield && !obj.Card.IsPermanentCard)
                        continue;
                    game.MoveToZone(id, zone, controller ?? (zone == Zone.Battlefield ? ctx.Controller : null), change.Tapped, position);
                }
                var moved = objects.Select(TargetRef.Of).ToImmutableList();
                return ctx with { Affected = moved, Last = moved };
            }

            case ShuffleInto shuffle:
            {
                var objects = Resolver.Objects(shuffle.Cards, ctx);
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
                    var hand = state.InZone(Zone.Hand, player).Where(x => discard.Filter is null || Filters.Matches(game.State, discard.Filter with { Zone = null }, x, ctx.Controller, ctx.Source, ctx: ctx)).Select(x => x.Id).ToList();
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
                return ctx;
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
                        default:
                            game.Skip(option, "mana option");
                            break;
                    }
                return ctx;
            }

            case GainControl control:
            {
                var players = Resolver.Players(control.Player, ctx);
                var player = players.Count > 0 ? players[0] : ctx.Controller;
                foreach (var id in Resolver.Objects(control.Objects, ctx))
                    if (state.Object(id)?.Zone == Zone.Battlefield)
                        game.Emit(new ControlChanged(id, player));
                if (control.Duration is not null)
                    game.Skip(control.Duration, "control change duration (treated as permanent)");
                return ctx;
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

            case LookAt or Reveal:
                return ctx;

            case LetVariable let:
            {
                var value = Quantities.Evaluate(let.Definition, ctx);
                if (value is null)
                    return Skipped(game, let, "variable definition", ctx);
                return Execute(let.Effect, ctx with { X = value.Value }) with { X = ctx.X };
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

            case Regenerate:
            case KeywordAction:
            default:
                return Skipped(game, effect, "effect not interpreted", ctx);
        }
    }

    static EffectContext Skipped(Game game, IrNode node, string why, EffectContext ctx)
    {
        game.Skip(node, why);
        return ctx;
    }

    /// <summary>The turn after which a duration ends: this turn for "until end of turn", -1 for no end, null for one the engine doesn't keep.</summary>
    public static int? UntilTurn(Duration duration, GameState state) =>
        duration switch
        {
            null => -1,
            ThisTurn => state.Turn,
            UntilPhase { Phase.Phase: "EndOfTurn" or "EndStep" or "Cleanup" or "End" } => state.Turn,
            UntilPhase { Phase.Phase: "Turn", Phase.Next: true } => state.Turn + 1,
            UntilPhase { Phase.Phase: "Untap" or "Upkeep" or "YourNextTurn" } => state.Turn + 1,
            _ => null,
        };

    static Effect WithDuration(Effect effect, Duration duration) =>
        effect switch
        {
            Pump { Duration: null } pump => pump with { Duration = duration },
            Animate { Duration: null } animate => animate with { Duration = duration },
            Sequence sequence => new Sequence(sequence.Steps.Select(x => WithDuration(x, duration)).ToList()),
            _ => effect,
        };

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
}
