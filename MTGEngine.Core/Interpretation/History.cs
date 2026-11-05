namespace MTGEngine.Core;

/// <summary>
/// "If a creature died this turn", "if you've cast another spell this turn", "if no spells were cast last turn",
/// "target creature that was dealt damage this turn": conditions over what happened, read from the events the state
/// keeps for this turn and the last (<see cref="GameState.ThisTurnEvents"/>).
/// </summary>
public static class History
{
    /// <summary>The events the state keeps as the turn's history.</summary>
    public static bool Kept(GameEvent evt) =>
        evt is SpellCast or BlockersDeclared or Drew { Card: not null } or Discarded or Sacrificed or LifeChanged or DamageDealt or ZoneMoved or AttackersDeclared or Scryed or LandPlayed or Milled;

    /// <summary>Whether an attacking creature was blocked this combat, even if its blockers have since left.</summary>
    public static bool WasBlocked(GameState state, ObjectId attacker) =>
        state.Object(attacker) is { Attacking: true } && state.ThisTurnEventList.OfType<BlockersDeclared>().Any(x => x.Blocks.Values.Contains(attacker));

    public static readonly ImmutableHashSet<string> NamedEvents =
    [
        "YouAttacked", "YouDidntAttack", "CreatureDied", "PermanentLeft", "YouGainedLife", "YouLostLife", "OpponentLostLife", "YouGainedOrLostLife",
        "YouDidntLoseLife", "CastAnotherSpell", "YouScriedOrSurveilled", "YouDidntPlayLand", "YouDescended",
    ];

    public static readonly ImmutableHashSet<string> PlayerActions =
    [
        "Cast", "DidntCast", "Discarded", "Drew", "Sacrificed", "GainedLife", "LostLife", "WasDealtDamage", "WasDealtNoncombatDamage", "WasDealtCombatDamage", "AttackedWith",
    ];

    public static readonly ImmutableHashSet<string> ObjectEvents =
    [
        "Died", "Entered", "EntersTheBattlefield", "EnteredUnderYourControl", "Attacked", "DidntAttack", "WasDealtDamage", "DealtDamage", "DealtDamageToYou",
    ];

    /// <summary>Whether <see cref="Evaluate"/> can answer this condition.</summary>
    public static bool Supported(Happened happened) =>
        happened.Span is "ThisTurn" or "LastTurn" or null && happened.Event switch
        {
            NamedEvent named => NamedEvents.Contains(named.Kind),
            PlayerAction action => PlayerActions.Contains(action.Action) && !action.FromHand,
            ObjectEvent objectEvent => objectEvent.Events.All(ObjectEvents.Contains),
            _ => false,
        };

