namespace MTGEngine.Core;

/// <summary>
/// Phasing: a permanent that phases out is treated as though it doesn't exist (the state's battlefield leaves it out)
/// until it phases in, without leaving the battlefield, so nothing about it changes and nothing triggers. What's
/// attached to it phases out and in with it. It phases in during its controller's next untap step, or, phased out
/// "until this leaves the battlefield", when that source leaves. A permanent with the phasing keyword phases out (or
/// in) as each of its controller's untap steps begins.
/// </summary>
public sealed partial class Game
{
    /// <summary>Phases <paramref name="id"/> and what's attached to it out; with <paramref name="until"/>, until that source leaves the battlefield.</summary>
    public void PhaseOut(ObjectId id, ObjectId? until = null)
    {
        if (State.Object(id) is not { Zone: Zone.Battlefield, PhasedOut: false } obj)
            return;
        if (obj.Attacking || obj.Blocking is not null)
            Emit(new RemovedFromCombat(id));
        var attached = State.Battlefield.Where(x => x.AttachedTo == id).Select(x => x.Id).ToList();
        Emit(new PhaseChanged(id, Out: true));
        if (until is { } source && State.Object(source) is { } holder)
            Emit(new ChoiceRecorded(id, "PhasedOutUntil", $"{source.Value}:{holder.ZoneChangeCount}"));
        foreach (var attachment in attached)
            Emit(new PhaseChanged(attachment, Out: true));
    }

    /// <summary>Phases <paramref name="id"/> and what's attached to it in.</summary>
    public void PhaseIn(ObjectId id)
    {
        if (State.Object(id) is not { Zone: Zone.Battlefield, PhasedOut: true })
            return;
        Emit(new PhaseChanged(id, Out: false));
        if (State.Object(id)?.Chosen("PhasedOutUntil") is not null)
            Emit(new ChoiceRecorded(id, "PhasedOutUntil", ""));
        foreach (var attachment in State.Objects.Values.Where(x => x is { Zone: Zone.Battlefield, PhasedOut: true } && x.AttachedTo == id).Select(x => x.Id).ToList())
            Emit(new PhaseChanged(attachment, Out: false));
    }

    /// <summary>As the untap step begins: the active player's phased-out permanents phase in, and their permanents with phasing phase out.</summary>
    void PhaseAtUntap(PlayerId active)
    {
        var phasingIn = State.Objects.Values.Where(x => x is { Zone: Zone.Battlefield, PhasedOut: true } && x.Controller == active && string.IsNullOrEmpty(x.Chosen("PhasedOutUntil"))
            && !(x.AttachedTo is { } host && State.Object(host) is { PhasedOut: true })).Select(x => x.Id).ToList();
        var phasingOut = State.Battlefield.Where(x => x.Controller == active && x.AttachedTo is null && Characteristics.Of(State, x).HasKeyword("Phasing")).Select(x => x.Id).ToList();
        foreach (var id in phasingIn)
            PhaseIn(id);
        foreach (var id in phasingOut)
            PhaseOut(id);
    }

    /// <summary>"Phases out until this leaves the battlefield": phased in once the source is gone. True when one did.</summary>
    bool PhaseInWhenSourcesLeave()
    {
        var changed = false;
        foreach (var obj in State.Objects.Values.Where(x => x is { Zone: Zone.Battlefield, PhasedOut: true } && !string.IsNullOrEmpty(x.Chosen("PhasedOutUntil"))).ToList())
        {
            var parts = obj.Chosen("PhasedOutUntil").Split(':');
            if (parts.Length == 2 && int.TryParse(parts[0], out var source) && int.TryParse(parts[1], out var changes)
                && State.Object(new ObjectId(source)) is { Zone: Zone.Battlefield } holder && holder.ZoneChangeCount == changes)
                continue;
            PhaseIn(obj.Id);
            changed = true;
        }
        return changed;
    }
}
