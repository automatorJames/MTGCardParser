namespace MTGRulesIR.Lowering;

/// <summary>The second round of rules, taken from the top of the opaque report after the first: sentence glue, entering the battlefield, searches, prevention, reveals, animation, and the selectors and conditions they use.</summary>
public sealed partial class Lowerer
{
    void RegisterRoundTwo()
    {
        // Sentence glue.
        Rule<G.ThenSequence>((g, ctx) => Seq(LowerAs<Effect>(g.First, ctx), LowerAs<Effect>(g.Next, ctx)));
        Rule<G.PredicatePair>((g, ctx) => Seq(LowerAs<Effect>(g.First, ctx), LowerAs<Effect>(g.Second, ctx)));
        Rule<G.ImperativePair>((g, ctx) => Lower(g.Actions, ctx));
        Rule<G.BothSubjectsDo>((g, ctx) => Seq(LowerAs<Effect>(g.First, ctx), LowerAs<Effect>(g.Second, ctx)));
        Rule<G.AndThen>((g, ctx) => LowerAs<Effect>(g.Effect, ctx));
        Rule<G.FlavorWordAbility>((g, ctx) => new LabeledAbility(ctx.TextOf(g)?.Split('—')[0].Trim(), AsAbility(Lower(g.Ability, ctx), ctx, g.Ability)));
        Rule<G.WhereXIs>((g, ctx) => new LetVariable(LowerAs<Effect>(g.Effect, ctx), "X", LowerAs<Quantity>(g.Definition, ctx)));
        Rule<G.DurationFirstEffect>((g, ctx) => WithSubject(ctx, g.Subject, () => Lasting(LowerAs<Effect>(g.Predicate, ctx), DurationOf(Lower(g.Duration, ctx)))));
        Rule<G.EffectIfCondition>((g, ctx) => WithSubject(ctx, g.Subject, () => new Conditional(LowerAs<Condition>(g.Condition, ctx), LowerAs<Effect>(g.Effect, ctx))));
        Rule<G.UnlessPaid>((g, ctx) => new UnlessPaid(LowerAs<Effect>(g.Effect, ctx), Lower(g.Payment, ctx)));
        Rule<G.OptionalPayment>((g, ctx) => Lower(g.Payment, ctx) is PayCost pay ? new Optional(pay, pay.Player) : Lower(g.Payment, ctx));
        Rule<G.OptionalPayCost>((g, ctx) =>
            new PayCost(
                CostOf(Lower(g.Cost, ctx), g.Cost, ctx),
                Lower(g.PayOptionType, ctx) as Selector,
                g.PerEach is null ? null : LowerAs<Quantity>(g.PerEach, ctx),
                g.Plus is null ? null : Lower(g.Plus, ctx)));
        Rule<G.ReflexiveTrigger>((g, ctx) => new WhenDone(Player(g.Player), LowerAs<Effect>(g.Effect, ctx)));
        Rule<G.AsThisEnters>((g, ctx) => new AsEventHappens(LowerAs<Event>(g.Moment, ctx), LowerAs<Effect>(g.Effect, ctx)));
        Rule<G.CastingThisSpell>((g, ctx) => new ObjectEvent(new Self(), ["Cast"]));
        Rule<G.PermanentAsEvent>((g, ctx) => new ObjectEvent(LowerAs<Selector>(g.Permanent, ctx), [g.Event.ToString()]));
        Rule<G.EntersOrTransformsInto>((g, ctx) => new Term("EntersOrTransforms", nameof(G.PermanentEvent)));
        Rule<G.PlayerEvent>((g, ctx) =>
            new PlayerEvent(
                Lower(g.Player, ctx) is Selector player ? player : new PlayerSelector(g.Player.GetResolvedValue()?.ToString()?.Contains("Opponent") == true ? PlayerKind.AnyOpponent : PlayerKind.AnyPlayer),
                g.Event.ToString(),
                g.Timing?.ToString()));
        Rule<G.AdditionalCastingCost>((g, ctx) => new AdditionalCost(LowerAs<Selector>(g.Spells, ctx), CostOf(Lower(g.Cost, ctx), g.Cost, ctx)));
        Rule<G.TurnFaceUp>((g, ctx) => null);

        // Entering the battlefield.
        Rule<G.EntersTheBattlefield>((g, ctx) =>
        {
            List<string> states = [];
            if (g.Way != G.EnteringWay.Enters)
                states.Add(g.Way.ToString());
            if (g.State is { } state)
                states.Add(state.ToString());
            if (g.Prepared)
                states.Add("Prepared");

            List<IrNode> with = [.. LowerAll(g.With, ctx), .. LowerAll(g.AlsoWith, ctx)];
            if (g.Duration is not null)
                with.Add(new ThisTurn(g.Duration.Period.ToString()));

            return new EntersBattlefield(ctx.Subject ?? new Self(), OrNull(states), OrNull(with), g.Condition is null ? null : LowerAs<Condition>(g.Condition, ctx));
        });
        Rule<G.AndEntersWith>((g, ctx) => Lower(g.With, ctx));
        Rule<G.EntersWithAbility>((g, ctx) =>
        {
            List<IrNode> other = [];
            var keywords = KeywordsOf(g.Ability, ctx, other);
            return other.Count == 0 ? new Pump(null, Keywords: keywords)
                : other.Count == 1 && keywords.Count == 0 ? new GrantAbility(null, AsAbility(other[0], ctx, g.Ability))
                : new Pump(null, Keywords: OrNull(keywords), OtherBuffs: other);
        });
        Rule<G.RiotChoice>((g, ctx) =>
            new ChoiceOf([
                Lower(g.Counter, ctx) is CounterSpec spec ? new PutCounters(null, spec) : (Effect)Opaque(typeof(Effect), ctx.TextOf(g.Counter), nameof(G.Counters)),
                new Pump(null, Keywords: [new KeywordInstance(g.Keyword.ToString())])]));

        // Moving cards.
        Rule<G.ReturnToBattlefield>((g, ctx) =>
        {
            var move = ToBattlefield(LowerAs<Selector>(g.Returned, ctx), g.Destination, ctx);
            return g.AndThen is null ? move : Seq(move, LowerAs<Effect>(g.AndThen, ctx));
        });
        Rule<G.SearchLibrary>((g, ctx) =>
            new SearchLibrary(
                g.Searcher is null ? Actor(ctx) : LowerAs<Selector>(g.Searcher.Player, ctx),
                g.Zones.ToString(),
                LowerAll(g.Wanted, ctx),
                g.Revealed,
                Lower(g.Placement, ctx),
                Lower(g.Ending, ctx),
                g.Searcher?.May == true));
        Rule<G.WantedCards>((g, ctx) =>
            new ObjectSelector(
                Quantifier.Count,
                FilterOf(ctx, kinds: g.Kind, qualifiers: g.Qualifiers, narrowing: [g.OrKind, g.Named, g.Limit]) with { Zone = new ZoneRef("Library") },
                Count: QuantityOf(g.Quantity),
                UpTo: ctx.TextOf(g)?.StartsWith("up to") == true));
        Rule<G.PutFoundCards>((g, ctx) =>
        {
            var found = new ObjectSelector(Quantifier.Those, new ObjectFilter(Kinds: ["card"], Qualifiers: [new Qualifier("Found")]));
            return g.Destination.GetResolvedValue() is G.ToTheBattlefieldUnderControl battlefield
                ? ToBattlefield(found, battlefield, ctx)
                : new ChangeZone(found, new ZoneRef(g.Destination.GetResolvedValue()?.ToString() ?? "?"));
        });
        Rule<G.ThenShuffle>((g, ctx) => new Shuffle());
        Rule<G.PutRestOnBottom>((g, ctx) =>
            new ChangeZone(new ObjectSelector(Quantifier.Rest, new ObjectFilter(Kinds: ["card"], Qualifiers: [new Qualifier(g.Rest.ToString())])), ZoneOf(g.Destination, ctx) ?? new ZoneRef("?")));
        Rule<G.LibraryPlacement>((g, ctx) =>
            new ZoneRef("Library", g.Library is null ? null : LowerAs<Selector>(g.Library.Owner, ctx), g.Order is null ? g.End.ToString() : $"{g.End} {g.Order}"));
        Rule<G.ExileTopOfLibrary>((g, ctx) => new Exile(LowerAs<Selector>(g.What, ctx), Manner: g.FaceDown ? "FaceDown" : null));
        Rule<G.TopCardsOfLibrary>((g, ctx) => TopOfLibrary(QuantityOf(g.Quantity), LowerAs<Selector>(g.Owner, ctx)));
        Rule<G.TopCardOfLibrary>((g, ctx) => TopOfLibrary(new Fixed(1), LowerAs<Selector>(g.Owner, ctx)));
        Rule<G.CardsFromTopOfLibrary>((g, ctx) => TopOfLibrary(g.EqualTo is null ? LowerAs<Quantity>(g.Count, ctx) : LowerAs<Quantity>(g.EqualTo, ctx), LowerAs<Selector>(g.Whose, ctx)));
        Rule<G.TopCardOfGraveyard>((g, ctx) =>
            new ObjectSelector(Quantifier.Count, new ObjectFilter(Kinds: g.CardType is { } type ? [type.ToString()] : ["card"], Qualifiers: [new Qualifier("Top")], Zone: new ZoneRef("Graveyard", PlayerOf(g.Whose))), Count: new Fixed(1)));
        Rule<G.Attach>((g, ctx) => new Attach(LowerAs<Selector>(g.Attachment, ctx), LowerAs<Selector>(g.AttachedTo, ctx)));
        Rule<G.Transform>((g, ctx) => new Transform(LowerAs<Selector>(g.Permanent, ctx)));

        // Looking, revealing, choosing.
        Rule<G.Reveal>((g, ctx) => new Reveal(g.Who is null ? Actor(ctx) : LowerAs<Selector>(g.Who, ctx), LowerAs<Selector>(g.What, ctx)));
        Rule<G.LookAt>((g, ctx) => new LookAt(Actor(ctx), LowerAs<Selector>(g.What, ctx)));
        Rule<G.ChooseCardFrom>((g, ctx) =>
            new ChooseCards(Actor(ctx), QuantityOf(g.Count), FilterOf(ctx, qualifiers: g.Qualifiers, narrowing: [g.With, g.TrailingWith]) with { Kinds = ["card"] }, LowerAs<Selector>(g.Source, ctx), ctx.TextOf(g)?.Contains("up to") == true));
        Rule<G.AmongThem>((g, ctx) => LowerAs<Selector>(g.Cards, ctx));
        Rule<G.PlayersZones>((g, ctx) =>
            new ObjectSelector(Quantifier.All, new ObjectFilter(Kinds: ["card"], Zone: new ZoneRef(string.Join("/", Terms(g.Zones, ctx)), LowerAs<Selector>(g.Whose, ctx)))));
        Rule<G.OfPlayersChoice>((g, ctx) => new ChosenBy(LowerAs<Selector>(g.Chooser, ctx)));
        Rule<G.ChooseNewTargets>((g, ctx) => new ChangeTargets(LowerAs<Selector>(g.Spell, ctx)));
        Rule<G.RetargetedObject>((retargeted, ctx) => new ObjectSelector(Quantifier.Those, new ObjectFilter(Kinds: [retargeted.ToString()])));
        Rule<G.DiscardThatCard>((g, ctx) =>
            g.Verb == G.CardVerb.Draw
                ? new Draw(Actor(ctx), new Special("Those"))
                : new Discard(Actor(ctx), null, new ObjectFilter(With: [LowerAs<Selector>(g.Card, ctx)])));
        Rule<G.Mill>((g, ctx) => new Mill(Actor(ctx), Multiply(QuantityOf(g.Quantity), g.PerEach is null ? null : LowerAs<Quantity>(g.PerEach, ctx))));
        Rule<G.CounterSpell>((g, ctx) => new CounterSpell(LowerAs<Selector>(g.Countered, ctx)));
        Rule<G.Regenerate>((g, ctx) => new Regenerate(LowerAs<Selector>(g.Regenerated, ctx)));
        Rule<G.PreventDamage>((g, ctx) =>
        {
            var protectedValue = g.Protected?.GetResolvedValue();
            return new PreventDamage(
                g.Amount.All ? new Special("All") : LowerAs<Quantity>(g.Amount.Next, ctx),
                g.Kind?.ToString(),
                protectedValue switch
                {
                    G.ProtectedRecipient recipient => LowerAs<Selector>(recipient.Who, ctx),
                    G.DealtToAndBy both => LowerAs<Selector>(both.Who, ctx),
                    _ => null,
                },
                g.Source is null ? null : LowerAs<Selector>(g.Source.Sources, ctx),
                g.ThisTurn || g.ThisTurnFirst || g.ThisTurnLeading ? new ThisTurn() : null,
                protectedValue is G.DealtToAndBy);
        });

        // Statics and continuous changes.
        Rule<G.GrantsQuotedAbility>((g, ctx) =>
            new GrantAbility(ctx.Subject ?? new Self(), AsAbility(Lower(g.Ability, ctx), ctx, g.Ability), g.Alongside is null ? null : OrNull(KeywordsOf(g.Alongside.Keywords.Items, ctx, []))));
        Rule<G.CostModification>((g, ctx) =>
            new CostModification(
                LowerAs<Selector>(g.Spells, ctx),
                new PayMana(g.Amount.Items.Select(SymbolText).ToList()),
                g.Change.ToString(),
                g.Action?.ToString(),
                g.PerEach is null ? null : LowerAs<Quantity>(g.PerEach, ctx),
                g.Condition is null ? null : Lower(g.Condition, ctx)));
        Rule<G.SpellsCast>((g, ctx) =>
            new ObjectSelector(Quantifier.All, FilterOf(ctx, qualifiers: g.Qualifiers, narrowing: [g.With, g.Caster, g.WithAfterCaster, g.Chosen, g.From, g.Targeting, g.Sharing]) with { Kinds = ["spell"] }));
        Rule<G.BecomesCreature>((g, ctx) =>
        {
            var stats = Lower(g.Stats, ctx) as StatsMod;
            List<IrNode> other = [];
            List<string> types = [];
            IrNode baseStats = null;

            switch (g.Types.GetResolvedValue())
            {
                case G.AnimatedTypes animated:
                    types = Terms(animated.Types.Items, ctx, other);
                    baseStats = Lower(animated.Base, ctx);
                    break;
                case G.WithBaseStats withBase:
                    baseStats = Lower(withBase.Stats, ctx);
                    break;
            }

            var keywords = g.With is null ? null : Lower(g.With, ctx) switch
            {
                HasKeywords has => has.Keywords.Select(x => new KeywordInstance(x)).ToList(),
                var node => node is null ? null : [new KeywordInstance(ctx.TextOf(g.With))],
            };

            return new Animate(
                ctx.Subject ?? new Self(),
                stats?.Power,
                stats?.Toughness,
                OrNull(Terms(g.Colors, ctx, other)),
                g.Subtypes is null ? null : OrNull(Terms(g.Subtypes.Items, ctx, other)),
                OrNull(types),
                baseStats,
                keywords,
                g.AllCreatureTypes,
                g.Still?.Kept.ToString(),
                g.InAddition,
                DurationOf(Lower(g.Duration, ctx)));
        });
        Rule<G.StillALand>((g, ctx) => new StillOfKind(Terms(g.Kind, ctx)));
        Rule<G.DoesntUntap>((g, ctx) =>
            new DoesntUntap(ctx.Subject ?? new Self(), PhaseRefOf(g.Step, ctx) ?? new PhaseRef(ctx.TextOf(g.Step)), g.Condition is null ? null : LowerAs<Condition>(g.Condition, ctx)));
        Rule<G.NextPlayersPhase>((g, ctx) => new PhaseRef(g.Phase.ToString(), Whose: PlayerOf(g.Whose), Next: true));
        Rule<G.OtherPermanents>((g, ctx) => Group(g.Permanents, Quantifier.All, ctx, other: true));
        Rule<G.ManaOfAnyColor>((g, ctx) => new ManaChoice(QuantityOf(g.Quantity), Lower(g.Choice, ctx)));
        Rule<G.KeywordWithAmountAndCost>((g, ctx) => new KeywordInstance(g.Keyword.ToString(), QuantityOf(g.Amount), new PayMana(g.Cost.Items.Select(SymbolText).ToList())));

        // Durations and phases.
        Rule<G.DuringPhase>((g, ctx) => new DuringPhase(PhaseRefOf(g.Step, ctx) ?? new PhaseRef(ctx.TextOf(g.Step))));
        Rule<G.DuringTurn>((g, ctx) => new DuringPhase(new PhaseRef("Turn", Whose: Lower(g.Turn, ctx) as Selector ?? new OpaqueSelector(ctx.TextOf(g.Turn), nameof(G.TurnOwner)))));
        Rule<G.UntilEndOfPlayersTurn>((g, ctx) => new UntilPhase((PhaseRefOf(g.Turn, ctx) ?? new PhaseRef("Turn")) with { Part = "End" }));

        // Conditions.
        Rule<G.PlayerControls>((g, ctx) => new Controls(Player(g.Controller), LowerAs<Selector>(g.Permanent, ctx)));
        Rule<G.PermanentsThreshold>((g, ctx) =>
            Group(g.Permanents, Quantifier.Count, ctx, other: g.Other, count: new Bound(QuantityOf(g.Count), g.Bound.ToString().Contains("More") || g.Bound.ToString().Contains("Greater") ? ">=" : "<=")));
        Rule<G.AtLeastPermanents>((g, ctx) =>
            Group(g.Kinds, Quantifier.Count, ctx, other: ctx.TextOf(g)?.Contains(" other ") == true, count: new Bound(QuantityOf(g.Count), ">=")));
        Rule<G.NoPermanents>((g, ctx) =>
            LowerAs<Selector>(g.Permanents, ctx) is ObjectSelector selector ? selector with { Quantifier = Quantifier.Count, Count = new Fixed(0) } : (Selector)Opaque(typeof(Selector), ctx.TextOf(g), nameof(G.NoPermanents)));
        Rule<G.PermanentIs>((g, ctx) =>
            new ObjectIs(LowerAs<Selector>(g.Subject, ctx), LowerAll(g.Aspect, ctx), g.Assertion is G.Assertion.Isnt or G.Assertion.Wasnt, g.Assertion is G.Assertion.Was or G.Assertion.Wasnt ? "Was" : "Is"));
        Rule<G.ItsAspect>((g, ctx) => new ObjectIs(LowerAs<Selector>(g.Subject, ctx), [Lower(g.Aspect, ctx)], g.Negated, "Is"));
        Rule<G.CardAspect>((g, ctx) =>
            g.CardType is { } cardType ? new Term(cardType.ToString(), nameof(G.CardType))
            : g.Supertype is { } supertype ? new Term(supertype.ToString(), nameof(G.Supertype))
            : g.ManaColor is { } color ? new Term(color.ToString(), nameof(G.ManaColor))
            : g.TapState is { } tapState ? new Term(tapState.ToString(), nameof(G.TapState))
            : g.CombatState is { } combatState ? new Term(combatState.ToString(), nameof(G.CombatState))
            : g.Status is { } status ? new Term(status.ToString(), nameof(G.PermanentDesignation))
            : g.CreatureType is { } creatureType ? new Term(creatureType.ToString(), nameof(G.CreatureType))
            : g.LandType is { } landType ? new Term(landType.ToString(), nameof(G.LandType))
            : g.Nonbasic ? new Term("Nonbasic", nameof(G.CardAspect))
            : g.Token ? new Term("Token", nameof(G.CardAspect))
            : new OpaqueNode(ctx.TextOf(g), nameof(G.CardAspect)));
        Rule<G.ObjectIsKind>((g, ctx) =>
        {
            var kind = LowerAs<Selector>(g.Kind, ctx) as ObjectSelector;
            var hasQualifiers = kind?.Filter is { } filter && (filter.Qualifiers is not null || filter.With is not null || filter.Controller is not null);
            return new IsKind(LowerAs<Selector>(g.Object, ctx), kind?.Filter?.Kinds ?? [], false, hasQualifiers ? [kind.Filter] : null);
        });
        Rule<G.ThatObjectIsOrWas>((g, ctx) => LowerAs<Selector>(g.Object, ctx));
        Rule<G.IfGameState>((g, ctx) => LowerAs<Condition>(g.State, ctx));
        Rule<G.IfState>((g, ctx) => LowerAs<Condition>(g.Condition, ctx));
        Rule<G.StateCondition>((g, ctx) => g.Connective == G.ConditionConnective.Unless ? new Not(LowerAs<Condition>(g.State, ctx)) : LowerAs<Condition>(g.State, ctx));
        Rule<G.IfHappenedThisTurn>((g, ctx) => LowerAs<Condition>(g.Event, ctx));
        Rule<G.HappenedThisTurn>((g, ctx) => new Happened(Lower(g.Event, ctx), g.Turn.ToString()));
        Rule<G.WhileCondition>((g, ctx) => LowerAs<Condition>(g.Condition, ctx));
        Rule<G.WasKicked>((g, ctx) =>
            new WasCastWith(LowerAs<Selector>(g.Subject, ctx), Lower(g.How, ctx) is Term how ? how.Value : ctx.TextOf(g.How), ctx.TextOf(g)?.Contains("wasn't") == true, g.Twice));
        Rule<G.HasCounters>((g, ctx) =>
        {
            var counters = Lower(g.Counters, ctx);
            var text = ctx.TextOf(g) ?? "";
            return new HasCountersCondition(LowerAs<Selector>(g.Subject, ctx), counters as CounterSpec, text.Contains("doesn't have"), text.Contains(" had ") ? "Had" : "Has", counters is CounterSpec ? null : counters);
        });
        Rule<G.CountersThere>((g, ctx) => new HasCountersCondition(LowerAs<Selector>(g.Holder, ctx), Lower(g.Counters, ctx) as CounterSpec));
        Rule<G.CardIsInZone>((g, ctx) => new IsInZone(LowerAs<Selector>(g.Card, ctx), Lower(g.Place, ctx)));

        // Selectors.
        Rule<G.ShortName>((g, ctx) => new Self());
        Rule<G.ShortNameSubject>((g, ctx) => new Self());
        Rule<G.ShortNameObject>((g, ctx) => new Self());
        Rule<G.PossessedShortName>((g, ctx) => new Self());
        Rule<G.PluralTargetKind>((g, ctx) => Lower(g.Kind, ctx));
        Rule<G.QualifiedTargetKind>((g, ctx) =>
            new QualifiedKind(Lower(g.Kind, ctx) is Term kind ? kind.Value : ctx.TextOf(g.Kind), LowerAll(g.Qualifiers, ctx).OfType<Qualifier>().ToList()));
        Rule<G.PluralKindsAndOr>((g, ctx) => new TermSet([.. Terms(g.First.Items, ctx), .. Terms(g.Last, ctx)], nameof(G.PermanentKind)));
        Rule<G.AndOrQualifiers>((g, ctx) => new QualifierChoice(LowerAll(new object[] { g.First, g.Second }, ctx).OfType<Qualifier>().ToList(), "and/or"));
        Rule<G.EachObject>((g, ctx) =>
            g.Kind.GetResolvedValue() is G.PlayerIdentity identity && g.Qualifiers is null && g.With is null
                ? new PlayerSelector(identity == G.PlayerIdentity.Opponent ? PlayerKind.EachOpponent : PlayerKind.EachPlayer)
                : new ObjectSelector(Quantifier.Each, FilterOf(ctx, kinds: g.Kind, qualifiers: g.Qualifiers, narrowing: [g.With])));
        Rule<G.EachDamagedObject>((g, ctx) =>
            new ObjectSelector(Quantifier.Each, FilterOf(ctx, kinds: g.Kind) with { Qualifiers = [new Qualifier("DealtDamage" + (Lower(g.When, ctx) is Term when ? when.Value : "ThisWay"))] }));
        Rule<G.UpToPermanents>((g, ctx) => Group(g.Permanents, Quantifier.Count, ctx, count: QuantityOf(g.Count), upTo: true));
        Rule<G.CountedObjects>((g, ctx) => g.Other ? Narrow(LowerAs<Selector>(g.Objects, ctx), new Qualifier("Other")) : LowerAs<Selector>(g.Objects, ctx));
        Rule<G.IndefiniteCardInZone>((g, ctx) => new ObjectSelector(Quantifier.Any, CardFilter(g.Card, ctx, g.Qualifiers), Other: ctx.TextOf(g)?.StartsWith("another") == true));
        Rule<G.CountedCardsInZone>((g, ctx) =>
        {
            var count = g.Count.GetResolvedValue();
            return new ObjectSelector(
                Quantifier.Count,
                CardFilter(g.Card, ctx, g.Qualifiers),
                Count: count switch { G.UpToCount upTo => LowerAs<Quantity>(upTo.Count, ctx), G.Quantity quantity => QuantityOf(quantity), _ => null },
                UpTo: count is G.UpToCount,
                Other: g.Other == true);
        });
        Rule<G.AllCardsFromZone>((g, ctx) => new ObjectSelector(Quantifier.All, CardFilter(g.Card, ctx, g.Qualifiers)));
        Rule<G.HandledObject>((g, ctx) =>
            new ObjectSelector(Quantifier.Those, new ObjectFilter(Kinds: g.CardType is { } type ? [type.ToString()] : ["card"], Qualifiers: [new Qualifier(g.How.ToString())])));
        Rule<G.OneOfThem>((g, ctx) => new ObjectSelector(Quantifier.Any, new ObjectFilter(With: [LowerAs<Selector>(g.Group, ctx)]), Count: new Fixed(1)));
        Rule<G.ObjectsAffectedThisWay>((g, ctx) =>
            new ObjectSelector(
                g.Quantifier.ToString() switch { "Each" => Quantifier.Each, "All" => Quantifier.All, _ => Quantifier.Any },
                new ObjectFilter(Qualifiers: [new Qualifier("AffectedThisWay")], With: [Lower(g.Affected, ctx)])));
        Rule<G.TargetSpell>((g, ctx) =>
            new ObjectSelector(Quantifier.Target, FilterOf(ctx, qualifiers: g.Qualifiers, narrowing: [g.Bound]) with { Kinds = ["spell"] }, Count: g.UpTo is null ? null : LowerAs<Quantity>(g.UpTo, ctx), UpTo: g.UpTo is not null));
        Rule<G.TargetAbility>((g, ctx) =>
            new ObjectSelector(Quantifier.Target, FilterOf(ctx, controller: g.Controller, narrowing: [g.Source]) with { Kinds = [g.Kind.ToString()] }));
        Rule<G.ThatStackObject>((g, ctx) => new ObjectSelector(Quantifier.Those, new ObjectFilter(Kinds: [g.Kind.ToString()])));
        Rule<G.ExtremePermanent>((g, ctx) =>
            new ObjectSelector(Quantifier.Any, new ObjectFilter(Kinds: [g.Kind.ToString()], Qualifiers: g.OrTied == true ? [new Qualifier("OrTied")] : null, With: [Lower(g.Extreme, ctx)]), Count: new Fixed(1)));
        Rule<G.ManaValueLimit>((g, ctx) => new CharacteristicBound(["ManaValue"], QuantityOf(g.Amount), ctx.TextOf(g)?.EndsWith("more") == true ? "Greater" : "Less"));
        Rule<G.SpellTargets>((g, ctx) => new Targeting(LowerAs<Selector>(g.Targeted, ctx), g.Count?.ToString()));

        // Amounts.
        Rule<G.LifeEqualTo>((g, ctx) => LowerAs<Quantity>(g.Amount, ctx));
        Rule<G.MeasuredDamage>((g, ctx) => g.Measure is null ? null : LowerAs<Quantity>(g.Measure, ctx));
        Rule<G.ScaledAmount>((g, ctx) =>
            new Scaled(LowerAs<Quantity>(g.Base, ctx), Lower(g.Factor, ctx) is Term factor ? factor.Value : ctx.TextOf(g.Factor), Lower(g.Rounded, ctx) is Term rounding ? rounding.Value : ctx.TextOf(g.Rounded)));
        Rule<G.CharacteristicOf>((g, ctx) => new CharacteristicOf(LowerAs<Selector>(g.Object, ctx), g.Characteristic.ToString()));
        Rule<G.CharacteristicOfObject>((g, ctx) => new CharacteristicOf(LowerAs<Selector>(g.Object, ctx), g.Characteristic.ToString()));
    }

