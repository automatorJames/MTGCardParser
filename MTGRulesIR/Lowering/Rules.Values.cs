namespace MTGRulesIR.Lowering;

/// <summary>Quantities, durations, phases, conditions, costs and keywords: the values the frames and effects take.</summary>
public sealed partial class Lowerer
{
    void RegisterValues()
    {
        Rule<G.Quantity>((quantity, ctx) => QuantityOf(quantity));

        Rule<G.EffectDuration>((g, ctx) => DurationOf(Lower(g.Span, ctx)));

        Rule<G.UntilPhase>((g, ctx) =>
            new UntilPhase(new PhaseRef(g.Phase.ToString(), Whose: g.Next is null ? null : NextOf(g.Next.Value), Next: g.Next is not null)));

        Rule<G.ThisPeriod>((g, ctx) => new ThisTurn(g.Period.ToString()));

        Rule<G.WhileState>((g, ctx) => new While(LowerAs<Condition>(g.State, ctx)));

        Rule<G.ForAsLongAs>((g, ctx) =>
        {
            var conditions = LowerAllAs<Condition>(g.Conditions, ctx);
            return new While(conditions.Count == 1 ? conditions[0] : new And(conditions));
        });

        Rule<G.ControlCondition>((g, ctx) =>
            new Controls(LowerAs<Selector>(g.Controller, ctx), LowerAs<Selector>(g.Permanent, ctx), Connective: g.Connective.ToString()));

        Rule<G.UntilThisLeaves>((g, ctx) => new UntilThisLeaves());

        Rule<G.AtOrUntilPlayerPhase>((g, ctx) =>
        {
            var phase = (PhaseRefOf(g.Step, ctx) ?? new PhaseRef(ctx.TextOf(g.Step))) with { Part = g.PhasePart.ToString() };
            return g.TemporalDisposition == G.TemporalDisposition.Until ? new UntilPhase(phase) : phase;
        });

        Rule<G.PlayersPhase>((g, ctx) =>
        {
            var whose = g.Whose?.GetResolvedValue();
            return new PhaseRef(g.Phase.ToString(), Whose: whose is null ? null : LowerAs<Selector>(whose, ctx), Next: whose is G.Whose possessive && possessive.ToString().Contains("Next"));
        });

        Rule<G.PhaseOnPlayersTurn>((g, ctx) => new PhaseRef(g.Phase.ToString(), Whose: LowerAs<Selector>(g.Whose, ctx)));
        Rule<G.PhaseOfPlayer>((g, ctx) => new PhaseRef(g.Phase.ToString(), Whose: LowerAs<Selector>(g.Player, ctx)));
        Rule<G.AtEndOfCombat>((g, ctx) => new PhaseRef("EndOfCombatStep", "End"));

        Rule<G.CardCountCondition>((g, ctx) =>
        {
            var cards = LowerAs<Selector>(g.Cards, ctx);

            return g.Count?.GetResolvedValue() switch
            {
                null => new CountCondition(cards, ">=", new Fixed(1)),
                G.AtLeastCount atLeast => new CountCondition(cards, ">=", LowerAs<Quantity>(atLeast.Count, ctx)),
                G.NoneCount => new CountCondition(cards, "==", new Fixed(0)),
                G.FewerThanCount fewer => new CountCondition(cards, "<", LowerAs<Quantity>(Property(fewer, "Count"), ctx)),
                var other => (Condition)Opaque(typeof(Condition), ctx.TextOf(g), other.GetType().Name),
            };
        });

        Rule<G.ControlsPermanent>((g, ctx) =>
            new Controls(LowerAs<Selector>(g.Controller, ctx), LowerAs<Selector>(g.Permanent, ctx), g.DoesNot, Tense: g.Tense.ToString()));

        Rule<G.Outcome>((g, ctx) =>
            new OutcomeCondition(LowerAs<Selector>(g.Player, ctx), Lower(g.Result, ctx) is Term term ? term.Value : ctx.TextOf(g.Result)));

        Rule<G.IsOnBattlefield>((g, ctx) => new SelfOnBattlefield());

        Rule<G.BothStates>((g, ctx) => new And([LowerAs<Condition>(g.First, ctx), LowerAs<Condition>(g.Second, ctx)]));

        Rule<G.CardIsKind>((g, ctx) =>
        {
            List<IrNode> qualified = [];
            var kinds = Terms(g.Kind, ctx, qualified);
            qualified.AddRange(LowerAll(g.Chosen, ctx));
            return new IsKind(LowerAs<Selector>(g.Card, ctx), kinds, g.Card.GetResolvedValue() is G.ItIsnt, OrNull(qualified));
        });

        Rule<G.Cost>((g, ctx) =>
        {
            var value = g.GetResolvedValue();

            switch (value)
            {
                case G.ManaCost mana:
                    return new PayMana(mana.Items.Select(SymbolText).ToList());
                case G.TapSymbol symbol:
                    return symbol == G.TapSymbol.Tap ? new TapSelf() : new UntapSelf();
                case G.LifeQuantity life:
                    return new PayLife(QuantityOf(life.Quantity));
            }

            return Lower(value, ctx) switch
            {
                Effect effect => new ActionCost(effect),
                Cost cost => cost,
                Quantity amount when value is G.LifeEqualTo => new PayLife(amount),
                IOpaque opaque => new OpaqueCost(opaque.Text, opaque.Glyph),
                null => null,
                var other => new OpaqueCost(ctx.TextOf(value), $"{value.GetType().Name}→{other.GetType().Name}"),
            };
        });

        Rule<G.TapSymbol>((symbol, ctx) => symbol == G.TapSymbol.Tap ? new TapSelf() : new UntapSelf());

        Rule<G.KeywordWithAmount>((g, ctx) => new KeywordInstance(g.Keyword.ToString(), QuantityOf(g.Amount)));

        Rule<G.KeywordWithCost>((g, ctx) =>
            new KeywordInstance(g.Keyword.ToString(), Cost: new PayMana(g.Cost.Items.Select(SymbolText).ToList()), Parameter: g.AndOr is null ? null : ctx.TextOf(g.AndOr)));

        Rule<G.Protection>((g, ctx) =>
            new KeywordInstance(g.Kind.ToString(), Parameter: string.Join(" ", new object[] { g.From, g.AndFrom }.Where(x => x is not null).Select(ctx.TextOf))));

        Rule<G.CombatEncounter>((g, ctx) =>
            new CombatEncounterEvent(null, g.Encounter.ToString(), LowerAs<Selector>(g.Other, ctx)));
    }

    static Selector NextOf(G.NextPhaseOf next) =>
        next switch
        {
            G.NextPhaseOf.YourNext => new PlayerSelector(PlayerKind.You),
            G.NextPhaseOf.ItsControllersNext => new PlayerSelector(PlayerKind.ControllerOf, new Reference(null, "its")),
            _ => new PlayerSelector(PlayerKind.ThatPlayer),
        };
}
