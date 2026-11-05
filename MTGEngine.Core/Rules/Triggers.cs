namespace MTGEngine.Core;

/// <summary>A triggered ability that has triggered: its source, the ability, and what it triggered on.</summary>
public sealed record PendingTrigger(GameObject Source, TriggeredAbility Ability, TargetRef? Subject, PlayerId Controller, DelayedTrigger Delayed = null, int AbilityIndex = -1);

/// <summary>Matches the IR's trigger events against the game's events.</summary>
public static class Triggers
{
    /// <summary>The triggered abilities that <paramref name="evt"/> sets off, given the state just after it (and the state just before, for leaves-the-battlefield triggers).</summary>
    public static IEnumerable<PendingTrigger> For(GameEvent evt, GameState after, GameState before)
    {
        // Permanents on the battlefield now, plus the object that just left it (its own leaves/dies triggers look back).
        var sources = after.Battlefield.Concat(after.Emblems).ToList();
        if (evt is ZoneMoved { From: Zone.Battlefield } left && before.Object(left.Object) is { } leaver)
            sources.Add(leaver);

        foreach (var source in sources)
        {
            if (after.Temporary.Any(x => x.Target == source.Id && x.LosesAbilities))
                continue;
            foreach (var ability in Characteristics.AbilitiesOf(after, source).OfType<TriggeredAbility>())
                if (ability.Trigger is CombatEncounterEvent encounter)
                {
                    foreach (var other in Encounters(encounter, evt, source, after))
                        yield return new PendingTrigger(source, ability, other, source.Controller);
                }
                else if (Match(ability.Trigger, evt, source, after, before) is { } matched && matched.Fires)
                    yield return new PendingTrigger(source, ability, matched.Subject, source.Controller);
        }

        // A saga's chapter abilities trigger as lore counters reach their numbers.
        if (evt is CountersChanged { Kind: "lore", Delta: > 0 } lore && after.Object(lore.Object) is { Zone: Zone.Battlefield } saga)
        {
            var now = saga.CountersOf("lore");
            var was = now - lore.Delta;
            foreach (var chapter in Characteristics.AbilitiesOf(after, saga).OfType<ChapterAbility>())
                if (chapter.Chapters.Any(n => n > was && n <= now))
                    yield return new PendingTrigger(saga, new TriggeredAbility(new NamedEvent("Chapter"), chapter.Effect), null, saga.Controller);
        }

        foreach (var delayed in after.DelayedTriggers)
        {
            var source = (delayed.Source is { } id ? after.Object(id) ?? before.Object(id) : null)
                ?? new GameObject(new ObjectId(-delayed.Id), new CardDefinition("delayed", ManaCost.Free, [], [], [], null, null, null, [], [], []), delayed.Controller, delayed.Controller, Zone.Command);
            var when = delayed.When;
            // "when that creature dies" names what the effect bound; the subject matched is the one the trigger was set up for.
            if (when is ObjectEvent { Subject: ObjectSelector { Quantifier: Quantifier.Target } or Reference } objectEvent && delayed.Subject is { IsObject: true } bound)
                when = objectEvent with { Subject = new Self() };
            var matched = when is ObjectEvent && delayed.Subject is { IsObject: true } subject
                ? Match(when, evt, (after.Object(subject.Object.Value) ?? before.Object(subject.Object.Value) ?? source) with { Controller = delayed.Controller }, after, before)
                : Match(when, evt, source, after, before);
            if (matched is { Fires: true })
                yield return new PendingTrigger(source, new TriggeredAbility(delayed.When, delayed.Effect), matched.Subject ?? delayed.Subject, delayed.Controller, delayed);
        }
    }

    sealed record Matched(bool Fires, TargetRef? Subject = null);

    static StackItem ItemOf(GameEvent evt) =>
        evt switch
        {
            SpellCast cast => cast.Item,
            AbilityActivated activated => activated.Item,
            Triggered triggered => triggered.Item,
            _ => null,
        };

    static ImmutableList<TargetRef> TargetsOf(GameEvent evt) => ItemOf(evt)?.Targets ?? [];