    /// <summary>Effects in order, nested sequences flattened.</summary>
    static Effect Seq(params Effect[] parts)
    {
        List<Effect> steps = [];
        foreach (var part in parts)
            if (part is Sequence sequence)
                steps.AddRange(sequence.Steps);
            else if (part is not null)
                steps.Add(part);

        return steps.Count == 1 ? steps[0] : new Sequence(steps);
    }

    /// <summary>Runs <paramref name="body"/> with <paramref name="subject"/> (a glyph, possibly null) as the clause's subject.</summary>
    IrNode WithSubject(LoweringContext ctx, object subject, Func<IrNode> body)
    {
        if (subject is null)
            return body();

        var previous = ctx.Subject;
        ctx.Subject = LowerAs<Selector>(subject, ctx);

        try
        {
            return body();
        }
        finally
        {
            ctx.Subject = previous;
        }
    }

    /// <summary>An effect given a duration: folded into a buff that has none, else wrapped as <see cref="Model.Lasting"/>.</summary>
    static Effect Lasting(Effect effect, Duration duration) =>
        duration is null ? effect
        : effect is Pump { Duration: null } pump ? pump with { Duration = duration }
        : effect is Animate { Duration: null } animate ? animate with { Duration = duration }
        : new Lasting(effect, duration);

