namespace MTGEngine.Core;

/// <summary>
/// Something that happened in the game. Every state change is one of these, applied by <see cref="Reducer"/>; the
/// game's log is the list of them, so a game replays from its events, and triggers, the UI and agents all read the
/// same record of what happened.
/// </summary>
public abstract record GameEvent;

public sealed record GameStarted(int Seed, ImmutableArray<string> PlayerNames) : GameEvent;
public sealed record TurnBegan(int Turn, PlayerId Player) : GameEvent;
public sealed record StepBegan(Step Step, PlayerId Player) : GameEvent;
public sealed record PriorityGiven(PlayerId Player) : GameEvent;
public sealed record Passed(PlayerId Player) : GameEvent;
public sealed record ObjectCreated(GameObject Object, ImmutableList<ObjectId> LibraryOrder = null) : GameEvent;
public sealed record ZoneMoved(ObjectId Object, Zone From, Zone To, PlayerId Controller, bool Tapped = false, int Position = -1, bool Attached = false, ObjectId? AttachTo = null, PlayerId? LeftController = null) : GameEvent;
public sealed record ObjectCeased(ObjectId Object) : GameEvent;
public sealed record LibraryShuffled(PlayerId Player, ImmutableList<ObjectId> Order) : GameEvent;
public sealed record Drew(PlayerId Player, ObjectId? Card) : GameEvent;
public sealed record Tapped(ObjectId Object) : GameEvent;
public sealed record Untapped(ObjectId Object) : GameEvent;
public sealed record DamageDealt(ObjectId? Source, TargetRef Target, int Amount, bool Combat, bool Lifelink = false, bool Deathtouch = false, bool AsCounters = false) : GameEvent;
public sealed record LifeChanged(PlayerId Player, int Delta, string Reason) : GameEvent;
public sealed record CountersChanged(ObjectId Object, string Kind, int Delta) : GameEvent;
public sealed record ManaAdded(PlayerId Player, Color? Color, int Amount) : GameEvent;
public sealed record ManaPaid(PlayerId Player, ManaPool Remaining, ImmutableList<RestrictedMana> RestrictedLeft = null) : GameEvent;

/// <summary>Mana just added to the pool becomes restricted: "add {r}{r}{r}. spend this mana only to cast artifact spells".</summary>
public sealed record ManaRestricted(PlayerId Player, RestrictedMana Mana) : GameEvent;
public sealed record PoolEmptied(PlayerId Player) : GameEvent;
public sealed record LandPlayed(PlayerId Player, ObjectId Land) : GameEvent;
public sealed record SpellCast(StackItem Item) : GameEvent;
public sealed record AbilityActivated(StackItem Item) : GameEvent;
public sealed record Triggered(StackItem Item) : GameEvent;
public sealed record ResolutionBegan(int StackId) : GameEvent;
public sealed record Resolved(int StackId) : GameEvent;
public sealed record Countered(int StackId) : GameEvent;
public sealed record Fizzled(int StackId) : GameEvent;
public sealed record AttackersDeclared(PlayerId Player, ImmutableList<ObjectId> Attackers) : GameEvent;
public sealed record BlockersDeclared(PlayerId Player, ImmutableDictionary<ObjectId, ObjectId> Blocks) : GameEvent;
public sealed record RemovedFromCombat(ObjectId Object) : GameEvent;
public sealed record CombatEnded : GameEvent;
public sealed record TemporaryEffectAdded(TemporaryEffect Effect) : GameEvent;
public sealed record TemporaryEffectsExpired(int Turn) : GameEvent;
public sealed record DamageRemoved : GameEvent;
public sealed record DamageRemovedFrom(ObjectId Object) : GameEvent;
public sealed record SummoningSicknessCleared(PlayerId Player) : GameEvent;
public sealed record ControlChanged(ObjectId Object, PlayerId Controller) : GameEvent;
public sealed record ControlEffectCreated(ControlEffect Effect) : GameEvent;
/// <summary>"You may play an additional land this turn."</summary>
public sealed record ExtraLandsGranted(PlayerId Player, int Count) : GameEvent;
public sealed record ControlEffectEnded(int Id) : GameEvent;
public sealed record Attached(ObjectId Attachment, ObjectId? To) : GameEvent;
public sealed record Discarded(PlayerId Player, ObjectId Card) : GameEvent;
public sealed record Sacrificed(PlayerId Player, ObjectId Object) : GameEvent;
public sealed record Destroyed(ObjectId Object) : GameEvent;
public sealed record Exiled(ObjectId Object) : GameEvent;
public sealed record Milled(PlayerId Player, ObjectId Card) : GameEvent;
public sealed record PoisonChanged(PlayerId Player, int Delta) : GameEvent;
public sealed record PlayerLost(PlayerId Player, string Reason) : GameEvent;
public sealed record GameEnded(PlayerId? Winner, string Reason) : GameEvent;
public sealed record ChoiceMade(PlayerId Player, string Prompt, string Choice) : GameEvent;
public sealed record Note(string Text) : GameEvent;
public sealed record EffectSkipped(string Kind, string Why) : GameEvent;
public sealed record DelayedTriggerCreated(DelayedTrigger Trigger) : GameEvent;
public sealed record DelayedTriggerRemoved(int Id) : GameEvent;
/// <summary>A player's speed became <see cref="Speed"/> ("start your engines!"), on turn <see cref="Turn"/>.</summary>
/// <summary>A permanent put onto the battlefield attacking (ninjutsu): attacking, but it wasn't declared as an attacker.</summary>
public sealed record EnteredAttacking(ObjectId Object) : GameEvent;