    public static readonly ImmutableHashSet<string> TargetedByKinds = ["SpellOrAbility", "Spell", "InstantOrSorcerySpell", "AuraSpell"];

    /// <summary>Whether what's targeting is what "becomes the target of …" names: any spell or ability, a spell, an instant or sorcery spell, an aura spell, one an opponent controls.</summary>
    static bool TargetedBy(ObjectEvent trigger, StackItem item, GameObject source, GameState after)
    {
        var spell = item.Spell is { } id ? after.Object(id) : null;
        var kind = trigger.By switch
        {
            null or "SpellOrAbility" => true,
            "Spell" => item.Kind == StackKind.Spell,
            "InstantOrSorcerySpell" => spell is not null && (spell.Card.IsInstant || spell.Card.IsSorcery),
            "AuraSpell" => spell is not null && spell.Card.IsAura,
            _ => false,
        };
        if (!kind)
            return false;
        if (trigger.ByController is { } controller)
        {
            var players = Resolver.Players(after, controller.Player, source.Controller, source.Id);
            if (players.Contains(item.Controller) == controller.Negated)
                return false;
        }
        return true;
    }

    /// <summary>"Whenever this creature blocks or becomes blocked by a nonblack creature": each pairing in a block declaration, with the other creature as the subject.</summary>
    static IEnumerable<TargetRef> Encounters(CombatEncounterEvent encounter, GameEvent evt, GameObject source, GameState after)
    {
        if (evt is not BlockersDeclared blocks)
            yield break;
        bool IsSubject(ObjectId id) => encounter.Subject switch
        {
            Self or null => id == source.Id,
            ObjectSelector selector => after.Object(id) is { } obj && Filters.Matches(after, selector.Filter, obj, source.Controller, source.Id),
            _ => false,
        };
        bool IsOther(ObjectId id) => encounter.Other switch
        {
            null => true,
            ObjectSelector selector => after.Object(id) is { } obj && Filters.Matches(after, selector.Filter, obj, source.Controller, source.Id),
            _ => false,
        };
        var blocking = encounter.Encounter is "Blocks" or "BlocksOrBecomesBlockedBy";
        var blocked = encounter.Encounter is "BecomesBlockedBy" or "BlocksOrBecomesBlockedBy";
        foreach (var (blocker, attacker) in blocks.Blocks)
        {
            if (blocking && IsSubject(blocker) && IsOther(attacker))
                yield return TargetRef.Of(attacker);
            if (blocked && IsSubject(attacker) && IsOther(blocker))
                yield return TargetRef.Of(blocker);
        }
    }

    public static readonly ImmutableHashSet<string> EncounterKinds = ["Blocks", "BecomesBlockedBy", "BlocksOrBecomesBlockedBy"];

    /// <summary>Whether <paramref name="player"/> is who a trigger's player selector names, from <paramref name="source"/>'s controller's side.</summary>
    static bool PlayerIs(Selector selector, PlayerId player, GameObject source) =>
        selector switch
        {
            null => true,
            PlayerSelector { Kind: PlayerKind.You } => player == source.Controller,
            PlayerSelector { Kind: PlayerKind.EachOpponent or PlayerKind.AnyOpponent or PlayerKind.YourOpponents } => player != source.Controller,
            PlayerSelector { Kind: PlayerKind.EachPlayer or PlayerKind.AnyPlayer or PlayerKind.Players } => true,
            _ => false,
        };

