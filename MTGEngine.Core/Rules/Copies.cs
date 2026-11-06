namespace MTGEngine.Core;

public sealed partial class Game
{
    /// <summary>
    /// Copies of a spell on the stack: each a token object with the spell's card and its own stack item (same
    /// effects, targets, X and bindings) under <paramref name="controller"/>, put on the stack without being cast;
    /// with new targets chosen when <paramref name="newTargets"/>. Returns the copies.
    /// </summary>
    public ImmutableList<ObjectId> CopySpell(ObjectId spell, PlayerId controller, int times, bool newTargets)
    {
        var item = State.Stack.LastOrDefault(x => x.Spell == spell);
        var original = State.Object(spell);
        if (item is null || original is null)
            return [];

        List<ObjectId> copies = [];
        for (var i = 0; i < times; i++)
        {
            var id = new ObjectId(State.NextObjectId);
            Emit(new ObjectCreated(new GameObject(id, original.Card with { IsToken = true }, controller, controller, Zone.Stack, CastWith: original.CastWith)));
            var copy = item with { Id = State.NextStackId, Controller = controller, Source = id, Spell = id, Description = $"copy of {item.Description}" };
            if (newTargets && RetargetFor(copy, controller) is { } targets)
                copy = copy with { Targets = targets, TargetCounts = _lastTargetCounts };
            Emit(new SpellCopied(copy));
            copies.Add(id);
        }
        return copies.ToImmutableList();
    }

    /// <summary>New targets for a stack item's target slots, chosen by <paramref name="chooser"/>; null when a required slot has none.</summary>
    ImmutableList<TargetRef> RetargetFor(StackItem item, PlayerId chooser)
    {
        var slots = TargetSlots.Of(item.Effects).ToList();
        return slots.Count == 0 ? null : ChooseTargetsFor(slots, chooser, item.Source);
    }

    /// <summary>"Change the target of target spell", "you may choose new targets for the copy": the item's targets chosen again (or set to <paramref name="to"/> where that's legal).</summary>
    public void ChangeTargets(ObjectId spell, PlayerId chooser, ObjectId? source, Selector to)
    {
        var item = State.Stack.LastOrDefault(x => x.Spell == spell || x.Source == spell && x.Spell is null);
        if (item is null || item.Targets.Count == 0)
            return;
        ImmutableList<TargetRef> targets;
        ImmutableList<int> counts = null;
        if (to is Self && source is { } self)
        {
            var slots = TargetSlots.Of(item.Effects).ToList();
            var legal = slots.Count > 0 && TargetSlots.Candidates(slots[0], this, item.Controller, item.Source).Contains(TargetRef.Of(self));
            if (!legal)
                return;
            targets = item.Targets.SetItem(0, TargetRef.Of(self));
        }
        else if (RetargetFor(item, chooser) is { } chosen)
        {
            targets = chosen;
            counts = _lastTargetCounts;
        }
        else
            return;
        Emit(new TargetsChanged(item.Id, targets, counts));
    }
}
