namespace MTGEngine.Core;

/// <summary>Combat: declaring attackers and blockers, and dealing combat damage (first strike, trample, deathtouch, lifelink).</summary>
public sealed partial class Game
{
    void DeclareAttackers()
    {
        var active = State.ActivePlayer;
        var opponents = State.OpponentsOf(active).ToList();
        if (opponents.Count == 0)
            return;
        _defender = opponents[0];

        var candidates = State.Battlefield
            .Where(x => x.Controller == active && !x.Tapped)
            .Where(x =>
            {
                var traits = Characteristics.Of(State, x);
                return traits.IsCreature && !traits.HasKeyword("Defender") && !traits.Cant("Attack") && (!x.EnteredThisTurn || traits.HasKeyword("Haste"));
            })
            .Select(x => x.Id)
            .ToImmutableList();

        if (candidates.Count == 0)
            return;

        var response = Decide(new ChooseAttackers(active, candidates, opponents.Select(TargetRef.Of).ToImmutableList()));
        var attackers = (response.Objects ?? ImmutableList<ObjectId>.Empty).Where(candidates.Contains).Distinct().ToImmutableList();
        if (attackers.Count == 0)
            return;

        _attackersThisCombat = true;
        Emit(new AttackersDeclared(active, attackers));
    }

    void DeclareBlockers()
    {
        var defender = _defender;
        var attackers = State.Battlefield.Where(x => x.Attacking).ToList();
        var blockers = State.Battlefield.Where(x => x.Controller == defender && !x.Tapped && Characteristics.Of(State, x).IsCreature).ToList();

        List<(ObjectId Blocker, ObjectId Attacker)> legal = [];
        foreach (var blocker in blockers)
        {
            var blockerTraits = Characteristics.Of(State, blocker);
            foreach (var attacker in attackers)
            {
                var attackerTraits = Characteristics.Of(State, attacker);
                if (CanBlock(blockerTraits, attackerTraits, blocker, attacker))
                    legal.Add((blocker.Id, attacker.Id));
            }
        }

        if (legal.Count == 0)
            return;

        var response = Decide(new ChooseBlockers(defender, legal.ToImmutableList()));
        var chosen = (response.Blocks ?? ImmutableList<(ObjectId, ObjectId)>.Empty)
            .Where(legal.Contains)
            .GroupBy(x => x.Blocker)
            .Select(x => x.First())
            .ToList();

        if (chosen.Count == 0)
            return;

        // Menace: an attacker blocked by exactly one creature isn't blocked.
        foreach (var attacker in attackers)
            if (Characteristics.Of(State, attacker).HasKeyword("Menace") && chosen.Count(x => x.Attacker == attacker.Id) == 1)
                chosen.RemoveAll(x => x.Attacker == attacker.Id);

        if (chosen.Count == 0)
            return;

        Emit(new BlockersDeclared(defender, chosen.ToImmutableDictionary(x => x.Blocker, x => x.Attacker)));
    }

    static bool CanBlock(Characteristics blocker, Characteristics attacker, GameObject blockerObj, GameObject attackerObj)
    {
        if (blocker.Cant("Block") || attacker.Cant("BeBlocked"))
            return false;
        if (attacker.HasKeyword("Flying") && !blocker.HasKeyword("Flying") && !blocker.HasKeyword("Reach"))
            return false;
        if (attacker.HasKeyword("Shadow") != blocker.HasKeyword("Shadow"))
            return false;
        if (attacker.HasKeyword("Horsemanship") && !blocker.HasKeyword("Horsemanship"))
            return false;
        if (attacker.HasKeyword("Fear") && !(blocker.Colors.Contains(Color.Black) || blocker.Types.Contains("Artifact")))
            return false;
        if (attacker.HasKeyword("Intimidate") && !(blocker.Types.Contains("Artifact") || blocker.Colors.Overlaps(attacker.Colors)))
            return false;
        if (attacker.HasKeyword("Unblockable") || attacker.HasKeyword("CantBeBlocked"))
            return false;
        return true;
    }

    void CombatDamage()
    {
        var attackers = State.Battlefield.Where(x => x.Attacking).ToList();
        if (attackers.Count == 0)
            return;

        bool HasFirstStrike(GameObject obj) => Characteristics.Of(State, obj).HasKeyword("FirstStrike") || Characteristics.Of(State, obj).HasKeyword("DoubleStrike");
        var anyFirstStrike = State.Battlefield.Any(x => (x.Attacking || x.Blocking is not null) && HasFirstStrike(x));

        if (anyFirstStrike)
        {
            DealCombatDamage(first: true);
            CheckStateAndTriggers();
            if (State.Over)
                return;
        }

        DealCombatDamage(first: false);
    }

    void DealCombatDamage(bool first)
    {
        var state = State;
        List<(ObjectId Source, TargetRef Target, int Amount)> assignments = [];

        bool Deals(GameObject obj)
        {
            var traits = Characteristics.Of(state, obj);
            var firstStrike = traits.HasKeyword("FirstStrike");
            var doubleStrike = traits.HasKeyword("DoubleStrike");
            return first ? firstStrike || doubleStrike : doubleStrike || !firstStrike;
        }

        foreach (var attacker in state.Battlefield.Where(x => x.Attacking).ToList())
        {
            if (!Deals(attacker))
                continue;
            var traits = Characteristics.Of(state, attacker);
            var power = traits.Power;
            if (power <= 0)
                continue;
            var blockers = state.Battlefield.Where(x => x.Blocking == attacker.Id).OrderBy(x => x.Id.Value).ToList();

            if (blockers.Count == 0)
            {
                assignments.Add((attacker.Id, TargetRef.Of(_defender), power));
                continue;
            }

            var remaining = power;
            foreach (var blocker in blockers)
            {
                if (remaining <= 0)
                    break;
                var blockerTraits = Characteristics.Of(state, blocker);
                var lethal = Math.Max(0, blockerTraits.Toughness - blocker.Damage);
                if (traits.HasKeyword("Deathtouch"))
                    lethal = Math.Min(lethal, 1);
                var amount = blocker == blockers[^1] && !traits.HasKeyword("Trample") ? remaining : Math.Min(remaining, Math.Max(lethal, 1));
                assignments.Add((attacker.Id, TargetRef.Of(blocker.Id), amount));
                remaining -= amount;
            }
            if (remaining > 0 && traits.HasKeyword("Trample"))
                assignments.Add((attacker.Id, TargetRef.Of(_defender), remaining));
        }

        foreach (var blocker in state.Battlefield.Where(x => x.Blocking is not null).ToList())
        {
            if (!Deals(blocker))
                continue;
            var traits = Characteristics.Of(state, blocker);
            if (traits.Power <= 0 || state.Object(blocker.Blocking.Value)?.Attacking != true)
                continue;
            assignments.Add((blocker.Id, TargetRef.Of(blocker.Blocking.Value), traits.Power));
        }

        foreach (var (source, target, amount) in assignments)
            DealDamage(source, target, amount, combat: true);
    }
}