    static Matched Match(Event trigger, GameEvent evt, GameObject source, GameState after, GameState before)
    {
        switch (trigger)
        {
            case ObjectEvent objectEvent:
                return MatchObjectEvent(objectEvent, evt, source, after, before);

            case PhaseEvent phase when evt is StepBegan step:
            {
                var at = phase.At;
                if (Conditions.StepOf(at.Phase) is not { } wanted || wanted != step.Step)
                    return null;
                var whose = at.Whose switch
                {
                    null => true,
                    PlayerSelector { Kind: PlayerKind.You } => step.Player == source.Controller,
                    PlayerSelector { Kind: PlayerKind.EachOpponent or PlayerKind.AnyOpponent or PlayerKind.YourOpponents } => step.Player != source.Controller,
                    PlayerSelector { Kind: PlayerKind.EachPlayer or PlayerKind.AnyPlayer or PlayerKind.Players } => true,
                    _ => false,
                };
                return new Matched(whose, TargetRef.Of(step.Player));
            }

            case SpellCastEvent cast when evt is SpellCast spellCast:
            {
                var caster = cast.Caster switch
                {
                    null => true,
                    PlayerSelector { Kind: PlayerKind.You } => spellCast.Item.Controller == source.Controller,
                    PlayerSelector { Kind: PlayerKind.EachOpponent or PlayerKind.AnyOpponent } => spellCast.Item.Controller != source.Controller,
                    PlayerSelector => true,
                    _ => false,
                };
                if (!caster || spellCast.Item.Spell is not { } spellId || after.Object(spellId) is not { } spell)
                    return null;
                var filter = cast.Spell is ObjectSelector selector ? selector.Filter : null;
                if (filter is not null && !Filters.Matches(after, filter with { Zone = null }, spell, source.Controller, source.Id, source.Id == spell.Id ? null : null))
                    return null;
                return new Matched(true, TargetRef.Of(spellId));
            }

            case PlayerEvent playerEvent when evt is LifeChanged life:
            {
                var fits = playerEvent.Event switch
                {
                    "GainsLife" => life.Delta > 0,
                    "LosesLife" => life.Delta < 0,
                    "GainsOrLosesLife" => life.Delta != 0,
                    _ => false,
                };
                if (!fits)
                    return null;
                var who = playerEvent.Player switch
                {
                    PlayerSelector { Kind: PlayerKind.You } => life.Player == source.Controller,
                    PlayerSelector { Kind: PlayerKind.EachOpponent or PlayerKind.AnyOpponent } => life.Player != source.Controller,
                    _ => true,
                };
                return new Matched(who, TargetRef.Of(life.Player));
            }

            case PlayerAction { Action: "AttackedWith" } attacked when evt is AttackersDeclared declared:
            {
                if (!PlayerIs(attacked.Player, declared.Player, source))
                    return null;
                var count = declared.Attackers.Count(id => attacked.What is not ObjectSelector selector || after.Object(id) is { } attacker && Filters.Matches(after, selector.Filter, attacker, source.Controller, source.Id));
                var wanted = attacked.What is ObjectSelector { Count: Bound bound } && bound.Value is Fixed least ? (bound.Comparison, least.Value) : (Comparison.GreaterOrEqual, 1);
                return new Matched(Filters.Compare(count, wanted.Item1, wanted.Item2), TargetRef.Of(declared.Player));
            }

            case PlayerAction { Action: "Activate" } activated when evt is AbilityActivated ability:
            {
                if (!PlayerIs(activated.Player, ability.Item.Controller, source))
                    return null;
                var holder = ability.Item.Source is { } id ? after.Object(id) : null;
                var fits = Characteristics.AbilityHolders(activated.What) switch
                {
                    Self => holder?.Id == source.Id,
                    ObjectSelector { Filter: var filter } => holder is not null && Filters.Matches(after, filter, holder, source.Controller, source.Id),
                    _ => true,
                };
                return new Matched(fits, TargetRef.Of(ability.Item.Controller));
            }

            case EffectEvent { Effect: Draw draw } when evt is Drew { Card: not null } drew:
                return new Matched(PlayerIs(draw.Player, drew.Player, source), TargetRef.Of(drew.Player));

            case EffectEvent { Effect: Sacrifice sacrifice } when evt is Sacrificed sacrificed:
            {
                if (!PlayerIs(sacrifice.Player, sacrificed.Player, source))
                    return null;
                var victim = before.Object(sacrificed.Object) ?? after.Object(sacrificed.Object);
                var fits = sacrifice.Objects switch
                {
                    Self => sacrificed.Object == source.Id,
                    ObjectSelector selector => victim is not null && Filters.Matches(before, selector.Filter, victim with { Zone = Zone.Battlefield }, source.Controller, source.Id, selector.Other ? source.Id : null),
                    _ => false,
                };
                return new Matched(fits, TargetRef.Of(sacrificed.Object));
            }

            case EitherEvent either:
                foreach (var option in either.Events)
                    if (Match(option, evt, source, after, before) is { Fires: true } matched)
                        return matched;
                return null;

            default:
                return null;
        }
    }

