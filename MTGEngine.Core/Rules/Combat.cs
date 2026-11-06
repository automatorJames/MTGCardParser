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
                return traits.IsCreature && (!traits.HasKeyword("Defender") || traits.Cant("AttackDespiteDefender")) && !traits.Cant("Attack") && !AttackForbidden(traits)
                    && (!x.EnteredThisTurn || traits.HasKeyword("Haste") || traits.Cant("AttackAsThoughHaste"));
            })
            .Select(x => x.Id)
            .ToImmutableList();

        if (candidates.Count == 0)
            return;

        var response = Decide(new ChooseAttackers(active, candidates, opponents.Select(TargetRef.Of).ToImmutableList()));
        var attackers = (response.Objects ?? ImmutableList<ObjectId>.Empty).Where(candidates.Contains).Distinct().ToImmutableList();
        // Creatures that attack each combat if able do.
        attackers = attackers.AddRange(candidates.Where(id => !attackers.Contains(id) && Characteristics.Of(State, State.Object(id)).Cant("MustAttack")));
        // "Can't attack alone."
        if (attackers.Count == 1 && Characteristics.Of(State, State.Object(attackers[0])).Rules.Any(r => r.Cant == "Attack" && r.Modifiers?.Contains("Alone") == true && RuleHolds(r)))
            attackers = [];
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
                if (CanBlock(blockerTraits, attackerTraits, blocker, attacker) && RulesAllowBlock(blockerTraits, blocker, attackerTraits, attacker))
                    legal.Add((blocker.Id, attacker.Id));
            }
        }

        if (attackers.Count == 0)
            return;
        if (legal.Count == 0)
        {
            Emit(new BlockersDeclared(defender, ImmutableDictionary<ObjectId, ObjectId>.Empty));
            return;
        }

        var response = Decide(new ChooseBlockers(defender, legal.ToImmutableList()));
        var chosen = (response.Blocks ?? ImmutableList<(ObjectId, ObjectId)>.Empty)
            .Where(legal.Contains)
            .GroupBy(x => x.Blocker)
            .Select(x => x.First())
            .ToList();

        // Required blocks: "target creature blocks this creature this turn if able", "all walls able to block this creature
        // do so", and "must be blocked if able", met where a legal block allows and the blocker isn't blocking already.
        var required = State.BlockRequirementList.Select(x => (x.Blocker, x.Attacker)).ToList();
        foreach (var (effect, objects, source) in Characteristics.StaticEffects(State))
            if (effect is MustBlock { Duration: null } must)
            {
                var ctx = EffectContext.Snapshot(State, source.Controller, source.Id);
                // With no attacker named ("creatures your opponents control block each combat if able"), any it can.
                var targets = must.Attacker is null ? attackers.Select(x => x.Id).ToImmutableList() : must.Attacker is Self ? [source.Id] : Resolver.Objects(must.Attacker, ctx);
                foreach (var blocker in blockers.Where(x => Characteristics.Covers(State, objects, source, x)))
                    foreach (var attacker in targets)
                        required.Add((blocker.Id, attacker));
            }
        foreach (var (mustBlock, attacker) in required)
        {
            if (mustBlock is { } blocker)
            {
                if (chosen.All(x => x.Blocker != blocker) && legal.Contains((blocker, attacker)))
                    chosen.Add((blocker, attacker));
            }
            else if (chosen.All(x => x.Attacker != attacker) && legal.FirstOrDefault(x => x.Attacker == attacker && chosen.All(y => y.Blocker != x.Blocker)) is { Blocker.Value: > 0 } any)
                chosen.Add(any);
        }

        // Creatures that block each combat if able do.
        foreach (var blocker in blockers)
            if (Characteristics.Of(State, blocker).Cant("MustBlock") && chosen.All(x => x.Blocker != blocker.Id) && legal.FirstOrDefault(x => x.Blocker == blocker.Id) is { Blocker.Value: > 0 } forced)
                chosen.Add(forced);

        // "Can't be blocked except by three or more creatures", "can't be blocked by more than one creature".
        foreach (var attacker in attackers)
            foreach (var rule in Characteristics.Of(State, attacker).Rules.Where(r => r.Cant == "BeBlocked" && !r.LimitsWhich && RuleHolds(r)))
            {
                var (least, most) = rule.BlockerCount;
                var count = chosen.Count(x => x.Attacker == attacker.Id);
                if (count > 0 && count < least)
                    chosen.RemoveAll(x => x.Attacker == attacker.Id);
                else if (count > most)
                    chosen.RemoveAll(x => x.Attacker == attacker.Id && chosen.Where(y => y.Attacker == attacker.Id).Skip(most).Contains(x));
            }

        // "Can't block alone."
        if (chosen.Count == 1 && Characteristics.Of(State, State.Object(chosen[0].Blocker)).Rules.Any(r => r.Cant == "Block" && r.Modifiers?.Contains("Alone") == true && RuleHolds(r)))
            chosen.Clear();

        // Menace: an attacker blocked by exactly one creature isn't blocked.
        foreach (var attacker in attackers)
            if (Characteristics.Of(State, attacker).HasKeyword("Menace") && chosen.Count(x => x.Attacker == attacker.Id) == 1)
                chosen.RemoveAll(x => x.Attacker == attacker.Id);

        // Declared even when nothing blocks: "whenever this creature attacks and isn't blocked" triggers then.
        Emit(new BlockersDeclared(defender, chosen.ToImmutableDictionary(x => x.Blocker, x => x.Attacker)));
    }

    /// <summary>Whether no one attacking may be attacked by this creature: "can't attack unless …", "creatures can't attack you".</summary>
    bool AttackForbidden(Characteristics traits) =>
        traits.Rules.Any(r => r.Cant == "Attack" && r.Modifiers is not { Count: > 0 } && r.By is null
            && (r.Recipient is null || r.Recipient is "You" or "YouOrYourPlaneswalkers" && r.Controller == _defender)
            && RuleHolds(r));

    /// <summary>The combat rules of the blocker and the attacker: "can't block creatures with power 2 or greater", "can't be blocked by walls", conditional "can't block".</summary>
    bool RulesAllowBlock(Characteristics blockerTraits, GameObject blocker, Characteristics attackerTraits, GameObject attacker)
    {
        // Protection: creatures with the quality can't block it.
        if (Keywords.ProtectedFrom(State, attacker, attackerTraits, blocker))
            return false;
        // Landwalk: unblockable while the defending player controls a land of the type.
        foreach (var keyword in attackerTraits.Keywords)
            if (Keywords.Landwalk(keyword) is { } landType && !attackerTraits.Cant("Landwalk")
                && State.Battlefield.Any(x => x.Controller == blocker.Controller && Characteristics.Of(State, x).Subtypes.Contains(landType)))
                return false;
        // "Can block only creatures with flying."
        foreach (var rule in blockerTraits.Rules.Where(r => r.Cant == "BlockOnly"))
            if (!RuleNames(rule, attacker))
                return false;
        foreach (var rule in blockerTraits.Rules.Where(r => r.Cant == "Block" && r.Modifiers is not { Count: > 0 }))
            if ((rule.By is null || RuleNames(rule, attacker)) && RuleHolds(rule))
                return false;
        foreach (var rule in attackerTraits.Rules.Where(r => r.Cant == "BeBlocked" && r.Modifiers is not { Count: > 0 }))
            if ((rule.By is null || rule.LimitsWhich && RuleNames(rule, blocker)) && RuleHolds(rule))
                return false;
        // "Can't be blocked except by walls."
        foreach (var rule in attackerTraits.Rules.Where(r => r.Cant == "BeBlocked" && r.Modifiers?.Contains("Except") == true && r.LimitsWhich))
            if (!RuleNames(rule, blocker) && RuleHolds(rule))
                return false;
        return true;
    }

    static bool CanBlock(Characteristics blocker, Characteristics attacker, GameObject blockerObj, GameObject attackerObj)
    {
        if (blocker.Cant("Block") || attacker.Cant("BeBlocked"))
            return false;
        if (attacker.HasKeyword("Flying") && !blocker.HasKeyword("Flying") && !blocker.HasKeyword("Reach"))
            return false;
        if (attacker.HasKeyword("Shadow") != blocker.HasKeyword("Shadow") && !(attacker.HasKeyword("Shadow") && blocker.Cant("BlockShadow")))
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
            // "This creature assigns no combat damage this turn."
            if (traits.Cant("AssignCombatDamage"))
                return false;
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
                // A blocked creature stays blocked when its blockers are gone; only trample gets its damage through.
                if (!History.WasBlocked(state, attacker.Id) || traits.HasKeyword("Trample"))
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
