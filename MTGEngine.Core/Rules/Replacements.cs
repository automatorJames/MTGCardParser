namespace MTGEngine.Core;

/// <summary>
/// A replacement set up by an effect for named objects: "if that creature would die this turn, exile it instead",
/// "if that spell is countered this way, exile it instead". Each object is held with its zone-change count, so the
/// replacement follows the object and not a new one the card becomes after it moves.
/// </summary>
public sealed record ZoneReplacement(string Event, ImmutableList<(ObjectId Object, int ZoneChangeCount)> Objects, Zone To, int? ExpiresAfterTurn);

public sealed partial class Game
{
    /// <summary>The events a zone replacement can stand in for, as the IR names them.</summary>
    public static readonly ImmutableHashSet<string> ZoneEvents = ["Die", "Dies", "IsPutIntoAGraveyard", "IsPutIntoAGraveyardFromAnywhere", "LeaveTheBattlefield", "LeavesTheBattlefield", "Countered"];

    /// <summary>Sets up a replacement an effect describes: a prevention shield for damage, a zone replacement for dying or leaving. False when it's neither.</summary>
    public bool CreateReplacement(Replacement replacement, EffectContext ctx)
    {
        var until = Interpreter.UntilTurn(replacement.Duration, State);
        var expires = until is null or -1 ? (int?)null : until;

        if (DamageReplacement(replacement) is { } damage)
        {
            var (to, toSelector) = Fix(damage.To, ctx, choose: false);
            var (from, fromSelector) = Fix(damage.From, ctx, choose: true);
            Emit(new ShieldCreated(new PreventionShield(0, ctx.Controller, ctx.Source, null, damage.CombatOnly, to, toSelector,
                from?.Where(x => x.IsObject).Select(x => x.Object.Value).ToImmutableList(), fromSelector, expires ?? State.Turn, replacement.NextTime, damage.Then)));
            return true;
        }

        if (ZoneReplacementOf(replacement) is { } zone)
        {
            var subjects = zone.Subject switch
            {
                Self => ctx.Source is { } self ? [self] : [],
                _ => Resolver.Objects(zone.Subject, ctx),
            };
            var held = subjects.Select(State.Object).Where(x => x is not null).Select(x => (x.Id, x.ZoneChangeCount)).ToImmutableList();
            Emit(new ZoneReplacementCreated(new ZoneReplacement(zone.Event, held, zone.To, expires)));
            return true;
        }

        return false;
    }

    /// <summary>
    /// A damage replacement that changes how much is dealt, to the same recipient by the same source: "it deals double
    /// that damage instead", "it deals that much damage plus 1 instead", "minus 1".
    /// </summary>
    public static (Selector From, Selector To, string Kind, Adjusted Amount)? DamageModifier(Replacement replacement)
    {
        if (replacement.Would is not WouldHappen { What: DealDamage dealt } would)
            return null;
        if (replacement.Instead is not Instead { Effect: DealDamage { Portions: [{ Amount: Adjusted { Base: Special { Kind: "ThatMuch" } } adjusted, To: Reference or Self or null }] } instead })
            return null;
        if (instead.Source is not (Reference or Self or null) || dealt.Portions?.Any(x => x.Amount is not null) == true)
            return null;
        var valid = adjusted.Factor == "Twice" && adjusted.Adjustment is null || adjusted.Factor is null && adjusted.Adjustment is Fixed;
        return valid ? (dealt.Source ?? would.Subject, dealt.Portions?.FirstOrDefault()?.To, dealt.Kind, adjusted) : null;
    }

