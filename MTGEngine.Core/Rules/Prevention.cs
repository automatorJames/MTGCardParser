namespace MTGEngine.Core;

/// <summary>
/// A damage prevention shield: "prevent the next 3 damage that would be dealt to target creature this turn",
/// "prevent all combat damage that would be dealt this turn". What it protects and whose damage it stops are either
/// fixed when it's made (the target, the chosen source) or a selector read when damage would be dealt ("creatures you
/// control"); none means any. <see cref="Remaining"/> null prevents all of it. A shield with
/// <see cref="RedirectTo"/> deals the damage it stops to that instead ("is dealt to this creature instead").
/// </summary>
public sealed record PreventionShield(
    int Id,
    PlayerId Controller,
    ObjectId? Source,
    int? Remaining,
    bool CombatOnly,
    ImmutableList<TargetRef> To,
    Selector ToSelector,
    ImmutableList<ObjectId> From,
    Selector FromSelector,
    int? ExpiresAfterTurn,
    bool OneShot = false,
    Effect Then = null,
    ImmutableList<TargetRef> RedirectTo = null);

public sealed partial class Game
{
    /// <summary>The redirection shields whose damage is being dealt now, so two can't send damage back and forth.</summary>
    readonly HashSet<int> _redirecting = [];

    /// <summary>Makes the shields a prevention effect describes (two for "dealt to and dealt by").</summary>
    public void CreateShields(PreventDamage prevent, EffectContext ctx)
    {
        var amount = prevent.Amount is Special { Kind: "All" } ? (int?)null : Quantities.Evaluate(prevent.Amount, ctx);
        if (amount is null && prevent.Amount is not Special { Kind: "All" })
        {
            Skip(prevent.Amount, "prevention amount");
            return;
        }
        var until = Interpreter.UntilTurn(prevent.Duration, State);
        var expires = until is null or -1 ? State.Turn : until;
        var combat = prevent.Kind == "Combat";
        var (to, toSelector) = Fix(prevent.To, ctx, choose: false);
        var (from, fromSelector) = Fix(prevent.Source, ctx, choose: true);

        Emit(new ShieldCreated(new PreventionShield(State.NextShieldId, ctx.Controller, ctx.Source, amount, combat, to, toSelector, from?.Where(x => x.IsObject).Select(x => x.Object.Value).ToImmutableList(), fromSelector, expires)));
        if (prevent.ToAndBy)
            Emit(new ShieldCreated(new PreventionShield(State.NextShieldId, ctx.Controller, ctx.Source, amount, combat, null, null, to?.Where(x => x.IsObject).Select(x => x.Object.Value).ToImmutableList(), toSelector, expires)));
    }

    /// <summary>
    /// Makes the shield a redirection describes: "the next 1 damage that would be dealt to target creature this turn is
    /// dealt to this creature instead", "all combat damage that would be dealt to you by unblocked creatures this turn".
    /// </summary>
    public bool CreateRedirect(RedirectDamage redirect, EffectContext ctx)
    {
        var amount = redirect.Amount is Special { Kind: "All" } ? (int?)null : Quantities.Evaluate(redirect.Amount, ctx);
        if (amount is null && redirect.Amount is not Special { Kind: "All" })
            return false;
        var to = Resolver.Targets(redirect.To, ctx);
        if (to.Count == 0)
            return true;
        var until = Interpreter.UntilTurn(redirect.Duration, State);
        var expires = until is null or -1 ? State.Turn : until;
        var (from, fromSelector) = Fix(redirect.From, ctx, choose: false);
        var (by, bySelector) = Fix(redirect.Source, ctx, choose: true);
        Emit(new ShieldCreated(new PreventionShield(State.NextShieldId, ctx.Controller, ctx.Source, amount, redirect.Combat, from, fromSelector,
            by?.Where(x => x.IsObject).Select(x => x.Object.Value).ToImmutableList(), bySelector, expires, RedirectTo: [to[0]])));
        return true;
    }

    /// <summary>A selector as the objects and players it means now, or kept as a selector when it describes a group to be read when damage happens. "A source of your choice" is chosen now.</summary>
    (ImmutableList<TargetRef> Fixed, Selector Later) Fix(Selector selector, EffectContext ctx, bool choose)
    {
        switch (selector)
        {
            case null:
                return (null, null);
            case ObjectSelector { Quantifier: Quantifier.All or Quantifier.Each } group:
                return (null, group);
            case PlayerSelector { Kind: PlayerKind.Players or PlayerKind.EachPlayer or PlayerKind.AnyPlayer } players:
                return (null, players);
            case ObjectSelector { Quantifier: Quantifier.Any } any when choose:
            {
                var candidates = State.Objects.Values
                    .Where(x => x.Zone is Zone.Battlefield or Zone.Stack && Filters.Matches(State, any.Filter with { Zone = null }, x with { Zone = Zone.Battlefield }, ctx.Controller, ctx.Source, ctx: ctx))
                    .Select(x => TargetRef.Of(x.Id)).ToImmutableList();
                if (candidates.Count == 0)
                    return ([], null);
                var picked = Decide(new ChooseTargets(ctx.Controller, "Choose a source", candidates, 1, 1)).Targets?.FirstOrDefault(candidates.Contains) ?? candidates[0];
                return ([picked], null);
            }
            default:
                return (Resolver.Targets(selector, ctx), null);
        }
    }