public sealed record BlockRequired(BlockRequirement Requirement) : GameEvent;

public sealed record SpeedChanged(PlayerId Player, int Speed, int Turn) : GameEvent;

public sealed record ModesChosen(PlayerId Player, ImmutableList<int> Modes) : GameEvent;
public sealed record CastOptionsChosen(ObjectId Spell, ImmutableHashSet<string> Options, ManaPool Spent = null) : GameEvent;
public sealed record SpellCopied(StackItem Item) : GameEvent;
public sealed record TargetsChanged(int StackId, ImmutableList<TargetRef> Targets, ImmutableList<int> Counts = null) : GameEvent;
public sealed record BecameCopy(ObjectId Object, CardDefinition Copy, int? UntilTurn) : GameEvent;
public sealed record TurnedFace(ObjectId Object, bool FaceDown) : GameEvent;
public sealed record StepSkipScheduled(PlayerId Player, Step Step) : GameEvent;
public sealed record StepSkipped(PlayerId Player, Step Step) : GameEvent;
public sealed record TappedForMana(ObjectId Object, PlayerId Player) : GameEvent;
public sealed record ExiledWith(ExileLink Link) : GameEvent;
public sealed record PlayRestricted(PlayRestriction Restriction) : GameEvent;
public sealed record EnergyChanged(PlayerId Player, int Delta) : GameEvent;
public sealed record LinkedExileCreated(LinkedExile Link) : GameEvent;
public sealed record LinkedExileEnded(ObjectId Source) : GameEvent;
public sealed record ZoneReplacementCreated(ZoneReplacement Replacement) : GameEvent;
public sealed record PermissionGranted(PlayPermission Permission) : GameEvent;
public sealed record ShieldCreated(PreventionShield Shield) : GameEvent;
public sealed record ShieldUsed(int Id, int Amount) : GameEvent;
public sealed record DamagePrevented(ObjectId? Source, TargetRef Target, int Amount) : GameEvent;
public sealed record ChoiceRecorded(ObjectId Object, string Kind, string Value) : GameEvent;
/// <summary>A permanent phased out (or back in).</summary>
public sealed record PhaseChanged(ObjectId Object, bool Out) : GameEvent;
public sealed record Scryed(PlayerId Player, ImmutableList<ObjectId> Top, ImmutableList<ObjectId> Bottom) : GameEvent;