    public static bool? Evaluate(Happened happened, EffectContext ctx)
    {
        if (!Supported(happened))
            return null;
        var state = ctx.State;
        var events = happened.Span == "LastTurn" ? state.LastTurnEventList : state.ThisTurnEventList;
        var you = ctx.Controller;

        switch (happened.Event)
        {
            case NamedEvent named:
            {
                int Life(Func<PlayerId, bool> who, Func<int, bool> delta) => events.OfType<LifeChanged>().Count(x => who(x.Player) && delta(x.Delta));
                return named.Kind switch
                {
                    "YouAttacked" => events.OfType<AttackersDeclared>().Any(x => x.Player == you && x.Attackers.Count > 0),
                    "YouDidntAttack" => !events.OfType<AttackersDeclared>().Any(x => x.Player == you && x.Attackers.Count > 0),
                    "CreatureDied" => events.OfType<ZoneMoved>().Any(x => x is { From: Zone.Battlefield, To: Zone.Graveyard } && state.Object(x.Object)?.Card.IsCreature == true),
                    "PermanentLeft" => events.OfType<ZoneMoved>().Any(x => x.From == Zone.Battlefield && (x.LeftController ?? state.Object(x.Object)?.Owner) == you),
                    "YouGainedLife" => Life(p => p == you, d => d > 0) > 0,
                    "YouLostLife" => Life(p => p == you, d => d < 0) > 0,
                    "OpponentLostLife" => Life(p => p != you, d => d < 0) > 0,
                    "YouGainedOrLostLife" => Life(p => p == you, d => d != 0) > 0,
                    "YouDidntLoseLife" => Life(p => p == you, d => d < 0) == 0,
                    "CastAnotherSpell" => events.OfType<SpellCast>().Any(x => x.Item.Controller == you && x.Item.Spell != ctx.Source),
                    "YouScriedOrSurveilled" => events.OfType<Scryed>().Any(x => x.Player == you),
                    "YouDidntPlayLand" => !events.OfType<LandPlayed>().Any(x => x.Player == you),
                    "YouDescended" => events.OfType<ZoneMoved>().Any(x => x.To == Zone.Graveyard && state.Object(x.Object) is { } card && card.Owner == you && card.Card.IsPermanentCard),
                    _ => null,
                };
            }

            case PlayerAction action:
            {
                var players = action.Player is null ? state.PlayersInGame.ToImmutableList() : Resolver.Players(action.Player, ctx);
                if (players.Count == 0)
                    players = state.PlayersInGame.ToImmutableList();
                bool Is(ObjectId id, Zone zone) => action.What is not ObjectSelector selector || state.Object(id) is { } obj && Filters.Matches(state, selector.Filter with { Zone = null }, obj with { Zone = zone }, you, ctx.Source, selector.Other ? ctx.Source : null, ctx: ctx);
                var count = action.Action switch
                {
                    "Cast" or "DidntCast" => events.OfType<SpellCast>().Count(x => players.Contains(x.Item.Controller) && x.Item.Spell is { } spell && spell != ctx.Source && Is(spell, Zone.Stack)),
                    "Discarded" => events.OfType<Discarded>().Count(x => players.Contains(x.Player) && Is(x.Card, Zone.Graveyard)),
                    "Drew" => events.OfType<Drew>().Count(x => players.Contains(x.Player)),
                    "Sacrificed" => events.OfType<Sacrificed>().Count(x => players.Contains(x.Player) && Is(x.Object, Zone.Battlefield)),
                    "GainedLife" => events.OfType<LifeChanged>().Where(x => players.Contains(x.Player) && x.Delta > 0).Sum(x => x.Delta),
                    "LostLife" => events.OfType<LifeChanged>().Where(x => players.Contains(x.Player) && x.Delta < 0).Sum(x => -x.Delta),
                    "WasDealtDamage" or "WasDealtNoncombatDamage" or "WasDealtCombatDamage" => events.OfType<DamageDealt>().Count(x =>
                        x.Target.IsPlayer && players.Contains(x.Target.Player.Value) && (action.Action == "WasDealtDamage" || x.Combat == (action.Action == "WasDealtCombatDamage"))),
                    "AttackedWith" => events.OfType<AttackersDeclared>().Where(x => players.Contains(x.Player)).Sum(x => x.Attackers.Count(id => Is(id, Zone.Battlefield))),
                    _ => 0,
                };
                var holds = action.Count is { } wanted ? Holds(count, wanted, ctx) : count > 0;
                return action.Action == "DidntCast" ? !holds : holds;
            }

            case ObjectEvent objectEvent:
            {
                // Who it's about: the object a filter is testing ("target creature that was dealt damage this turn"), the source, or a selector's matches.
                Func<ObjectId, Zone, bool> about = objectEvent.Subject switch
                {
                    null => (id, _) => ctx.Subject is { IsObject: true } subject ? subject.Object == id : id == ctx.Source,
                    Self => (id, _) => id == ctx.Source,
                    ObjectSelector selector => (id, zone) => state.Object(id) is { } obj && Filters.Matches(state, selector.Filter with { Zone = null }, obj with { Zone = zone }, you, ctx.Source, selector.Other ? ctx.Source : null, ctx: ctx),
                    var other => (id, _) => Resolver.Objects(other, ctx).Contains(id),
                };
                var count = objectEvent.Events.Sum(name => name switch
                {
                    "Died" => events.OfType<ZoneMoved>().Count(x => x is { From: Zone.Battlefield, To: Zone.Graveyard } && state.Object(x.Object)?.Card.IsCreature == true && about(x.Object, Zone.Battlefield)
                        && (objectEvent.UnderControlOf is null || Resolver.Players(objectEvent.UnderControlOf, ctx).Contains(x.LeftController ?? state.Object(x.Object).Owner))),
                    "Entered" or "EntersTheBattlefield" => events.OfType<ZoneMoved>().Count(x => x.To == Zone.Battlefield && about(x.Object, Zone.Battlefield)
                        && (objectEvent.UnderControlOf is null || Resolver.Players(objectEvent.UnderControlOf, ctx).Contains(x.Controller))),
                    "EnteredUnderYourControl" => events.OfType<ZoneMoved>().Count(x => x.To == Zone.Battlefield && x.Controller == you && about(x.Object, Zone.Battlefield)),
                    "Attacked" => events.OfType<AttackersDeclared>().Sum(x => x.Attackers.Count(id => about(id, Zone.Battlefield))),
                    "DidntAttack" => events.OfType<AttackersDeclared>().Any(x => x.Attackers.Any(id => about(id, Zone.Battlefield))) ? 0 : 1,
                    "WasDealtDamage" => events.OfType<DamageDealt>().Count(x => x.Target.IsObject && about(x.Target.Object.Value, Zone.Battlefield)),
                    "DealtDamage" => events.OfType<DamageDealt>().Count(x => x.Source is { } dealer && about(dealer, Zone.Battlefield)),
                    "DealtDamageToYou" => events.OfType<DamageDealt>().Count(x => x.Source is { } dealer && x.Target.Player == you && about(dealer, Zone.Battlefield)),
                    _ => 0,
                });
                return objectEvent.Subject is ObjectSelector { Count: { } wanted } ? Holds(count, wanted, ctx) : count > 0;
            }

            default:
                return null;
        }
    }

    static bool Holds(int count, Quantity wanted, EffectContext ctx) =>
        wanted switch
        {
            Bound bound when Quantities.Evaluate(bound.Value, ctx) is { } threshold => Filters.Compare(count, bound.Comparison, threshold),
            Fixed { Value: 0 } => count == 0,
            Fixed exactly => count >= exactly.Value,
            _ => count > 0,
        };
}