    static Matched MatchObjectEvent(ObjectEvent trigger, GameEvent evt, GameObject source, GameState after, GameState before)
    {
        foreach (var name in trigger.Events)
        {
            var result = name switch
            {
                "EntersTheBattlefield" or "Enters" or "EntersOrTransforms" or "EntersUntapped" when evt is ZoneMoved { To: Zone.Battlefield } entered =>
                    Subject(trigger, entered.Object, source, after, before: null, under: trigger.UnderControlOf),
                "Dies" or "Die" or "IsPutIntoAGraveyardFromTheBattlefield" when evt is ZoneMoved { From: Zone.Battlefield, To: Zone.Graveyard } died =>
                    Subject(trigger, died.Object, source, after, before),
                "IsPutIntoAGraveyardFromAnywhere" or "IsPutIntoAGraveyard" when evt is ZoneMoved { To: Zone.Graveyard } toGraveyard =>
                    Subject(trigger, toGraveyard.Object, source, after, before),
                "LeavesTheBattlefield" or "LeaveTheBattlefield" or "LeavesTheBattlefieldWithoutDying" when evt is ZoneMoved { From: Zone.Battlefield } leaves =>
                    Subject(trigger, leaves.Object, source, after, before),
                "IsPutIntoExileFromTheBattlefield" when evt is ZoneMoved { From: Zone.Battlefield, To: Zone.Exile } exiled =>
                    Subject(trigger, exiled.Object, source, after, before),
                "Attacks" or "AttacksOrBlocks" or "AttacksAlone" or "AttacksYou" or "AttacksAPlayer" when evt is AttackersDeclared attackers =>
                    name == "AttacksAlone" && attackers.Attackers.Count != 1 ? null : AnySubject(trigger, attackers.Attackers, source, after),
                "Blocks" or "AttacksOrBlocks" or "BlocksOrBecomesBlocked" or "BlocksACreature" when evt is BlockersDeclared blockers =>
                    AnySubject(trigger, blockers.Blocks.Keys, source, after),
                "BecomesBlocked" or "BlocksOrBecomesBlocked" when evt is BlockersDeclared blocked =>
                    AnySubject(trigger, blocked.Blocks.Values.Distinct(), source, after),
                "BecomesTapped" when evt is Tapped tapped => Subject(trigger, tapped.Object, source, after, before),
                "BecomesUntapped" when evt is Untapped untapped => Subject(trigger, untapped.Object, source, after, before),
                "DealsCombatDamageToAPlayer" or "DealsCombatDamageToAPlayerOrPlaneswalker" or "DealsCombatDamageToAnOpponent" or "DealsCombatDamageToYou"
                    when evt is DamageDealt { Combat: true, Target.IsPlayer: true, Source: { } dealer } combat && (name != "DealsCombatDamageToYou" || combat.Target.Player == source.Controller) && (name != "DealsCombatDamageToAnOpponent" || combat.Target.Player != source.Controller) =>
                    Subject(trigger, dealer, source, after, before) is { Fires: true } m ? m with { Subject = combat.Target } : null,
                "DealsCombatDamage" when evt is DamageDealt { Combat: true, Source: { } dealer2 } combat2 =>
                    Subject(trigger, dealer2, source, after, before) is { Fires: true } m2 ? m2 with { Subject = combat2.Target } : null,
                "DealsDamage" or "DealsDamageToAPlayer" or "DealsDamageToAnOpponent" or "DealsDamageToACreature" or "DealsDamageToYou" or "DealsDamageToAPlayerOrPlaneswalker"
                    when evt is DamageDealt { Source: { } dealer3 } any && Recipient(name, any, source, after) =>
                    Subject(trigger, dealer3, source, after, before) is { Fires: true } m3 ? m3 with { Subject = any.Target } : null,
                "IsDealtDamage" or "IsDealtCombatDamage" when evt is DamageDealt { Target.IsObject: true } hit && (name == "IsDealtDamage" || hit.Combat) =>
                    Subject(trigger, hit.Target.Object.Value, source, after, before) is { Fires: true } m4 ? m4 with { Subject = hit.Source is { } s ? TargetRef.Of(s) : null } : null,
                "Cast" when evt is SpellCast cast && cast.Item.Spell is { } spellId =>
                    Subject(trigger, spellId, source, after, before),
                "Countered" when evt is Countered countered && before.Stack.FirstOrDefault(x => x.Id == countered.StackId)?.Spell is { } counteredSpell =>
                    Subject(trigger, counteredSpell, source, after, before),
                "Discarded" when evt is Discarded discarded =>
                    Subject(trigger, discarded.Card, source, after, before),
                "CountersPut" when evt is CountersChanged { Delta: > 0 } put =>
                    Subject(trigger, put.Object, source, after, before),
                "BecomesTarget" when TargetsOf(evt) is { Count: > 0 } targeted && TargetedBy(trigger, ItemOf(evt), source, after) =>
                    targeted.Where(x => x.IsObject).Select(x => Subject(trigger, x.Object.Value, source, after, before)).FirstOrDefault(x => x is { Fires: true }) is { } hit
                        ? hit with { Subject = ItemOf(evt)?.Spell is { } spell ? TargetRef.Of(spell) : hit.Subject }
                        : null,
                _ => null,
            };

            if (result is { Fires: true })
                return result;
        }

        return null;
    }

