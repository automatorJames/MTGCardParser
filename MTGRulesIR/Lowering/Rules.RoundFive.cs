namespace MTGRulesIR.Lowering;

/// <summary>
/// The fifth round, from the opaque report once every slot was typed: three-step sequences, keyword costs with
/// actions, token copies and becoming a copy, combat restrictions' parts, fights, convoke, "each of", what was done
/// "this way", losing the game, block permissions, cast restrictions, emblems, divided damage, the greatest of a
/// characteristic, coin flips, casting as though with flash, and a dozen smaller shapes.
/// </summary>
public sealed partial class Lowerer
{
    /// <summary>A copy's original, how long the copy lasts, and the exceptions the text makes ("except it has flying") - before it becomes a token or an animation.</summary>
    sealed record CopySpec(Selector Original, Duration Duration, IReadOnlyList<Effect> Exceptions) : IrNode;

    void RegisterRoundFive()
    {
        // Sequences and wrappers.
        Rule<G.ThreeStepSequence>((g, ctx) => Seq(LowerAs<Effect>(g.First, ctx), LowerAs<Effect>(g.Second, ctx), LowerAs<Effect>(g.Next, ctx)));
        Rule<G.InsteadThenEffect>((g, ctx) => new Instead(LowerAs<Effect>(g.Effect, ctx)));
        Rule<G.EachOf>((g, ctx) => LowerAs<Selector>(g.Group, ctx) is ObjectSelector objects ? objects with { Quantifier = Quantifier.Each } : LowerAs<Selector>(g.Group, ctx));

        // Keywords with costs.
        Rule<G.KeywordWithActionCost>((g, ctx) => new KeywordInstance(g.Keyword.ToString(), Cost: CostsOf(g.Costs, ctx)));
        Rule<G.CostEither>((g, ctx) => new CostChoice([CostOf(Lower(g.First, ctx), g.First, ctx), CostOf(Lower(g.Second, ctx), g.Second, ctx)]));
        Rule<G.PayLifeEqualTo>((g, ctx) => new PayLife(LowerAs<Quantity>(g.Measure, ctx)));
        Rule<G.CumulativeUpkeep>((g, ctx) =>
            new KeywordInstance(nameof(G.Keyword.CumulativeUpkeep), Cost: g.OrMana is null
                ? CostOf(Lower(g.Cost, ctx), g.Cost, ctx)
                : new CostChoice([CostOf(Lower(g.Cost, ctx), g.Cost, ctx), new PayMana(g.OrMana.Mana.Items.Select(SymbolText).ToList())])));

        // Copies.
        Rule<G.CopiedWithExceptions>((g, ctx) =>
            new CopySpec(LowerAs<Selector>(g.Original, ctx), DurationOf(Lower(g.Duration, ctx)), [EffectOf(Lower(g.Change, ctx) ?? new OpaqueNode(ctx.TextOf(g.Change), g.Change.GetResolvedType()?.Name))]));
        Rule<G.TokenCopy>((g, ctx) =>
        {
            var copy = CopyOf(g.Original, ctx);
            return new TokenSpecWithCount(
                new TokenCopy(copy.Original, g.Tapped is G.EntryState.Tapped or G.EntryState.TappedAndAttacking, Lower(g.Stats, ctx) as StatsMod, copy.Exceptions),
                QuantityOf(g.Quantity));
        });
        Rule<G.BecomesCopy>((g, ctx) =>
        {
            var copy = CopyOf(g.Original, ctx);
            return new BecomeCopy(ctx.Subject ?? new Self(), copy.Original, g.Verb.ToString(), DurationOf(Lower(g.Duration, ctx)) ?? copy.Duration, copy.Exceptions);
        });

        // Combat.
        Rule<G.Fights>((g, ctx) =>
            g.Opponent.GetResolvedValue() is G.EachOther
                ? new Fight(ctx.Subject ?? new Self(), EachOther: true)
                : new Fight(ctx.Subject ?? new Self(), LowerAs<Selector>(g.Opponent, ctx)));
        Rule<G.CombatantsBy>((g, ctx) =>
            new ObjectSelector(Quantifier.All, FilterOf(ctx, kinds: g.Kind, qualifiers: g.Qualifiers, narrowing: [g.With]), Count: g.Count is null ? null : LowerAs<Quantity>(g.Count, ctx)));
        Rule<G.BlockedKinds>((g, ctx) => new ObjectSelector(Quantifier.All, FilterOf(ctx, kinds: g.Kind, qualifiers: g.Qualifiers, narrowing: [g.With])));
        Rule<G.BlockedObject>((g, ctx) => LowerAs<Selector>(g.Object, ctx));
        Rule<G.AndOrKind>((g, ctx) => new ObjectSelector(Quantifier.All, FilterOf(ctx, kinds: g.Kind, qualifiers: g.Qualifiers, narrowing: [g.Narrowed])));
        Rule<G.OrCombatAction>((g, ctx) => new Term(g.Action.ToString(), nameof(G.CombatAction)));
        Rule<G.BlockPermission>((g, ctx) =>
            new CanBlock(
                ctx.Subject ?? new Self(),
                g.Scope.GetResolvedValue() is G.OnlyCreaturesWith only ? new ObjectSelector(Quantifier.All, FilterOf(ctx, narrowing: [only.With]) with { Kinds = [Kind(G.CardType.Creature)] }) : null,
                g.Scope.GetResolvedValue() is G.BlockCount count ? count.ToString() : null,
                g.Period?.ToString()));
        Rule<G.ConvokePayment>((g, ctx) => new Convoke(LowerAs<Selector>(g.Spell, ctx)));
        Rule<G.DealDividedDamage>((g, ctx) =>
            new DealDamage(ctx.Subject ?? new Self(), [new DamagePortion(LowerAs<Quantity>(g.Amount, ctx), LowerAs<Selector>(g.Recipients, ctx))], Divided: g.Division.ToString()));
        Rule<G.NextTimeWouldDeal>((g, ctx) =>
        {
            var source = LowerAs<Selector>(g.Source, ctx);
            var to = g.Protected is null ? null : LowerAs<Selector>(g.Protected.Who, ctx);
            return new Replacement(
                new WouldHappen(source, new DealDamage(source, [new DamagePortion(null, to)], Kind: g.Kind?.ToString())),
                LowerAs<Effect>(g.Replacement, ctx),
                new ThisTurn(ctx.TextOf(g)?.Contains("this combat") == true ? "Combat" : "Turn"),
                NextTime: true);
        });
        Rule<G.PreventThatDamage>((g, ctx) =>
            new PreventDamage(g.Portion is null ? new Special("ThatMuch")
                : g.Portion.AllBut ? new Adjusted(new Special("ThatMuch"), Adjustment: QuantityOf(g.Portion.Amount), Minus: true)
                : QuantityOf(g.Portion.Amount)));

        // Selectors and predicates.
        Rule<G.ActedOnThisWay>((g, ctx) => new WasAffectedThisWay(LowerAs<Selector>(g.Object, ctx), g.Action.ToString()));
        Rule<G.EachControlledPermanent>((g, ctx) =>
            new ObjectSelector(Quantifier.Each, FilterOf(ctx, kinds: g.Kind, qualifiers: g.Qualifiers, controller: g.Controller, narrowing: [g.Narrowed, g.Relation])));
        Rule<G.ThatDidThisTurn>((g, ctx) =>
            new Satisfying(new Happened(
                new ObjectEvent(null, [g.Event.ToString()],
                    UnderControlOf: g.Detail?.GetResolvedValue() is G.UnderYourControl ? new PlayerSelector(PlayerKind.You) : null,
                    Other: g.Detail?.GetResolvedValue() is G.CombatPartner partner ? LowerAs<Selector>(partner.Partner, ctx) : null),
                nameof(G.TurnSpan.ThisTurn))));
        Rule<G.PlayableCards>((g, ctx) =>
            new ObjectSelector(Quantifier.All, FilterOf(ctx, qualifiers: g.Kinds, narrowing: [g.With]) with { Kinds = [Kind(ctx.TextOf(g)?.Contains("card") == true ? "card" : "spell")] }));
        Rule<G.CardsExiledWithThis>((g, ctx) =>
            new ObjectSelector(
                g.Quantifier switch
                {
                    G.ExiledCardsQuantifier.A => Quantifier.Any,
                    G.ExiledCardsQuantifier.Each or G.ExiledCardsQuantifier.EachOther => Quantifier.Each,
                    _ => Quantifier.All,
                },
                FilterOf(ctx, qualifiers: g.Cards.GetResolvedValue() is G.QualifiedCards qualified ? qualified.Qualifiers : null, narrowing: [new In(Relation.ExiledWith, g.Source)])
                    with { Kinds = [Kind("card")], Controller = g.YouOwn == true ? new Controlled(new PlayerSelector(PlayerKind.You), "Own") : null },
                Other: g.Quantifier is G.ExiledCardsQuantifier.AllOther or G.ExiledCardsQuantifier.EachOther));
        Rule<G.DistinctKinds>((g, ctx) =>
            new DistinctCount(g.Aspect.ToString(), g.Among.GetResolvedValue() is G.ManaSpentToCast
                ? new ObjectSelector(Quantifier.All, new ObjectFilter(Kinds: [Kind(ObjectKind.ManaSpent)]))
                : LowerAs<Selector>(g.Among, ctx)));
        Rule<G.GreatestAmong>((g, ctx) => new Extreme(g.Characteristic.ToString(), LowerAs<Selector>(g.Group, ctx), Other: ctx.TextOf(g)?.Contains("among other") == true));
        Rule<G.ThatDamage>((g, ctx) => AdjustedOf(new Special("ThatMuch"), g.Factor, g.Adjustment, ctx));
        Rule<G.CharacteristicBoundCondition>((g, ctx) =>
        {
            var (comparison, amount) = g.Bound.GetResolvedValue() switch
            {
                G.NumericBound numeric => (numeric.Comparison == G.Comparison.Less ? Comparison.LessOrEqual : Comparison.GreaterOrEqual, QuantityOf(numeric.Quantity)),
                G.BoundByAmount by => (by.Comparison == G.Comparison.Less ? Comparison.LessOrEqual : Comparison.GreaterOrEqual, LowerAs<Quantity>(by.Amount, ctx)),
                _ => (Comparison.Equal, (Quantity)null),
            };
            return new CompareCount(new CharacteristicOf(LowerAs<Selector>(g.Object, ctx), g.Characteristic.ToString()), comparison, amount);
        });

        // Effects.
        Rule<G.ShuffleCardsIntoLibrary>((g, ctx) => new ShuffleInto(LowerAs<Selector>(g.Shuffled, ctx), Lower(g.Library, ctx) as ZoneRef ?? new ZoneRef("Library")));
        Rule<G.TokenHasAbility>((g, ctx) =>
        {
            Selector token = g.Token.GetResolvedValue() switch
            {
                G.NamedTokenIsArtifact named => new ObjectSelector(Quantifier.Those, new ObjectFilter(Kinds: [Kind(named.Kind)])),
                _ => new Reference(null, "it"),
            };
            List<Term> aspects = [new Term(nameof(G.CardType.Artifact), nameof(G.CardType))];
            if (g.Token.GetResolvedValue() is G.ColoredTokenIsArtifact colored)
                aspects.AddRange(TermsOf(colored.Colors, ctx));
            return Seq(new BecomesAspect(token, aspects, Verb: nameof(G.Assertion.Is)), new GrantAbility(token, AsAbility(Lower(g.Ability, ctx), ctx, g.Ability)));
        });
        Rule<G.LoseTheGame>((g, ctx) => new GameOutcome(ctx.Subject ?? Actor(ctx), g.Result.ToString(), g.Negation is not null, g.ThisTurn, g.ForZeroLife));
        Rule<G.CantCast>((g, ctx) =>
        {
            var what = g.What.GetResolvedValue();
            return new CantPlay(
                ctx.Subject ?? Actor(ctx),
                g.Verb.ToString(),
                what is G.MoreThanOneSpell more ? new ObjectSelector(Quantifier.All, FilterOf(ctx, qualifiers: more.Kind) with { Kinds = [Kind("spell")] }) : LowerAs<Selector>(g.What, ctx),
                what is G.MoreThanOneSpell ? new Fixed(1) : null,
                g.OrActivate is null ? null : LowerAs<Selector>(g.OrActivate.Abilities, ctx),
                g.Period is null ? null : DurationOf(Lower(g.Period, ctx)));
        });
        Rule<G.GetEmblem>((g, ctx) => new CreateEmblem(ctx.Subject ?? Actor(ctx), AsAbility(Lower(g.Ability, ctx), ctx, g.Ability)));
        Rule<G.CardZoneTrigger>((g, ctx) =>
        {
            List<IrNode> rest = [];
            var events = Terms(g.Event, ctx, rest);
            return rest.Count > 0 || events.Count == 0
                ? Opaque(typeof(Ability), ctx.TextOf(g), nameof(G.CardZoneTrigger))
                : new TriggeredAbility(new ObjectEvent(LowerAs<Selector>(g.Cards, ctx), events, Only: g.During?.Turn.ToString()), LowerAs<Effect>(g.Effect, ctx));
        });
        Rule<G.ChooseOfThose>((g, ctx) =>
        {
            var chosen = new ObjectSelector(Quantifier.Count, new ObjectFilter(With: [new Related(Relation.Among, new Reference(null, "those"))]), Count: QuantityOf(g.Count));
            var choose = new ChooseEffect(Actor(ctx), From: [chosen]);
            return g.AndSacrifices == true
                ? Seq(choose, new Sacrifice(Actor(ctx), new ObjectSelector(Quantifier.Those, new ObjectFilter(Qualifiers: [Q(Provenance.Chosen)]))))
                : choose;
        });
        Rule<G.ShuffleIfSearched>((g, ctx) =>
            new Conditional(
                new Happened(new PlayerAction(new PlayerSelector(PlayerKind.You), nameof(G.PastPlayerAction.SearchedLibrary)), nameof(EventSpan.ThisWay)),
                new Shuffle(new PlayerSelector(PlayerKind.You))));
        Rule<G.FlipACoin>((g, ctx) => new FlipCoin(Actor(ctx)));
        Rule<G.CastAsThoughFlash>((g, ctx) =>
            new CastAsThoughFlash(
                LowerAs<Selector>(g.Spells, ctx),
                g.Period is null ? null : DurationOf(Lower(g.Period, ctx)),
                g.ExtraCost is null ? null : new PayMana(g.ExtraCost.Cost.Items.Select(SymbolText).ToList())));
        Rule<G.ThenThatPlayerShuffles>((g, ctx) => new Shuffle(new PlayerSelector(PlayerKind.ThatPlayer)));
        Rule<G.MayChooseNotToUntap>((g, ctx) =>
            new MayNotUntap(
                LowerAs<Selector>(g.Permanent, ctx),
                PlayersPhaseOf(g.Step, ctx)));
    }

