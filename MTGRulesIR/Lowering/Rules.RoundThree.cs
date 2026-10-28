namespace MTGRulesIR.Lowering;

/// <summary>The third round of rules, from the opaque report after the second: choices, playing cards, type and color changes, control, copies, replacements, forced combat, and more conditions and selectors.</summary>
public sealed partial class Lowerer
{
    void RegisterRoundThree()
    {
        // Glue and frames.
        Rule<G.ActionAndSubjectDoes>((g, ctx) => Seq(LowerAs<Effect>(g.First, ctx), LowerAs<Effect>(g.Second, ctx)));
        Rule<G.EffectForEach>((g, ctx) => WithSubject(ctx, g.Subject, () => new ForEachOf(LowerAs<Effect>(g.Effect, ctx), LowerAs<Quantity>(g.PerEach, ctx))));
        Rule<G.ForEachLeading>((g, ctx) => new ForEachOf(LowerAs<Effect>(g.Effect, ctx), LowerAs<Quantity>(g.Multiplier, ctx)));
        Rule<G.Otherwise>((g, ctx) => WithSubject(ctx, g.Subject, () => new Otherwise(LowerAs<Effect>(g.Predicate, ctx))));
        Rule<G.PredicateInstead>((g, ctx) => new Instead(LowerAs<Effect>(g.Predicate, ctx)));
        Rule<G.EffectThenInstead>((g, ctx) => new Instead(LowerAs<Effect>(g.Effect, ctx)));
        Rule<G.AsLongAs>((g, ctx) =>
            new StaticAbility(
                LowerAs<Effect>(g.Effect, ctx),
                new ObjectIs(LowerAs<Selector>(g.Subject, ctx), [Lower(g.Aspect, ctx)], g.Assertion is G.Assertion.Isnt or G.Assertion.Wasnt, g.Assertion is G.Assertion.Was or G.Assertion.Wasnt ? "Was" : "Is")));
        Rule<G.AlternativeCost>((g, ctx) =>
            new AlternativeCost(LowerAs<Cost>(g.Cost, ctx), Lower(g.Replaced, ctx), g.AdditionalCost is null ? null : CostOf(Lower(g.AdditionalCost, ctx), g.AdditionalCost, ctx), g.Provided is null ? null : LowerAs<Condition>(g.Provided, ctx)));
        Rule<G.BecomesTargetTrigger>((g, ctx) =>
            new TriggeredAbility(
                new ObjectEvent(LowerAs<Selector>(g.Subject, ctx), ["BecomesTarget"], Only: g.FirstTime ? "FirstTimeEachTurn" : null, Details: OrNull(LowerAll(new object[] { g.Source, g.Controller }, ctx))),
                LowerAs<Effect>(g.Effect, ctx)));
        Rule<G.DieReplacement>((g, ctx) =>
        {
            var dying = LowerAs<Selector>(g.Dying, ctx);
            return new Replacement(new ObjectEvent(dying, [g.Event.ToString()]), new Exile(dying, Manner: g.Anywhere ? "InsteadOfAnywhereElse" : null), g.Period is null ? null : new ThisTurn(g.Period.Period.ToString()));
        });
        Rule<G.WouldHappen>((g, ctx) =>
        {
            var subject = LowerAs<Selector>(g.Subject, ctx);
            var previous = ctx.Subject;
            ctx.Subject = subject;
            try
            {
                return new WouldHappen(subject, LowerAs<Effect>(g.Predicate, ctx));
            }
            finally
            {
                ctx.Subject = previous;
            }
        });
        Rule<G.LevelUpRestriction>((g, ctx) => new ActivationLimits([new Timing("SorcerySpeed", Detail: new Term(g.Keyword.ToString(), nameof(G.Keyword)))]));
        Rule<G.SagaSacrificeAfter>((g, ctx) => null);
        Rule<G.ScryOneLook>((g, ctx) => new LookAt(new PlayerSelector(PlayerKind.You), TopOfLibrary(new Fixed(1), new PlayerSelector(PlayerKind.You))));
        Rule<G.QuotedAbility>((g, ctx) => AsAbility(Lower(g.Ability, ctx), ctx, g.Ability));

        // Effects.
        Rule<G.Choose>((g, ctx) => new ChooseEffect(Actor(ctx), LowerAll(g.Choices, ctx), g.Except is null ? null : Lower(g.Except, ctx)));
        Rule<G.PlayCard>((g, ctx) =>
            new PlayCard(Actor(ctx), LowerAs<Selector>(g.Card, ctx), g.From?.ToString(), g.Free is not null, g.ByPaying is null ? null : Lower(g.ByPaying, ctx), DurationOf(Lower(g.Period, ctx))));
        Rule<G.IsOfType>((g, ctx) =>
        {
            List<IrNode> aspects = LowerAll(g.Kind, ctx);
            if (g.Legendary == true)
                aspects.Insert(0, new Term("Legendary", nameof(G.Supertype)));
            return new BecomesAspect(ctx.Subject ?? new Self(), aspects, g.InAddition, Duration: DurationOf(Lower(g.Duration, ctx)), Verb: g.Verb.ToString());
        });
        Rule<G.BecomesCharacteristic>((g, ctx) =>
            new BecomesAspect(ctx.Subject ?? new Self(), [Lower(g.Value, ctx)], g.InAddition, Duration: DurationOf(Lower(g.Duration, ctx)), Verb: g.Verb.ToString()));
        Rule<G.PayLifeOrEnergy>((g, ctx) =>
            new PayCost(g.Amount.GetResolvedValue() switch
            {
                G.LifeQuantity life => new PayLife(QuantityOf(life.Quantity)),
                G.ManaCost energy => new PayMana(energy.Items.Select(SymbolText).ToList()),
                var other => CostOf(Lower(other, ctx) is Quantity amount ? new PayLife(amount) : Lower(other, ctx), other, ctx),
            }, ctx.Subject));
        Rule<G.PerformsKeywordAction>((g, ctx) =>
            new KeywordAction([new KeywordInstance(g.Action.ToString(), g.Amount is { } amount ? QuantityOf(amount) : null)], g.Again ? [new Term("Again", nameof(G.PerformsKeywordAction))] : null));
        Rule<G.GainControl>((g, ctx) => new GainControl(Actor(ctx), LowerAs<Selector>(g.Permanent, ctx), DurationOf(Lower(g.Duration, ctx))));
        Rule<G.ManaSpendingRestriction>((g, ctx) => new ManaRestriction(g.Rule.ToString(), LowerAll(g.Uses, ctx)));
        Rule<G.CopySpell>((g, ctx) =>
            new CopySpell(LowerAs<Selector>(g.Spell, ctx), g.Times is null ? null : Lower(g.Times, ctx), g.NewTargets, g.Except is null ? null : Lower(g.Except, ctx)));
        Rule<G.DiscardHand>((g, ctx) => new Discard(Actor(ctx), new Special("All")));
        Rule<G.DiscardThisCard>((g, ctx) => new Discard(Actor(ctx), new Fixed(1), new ObjectFilter(With: [new Self()])));
        Rule<G.GetEnergy>((g, ctx) => new GetEnergy(Actor(ctx), Multiply(LowerAs<Quantity>(g.Amount, ctx), g.Multiplier is null ? null : LowerAs<Quantity>(g.Multiplier, ctx))));
        Rule<G.AttacksIfAble>((g, ctx) =>
            new MustCombat(ctx.Subject ?? new Self(), g.OrBlocks ? [g.Action.ToString(), "Block"] : [g.Action.ToString()], g.Period?.ToString()));
        Rule<G.CanAttackAsThough>((g, ctx) =>
            new CanAsThough(
                ctx.Subject ?? new Self(),
                g.Action.ToString(),
                g.Keyword.ToString(),
                ctx.TextOf(g.Possession)?.Contains("had") == true && ctx.TextOf(g.Possession)?.Contains("didn't") != true,
                OrNull(LowerAll(new object[] { g.Blocked, g.Duration }, ctx)),
                DurationOf(Lower(g.While, ctx))));
        Rule<G.CantBeRegenerated>((g, ctx) => new CantBeRegenerated(ctx.Subject ?? new Self(), g.Duration is null ? null : new ThisTurn(g.Duration.Period.ToString())));
        Rule<G.RevealCards>((g, ctx) => new Reveal(Actor(ctx), LowerAs<Selector>(g.Cards, ctx)));
        Rule<G.RevealUntil>((g, ctx) => new RevealUntil(Actor(ctx), g.Verb.ToString(), ZoneOf(g.Library, ctx), Lower(g.StopCondition, ctx)));
        Rule<G.LookAndRearrange>((g, ctx) => Seq(new LookAt(Actor(ctx), LowerAs<Selector>(g.What, ctx)), LowerAs<Effect>(g.Rearrangement, ctx)));
        Rule<G.PlayFromZone>((g, ctx) =>
            new MayPlayFrom(Actor(ctx), LowerAll(g.Plays, ctx), Lower(g.Source, ctx), g.Free, g.Using is null ? null : Lower(g.Using, ctx), DurationOf(Lower(g.Duration, ctx))));
        Rule<G.PowerToughnessEqual>((g, ctx) => new SetStats(LowerAs<Selector>(g.Object, ctx), g.PowerAndOrToughness.ToString(), LowerAs<Quantity>(g.Amount, ctx)));
        Rule<G.SearchFor>((g, ctx) =>
            new SearchLibrary(LowerAs<Selector>(g.Owner, ctx), g.Zones.ToString(), LowerAll(g.Wanted, ctx), Ending: g.FollowUp is null ? null : new Term(g.FollowUp.ToString(), "SearchFollowUp")));
        Rule<G.DrawOrDiscardCardsEqualTo>((g, ctx) =>
        {
            var count = LowerAs<Quantity>(g.Count, ctx);
            return g.CardVerb.GetResolvedValue() switch
            {
                G.CardVerb.Draw => new Draw(Actor(ctx), count),
                G.CardVerb.Discard => new Discard(Actor(ctx), count),
                _ => new Mill(Actor(ctx), count),
            };
        });
        Rule<G.ShuffleLibrary>((g, ctx) => new Shuffle(g.Library is null ? Actor(ctx) : LowerAs<Selector>(g.Library.Whose, ctx)));
        Rule<G.DestroyAll>((g, ctx) =>
            new Destroy(LowerAs<Selector>(g.Destroyed, ctx) is ObjectSelector selector ? selector with { Quantifier = Quantifier.All } : LowerAs<Selector>(g.Destroyed, ctx)));

        // Conditions.
        Rule<G.PlayerDidThisTurn>((g, ctx) =>
            new Happened(
                new PlayerAction(LowerAs<Selector>(g.Player, ctx), g.Action.ToString(), g.What is null ? null : LowerAs<Selector>(g.What, ctx), g.FromHand),
                g.Turn.ToString()));
        Rule<G.PermanentDidThisTurn>((g, ctx) =>
            new Happened(
                new ObjectEvent(LowerAs<Selector>(g.Who, ctx), [g.Event.ToString()], g.UnderYourControl ? new PlayerSelector(PlayerKind.You) : null),
                g.Turn.ToString()));
        Rule<G.CardTypesCountCondition>((g, ctx) =>
        {
            var count = new DistinctCount(Lower(g.Kinds, ctx) is Term kinds ? kinds.Value : ctx.TextOf(g.Kinds), LowerAs<Selector>(g.Among, ctx));
            return g.Count.GetResolvedValue() switch
            {
                G.AtLeastCount atLeast => new CompareCount(count, ">=", LowerAs<Quantity>(atLeast.Count, ctx)),
                G.Quantity exact => new CompareCount(count, "==", QuantityOf(exact)),
                _ => new CompareCount(count, ">=", new Fixed(1)),
            };
        });
        Rule<G.ControlsMoreThan>((g, ctx) =>
            new CompareControl(LowerAs<Selector>(g.Controller, ctx), LowerAs<Selector>(g.Permanents, ctx), g.Bound.ToString(), LowerAs<Selector>(g.Compared, ctx)));
        Rule<G.DevotionThreshold>((g, ctx) => new CompareCount(LowerAs<Quantity>(g.Devotion, ctx), "<", QuantityOf(g.Threshold)));
        Rule<G.HandSizeCondition>((g, ctx) => new HandSize(LowerAs<Selector>(g.Player, ctx), Lower(g.Size, ctx), g.Verb.ToString()));
        Rule<G.InZone>((g, ctx) => ZoneOf(g.Zone, ctx));
        Rule<G.ExiledState>((g, ctx) => new ZoneRef("Exile", Position: g.Counters is null ? g.State.ToString() : $"{g.State} {ctx.TextOf(g.Counters)}"));

        // Selectors and qualifiers.
        Rule<G.ThisObject>((g, ctx) => new Self());
        Rule<G.PlayersHand>((g, ctx) => new ObjectSelector(Quantifier.All, new ObjectFilter(Kinds: ["card"], Zone: new ZoneRef("Hand", LowerAs<Selector>(g.Owner, ctx)))));
        Rule<G.NextDamage>((g, ctx) => QuantityOf(g.Quantity));
        Rule<G.SpellCaster>((g, ctx) => new CastBy(Player(g.Player), g.Relation.ToString(), g.From?.ToString(), g.ThisTurn == true, g.During is null ? null : Lower(g.During, ctx)));
        Rule<G.WithCharacteristicValue>((g, ctx) => new CharacteristicBound([g.Characteristic.ToString()], LowerAs<Quantity>(g.Value, ctx), "Equal"));
        Rule<G.WithCountersOnIt>((g, ctx) => Lower(g.Counters, ctx) is CounterSpec spec ? new WithCounters(spec) : new WithCounters(null, Lower(g.Counters, ctx)));
        Rule<G.ThatMuchLife>((g, ctx) =>
            g.Factor is null && g.Adjustment is null ? new Special("ThatMuch") : new Adjusted(new Special("ThatMuch"), g.Factor?.ToString(), g.Adjustment is null ? null : ctx.TextOf(g.Adjustment)));
        Rule<G.CombatantsBy>((g, ctx) =>
            new ObjectSelector(Quantifier.Any, FilterOf(ctx, kinds: g.Kind, qualifiers: g.Qualifiers, narrowing: [g.With]), Count: g.Count is null ? null : LowerAs<Quantity>(g.Count, ctx)));
        Rule<G.AffectedThisWay>((g, ctx) =>
        {
            var filter = FilterOf(ctx, kinds: g.Kind, qualifiers: g.Qualifiers, narrowing: [g.Chosen, g.From]);
            return new ObjectSelector(Quantifier.Those, filter with { Qualifiers = [.. filter.Qualifiers ?? [], new Qualifier(g.Action + "ThisWay")] });
        });
        Rule<G.RevealedFromHand>((g, ctx) =>
        {
            var hand = new ZoneRef("Hand", PlayerOf(g.Whose), g.Opening ? "Opening" : null);
            var wanted = LowerAs<Selector>(g.Wanted, ctx);
            var selector = wanted is ObjectSelector objects
                ? objects with { Filter = (objects.Filter ?? new ObjectFilter()) with { Zone = hand, Qualifiers = g.AtRandom ? [.. objects.Filter?.Qualifiers ?? [], new Qualifier("AtRandom")] : objects.Filter?.Qualifiers } }
                : wanted;
            return g.Count is null || selector is not ObjectSelector counted ? selector : counted with { Count = LowerAs<Quantity>(g.Count, ctx) };
        });
        Rule<G.ThisOrAnother>((g, ctx) =>
            new Union([new Self(), new ObjectSelector(Quantifier.Any, FilterOf(ctx, kinds: g.Kind, qualifiers: g.Qualifier, controller: g.Controller, narrowing: [g.With]), Other: true)], "or"));
        Rule<G.PlayerOrPermanent>((g, ctx) => new Union([Player(g.Player), LowerAs<Selector>(g.Permanent, ctx)], "or"));
        Rule<G.PermanentOrPlayer>((g, ctx) => new ObjectSelector(Quantifier.Any, new ObjectFilter(Kinds: ["permanent", "player"])));
        Rule<G.YouAndEach>((g, ctx) => new Union([new PlayerSelector(PlayerKind.You), LowerAs<Selector>(g.Each, ctx)]));
        Rule<G.TargetAndEach>((g, ctx) => new Union([LowerAs<Selector>(g.Target, ctx), LowerAs<Selector>(g.Each, ctx)]));
        Rule<G.TargetSpellOrPermanent>((g, ctx) => new ObjectSelector(Quantifier.Target, FilterOf(ctx, narrowing: [g.With]) with { Kinds = ["spell", g.Kind.ToString()] }));
        Rule<G.ControlledByWho>((g, ctx) => new Controlled(Player(g.Controller)));
        Rule<G.TargetingObjects>((g, ctx) =>
        {
            var qualifiers = LowerAll(g.Qualifier, ctx).OfType<Qualifier>().ToList();
            return qualifiers.Count == 0 && g.From is null
                ? new Term(g.Kinds.ToString(), nameof(G.TargetingObjects))
                : new QualifiedKind(g.Kinds.ToString(), [.. qualifiers, .. g.From is null ? [] : new[] { new Qualifier("From:" + ctx.TextOf(g.From)) }]);
        });
        Rule<G.TokenWithQuotedAbility>((g, ctx) =>
            Lower(g.Token, ctx) is TokenSpecWithCount token
                ? token with { Token = token.Token with { Other = [.. token.Token.Other ?? [], Lower(g.Quoted, ctx)] } }
                : new OpaqueNode(ctx.TextOf(g), nameof(G.TokenWithQuotedAbility)));
    }
}