    /// <summary>The damage <paramref name="source"/> would deal to <paramref name="target"/> after the modifiers on permanents and emblems: doubled, or more or less by a number.</summary>
    int Modify(ObjectId? source, TargetRef target, int amount, bool combat)
    {
        foreach (var holder in State.Battlefield.Concat(State.Emblems).ToList())
            foreach (var ability in Characteristics.AbilitiesOf(State, holder))
            {
                var (replacement, holds) = CardDefinition.Unwrap(ability) switch
                {
                    ReplacementAbility { Effect: Replacement stated } => (stated, (Condition)null),
                    StaticAbility { Effect: Replacement stated, While: var condition } => (stated, condition),
                    _ => (null, null),
                };
                if (replacement is null || DamageModifier(replacement) is not { } modifier)
                    continue;
                if (modifier.Kind == "Combat" && !combat || modifier.Kind == "Noncombat" && combat)
                    continue;
                var ctx = EffectContext.Snapshot(State, holder.Controller, holder.Id);
                if (holds is not null && Conditions.Evaluate(holds, ctx) != true || replacement.Would is WouldHappen { While: { } whilst } && Conditions.Evaluate(whilst, ctx) != true)
                    continue;
                if (!DamageSide(modifier.From, source is { } dealer ? TargetRef.Of(dealer) : null, holder) || !DamageSide(modifier.To, target, holder))
                    continue;
                amount = modifier.Amount.Factor == "Twice" ? amount * 2 : amount + (modifier.Amount.Minus ? -1 : 1) * ((Fixed)modifier.Amount.Adjustment).Value;
            }
        return Math.Max(0, amount);
    }

    /// <summary>Whether a modifier's source or recipient selector means <paramref name="side"/>: "a source you control", "a permanent or player", "an opponent", "this creature".</summary>
    bool DamageSide(Selector selector, TargetRef? side, GameObject holder)
    {
        switch (selector)
        {
            case null or Reference:
                return true;
            case Self:
                return side is { IsObject: true } self && self.Object == holder.Id;
            case Union union:
                return union.Members.Any(x => DamageSide(x, side, holder));
            case PlayerSelector players:
                return side is { IsPlayer: true } named && Resolver.Players(State, players, holder.Controller, holder.Id).Contains(named.Player.Value);
            case ObjectSelector objects when side is { IsPlayer: true } player:
            {
                if (objects.Filter?.Kinds?.Any(x => x.Value == nameof(ObjectKind.Player)) != true)
                    return false;
                var whose = objects.Filter.Controller;
                return whose is null || Resolver.Players(State, whose.Player, holder.Controller, holder.Id).Contains(player.Player.Value) != whose.Negated;
            }
            case ObjectSelector objects when side is { IsObject: true } thing && State.Object(thing.Object.Value) is { } obj:
            {
                var kinds = objects.Filter?.Kinds?.Where(x => x.Value != nameof(ObjectKind.Player)).ToList();
                if (objects.Filter?.Kinds is { Count: > 0 } && kinds.Count == 0)
                    return false;
                var filter = (objects.Filter ?? new ObjectFilter()) with { Kinds = kinds is { Count: > 0 } ? kinds : null };
                return Filters.Matches(State, filter, obj, holder.Controller, holder.Id, objects.Other ? holder.Id : null);
            }
            default:
                return false;
        }
    }

    /// <summary>A damage replacement's parts: "the next time target creature would deal damage, prevent that damage", "if damage would be dealt to this creature, prevent that damage. remove a counter".</summary>
    public static (Selector From, Selector To, bool CombatOnly, Effect Then)? DamageReplacement(Replacement replacement)
    {
        if (replacement.Would is not WouldHappen { What: DealDamage dealt } would)
            return null;
        PreventDamage prevent = null;
        Effect then = null;
        if (replacement.Instead is PreventDamage only)
            prevent = only;
        else if (replacement.Instead is Sequence { Steps: [PreventDamage first, ..] } sequence)
        {
            prevent = first;
            var rest = sequence.Steps.Skip(1).ToList();
            then = rest.Count == 0 ? null : rest.Count == 1 ? rest[0] : new Sequence(rest);
        }
        if (prevent is null || prevent.Amount is not (Special { Kind: "ThatMuch" or "All" } or null))
            return null;
        return (dealt.Source ?? would.Subject, dealt.Portions.FirstOrDefault()?.To, dealt.Kind == "Combat", then);
    }