    /// <summary>One cost, or several written together, as a cost.</summary>
    Cost CostsOf(object costs, LoweringContext ctx)
    {
        var parts = LowerAll(costs, ctx).Select(x => CostOf(x, costs, ctx)).ToList();
        return parts.Count == 1 ? parts[0] : new CompoundCost(parts);
    }

    /// <summary>What a copy copies: a spec when the text says what the copy differs in, else the original alone.</summary>
    CopySpec CopyOf(object original, LoweringContext ctx) =>
        Lower(original, ctx) switch
        {
            CopySpec spec => spec,
            Selector selector => new CopySpec(selector, null, null),
            var other => new CopySpec(Coerce<Selector>(other, original, ctx), null, null),
        };

    /// <summary>An amount scaled and shifted: "twice that damage", "that much damage minus 1"; the base alone when neither applies.</summary>
    static Quantity AdjustedOf(Quantity amount, Enum factor, G.AmountAdjustment adjustment, LoweringContext ctx) =>
        factor is null && adjustment is null
            ? amount
            : new Adjusted(amount, factor?.ToString(), adjustment is null ? null : QuantityOf(adjustment.Amount), adjustment is not null && ctx.TextOf(adjustment)?.TrimStart().StartsWith("minus", StringComparison.OrdinalIgnoreCase) == true);
}