    /// <summary>The damage left after prevention shields and static prevention abilities: what would be dealt by <paramref name="source"/> to <paramref name="target"/>.</summary>
    int Prevent(ObjectId? source, TargetRef target, int amount, bool combat)
    {
        // Protection: damage from what a permanent has protection from is prevented.
        if (target.IsObject && State.Object(target.Object.Value) is { } victim && source is { } from && State.Object(from) is { } dealer
            && Keywords.ProtectedFrom(State, victim, Characteristics.Of(State, victim), dealer))
        {
            Emit(new DamagePrevented(source, target, amount));
            return 0;
        }
        var redirected = new List<(TargetRef To, int Amount, int Shield)>();
        foreach (var shield in State.ShieldList.ToList())
        {
            if (amount <= 0)
                break;
            if (!Applies(shield, source, target, combat))
                continue;
            // Damage isn't redirected to what it would already be dealt to, to a permanent that's gone, or by a shield that already redirected it.
            if (shield.RedirectTo is { } redirectTo && (_redirecting.Contains(shield.Id) || redirectTo.Contains(target) || redirectTo[0].IsObject && State.Object(redirectTo[0].Object.Value) is not { Zone: Zone.Battlefield }))
                continue;
            var prevented = shield.Remaining is { } remaining ? Math.Min(remaining, amount) : amount;
            if (prevented <= 0)
                continue;
            amount -= prevented;
            if (shield.RedirectTo is { } instead)
                redirected.Add((instead[0], prevented, shield.Id));
            else
                Emit(new DamagePrevented(source, target, prevented));
            if (shield.Remaining is not null || shield.OneShot)
                Emit(new ShieldUsed(shield.Id, shield.OneShot ? int.MaxValue : prevented));
            AfterPreventing(shield, target, prevented);
        }

        foreach (var shield in StaticShields().Concat(ReplacementShields()))
        {
            if (amount <= 0)
                break;
            if (!Applies(shield, source, target, combat))
                continue;
            var prevented = shield.Remaining is { } remaining ? Math.Min(remaining, amount) : amount;
            amount -= prevented;
            Emit(new DamagePrevented(source, target, prevented));
            AfterPreventing(shield, target, prevented);
        }

        foreach (var (to, dealt, shieldId) in redirected)
        {
            _redirecting.Add(shieldId);
            try { DealDamage(source, to, dealt, combat); }
            finally { _redirecting.Remove(shieldId); }
        }
        return amount;
    }

    /// <summary>What a replacement does besides preventing: "prevent that damage. remove a +1/+1 counter from this creature".</summary>
    void AfterPreventing(PreventionShield shield, TargetRef target, int prevented)
    {
        if (shield.Then is not { } then)
            return;
        var ctx = new EffectContext(this, shield.Controller, shield.Source, new Dictionary<IrNode, ImmutableList<TargetRef>>(ReferenceEqualityComparer.Instance), Last: [target], LastAmount: prevented);
        Interpreter.Execute(then, ctx);
    }

    /// <summary>Prevention stated by static abilities on the battlefield ("prevent all damage that would be dealt to this creature by artifact creatures").</summary>
    IEnumerable<PreventionShield> StaticShields()
    {
        foreach (var (effect, _, source) in Characteristics.StaticEffects(State))
            if (effect is PreventDamage { Duration: null } prevent && prevent.Amount is Special { Kind: "All" } or Fixed)
            {
                var amount = prevent.Amount is Fixed fixedAmount ? fixedAmount.Value : (int?)null;
                ImmutableList<TargetRef> FixedOf(Selector selector) =>
                    selector switch
                    {
                        Self => [TargetRef.Of(source.Id)],
                        ObjectSelector { Quantifier: Quantifier.Attached } when source.AttachedTo is { } host => [TargetRef.Of(host)],
                        PlayerSelector { Kind: PlayerKind.You } => [TargetRef.Of(source.Controller)],
                        _ => null,
                    };
                Selector Later(Selector selector) => FixedOf(selector) is null ? selector : null;
                var to = FixedOf(prevent.To);
                var shield = new PreventionShield(0, source.Controller, source.Id, amount, prevent.Kind == "Combat", to, Later(prevent.To), FixedOf(prevent.Source)?.Select(x => x.Object ?? default).ToImmutableList(), Later(prevent.Source), null);
                yield return shield;
                if (prevent.ToAndBy)
                    yield return shield with { To = null, ToSelector = null, From = to?.Where(x => x.IsObject).Select(x => x.Object.Value).ToImmutableList(), FromSelector = Later(prevent.To) };
            }
    }

    bool Applies(PreventionShield shield, ObjectId? source, TargetRef target, bool combat)
    {
        if (shield.CombatOnly && !combat)
            return false;
        if (shield.To is { } to && !to.Contains(target))
            return false;
        if (shield.ToSelector is { } toSelector && !Means(toSelector, target, shield))
            return false;
        if (shield.From is { } from && (source is not { } dealer || !from.Contains(dealer)))
            return false;
        if (shield.FromSelector is { } fromSelector && (source is not { } by || !Means(fromSelector, TargetRef.Of(by), shield)))
            return false;
        return true;
    }

    /// <summary>Whether a selector kept for later means <paramref name="target"/> now, read from the shield's controller and source.</summary>
    bool Means(Selector selector, TargetRef target, PreventionShield shield)
    {
        switch (selector)
        {
            case PlayerSelector players:
                return target.IsPlayer && (players.Kind is PlayerKind.Players or PlayerKind.EachPlayer or PlayerKind.AnyPlayer || Resolver.Players(State, players, shield.Controller, shield.Source).Contains(target.Player.Value));
            case ObjectSelector objects when target.IsObject && State.Object(target.Object.Value) is { } obj:
                return Filters.Matches(State, objects.Filter, obj, shield.Controller, shield.Source, objects.Other ? shield.Source : null);
            case Union union:
                return union.Members.Any(x => Means(x, target, shield));
            default:
                return false;
        }
    }
}