    /// <summary>A lowered cost: a cost as it is, an effect as an action cost, anything else opaque.</summary>
    Cost CostOf(IrNode node, object source, LoweringContext ctx) =>
        node switch
        {
            Cost cost => cost,
            Effect effect => new ActionCost(effect),
            _ => Coerce<Cost>(node, source, ctx),
        };

    /// <summary>Objects put onto the battlefield as <paramref name="destination"/> says: tapped, under whose control, transformed, attached, with counters.</summary>
    ChangeZone ToBattlefield(Selector objects, G.ToTheBattlefieldUnderControl destination, LoweringContext ctx)
    {
        List<IrNode> details = [];
        var tapped = destination.State is G.EntryState.Tapped or G.EntryState.TappedAndAttacking || destination.StateAfterControl is G.EntryState.Tapped or G.EntryState.TappedAndAttacking;

        details.AddRange(LowerAll(destination.WithCounters, ctx));
        details.AddRange(LowerAll(destination.Attached, ctx));
        if (destination.Transformed == true || destination.AndTransformed == true)
            details.Add(new Term("Transformed", nameof(G.EntryState)));
        if (destination.State is G.EntryState.TappedAndAttacking || destination.StateAfterControl is G.EntryState.TappedAndAttacking)
            details.Add(new Term("Attacking", nameof(G.EntryState)));
        if (destination.State is G.EntryState.FaceDown || destination.StateAfterControl is G.EntryState.FaceDown)
            details.Add(new Term("FaceDown", nameof(G.EntryState)));

        return new ChangeZone(objects, new ZoneRef("Battlefield"), tapped, destination.Control is null ? null : LowerAs<Selector>(destination.Control.Whose, ctx), OrNull(details));
    }

    static ObjectSelector TopOfLibrary(Quantity count, Selector owner) =>
        new(Quantifier.Count, new ObjectFilter(Kinds: ["card"], Qualifiers: [new Qualifier("Top")], Zone: new ZoneRef("Library", owner)), Count: count);
}
