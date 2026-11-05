namespace MTGRulesIR.Lowering;

/// <summary>
/// The eleventh round: unleash's blocking rule, "that player or that planeswalker's controller", plural cards, riders
/// on what an effect affected ("creatures destroyed this way can't be regenerated"), losing all abilities and
/// becoming something, removing counters and sacrificing as one cost, a card being in a zone, sharing a trait,
/// several kinds of spells cast, unattaching, repeated actions, counted cards, each card in a zone, several mana
/// symbols spent, "it's a … in addition to its other types", several counter placements, totals of a
/// characteristic, and players who would gain or lose life. "X can't be 0" and overload's "change target to each"
/// lower to nothing: the engine doesn't need them.
/// </summary>
public sealed partial class Lowerer
{
    void RegisterRoundEleven()
    {
        // Text the engine doesn't need.
        Rule<G.XCantBeZero>((g, ctx) => null);
        Rule<G.ChangeTargetToEach>((g, ctx) => null);

        Rule<G.UnleashCantBlock>((g, ctx) =>
            new CombatRestriction(new Self(), ["Block"], If: new HasCountersCondition(new Self(), new CounterSpec("+1/+1", null))));

        // Selectors.
        Rule<G.ThatPlayerOrPlaneswalkersController>((g, ctx) =>
            new Union([new PlayerSelector(PlayerKind.ThatPlayer), new PlayerSelector(PlayerKind.ControllerOf, new Reference(null, "that planeswalker"))], Connective.Or));
        Rule<G.PluralCards>((g, ctx) => new ObjectSelector(Quantifier.All, new ObjectFilter(Kinds: [Kind("card")])));
        Rule<G.CountedCards>((g, ctx) =>
            new ObjectSelector(Quantifier.Count, FilterOf(ctx, qualifiers: g.Qualifiers) with { Kinds = [Kind("card")] }, Count: LowerAs<Quantity>(g.Count, ctx)));
        Rule<G.EachCardInZone>((g, ctx) => LowerAs<Selector>(g.Cards, ctx) is ObjectSelector cards ? cards with { Quantifier = Quantifier.Each } : LowerAs<Selector>(g.Cards, ctx));
        Rule<G.SpellKindsCast>((g, ctx) =>
        {
            var caster = g.Caster is null ? null : Lower(g.Caster, ctx) as Predicate;
            List<Selector> kinds = [];
            foreach (var kind in g.Kinds.Items)
            {
                var filter = FilterOf(ctx, qualifiers: kind.Qualifiers) with { Kinds = [Kind("spell")] };
                kinds.Add(new ObjectSelector(Quantifier.All, caster is null ? filter : filter with { With = [.. filter.With ?? [], caster] }));
            }
            return kinds.Count == 1 ? kinds[0] : new Union(kinds, Connective.Or);
        });

        // Effects.
        Rule<G.ThisWayRider>((g, ctx) => WithSubject(ctx, g.Affected, () => Lower(g.Predicate, ctx)));
        Rule<G.LosesAbilitiesAndBecomes>((g, ctx) =>
            new Sequence([new LoseAllAbilities(ctx.Subject ?? new Self()), LowerAs<Effect>(g.Becomes, ctx)]));
        Rule<G.Unattach>((g, ctx) => new Attach(LowerAs<Selector>(g.Attachment, ctx), null));
        Rule<G.RepeatedAction>((g, ctx) =>
            new ForEachOf(LowerAs<Effect>(g.Action, ctx), g.Times switch
            {
                G.CopyRepetition.Twice => new Fixed(2),
                G.CopyRepetition.ThreeTimes => new Fixed(3),
                G.CopyRepetition.XTimes => new Variable("X"),
                _ => new Fixed(1),
            }));
        Rule<G.PlaceSeveralCounters>((g, ctx) =>
        {
            List<Effect> steps = [];
            foreach (var placement in g.Placements.Items)
            {
                if (Lower(placement.Counters, ctx) is not CounterSpec counters)
                    return Opaque(typeof(Effect), ctx.TextOf(g), nameof(G.PlaceSeveralCounters));
                var objects = LowerAs<Selector>(placement.Place.Permanent, ctx);
                steps.Add(g.Action == G.CounterAction.Remove ? new RemoveCounters(objects, counters) : new PutCounters(objects, counters));
            }
            return new Sequence(steps);
        });
        Rule<G.ItIsOfType>((g, ctx) =>
        {
            var aspects = Lower(g.Kind, ctx) switch
            {
                Term term => [term],
                QualifiedKind qualified => [qualified.Kind, .. qualified.Qualifiers.Select(q => new Term(q.Quality, q.Vocabulary))],
                ObjectSelector { Filter.Kinds: { } kinds } => kinds.ToList(),
                _ => null,
            };
            if (aspects is null)
                return Opaque(typeof(Effect), ctx.TextOf(g), nameof(G.ItIsOfType));
            return new BecomesAspect(ctx.Subject ?? new Reference(null, "it"), aspects, InAddition: true, Duration: DurationOf(Lower(g.Duration, ctx)));
        });

        // Costs: "remove three study counters from this and sacrifice it".
        Rule<G.CountersAndSacrificeCost>((g, ctx) => new Sequence([LowerAs<Effect>(g.Remove, ctx), LowerAs<Effect>(g.SacrificeOrReturn, ctx)]));

        // Conditions.
        Rule<G.CardInZoneCondition>((g, ctx) =>
            new ThereAre(new ObjectSelector(Quantifier.Any, FilterOf(ctx, qualifiers: g.Cards.Qualifiers, zone: g.Zone) with { Kinds = [Kind("card")] })));
        Rule<G.SharesTraitCondition>((g, ctx) =>
            new ThereAre(new ObjectSelector(Quantifier.Any, new ObjectFilter(With:
            [
                new Related(Relation.Is, LowerAs<Selector>(g.Object, ctx)),
                new SharesCharacteristic(g.Trait.ToString(), LowerAs<Selector>(g.Other, ctx)),
            ]))));

        // Events: "if you would gain life".
        Rule<G.WouldGainOrLoseLife>((g, ctx) =>
        {
            var player = LowerAs<Selector>(g.Player, ctx);
            return new WouldHappen(player, g.GainOrLose == G.GainOrLose.Gain ? new GainLife(player, new Special("ThatMuch")) : new LoseLife(player, new Special("ThatMuch")));
        });

        // Quantities: "the total power of creatures you control".
        Rule<G.TotalOf>((g, ctx) => new TotalOf(LowerAs<Selector>(g.Group, ctx), g.Characteristic.ToString()));
    }
}
