namespace MTGEngine.Core;

/// <summary>
/// An effect that gives <see cref="Player"/> control of a permanent: one-shot ("gain control of target creature"),
/// for a while ("until end of turn", "for as long as you control this creature"), or a static ability's ("you control
/// enchanted creature", <see cref="Static"/>), which lasts while the static covers the permanent. Effects apply in the
/// order they began, the latest winning; when none is left, the permanent goes back to <see cref="From"/> of the first.
/// </summary>
public sealed record ControlEffect(
    int Id,
    ObjectId Object,
    int ZoneChangeCount,
    PlayerId Player,
    PlayerId From,
    int? ExpiresAfterTurn = null,
    Condition While = null,
    ObjectId? Source = null,
    PlayerId? SourceController = null,
    bool Static = false,
    bool Expired = false);

public sealed partial class Game
{
    /// <summary>Gives <paramref name="player"/> control of <paramref name="objects"/> for <paramref name="duration"/>; false when the duration isn't one the engine keeps.</summary>
    public bool GainControl(IEnumerable<ObjectId> objects, PlayerId player, Duration duration, EffectContext ctx)
    {
        int? expires = null;
        Condition holds = null;
        switch (duration)
        {
            case null:
                break;
            case While { Condition: var condition }:
                holds = condition;
                break;
            default:
                if (Interpreter.UntilTurn(duration, State) is not { } until || until < 0)
                    return false;
                expires = until;
                break;
        }
        foreach (var id in objects)
            if (State.Object(id) is { Zone: Zone.Battlefield } obj)
                Emit(new ControlEffectCreated(new ControlEffect(NextControlId(), id, obj.ZoneChangeCount, player, obj.Controller, expires, holds, ctx.Source, ctx.Controller)));
        UpdateControl();
        return true;
    }

    int NextControlId() => (State.ControlEffectList.Count == 0 ? 0 : State.ControlEffectList.Max(x => x.Id)) + 1;

    /// <summary>Whether the engine keeps a control change's duration: none, "until end of turn" and the like, or "for as long as …".</summary>
    public static bool ControlDurationKept(Duration duration) =>
        duration switch
        {
            null or While or ThisTurn => true,
            UntilPhase { Phase.Phase: "EndOfTurn" or "EndStep" or "Cleanup" or "End" or "Untap" or "Upkeep" or "YourNextTurn" } => true,
            UntilPhase { Phase.Phase: "Turn", Phase.Next: true } => true,
            _ => false,
        };

    /// <summary>
    /// Brings every permanent's controller in line with the control effects: starts the statics' that newly cover a
    /// permanent, ends those that lapsed (expired, condition false, static gone, permanent left), and sets each
    /// permanent's controller to the latest remaining effect's player, or back to where it began.
    /// </summary>
    void UpdateControl()
    {
        // Statics that cover a permanent now, by (source, permanent), with the player they give it to.
        var covering = new Dictionary<(ObjectId Source, ObjectId Object), PlayerId>();
        foreach (var (effect, objects, source) in Characteristics.StaticEffects(State))
            if (effect is GainControl { Duration: null } gain)
            {
                var to = Resolver.Players(State, gain.Player ?? new PlayerSelector(PlayerKind.You), source.Controller, source.Id).FirstOrDefault(source.Controller);
                foreach (var obj in State.Battlefield.Where(x => Characteristics.Covers(State, objects, source, x)))
                    covering[(source.Id, obj.Id)] = to;
            }
        foreach (var ((source, id), to) in covering)
            if (!State.ControlEffectList.Any(x => x.Static && x.Source == source && x.Object == id && x.ZoneChangeCount == State.Object(id).ZoneChangeCount))
                Emit(new ControlEffectCreated(new ControlEffect(NextControlId(), id, State.Object(id).ZoneChangeCount, to, State.Object(id).Controller, Source: source, Static: true)));

        foreach (var group in State.ControlEffectList.GroupBy(x => x.Object).ToList())
        {
            var obj = State.Object(group.Key);
            var effects = group.OrderBy(x => x.Id).ToList();
            if (obj is not { Zone: Zone.Battlefield })
            {
                foreach (var gone in effects)
                    Emit(new ControlEffectEnded(gone.Id));
                continue;
            }
            var stale = effects.Where(x => x.ZoneChangeCount != obj.ZoneChangeCount).ToList();
            var current = effects.Except(stale).ToList();
            var ending = current.Where(x => !Lasts(x, covering)).ToList();
            var remaining = current.Except(ending).ToList();
            foreach (var ended in stale.Concat(ending))
                Emit(new ControlEffectEnded(ended.Id));
            PlayerId? desired = remaining.Count > 0 ? remaining[^1].Player : ending.Count > 0 ? current[0].From : null;
            if (desired is { } controller && controller != obj.Controller)
            {
                Emit(new ControlChanged(obj.Id, controller));
                if (obj.Attacking || obj.Blocking is not null)
                    Emit(new RemovedFromCombat(obj.Id));
            }
        }
    }

    bool Lasts(ControlEffect effect, Dictionary<(ObjectId Source, ObjectId Object), PlayerId> covering)
    {
        if (effect.Expired)
            return false;
        if (effect.Static)
            return effect.Source is { } source && covering.ContainsKey((source, effect.Object));
        if (effect.While is { } condition)
        {
            var ctx = new EffectContext(this, effect.SourceController ?? effect.Player, effect.Source, new Dictionary<IrNode, ImmutableList<TargetRef>>(ReferenceEqualityComparer.Instance));
            return Conditions.Evaluate(condition, ctx) == true;
        }
        return true;
    }
}