    /// <summary>A zone replacement's parts: "if this creature would die, exile it instead".</summary>
    public static (Selector Subject, string Event, Zone To)? ZoneReplacementOf(Replacement replacement)
    {
        if (replacement.Would is not ObjectEvent { Events: [var name, ..] } would || !ZoneEvents.Contains(name))
            return null;
        var to = replacement.Instead switch
        {
            Exile => Zone.Exile,
            ChangeZone { To.Zone: var zone } when Enum.TryParse<Zone>(zone, out var parsed) => parsed,
            ShuffleInto => Zone.Library,
            _ => (Zone?)null,
        };
        return to is { } destination ? (would.Subject ?? new Self(), name, destination) : null;
    }

    /// <summary>Where a move to <paramref name="zone"/> really goes, after the replacements that apply: one-shot ones for this object and static ones on permanents.</summary>
    Zone Redirect(GameObject obj, Zone zone, bool countered)
    {
        bool Fits(string name) => name switch
        {
            "Die" or "Dies" => obj.Zone == Zone.Battlefield && zone == Zone.Graveyard && Characteristics.Of(State, obj).IsCreature,
            "IsPutIntoAGraveyard" or "IsPutIntoAGraveyardFromAnywhere" => zone == Zone.Graveyard,
            "LeaveTheBattlefield" or "LeavesTheBattlefield" => obj.Zone == Zone.Battlefield && zone != Zone.Battlefield,
            "Countered" => countered,
            "LeaveTheStack" => obj.Zone == Zone.Stack,
            _ => false,
        };

        foreach (var replacement in State.ZoneReplacementList)
            if (replacement.To != zone && Fits(replacement.Event) && replacement.Objects.Contains((obj.Id, obj.ZoneChangeCount)))
                return replacement.To;

        foreach (var source in State.Battlefield.ToList())
            foreach (var ability in Characteristics.AbilitiesOf(State, source).OfType<ReplacementAbility>())
                if (ability.Effect is Replacement replacement && ZoneReplacementOf(replacement) is { } found && found.To != zone && Fits(found.Event))
                {
                    var applies = found.Subject switch
                    {
                        Self => obj.Id == source.Id,
                        ObjectSelector selector => Filters.Matches(State, selector.Filter with { Zone = null }, obj, source.Controller, source.Id, selector.Other ? source.Id : null),
                        _ => false,
                    };
                    if (applies)
                        return found.To;
                }

        return zone;
    }

    /// <summary>Prevention stated as a replacement by permanents on the battlefield ("if damage would be dealt to this creature, prevent that damage").</summary>
    IEnumerable<PreventionShield> ReplacementShields()
    {
        foreach (var source in State.Battlefield.ToList())
            foreach (var ability in Characteristics.AbilitiesOf(State, source).OfType<ReplacementAbility>())
                if (ability.Effect is Replacement replacement && DamageReplacement(replacement) is { } damage)
                {
                    ImmutableList<TargetRef> FixedOf(Selector selector) =>
                        selector switch
                        {
                            Self => [TargetRef.Of(source.Id)],
                            ObjectSelector { Quantifier: Quantifier.Attached } when source.AttachedTo is { } host => [TargetRef.Of(host)],
                            PlayerSelector { Kind: PlayerKind.You } => [TargetRef.Of(source.Controller)],
                            _ => null,
                        };
                    var to = FixedOf(damage.To);
                    var from = FixedOf(damage.From);
                    yield return new PreventionShield(0, source.Controller, source.Id, null, damage.CombatOnly, to, to is null ? damage.To : null,
                        from?.Where(x => x.IsObject).Select(x => x.Object.Value).ToImmutableList(), from is null ? damage.From : null, null, false, damage.Then);
                }
    }
}
