namespace MTGRulesIR.Lowering;

/// <summary>
/// The tenth round: amounts of mana, either-or imperatives, prototype, total characteristics, prevention from a
/// source, switching and doubling stats, life totals, players who did something, sources dealing damage, turning
/// face up or down, piles, quoted abilities granted for a while, spell narrowings of a chosen aspect, halving
/// actions, life changed, tiered and pawprint modes, not having put a card somewhere, "this way" damage, life change
/// thresholds, combat relations, characteristic bounds by a count, names, spending mana as though, named options,
/// ordinal spells, playing with any mana, crewing as though stronger, resolution counts, colored predicates, copy
/// exceptions, permanents that entered, "that card is", the same being true for, becoming an aura, playing with the
/// top card revealed, durations over paired clauses, what's on the battlefield, exiling what was found, granted
/// keyword costs, being made to discard, uncounterable spells, poison counts, kinds predicates, counters on an
/// object, and top-card counts. Reminder text for prototype, enchant, manifest dread, phasing and backup lowers to
/// nothing.
/// </summary>
public sealed partial class Lowerer
{
    void RegisterRoundTen()
    {
        // Text restating a keyword's rules.
        Rule<G.KeepsAbilitiesAndTypes>((g, ctx) => null);
        Rule<G.AuraEntersAttached>((g, ctx) => null);
        Rule<G.ManifestDreadRule>((g, ctx) => null);
        Rule<G.PhasedOutRule>((g, ctx) => null);
        Rule<G.PhasesBackInRule>((g, ctx) => null);
        Rule<G.FollowingAbilities>((g, ctx) => null);

        // Effects.
        Rule<G.ImperativeEither>((g, ctx) => new ChoiceOf([LowerAs<Effect>(g.Actions.First, ctx), LowerAs<Effect>(g.Actions.Second, ctx)]));
        Rule<G.KeywordWithCostAndStats>((g, ctx) =>
            new KeywordInstance(g.Keyword.ToString(), Cost: new PayMana(g.Cost.Items.Select(SymbolText).ToList()), Stats: Lower(g.Stats, ctx) as StatsMod));
        Rule<G.PreventDamageFromSource>((g, ctx) =>
            new PreventDamage(
                g.Amount.All ? new Special("All") : LowerAs<Quantity>(g.Amount.Next, ctx),
                g.Kind?.ToString(),
                g.Protected is null ? null : LowerAs<Selector>(g.Protected.Who, ctx),
                LowerAs<Selector>(g.Source, ctx),
                g.ThisTurn ? new ThisTurn() : null));
        Rule<G.SwitchPowerToughness>((g, ctx) => new SwitchStats(LowerAs<Selector>(g.Creature, ctx), DurationOf(Lower(g.Duration, ctx))));
        Rule<G.DoublePowerToughness>((g, ctx) =>
            new DoubleStats(
                g.Doubled is not null ? LowerAs<Selector>(g.Doubled.Objects, ctx) : g.Creature is not null ? LowerAs<Selector>(g.Creature, ctx) : ctx.Subject ?? new Self(),
                g.AndToughness ? nameof(G.PowerAndOrToughness.PowerAndToughness) : nameof(G.PowerAndOrToughness.Power),
                DurationOf(Lower(g.Duration, ctx))));
        Rule<G.TurnFace>((g, ctx) => new TurnFace(LowerAs<Selector>(g.Permanent, ctx), g.Face.ToString()));
        Rule<G.SeparateIntoPiles>((g, ctx) =>
        {
            var text = ctx.TextOf(g.Piles) ?? "";
            return new SeparateIntoPiles(LowerAs<Selector>(g.Separated, ctx), text.Contains("three") ? 3 : 2, text.Contains("face-down"));
        });
        Rule<G.GrantsQuotedAbilityFor>((g, ctx) =>
        {
            List<IrNode> rest = [];
            var alongside = KeywordsOf(g.Alongside, ctx, rest);
            var grant = new GrantAbility(ctx.Subject ?? new Self(), AsAbility(Lower(g.Ability, ctx), ctx, g.Ability), OrNull(alongside));
            return DurationOf(Lower(g.Duration, ctx)) is { } duration ? new Lasting(grant, duration) : grant;
        });
        Rule<G.HalvingActionRounded>((g, ctx) =>
        {
            var objects = g.What.GetResolvedValue() switch
            {
                G.PlayersZone zone => new ObjectSelector(Quantifier.All, new ObjectFilter(Kinds: [Kind("card")], Zone: ZoneOf(zone, ctx))),
                G.HalvedObjects halved => LowerAs<Selector>(halved.Objects, ctx),
                _ => LowerAs<Selector>(g.What, ctx),
            };
            var count = new Scaled(new CountOf(objects), g.Factor.ToString(), g.Rounded.ToString());
            var actor = Actor(ctx);
            return g.Action switch
            {
                G.HalvingAction.Mill => new Mill(actor, count),
                G.HalvingAction.Discard => new Discard(actor, count, (objects as ObjectSelector)?.Filter),
                _ => new Sacrifice(actor, objects is ObjectSelector selector ? selector with { Quantifier = Quantifier.Count, Count = count } : objects),
            };
        });
        Rule<G.TieredModeEffect>((g, ctx) => new Mode(LowerAs<Effect>(g.Effect, ctx), new PayMana(g.Cost.Items.Select(SymbolText).ToList())));
        Rule<G.PawprintMode>((g, ctx) => new Mode(LowerAs<Effect>(g.Effect, ctx), new PawprintCost(g.Cost switch { G.Pawprints.Three => 3, G.Pawprints.Two => 2, _ => 1 })));
        Rule<G.SpendManaAsThough>((g, ctx) =>
            new SpendManaAsThough(
                ctx.Subject ?? Actor(ctx),
                g.Spent?.ToString(),
                g.TreatedAs.GetResolvedValue() switch { G.ManaOfAnyKind any => any.Kind.ToString(), G.ManaOfColor color => color.Color.ToString(), _ => null },
                g.Purpose?.GetResolvedValue() switch
                {
                    G.SpendToCast cast => [new Playable(nameof(G.PlayVerb.Cast), LowerAs<Selector>(cast.Spells, ctx))],
                    G.SpendToActivate activate => [new Playable(nameof(UseVerb.Activate), LowerAs<Selector>(activate.Abilities, ctx))],
                    _ => null,
                }));
        Rule<G.ChooseNamedOption>((g, ctx) => new ChooseOption(Actor(ctx), TermsOf(g.Options, ctx)));
        Rule<G.PlayCardWithAnyMana>((g, ctx) =>
        {
            var play = LowerAs<Effect>(g.Play, ctx);
            var spell = LowerAs<Selector>(g.Mana.Spell, ctx);
            var spend = g.Mana.Permission.GetResolvedValue() switch
            {
                G.ManaCanBeSpent can => new SpendManaAsThough(Actor(ctx), AsThough: can.Kind.ToString(), For: [new Playable(nameof(G.PlayVerb.Cast), spell)]),
                G.MaySpendManaAsThough may => new SpendManaAsThough(
                    may.Player.GetResolvedValue() is G.WhichPlayer which ? Player(which) : new PlayerSelector(PlayerKind.ThatPlayer),
                    AsThough: may.Kind.ToString(), For: [new Playable(nameof(G.PlayVerb.Cast), spell)]),
                _ => null,
            };
            return spend is null ? play : Seq(play, spend);
        });
        Rule<G.SpellsCastThisWay>((g, ctx) =>
            new ObjectSelector(Quantifier.All, new ObjectFilter(Kinds: [Kind("spell")], Qualifiers: [Q(AffectedThisWay.CastThisWay)])));
        Rule<G.CrewsAsThoughPowerGreater>((g, ctx) => new CrewsAsThoughPowerGreater(ctx.Subject ?? new Self(), g.Action.ToString(), QuantityOf(g.Amount)));
        Rule<G.CopyHasAbility>((g, ctx) =>
        {
            var it = new Reference(null, "it");
            List<IrNode> rest = [];
            var keywords = g.Keywords.Keywords.GetResolvedValue() is G.ThisAbility ? [] : KeywordsOf(g.Keywords.Keywords, ctx, rest);
            var grant = g.Keywords.Keywords.GetResolvedValue() is G.ThisAbility || g.Keywords.AndThisAbility ? new GrantAbility(it, new SameAbility()) : null;
            return keywords.Count == 0 ? grant : grant is null ? new Pump(it, Keywords: keywords) : Seq(new Pump(it, Keywords: keywords), grant);
        });
        Rule<G.SameIsTrueFor>((g, ctx) =>
            g.Extended.GetResolvedValue() switch
            {
                G.SpellsAndCardsOffBattlefield objects => new SameIsTrueFor(Objects: new Union(
                [
                    new ObjectSelector(Quantifier.All, FilterOf(ctx, qualifiers: objects.SpellQualifiers, controller: new Controlled(new PlayerSelector(PlayerKind.You))) with { Kinds = [Kind("spell")] }),
                    new ObjectSelector(Quantifier.All, FilterOf(ctx, qualifiers: objects.CardQualifiers) with { Kinds = [Kind("card")], Controller = new Controlled(new PlayerSelector(PlayerKind.You), "Own") } is { } cards
                        ? cards with { Qualifiers = [.. cards.Qualifiers ?? [], Q(Distinction.NotOnTheBattlefield)] }
                        : null),
                ])),
                _ => new SameIsTrueFor(KeywordsOf(g.Extended, ctx, [])),
            });
        Rule<G.BecomesAura>((g, ctx) =>
        {
            var subject = ctx.Subject ?? new Self();
            return Seq(
                new Animate(subject, Types: [nameof(G.CardType.Enchantment)], Subtypes: ["Aura"]),
                new GrantAbility(subject, AsAbility(Lower(g.Enchant, ctx), ctx, g.Enchant)));
        });
        Rule<G.PlayWithTopCardRevealed>((g, ctx) => new PlayWithTopCardRevealed(g.AllPlayers ? new PlayerSelector(PlayerKind.EachPlayer) : PlayerOf(g.Whose)));
        Rule<G.DurationFirstPair>((g, ctx) =>
            new Lasting(Seq(LowerAs<Effect>(g.Clauses.First, ctx), LowerAs<Effect>(g.Clauses.Second, ctx)), DurationOf(Lower(g.Duration, ctx))));
        Rule<G.ExileFoundCards>((g, ctx) =>
            new Exile(new ObjectSelector(Quantifier.Those, new ObjectFilter(Kinds: [Kind("card")], Qualifiers: [Q(Provenance.Found)]))));
        Rule<G.GrantedKeywordCost>((g, ctx) => new KeywordCostIsManaCost(g.Keyword.ToString(), LowerAs<Selector>(g.Card, ctx)));
        Rule<G.SpellsCantBeCountered>((g, ctx) =>
        {
            var spells = LowerAs<Selector>(g.Spells, ctx);
            if (g.YouControl && spells is ObjectSelector objects)
                spells = objects with { Filter = (objects.Filter ?? new ObjectFilter()) with { Controller = new Controlled(new PlayerSelector(PlayerKind.You)) } };
            return new CantBeTargeted(spells, [nameof(TargetingBy.Countered)], Duration: g.ThisTurn ? new ThisTurn() : null);
        });
        Rule<G.ManaDoesntEmpty>((g, ctx) => new ManaDoesntEmpty((ctx.TextOf(g) ?? "").StartsWith("until end of turn") ? new UntilPhase(new PhaseRef("EndOfTurn")) : null));
        Rule<G.BeginGameOnBattlefield>((g, ctx) => new BeginGameOnBattlefield());
        Rule<G.WithBaseStats>((g, ctx) => Lower(g.Stats, ctx));

        // Mana.
        Rule<G.AmountOfMana>((g, ctx) =>
        {
            var amount = LowerAs<Quantity>(g.Amount, ctx);
            return g.Mana.GetResolvedValue() switch
            {
                G.ManaCost cost => new ManaAmount(cost.Items.Select(SymbolText).ToList(), amount),
                G.ManaOfChoice choice => new ManaChoice(amount, choice.Choice.ToString()),
                _ => new OpaqueManaOption(ctx.TextOf(g), nameof(G.AmountOfMana)),
            };
        });

        // Selectors and predicates.
        Rule<G.ArticleKind>((g, ctx) => new ObjectSelector(Quantifier.Any, FilterOf(ctx, kinds: g.Kind)));
        Rule<G.PlayersWho>((g, ctx) =>
            new ObjectSelector(
                Quantifier.Each,
                new ObjectFilter(
                    Kinds: [Kind("player")],
                    Qualifiers: g.Player.ToString().Contains("Opponent") ? [Q(G.PlayerIdentity.Opponent)] : null,
                    With: [PlayerRelativeOf(g.Which, ctx)])));
        Rule<G.ThatsInCombat>((g, ctx) =>
        {
            Selector whom = g.Whom?.GetResolvedValue() switch
            {
                null => null,
                G.AttackRecipient recipient => recipient.ToString() == "You" ? new PlayerSelector(PlayerKind.You) : new Reference(null, PatternOf(recipient)),
                var other => LowerAs<Selector>(other, ctx),
            };
            return (g.Status, whom) switch
            {
                (G.InCombatStatus.AttackingOrBlocking, null) => new QualifierChoice([Q(G.CombatState.Attacking), Q(G.CombatState.Blocking)]),
                (G.InCombatStatus.Attacking, null) => new Qualifier(nameof(G.CombatState.Attacking), Vocabulary: nameof(G.CombatState)),
                (G.InCombatStatus.Blocking, null) => new Qualifier(nameof(G.CombatState.Blocking), Vocabulary: nameof(G.CombatState)),
                (G.InCombatStatus.Attacking, _) => new InCombatWith(whom, nameof(CombatRelation.Attacking)),
                (G.InCombatStatus.Blocking, _) => new InCombatWith(whom, nameof(CombatRelation.Blocking)),
                _ => (IrNode)new OpaquePredicate(ctx.TextOf(g), nameof(G.ThatsInCombat)),
            };
        });
        Rule<G.WithCharacteristicBoundByCount>((g, ctx) =>
            new CharacteristicBound(
                [g.Characteristic.ToString()],
                LowerAs<Quantity>(g.Count, ctx),
                g.Comparison == G.Comparison.Less ? Comparison.LessOrEqual : Comparison.GreaterOrEqual));
        Rule<G.WithName>((g, ctx) => new OfChosen(nameof(ChosenCharacteristic.Name)));
        Rule<G.WithChosenName>((g, ctx) => new OfChosen(nameof(ChosenCharacteristic.Name)));
        Rule<G.OrdinalSpellCast>((g, ctx) =>
        {
            var filter = FilterOf(ctx, qualifiers: (g.Spell.GetResolvedValue() as G.QualifiedSpellNoun)?.Qualifiers);
            return new ObjectSelector(Quantifier.Any, filter with
            {
                Kinds = [Kind("spell")],
                Qualifiers = [Q(g.Ordinal), Q(g.Scope), .. filter.Qualifiers ?? []],
                With = [.. filter.With ?? [], new CastBy(new PlayerSelector(PlayerKind.You), nameof(G.SpellRelation.Cast), ThisTurn: g.Scope == G.SpellTurnScope.ThisTurn)],
            });
        });
        Rule<G.ThatsColored>((g, ctx) =>
            g.Colors.GetResolvedValue() switch
            {
                G.ColorCount count => new Qualifier(count.ToString(), Vocabulary: nameof(G.ColorCount)),
                G.ColorsAndOr both => new QualifierChoice([Q(both.First), Q(both.Second)], Connective.AndOr),
                var colors => new QualifierChoice(TermsOf(colors, ctx).Select(x => new Qualifier(x.Value, Vocabulary: x.Vocabulary)).ToList()),
            });
        Rule<G.ThatsOneOfKinds>((g, ctx) =>
        {
            var kinds = g.Kinds.GetResolvedValue() switch
            {
                ManyOf<G.ArticleKind> articles => articles.Items.SelectMany(x => TermsOf(x.Kind, ctx)).ToList(),
                G.PluralKindsAndOr andOr => [.. andOr.First.Items.SelectMany(x => TermsOf(x, ctx)), .. TermsOf(andOr.Last, ctx)],
                _ => [],
            };
            return new QualifierChoice(kinds.Select(x => new Qualifier(x.Value, Vocabulary: x.Vocabulary)).ToList(), g.Kinds.GetResolvedValue() is G.PluralKindsAndOr ? Connective.AndOr : Connective.Or);
        });
        Rule<G.ThatCardIs>((g, ctx) => LowerAs<Selector>(g.Card, ctx));
        Rule<G.ThisWay>((g, ctx) => new Term(nameof(EventSpan.ThisWay), nameof(EventSpan)));

        // Quantities.
        Rule<G.LifeTotalOf>((g, ctx) => new LifeTotal(PlayerOf(g.Whose)));
        Rule<G.LifeChangedAmount>((g, ctx) =>
            new DamageOrLifeTotal(
                nameof(G.DamageOrLife.Life),
                g.Change.ToString(),
                g.Player.GetResolvedValue() switch
                {
                    G.WhichPlayer which => Player(which),
                    G.PerfectSubject.They or G.TheyPlayer => new PlayerSelector(PlayerKind.ThatPlayer),
                    G.PerfectSubject.You => new PlayerSelector(PlayerKind.You),
                    var other => LowerAs<Selector>(other, ctx),
                },
                g.Turn?.ToString()));
        Rule<G.CountersOnObjectCount>((g, ctx) =>
            new CountOf(new ObjectSelector(Quantifier.All, new ObjectFilter(Kinds: [Kind("counter")], Qualifiers: [Q(PatternOf(g.Kind), "CounterKind")], With: [new Related(Relation.On, LowerAs<Selector>(g.Holder, ctx))]))));
        Rule<G.ControlledKindCount>((g, ctx) => new CountOf(new ObjectSelector(Quantifier.All, FilterOf(ctx, kinds: g.Kind, controller: g.Controller))));
        Rule<G.CardsInZoneCount>((g, ctx) => new CountOf(new ObjectSelector(Quantifier.All, FilterOf(ctx, qualifiers: g.Qualifier, zone: g.Zone) with { Kinds = [Kind("card")] })));
        Rule<G.TopCardCount>((count, ctx) =>
            count switch
            {
                G.TopCardCount.ThatMany => new Special("ThatMany"),
                G.TopCardCount.TwiceX => new Product(new Fixed(2), new Variable("X")),
                _ => null,
            });

        // Conditions.
        Rule<G.TotalCharacteristicCondition>((g, ctx) =>
            new CompareCount(
                new CharacteristicOf(LowerAs<Selector>(g.Group, ctx), g.Characteristic.ToString()),
                g.Comparison == G.Comparison.Less ? Comparison.LessOrEqual : Comparison.GreaterOrEqual,
                QuantityOf(g.Quantity)));
        Rule<G.DidntPutCardCondition>((g, ctx) =>
            new Not(new Did(new ChangeZone(new ObjectSelector(Quantifier.Those, new ObjectFilter(Kinds: [Kind("card")])), ZoneOf(g.Destination, ctx) ?? new ZoneRef(OpenSlots.Placeholder)))));
        Rule<G.LifeChangeThreshold>((g, ctx) =>
            new Happened(
                new PlayerAction(
                    g.Player.GetResolvedValue() switch
                    {
                        G.WhichPlayer which => Player(which),
                        G.PerfectSubject.They => new PlayerSelector(PlayerKind.ThatPlayer),
                        _ => new PlayerSelector(PlayerKind.You),
                    },
                    g.Change == G.LifeChange.Gained ? nameof(G.PastPlayerAction.GainedLife) : nameof(G.PastPlayerAction.LostLife),
                    Count: new Bound(QuantityOf(g.Amount), Comparison.GreaterOrEqual)),
                null));
        Rule<G.AbilityResolvedNthTime>((g, ctx) => new AbilityResolvedNthTime(g.Ordinal.ToString()));
        Rule<G.PlayerHadPermanentsEnter>((g, ctx) =>
            new Happened(
                new ObjectEvent(
                    LowerAs<Selector>(g.What, ctx),
                    [nameof(G.PermanentEvent.EntersTheBattlefield)],
                    g.Player.GetResolvedValue() switch
                    {
                        G.WhichPlayer which => Player(which),
                        G.PerfectSubject.They => new PlayerSelector(PlayerKind.ThatPlayer),
                        G.PerfectSubject.You => new PlayerSelector(PlayerKind.You),
                        var other => LowerAs<Selector>(other, ctx),
                    }),
                g.Turn.ToString()));
        Rule<G.PermanentsOnBattlefield>((g, ctx) => new ThereAre(LowerAs<Selector>(g.What, ctx)));
        Rule<G.PlayerPoisonCount>((g, ctx) =>
            new HasCountersCondition(
                g.Player.GetResolvedValue() is G.WhichPlayer which ? Player(which) : LowerAs<Selector>(g.Player, ctx),
                new CounterSpec("poison", new Bound(QuantityOf(g.Count.Count), Comparison.GreaterOrEqual))));

        // Events.
        Rule<G.SourceDealsDamageTo>((g, ctx) =>
        {
            var controller = Lower(g.Controller, ctx) as Controlled;
            Selector source = g.Source.GetResolvedValue() is G.DamageSourceNoun
                ? new ObjectSelector(Quantifier.Any, new ObjectFilter(Kinds: [Kind(ObjectKind.Source)], Controller: controller))
                : LowerAs<Selector>(g.Source, ctx);
            if (source is ObjectSelector objects)
                source = objects with { Filter = (objects.Filter ?? new ObjectFilter()) with { Controller = objects.Filter?.Controller ?? controller }, Binding = ctx.Bind(g) };
            var recipient = g.Recipient?.Recipient.GetResolvedValue();
            return recipient switch
            {
                null => new ObjectEvent(source, [nameof(G.PermanentEvent.DealsDamage)]),
                G.WhichPlayer.You => new ObjectEvent(source, [nameof(G.PermanentEvent.DealsDamageToYou)]),
                _ => new ObjectEvent(source, [nameof(G.PermanentEvent.DealsDamage)], Other: LowerAs<Selector>(g.Recipient.Recipient, ctx)),
            };
        });
        Rule<G.CausesYouToDiscardThis>((g, ctx) =>
            new ObjectEvent(
                g.Discarded == G.DiscardedByEffect.ThisCard ? new Self() : new ObjectSelector(Quantifier.Any, new ObjectFilter(Kinds: [Kind("card")])),
                [nameof(ObjectEventKind.Discarded)],
                Only: g.Period is null ? null : g.Period.Period == G.TurnPeriod.Combat ? nameof(G.TurnSpan.ThisCombat) : nameof(G.TurnSpan.ThisTurn),
                By: g.Source.ToString(),
                ByController: Lower(g.Controller, ctx) as Controlled));
    }

