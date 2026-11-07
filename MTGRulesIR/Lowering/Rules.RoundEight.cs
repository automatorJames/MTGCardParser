namespace MTGRulesIR.Lowering;

/// <summary>
/// The eighth round: abilities that can't be activated, same-name and shared-aspect predicates, phasing, moving and
/// distributing and doubling counters, amounts "this way", shuffling zones into libraries, tap-for-mana and
/// counters-put and next-spell and door triggers, copy descriptions, intervening-if triggers, basic land types among,
/// any player activating, designations, repeating, transforming into, paired creatures, party creatures, counted
/// player actions, getting counters, damage that would be dealt, casting choices, extreme characteristics, attached
/// hosts, counters removed this way, whose turn it is, attacking with, additional phases, no maximum hand size,
/// scaled amounts, must-block, having keywords, and piles. Mutate, explore, clash and splice reminders lower to nothing.
/// </summary>
public sealed partial class Lowerer
{
    void RegisterRoundEight()
    {
        // Text restating a keyword's rules.
        Rule<G.ExploreNonlandRule>((g, ctx) => null);
        Rule<G.MutateMerge>((g, ctx) => null);
        Rule<G.ClashWinner>((g, ctx) => null);
        Rule<G.AddThisCardsEffects>((g, ctx) => null);

        // Effects.
        Rule<G.CantBeActivated>((g, ctx) => new CantBeActivated(ctx.Subject ?? new SelfAbility(), g.ExceptMana));
        Rule<G.PhasesOutOrIn>((g, ctx) => new Phasing(ctx.Subject ?? new Self(), g.Direction.ToString(), DurationOf(Lower(g.Duration, ctx))));
        Rule<G.MoveCounters>((g, ctx) =>
            Lower(g.Counters, ctx) is CounterSpec counters
                ? new MoveCounters(counters, LowerAs<Selector>(g.Source, ctx), LowerAs<Selector>(g.Destination, ctx))
                : Opaque(typeof(Effect), ctx.TextOf(g), nameof(G.MoveCounters)));
        Rule<G.ShuffleZoneIntoLibrary>((g, ctx) =>
        {
            var owner = PlayerOf(g.Whose);
            var zones = Terms(g.Zones, ctx).Select(zone => (Selector)new ObjectSelector(Quantifier.All, new ObjectFilter(Kinds: [Kind("card")], Zone: new ZoneRef(zone, owner)))).ToList();
            return new ShuffleInto(zones.Count == 1 ? zones[0] : new Union(zones, Connective.And), Lower(g.Library, ctx) as ZoneRef ?? new ZoneRef("Library", owner));
        });
        Rule<G.CopyIsDescribed>((g, ctx) =>
        {
            var stats = Lower(g.Stats, ctx) as StatsMod;
            List<IrNode> other = [];
            List<KeywordInstance> keywords = [];
            if (g.With is not null)
                TokenAbilitiesOf([Lower(g.With, ctx)], keywords);
            var types = g.Types is null ? null : Terms(g.Types.Items, ctx, other);
            var subtypes = g.Subtypes is null ? null : Terms(g.Subtypes.Items, ctx, other);
            var colors = Terms(g.Colors, ctx, other);
            return other.Count > 0 || g.NoManaCost
                ? Opaque(typeof(Effect), ctx.TextOf(g), nameof(G.CopyIsDescribed))
                : new Animate(new Reference(null, "it"), stats?.Power, stats?.Toughness, OrNull(colors), subtypes is null ? null : OrNull(subtypes), types is null ? null : OrNull(types), Keywords: OrNull(keywords), InAddition: g.Additional is not null);
        });
        Rule<G.TransformsInto>((g, ctx) =>
        {
            var stats = Lower(g.Stats, ctx) as StatsMod;
            List<IrNode> other = [];
            var types = g.Types is null ? null : Terms(g.Types.Items, ctx, other);
            var subtypes = g.Subtypes is null ? null : Terms(g.Subtypes.Items, ctx, other);
            return other.Count > 0
                ? Opaque(typeof(Effect), ctx.TextOf(g), nameof(G.TransformsInto))
                : new Animate(ctx.Subject ?? new Self(), stats?.Power, stats?.Toughness, null, subtypes is null ? null : OrNull(subtypes), types is null ? null : OrNull(types), Transforms: true);
        });
        Rule<G.AnyPlayerMayActivate>((g, ctx) => new ActivationLimits([new Timing(nameof(TimingKind.AnyPlayer))]));
        Rule<G.RepeatProcess>((g, ctx) =>
            new Repeat(
                g.Times?.GetResolvedValue() switch
                {
                    G.RepeatCount count => new Special(count.ToString()),
                    G.MoreTimes more => QuantityOf(more.Count),
                    _ => null,
                },
                g.ForEach is null ? null : LowerAs<Selector>(g.ForEach.Items, ctx)));
        Rule<G.DistributeCounters>((g, ctx) =>
            Lower(g.Counters, ctx) is CounterSpec counters
                ? new DistributeCounters(counters, LowerAs<Selector>(g.Among, ctx), g.Range.ToString(), g.Targeted)
                : Opaque(typeof(Effect), ctx.TextOf(g), nameof(G.DistributeCounters)));
        Rule<G.GetsCounters>((g, ctx) =>
            Lower(g.Counters, ctx) is CounterSpec counters
                ? new PutCounters(ctx.Subject ?? new Self(), counters)
                : Opaque(typeof(Effect), ctx.TextOf(g), nameof(G.GetsCounters)));
        Rule<G.AdditionalPhase>((g, ctx) =>
            new AdditionalPhase(g.Phase.ToString(), g.Before?.ToString(), g.Count.ToString(), g.After == true, g.FollowedByMainPhase));
        Rule<G.NoMaximumHandSize>((g, ctx) => new NoMaximumHandSize(ctx.Subject ?? Actor(ctx), DurationOf(Lower(g.Duration, ctx))));
        Rule<G.DoubleCounters>((g, ctx) =>
            new DoubleCounters(
                g.Kind.GetResolvedValue() is G.CounterType kind ? PatternOf(kind) : null,
                LowerAs<Selector>(g.Place.Permanent, ctx),
                g.Kind.GetResolvedValue() is G.EachKindOfCounter));
        Rule<G.MustBlockAll>((g, ctx) => new MustBlock(LowerAs<Selector>(g.Blockers, ctx), LowerAs<Selector>(g.Attacker, ctx), g.Duration is null ? null : new ThisTurn(g.Duration.Period.ToString())));

        // Triggers and events.
        Rule<G.TapForManaTrigger>((g, ctx) =>
            new TriggeredAbility(
                new ObjectEvent(LowerAs<Selector>(g.Permanent, ctx), [nameof(G.PermanentEvent.IsTappedForMana)], Actor: Player(g.Player)),
                LowerAs<Effect>(g.Effect, ctx)));
        Rule<G.InterveningIfTrigger>((g, ctx) =>
            new TriggeredAbility(LowerAs<Event>(g.Event, ctx), LowerAs<Effect>(g.Effect, ctx), LowerAs<Condition>(g.Condition, ctx)));
        Rule<G.CountersPutTrigger>((g, ctx) =>
        {
            if (Lower(g.Counters, ctx) is not CounterSpec counters)
                return Opaque(typeof(Ability), ctx.TextOf(g), nameof(G.CountersPutTrigger));
            // "When the seventh plan counter is put on this enchantment": the counters reaching that many.
            var period = g.Period?.GetResolvedValue();
            return new TriggeredAbility(
                new ObjectEvent(
                    LowerAs<Selector>(g.Permanent, ctx),
                    [g.Movement == G.CounterMovement.PutOn ? nameof(ObjectEventKind.CountersPut) : nameof(ObjectEventKind.CountersRemoved)],
                    Only: period switch { G.ThisPeriod turn => "This" + turn.Period, G.PlayerEventTiming timing => timing.ToString(), _ => null },
                    Counters: counters),
                LowerAs<Effect>(g.Effect, ctx));
        });
        Rule<G.NextSpellCastTrigger>((g, ctx) =>
            new TriggeredAbility(
                new SpellCastEvent(new PlayerSelector(PlayerKind.You), g.Narrowed is null ? LowerAs<Selector>(g.Spell, ctx) : Narrow(LowerAs<Selector>(g.Spell, ctx), Lower(g.Narrowed, ctx), ctx, g.Narrowed), NextThisTurn: true),
                LowerAs<Effect>(g.Effect, ctx)));
        Rule<G.QualifiedSpellNoun>((g, ctx) => new ObjectSelector(Quantifier.Any, FilterOf(ctx, qualifiers: g.Qualifiers) with { Kinds = [Kind("spell")] }));
        Rule<G.DoorOrLevelTrigger>((g, ctx) =>
            new TriggeredAbility(
                g.Event.Unlock is not null
                    ? new NamedEvent(nameof(EventKind.UnlockThisDoor))
                    : new NamedEvent(nameof(EventKind.ClassBecomesLevel), int.TryParse(new string((ctx.TextOf(g.Event.Level) ?? "").Where(char.IsDigit).ToArray()), out var level) ? new Fixed(level) : null),
                LowerAs<Effect>(g.Effect, ctx)));
        Rule<G.DamageWouldBeDealt>((g, ctx) =>
            new WouldHappen(null, new DealDamage(null, [new DamagePortion(null, LowerAs<Selector>(g.Recipient, ctx))], Kind: g.Combat ? nameof(G.DamageKind.Combat) : null), g.While is null ? null : LowerAs<Condition>(g.While, ctx)));
        Rule<G.PlayerAttacksWith>((g, ctx) =>
            new PlayerAction(Player(g.Player), nameof(G.PastPlayerAction.AttackedWith), LowerAs<Selector>(g.Attackers, ctx)));
        Rule<G.NOrMorePermanents>((g, ctx) => Group(g.Permanents, Quantifier.Count, ctx, count: new Bound(QuantityOf(g.Count), Comparison.GreaterOrEqual)));

        // Selectors and predicates.
        Rule<G.WithSameName>((g, ctx) => new SharesCharacteristic(nameof(G.Characteristic.Name), LowerAs<Selector>(g.Object, ctx)));
        Rule<G.SharesCreatureType>((g, ctx) => new SharesCharacteristic(g.Aspect.ToString(), LowerAs<Selector>(g.With, ctx)));
        Rule<G.NamesakePermanent>((g, ctx) =>
            new ObjectSelector(Quantifier.Any, FilterOf(ctx, kinds: g.Kind, qualifiers: g.Qualifier, controller: g.Controller), Other: g.Determiner == G.IndefiniteDeterminer.Another));
        Rule<G.NamesakeCard>((g, ctx) =>
            new ObjectSelector(Quantifier.Any, new ObjectFilter(Kinds: [Kind("card")], Zone: Lower(g.Zone, ctx) as ZoneRef), Other: g.Determiner == G.IndefiniteDeterminer.Another));
        Rule<G.PairedCreatures>((g, ctx) => new ObjectSelector(Quantifier.Those, new ObjectFilter(Kinds: [Kind(G.CardType.Creature)])));
        Rule<G.PartyCreatures>((g, ctx) =>
            new ObjectSelector(Quantifier.All, new ObjectFilter(Kinds: [Kind(G.CardType.Creature)], With: [new Related(Relation.InPartyOf, PlayerOf(g.Whose))])));
        Rule<G.WithExtremeCharacteristic>((g, ctx) =>
            new HasExtreme(g.Characteristic.ToString(), g.Extreme.ToString(), g.Among is null ? null : LowerAs<Selector>(g.Among, ctx)));
        Rule<G.AmongGroup>((g, ctx) => new ObjectSelector(Quantifier.All, FilterOf(ctx, kinds: g.Kind, controller: g.Scope)));
        Rule<G.AttachedToHost>((g, ctx) => new Related(Relation.AttachedTo, LowerAs<Selector>(g.Host, ctx)));
        Rule<G.CountersRemovedThisWay>((g, ctx) =>
            new ObjectSelector(Quantifier.All, new ObjectFilter(
                Kinds: [Kind("counter")],
                Qualifiers: [.. g.CounterType is { } kind ? [Q(PatternOf(kind), "CounterKind")] : Array.Empty<Qualifier>(), .. g.ThisWay ? [Q(AffectedThisWay.RemovedThisWay)] : Array.Empty<Qualifier>()])));
        Rule<G.OneOfTwoPiles>((g, ctx) => new ObjectSelector(Quantifier.Any, new ObjectFilter(Kinds: [Kind(ObjectKind.Pile)]), Count: new Fixed(1)));
        Rule<G.CounterPlace>((g, ctx) => LowerAs<Selector>(g.Permanent, ctx));

        // Quantities.
        Rule<G.AmountThisWay>((g, ctx) => new AmountThisWay(g.Kind.ToString()));
        Rule<G.BasicLandTypesAmong>((g, ctx) => new DistinctCount(nameof(G.CountedAspect.BasicLandTypes), LowerAs<Selector>(g.Lands, ctx)));
        Rule<G.ScaledAmountExact>((g, ctx) => new Scaled(LowerAs<Quantity>(g.Base, ctx), g.Factor.ToString()));

        // Conditions.
        Rule<G.PlayerHasDesignation>((g, ctx) => new HasDesignation(Player(g.Player), g.Designation.ToString()));
        Rule<G.PlayerActionCount>((g, ctx) =>
            new Happened(
                new PlayerAction(
                    g.Player?.GetResolvedValue() is G.WhichPlayer which ? Player(which) : new PlayerSelector(g.Player?.GetResolvedValue() is G.PerfectSubject.They ? PlayerKind.ThatPlayer : PlayerKind.You),
                    g.Action.ToString(),
                    LowerAs<Selector>(g.What, ctx),
                    Count: new Bound(QuantityOf(g.Amount), Comparison.GreaterOrEqual)),
                nameof(G.TurnSpan.ThisTurn)));
        Rule<G.CastingChoiceMade>((g, ctx) => new ChoiceMade(g.Choice.ToString()));
        Rule<G.WhoseTurnCondition>((g, ctx) => new ItsPhase(new PhaseRef(g.Phase.ToString(), Whose: PlayerOf(g.Whose)), g.Copula == G.TurnCopula.IsNot));
        Rule<G.ObjectHasKeyword>((g, ctx) =>
            new HasKeywordsCondition(LowerAs<Selector>(g.Subject, ctx), Terms(g.Keywords, ctx), (ctx.TextOf(g) ?? "").Contains("n't")));
    }
}