/// <summary>Applies an event to a state. The only place state changes; the rules kernel only decides which events happen.</summary>
public static class Reducer
{
    public static GameState Apply(GameState state, GameEvent evt)
    {
        var next = ApplyCore(state, evt);
        if (evt is TurnBegan)
            return next with { LastTurnEvents = state.ThisTurnEventList, ThisTurnEvents = ImmutableList<GameEvent>.Empty };
        return History.Kept(evt) ? next with { ThisTurnEvents = next.ThisTurnEventList.Add(evt) } : next;
    }

    static GameState ApplyCore(GameState state, GameEvent evt)
    {
        switch (evt)
        {
            case TurnBegan turn:
                return state with
                {
                    Turn = turn.Turn,
                    ActivePlayer = turn.Player,
                    Step = Step.Untap,
                    Priority = turn.Player,
                    ConsecutivePasses = 0,
                    CombatDamageDealt = false,
                    AttackedThisTurn = ImmutableList<ObjectId>.Empty,
                    DiedThisTurn = ImmutableList<ObjectId>.Empty,
                    ActivationsThisTurn = ImmutableDictionary<string, int>.Empty,
                    AbilityResolutionsThisTurn = ImmutableDictionary<ObjectId, int>.Empty,
                    Players = state.Players.Select(p => p with { LandsPlayedThisTurn = 0, ExtraLandsThisTurn = 0, LifeGainedThisTurn = 0, LifeLostThisTurn = 0, SpellsCastThisTurn = 0 }).ToImmutableArray(),
                };

            case StepBegan step:
                return state with { Step = step.Step, Priority = step.Player, ConsecutivePasses = 0 };

            case PriorityGiven given:
                return state with { Priority = given.Player };

            case Passed:
                return state with { ConsecutivePasses = state.ConsecutivePasses + 1 };

            case ObjectCreated created:
            {
                var next = state with { Objects = state.Objects.SetItem(created.Object.Id, created.Object), NextObjectId = Math.Max(state.NextObjectId, created.Object.Id.Value + 1) };
                var obj = created.Object;
                if (obj.Zone is Zone.Library or Zone.Hand or Zone.Graveyard or Zone.Exile)
                {
                    var owner = next.Player(obj.Owner);
                    next = next.WithPlayer(owner.WithZoneList(obj.Zone, owner.ZoneList(obj.Zone).Add(obj.Id)));
                }
                return next;
            }

            case ZoneMoved moved:
            {
                var obj = state.Object(moved.Object);
                if (obj is null)
                    return state;
                var next = state;
                var owner = next.Player(obj.Owner);
                if (moved.From is Zone.Library or Zone.Hand or Zone.Graveyard or Zone.Exile)
                    next = next.WithPlayer(owner.WithZoneList(moved.From, owner.ZoneList(moved.From).Remove(obj.Id)));
                owner = next.Player(obj.Owner);
                if (moved.To is Zone.Library or Zone.Hand or Zone.Graveyard or Zone.Exile)
                {
                    var list = owner.ZoneList(moved.To);
                    list = moved.Position >= 0 && moved.Position <= list.Count ? list.Insert(moved.Position, obj.Id) : list.Add(obj.Id);
                    next = next.WithPlayer(owner.WithZoneList(moved.To, list));
                }
                var entering = moved.To == Zone.Battlefield;
                // An object that changes zones is a new object: effects on the old one don't follow it.
                if (state.Temporary.Any(x => x.Target == obj.Id))
                    next = next with { Temporary = next.Temporary.RemoveAll(x => x.Target == obj.Id) };
                if (moved.From == Zone.Battlefield)
                    next = next with { LastKnown = (next.LastKnown ?? ImmutableDictionary<ObjectId, GameObject>.Empty).SetItem(obj.Id, obj) };
                next = next.WithObject(obj with
                {
                    // A copy is a copy only while it stays where it became one.
                    Card = obj.OriginalCard ?? obj.Card,
                    OriginalCard = null,
                    CopyUntilTurn = null,
                    Zone = moved.To,
                    Controller = moved.Controller,
                    Tapped = entering && moved.Tapped,
                    Damage = 0,
                    EnteredThisTurn = entering,
                    Counters = entering ? obj.Counters : null,
                    AttachedTo = entering ? moved.AttachTo : null,
                    Attacking = false,
                    Blocking = null,
                    // A face-down spell becomes a face-down permanent; anything else that moves is face up.
                    // (A card manifested from the library enters face down too.)
                    FaceDown = entering && moved.From is Zone.Stack or Zone.Library && obj.FaceDown,
                    ZoneChangeCount = obj.ZoneChangeCount + 1,
                    // A permanent spell keeps how it was cast; anything else is a new object without history.
                    CastWith = entering && moved.From == Zone.Stack ? obj.CastWith : null,
                    SpentToCast = entering && moved.From == Zone.Stack ? obj.SpentToCast : null,
                    Choices = null,
                });
                // Attachments to something that left come off.
                foreach (var attached in next.Objects.Values.Where(x => x.AttachedTo == obj.Id))
                    if (!entering || moved.From == Zone.Battlefield)
                        next = next.WithObject(attached with { AttachedTo = null });
                return next;
            }

            case ObjectCeased ceased:
            {
                var obj = state.Object(ceased.Object);
                if (obj is null)
                    return state;
                var next = state;
                var owner = next.Player(obj.Owner);
                if (obj.Zone is Zone.Library or Zone.Hand or Zone.Graveyard or Zone.Exile)
                    next = next.WithPlayer(owner.WithZoneList(obj.Zone, owner.ZoneList(obj.Zone).Remove(obj.Id)));
                return next with { Objects = next.Objects.Remove(obj.Id) };
            }

            case LibraryShuffled shuffled:
                return state.WithPlayer(state.Player(shuffled.Player) with { Library = shuffled.Order });

            case Drew drew:
            {
                var player = state.Player(drew.Player);
                if (drew.Card is null)
                    return state.WithPlayer(player with { DrewFromEmptyLibrary = true });
                var next = state.WithPlayer(player with { Library = player.Library.Remove(drew.Card.Value), Hand = player.Hand.Add(drew.Card.Value) });
                return next.WithObject(next.Object(drew.Card.Value) with { Zone = Zone.Hand });
            }

            case Tapped tapped:
                return state.Object(tapped.Object) is { } toTap ? state.WithObject(toTap with { Tapped = true }) : state;

            case Untapped untapped:
                return state.Object(untapped.Object) is { } toUntap ? state.WithObject(toUntap with { Tapped = false }) : state;

            case DamageDealt damage:
            {
                if (damage.Target.Object is { } id && state.Object(id) is { } obj)
                    return state.WithObject(obj with { Damage = obj.Damage + (damage.AsCounters ? 0 : damage.Amount), Counters = damage.Deathtouch ? obj.CounterMap.SetItem("deathtouch-damage", 1) : obj.Counters });
                return state;
            }

            case EnteredAttacking attacking:
                return state.Object(attacking.Object) is { } entered ? state.WithObject(entered with { Attacking = true }) : state;

            case BlockRequired required:
                return state with { BlockRequirements = state.BlockRequirementList.Add(required.Requirement) };

            case SpeedChanged speed:
                return state.WithPlayer(state.Player(speed.Player) with { Speed = speed.Speed, SpeedRaisedOnTurn = speed.Turn });

            case LifeChanged life:
            {
                var player = state.Player(life.Player);
                return state.WithPlayer(player with
                {
                    Life = player.Life + life.Delta,
                    LifeGainedThisTurn = player.LifeGainedThisTurn + Math.Max(0, life.Delta),
                    LifeLostThisTurn = player.LifeLostThisTurn + Math.Max(0, -life.Delta),
                });
            }

            case CountersChanged counters:
            {
                var obj = state.Object(counters.Object);
                if (obj is null)
                    return state;
                var amount = obj.CountersOf(counters.Kind) + counters.Delta;
                return state.WithObject(obj with { Counters = amount <= 0 ? obj.CounterMap.Remove(counters.Kind) : obj.CounterMap.SetItem(counters.Kind, amount) });
            }

            case ManaAdded mana:
            {
                var player = state.Player(mana.Player);
                return state.WithPlayer(player with { Pool = mana.Color is { } color ? player.Pool.Add(color, mana.Amount) : player.Pool.AddColorless(mana.Amount) });
            }

            case ManaPaid paid:
                return state.WithPlayer(state.Player(paid.Player) with { Pool = paid.Remaining, Restricted = paid.RestrictedLeft ?? state.Player(paid.Player).Restricted });

            case ManaRestricted restricted:
            {
                var player = state.Player(restricted.Player);
                var mana = restricted.Mana;
                var pool = mana.Color is { } color
                    ? player.Pool with { Colored = player.Pool.Colored.SetItem(color, Math.Max(0, player.Pool.Of(color) - mana.Amount)) }
                    : player.Pool with { Colorless = Math.Max(0, player.Pool.Colorless - mana.Amount) };
                return state.WithPlayer(player with { Pool = pool, Restricted = player.RestrictedList.Add(mana) });
            }

            case PoolEmptied emptied:
                return state.WithPlayer(state.Player(emptied.Player) with { Pool = ManaPool.Empty, Restricted = null });

            case LandPlayed land:
            {
                var player = state.Player(land.Player);
                return state.WithPlayer(player with { LandsPlayedThisTurn = player.LandsPlayedThisTurn + 1 });
            }

            case SpellCast cast:
            {
                var player = state.Player(cast.Item.Controller);
                return (state with { Stack = state.Stack.Add(cast.Item), NextStackId = Math.Max(state.NextStackId, cast.Item.Id + 1), ConsecutivePasses = 0 })
                    .WithPlayer(player with { SpellsCastThisTurn = player.SpellsCastThisTurn + 1 });
            }

            case AbilityActivated activated:
            {
                var key = $"{activated.Item.Source}:{activated.Item.AbilityIndex}";
                return state with
                {
                    Stack = state.Stack.Add(activated.Item),
                    NextStackId = Math.Max(state.NextStackId, activated.Item.Id + 1),
                    ConsecutivePasses = 0,
                    ActivationsThisTurn = state.Activations.SetItem(key, state.Activations.GetValueOrDefault(key) + 1),
                };
            }

            case SpellCopied copied:
                return state with { Stack = state.Stack.Add(copied.Item), NextStackId = Math.Max(state.NextStackId, copied.Item.Id + 1), ConsecutivePasses = 0 };

            case TargetsChanged changed:
                return state with { Stack = state.Stack.Select(x => x.Id == changed.StackId ? x with { Targets = changed.Targets, TargetCounts = changed.Counts ?? x.TargetCounts } : x).ToImmutableList() };

            case Triggered triggered:
                return state with { Stack = state.Stack.Add(triggered.Item), NextStackId = Math.Max(state.NextStackId, triggered.Item.Id + 1), ConsecutivePasses = 0 };

            case Resolved resolved:
                return state with { Stack = state.Stack.RemoveAll(x => x.Id == resolved.StackId), ConsecutivePasses = 0 };

            case Countered countered:
                return state with { Stack = state.Stack.RemoveAll(x => x.Id == countered.StackId) };

            case Fizzled fizzled:
                return state with { Stack = state.Stack.RemoveAll(x => x.Id == fizzled.StackId) };

            case AttackersDeclared attackers:
            {
                var next = state with { AttackedThisTurn = (state.AttackedThisTurn ?? ImmutableList<ObjectId>.Empty).AddRange(attackers.Attackers) };
                foreach (var id in attackers.Attackers)
                    if (next.Object(id) is { } obj)
                        next = next.WithObject(obj with { Attacking = true, Tapped = obj.Tapped || !Characteristics.Of(next, obj).HasKeyword("Vigilance") });
                return next;
            }

            case BlockersDeclared blockers:
            {
                var next = state;
                foreach (var (blocker, attacker) in blockers.Blocks)
                    if (next.Object(blocker) is { } obj)
                        next = next.WithObject(obj with { Blocking = attacker });
                return next;
            }

            case RemovedFromCombat removed:
                return state.Object(removed.Object) is { } fighter ? state.WithObject(fighter with { Attacking = false, Blocking = null }) : state;

            case CombatEnded:
            {
                var next = state;
                foreach (var obj in next.Objects.Values.Where(x => x.Attacking || x.Blocking is not null))
                    next = next.WithObject(obj with { Attacking = false, Blocking = null });
                return next;
            }

            case TemporaryEffectAdded temporary:
                return state with { Temporary = state.Temporary.Add(temporary.Effect) };

            case TemporaryEffectsExpired expired when state.Objects.Values.Any(x => x.CopyUntilTurn is { } until && until <= expired.Turn):
            {
                var reverted = state;
                foreach (var copy in state.Objects.Values.Where(x => x.CopyUntilTurn is { } until && until <= expired.Turn))
                    reverted = reverted.WithObject(copy with { Card = copy.OriginalCard ?? copy.Card, OriginalCard = null, CopyUntilTurn = null });
                return ApplyCore(reverted, expired);
            }

            case TemporaryEffectsExpired expired:
                return state with
                {
                    Temporary = state.Temporary.RemoveAll(x => x.UntilEndOfTurn is { } until && until <= expired.Turn),
                    Delayed = state.DelayedTriggers.RemoveAll(x => x.ExpiresAfterTurn is { } after && after <= expired.Turn),
                    Shields = state.ShieldList.RemoveAll(x => x.ExpiresAfterTurn is { } after && after <= expired.Turn),
                    Permissions = state.PermissionList.RemoveAll(x => x.ExpiresAfterTurn is { } after && after <= expired.Turn),
                    ZoneReplacements = state.ZoneReplacementList.RemoveAll(x => x.ExpiresAfterTurn is { } after && after <= expired.Turn),
                    PlayRestrictions = state.PlayRestrictionList.RemoveAll(x => x.ExpiresAfterTurn is { } after && after <= expired.Turn),
                    // Control effects that run out end at the next check, which gives the permanent back.
                    ControlEffects = state.ControlEffects?.Select(x => x.ExpiresAfterTurn is { } after && after <= expired.Turn ? x with { Expired = true } : x).ToImmutableList(),
                };

            case BecameCopy copied:
                return state.Object(copied.Object) is { } copier
                    ? state.WithObject(copier with { OriginalCard = copier.OriginalCard ?? copier.Card, Card = copied.Copy with { IsToken = copier.Card.IsToken }, CopyUntilTurn = copied.UntilTurn })
                    : state;

            case TurnedFace turned:
                return state.Object(turned.Object) is { } turning ? state.WithObject(turning with { FaceDown = turned.FaceDown }) : state;

            case StepSkipScheduled scheduled:
                return state with { SkipNext = state.SkipList.Add((scheduled.Player, scheduled.Step)) };

            case StepSkipped skipped:
                return state with { SkipNext = state.SkipList.Remove((skipped.Player, skipped.Step)) };

            case ExiledWith exiled:
                return state with { ExileLinks = state.ExileLinkList.Add(exiled.Link) };

            case PlayRestricted restricted:
                return state with { PlayRestrictions = state.PlayRestrictionList.Add(restricted.Restriction) };

            case EnergyChanged energy:
            {
                var player = state.Player(energy.Player);
                return state.WithPlayer(player with { Energy = Math.Max(0, player.Energy + energy.Delta) });
            }

            case LinkedExileCreated linked:
                return state with { LinkedExiles = state.LinkedExileList.Add(linked.Link) };

            case LinkedExileEnded ended:
                return state with { LinkedExiles = state.LinkedExileList.RemoveAll(x => x.Source == ended.Source) };

            case ZoneReplacementCreated replacement:
                return state with { ZoneReplacements = state.ZoneReplacementList.Add(replacement.Replacement) };

            case PermissionGranted granted:
                return state with { Permissions = state.PermissionList.Add(granted.Permission) };

            case ShieldCreated created:
                return state with { Shields = state.ShieldList.Add(created.Shield with { Id = state.NextShieldId }), NextShieldId = state.NextShieldId + 1 };

            case ShieldUsed used:
            {
                var shield = state.ShieldList.FirstOrDefault(x => x.Id == used.Id);
                if (shield is null)
                    return state;
                var left = (shield.Remaining ?? 0) - used.Amount;
                return state with { Shields = left <= 0 ? state.ShieldList.Remove(shield) : state.ShieldList.Replace(shield, shield with { Remaining = left }) };
            }

            case DelayedTriggerCreated created:
                return state with { Delayed = state.DelayedTriggers.Add(created.Trigger), NextDelayedId = Math.Max(state.NextDelayedId, created.Trigger.Id + 1) };

            case DelayedTriggerRemoved removed:
                return state with { Delayed = state.DelayedTriggers.RemoveAll(x => x.Id == removed.Id) };

            case DamageRemoved:
            {
                var next = state;
                foreach (var obj in next.Objects.Values.Where(x => x.Damage > 0 || x.CounterMap.ContainsKey("deathtouch-damage")))
                    next = next.WithObject(obj with { Damage = 0, Counters = obj.CounterMap.Remove("deathtouch-damage") });
                return next;
            }

            case DamageRemovedFrom removedFrom:
                return state.Object(removedFrom.Object) is { } healed ? state.WithObject(healed with { Damage = 0, Counters = healed.CounterMap.Remove("deathtouch-damage") }) : state;

            case SummoningSicknessCleared cleared:
            {
                var next = state;
                foreach (var obj in next.Objects.Values.Where(x => x.EnteredThisTurn && x.Controller == cleared.Player))
                    next = next.WithObject(obj with { EnteredThisTurn = false });
                return next;
            }

            case ExtraLandsGranted extra:
            {
                var player = state.Player(extra.Player);
                return state.WithPlayer(player with { ExtraLandsThisTurn = player.ExtraLandsThisTurn + extra.Count });
            }

            case ControlEffectCreated created:
                return state with { ControlEffects = state.ControlEffectList.Add(created.Effect) };

            case ControlEffectEnded ended:
                return state with { ControlEffects = state.ControlEffectList.RemoveAll(x => x.Id == ended.Id) };

            case ControlChanged control:
                return state.Object(control.Object) is { } controlled ? state.WithObject(controlled with { Controller = control.Controller, EnteredThisTurn = true }) : state;

            case Attached attached:
                return state.Object(attached.Attachment) is { } attachment ? state.WithObject(attachment with { AttachedTo = attached.To }) : state;

            case CastOptionsChosen options:
                return state.Object(options.Spell) is { } spell ? state.WithObject(spell with { CastWith = options.Options, SpentToCast = options.Spent, FaceDown = options.Options.Contains("FaceDown") }) : state;

            case PhaseChanged phase:
                return state.Object(phase.Object) is { } phased ? state.WithObject(phased with { PhasedOut = phase.Out, Attacking = phase.Out ? false : phased.Attacking, Blocking = phase.Out ? null : phased.Blocking }) : state;

            // "If this is the second time this ability has resolved this turn": abilities' resolutions this turn, by source.
            case ResolutionBegan began when state.Stack.FirstOrDefault(x => x.Id == began.StackId) is { Kind: not StackKind.Spell, Source: { } resolving }:
            {
                var counts = state.AbilityResolutionsThisTurn ?? ImmutableDictionary<ObjectId, int>.Empty;
                return state with { AbilityResolutionsThisTurn = counts.SetItem(resolving, counts.GetValueOrDefault(resolving) + 1) };
            }

            case ChoiceRecorded choice:
                return state.Object(choice.Object) is { } chooser ? state.WithObject(chooser with { Choices = (chooser.Choices ?? ImmutableDictionary<string, string>.Empty).SetItem(choice.Kind, choice.Value) }) : state;

            case PoisonChanged poison:
            {
                var player = state.Player(poison.Player);
                return state.WithPlayer(player with { Poison = player.Poison + poison.Delta });
            }

            case PlayerLost lost:
                return state.WithPlayer(state.Player(lost.Player) with { Lost = true });

            case GameEnded ended:
                return state with { Over = true, Winner = ended.Winner };

            default:
                return state;
        }
    }
}