    /// <summary>What a player did or has, as a predicate on the player: controls a permanent, has poison, gained or lost life, did something this turn.</summary>
    Predicate PlayerRelativeOf(G.PlayerRelative which, LoweringContext ctx)
    {
        var who = new Reference(null, "who");
        return which.GetResolvedValue() switch
        {
            G.ControlsAPermanent controls => new Satisfying(new Controls(who, LowerAs<Selector>(controls.Permanent, ctx))),
            G.HasPoisonCounters poison => new Satisfying(new HasCountersCondition(who, new CounterSpec("poison", new Bound(QuantityOf(poison.Count.Count), Comparison.GreaterOrEqual)))),
            G.LifeChangedThisTurn life => new Satisfying(new Happened(new PlayerEvent(who, life.Change == G.LifeChange.Gained ? nameof(G.PlayerEventKind.GainsLife) : nameof(G.PlayerEventKind.LosesLife)), life.Turn.ToString())),
            G.PastActionThisTurn past => new Satisfying(new Happened(new PlayerAction(who, past.Action.ToString(), past.What is null ? null : LowerAs<Selector>(past.What, ctx), past.FromHand), past.Turn.ToString())),
            G.SearchedThisWay => new Satisfying(new Happened(new PlayerAction(who, nameof(G.PastPlayerAction.SearchedLibrary)), nameof(EventSpan.ThisWay))),
            // "Each opponent who can't", "each player who does": how they answered the demand before.
            G.PlayerResponse response => new Satisfying(new OutcomeCondition(who, response switch { G.PlayerResponse.Cant => nameof(G.OutcomeResult.Cannot), G.PlayerResponse.Doesnt => nameof(G.OutcomeResult.DidNot), _ => nameof(G.OutcomeResult.Did) })),
            _ => new OpaquePredicate(ctx.TextOf(which), nameof(G.PlayerRelative)),
        };
    }
}