    static bool Recipient(string name, DamageDealt damage, GameObject source, GameState state) =>
        name switch
        {
            "DealsDamageToAPlayer" or "DealsDamageToAPlayerOrPlaneswalker" => damage.Target.IsPlayer,
            "DealsDamageToAnOpponent" => damage.Target.IsPlayer && damage.Target.Player != source.Controller,
            "DealsDamageToYou" => damage.Target.Player == source.Controller,
            "DealsDamageToACreature" => damage.Target.IsObject && state.Object(damage.Target.Object.Value)?.Card.IsCreature == true,
            _ => true,
        };

    /// <summary>Whether the trigger's subject describes <paramref name="id"/>: the source itself, or a filter it matches.</summary>
    static Matched Subject(ObjectEvent trigger, ObjectId id, GameObject source, GameState after, GameState before, Selector under = null)
    {
        var state = after.Object(id) is not null ? after : before;
        var obj = state?.Object(id) ?? before?.Object(id);
        if (obj is null)
            return null;

        bool fits;
        switch (trigger.Subject)
        {
            case Self:
            case null:
                fits = id == source.Id;
                break;
            case ObjectSelector selector:
            {
                var lookback = before?.Object(id) ?? obj;
                var zoneless = selector.Filter is null ? null : selector.Filter with { Zone = null };
                fits = Filters.Matches(before ?? after, zoneless, lookback with { Zone = Zone.Battlefield }, source.Controller, source.Id, selector.Other ? source.Id : null)
                    || Filters.Matches(after, zoneless, obj with { Zone = Zone.Battlefield }, source.Controller, source.Id, selector.Other ? source.Id : null);
                if (fits && selector.Other && id == source.Id)
                    fits = false;
                break;
            }
            case Union union:
                fits = union.Members.Any(member => Subject(trigger with { Subject = member }, id, source, after, before, under)?.Fires == true);
                break;
            default:
                return null;
        }

        if (fits && (under ?? trigger.UnderControlOf) is PlayerSelector controller)
            fits = controller.Kind switch
            {
                PlayerKind.You => obj.Controller == source.Controller,
                PlayerKind.EachOpponent or PlayerKind.AnyOpponent => obj.Controller != source.Controller,
                _ => true,
            };

        return new Matched(fits, TargetRef.Of(id));
    }

    static Matched AnySubject(ObjectEvent trigger, IEnumerable<ObjectId> ids, GameObject source, GameState after)
    {
        foreach (var id in ids)
            if (Subject(trigger, id, source, after, null) is { Fires: true } matched)
                return matched;
        return null;
    }
}
