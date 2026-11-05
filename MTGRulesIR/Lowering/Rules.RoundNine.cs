namespace MTGRulesIR.Lowering;

/// <summary>
/// The ninth round: blocking if able and being blocked, assigning no combat damage, life totals set, exchanging
/// control, removing from combat, damage that can't be prevented, dice, not gaining life, changing targets, keyword
/// actions with amounts and as costs, beholding, exerting as it attacks, choices of keywords, buffs with a quoted
/// ability, abilities of an object or a group, mana a land could produce, targets beyond the first, chosen numbers
/// of targets, attached permanents, any object in a zone, the copy, possessives, "that permanent or player", "of
/// those", spells of a kind, cards handled this turn, half a life total, X thresholds, this and others, counted
/// object events, card counts, keyword costs paid, cast from where, pairing, state triggers, ability activation
/// triggers, player triggers, and a point of damage or life. Reminder text for power-up, doors, earthbend, evoke,
/// improvise, waterbend, gift, firebending, soulbond, dash and station lowers to nothing.
/// </summary>
public sealed partial class Lowerer
{
    void RegisterRoundNine()
    {
        // Text restating a keyword's rules.
        Rule<G.PowerUpCostReduction>((g, ctx) => null);
        Rule<G.DoorUnlocksOnEntering>((g, ctx) => null);
        Rule<G.UnlockDoorRule>((g, ctx) => null);
        Rule<G.EarthbendReturn>((g, ctx) => null);
        Rule<G.EvokeSacrificeRule>((g, ctx) => null);
        Rule<G.EachOnePaysForOne>((g, ctx) => null);
        Rule<G.ManaLastsUntilEndOfCombat>((g, ctx) => null);
        Rule<G.SoulbondPaired>((g, ctx) => null);
        Rule<G.GainsHasteReturnedAtEnd>((g, ctx) => null);
        Rule<G.ImproviseRule>((g, ctx) => null);
        Rule<G.StationThreshold>((g, ctx) => null);
        Rule<G.BeforeOtherEffects>((g, ctx) => Lower(g.Effect, ctx));

        // Effects.
        Rule<G.BlocksIfAble>((g, ctx) =>
        {
            var blocker = ctx.Subject ?? new Self();
            var must = new MustBlock(blocker, LowerAs<Selector>(g.Attacker, ctx), DurationOf(Lower(g.Period, ctx)));
            return g.UntapsFirst ? Seq(new Untap(blocker), must) : must;
        });
        Rule<G.MustBeBlocked>((g, ctx) => new MustBeBlocked(ctx.Subject ?? new Self(), DurationOf(Lower(g.Period, ctx))));
        Rule<G.AssignsNoCombatDamage>((g, ctx) => new AssignsNoCombatDamage(ctx.Subject ?? new Self(), DurationOf(Lower(g.Period, ctx))));
        Rule<G.LifeTotalBecomes>((g, ctx) => new SetLifeTotal(LowerAs<Selector>(g.Player, ctx), LowerAs<Quantity>(g.NewTotal, ctx)));
        Rule<G.LifeTotalValue>((value, ctx) => new Special(value.ToString()));
        Rule<G.ExchangeControl>((g, ctx) =>
            new ExchangeControl(g.Exchanged.GetResolvedValue() is G.PermanentPair pair
                ? [LowerAs<Selector>(pair.First, ctx), LowerAs<Selector>(pair.Second, ctx)]
                : [LowerAs<Selector>(g.Exchanged, ctx)]));
        Rule<G.RemoveFromCombat>((g, ctx) => new RemoveFromCombat(LowerAs<Selector>(g.Removed, ctx)));
        Rule<G.CantGainLife>((g, ctx) => new CantGainLife(ctx.Subject ?? Actor(ctx), DurationOf(Lower(g.Duration, ctx)), g.RestOfGame));
        Rule<G.RollDie>((g, ctx) => new RollDice(Actor(ctx), QuantityOf(g.Count), g.Die.Faces));
        Rule<G.ChangeTarget>((g, ctx) =>
        {
            var spell = LowerAs<Selector>(g.Retargeted, ctx);
            if (g.Restriction is { } restriction)
                spell = Narrow(spell, new Targeting(restriction.OnlyTarget is null ? null : LowerAs<Selector>(restriction.OnlyTarget, ctx), nameof(G.SpellTargetCount.OnlyASingle)), ctx, restriction);
            return new ChangeTargets(spell, g.NewTarget is null ? null : LowerAs<Selector>(g.NewTarget.Target, ctx));
        });
        Rule<G.DamageCantBePrevented>((g, ctx) =>
            new DamageCantBePrevented(
                g.Which?.ToString(),
                g.Dealt is null ? null : LowerAs<Selector>(g.Dealt.Source, ctx),
                g.Turn switch { G.TurnSpan.ThisTurn => new ThisTurn(), G.TurnSpan.ThisCombat => new ThisTurn(nameof(G.TurnPeriod.Combat)), _ => null }));
        Rule<G.KeywordActionAmount>((g, ctx) => new KeywordAction([new KeywordInstance(g.Action.ToString(), QuantityOf(g.Amount))]));
        Rule<G.KeywordActionCost>((g, ctx) =>
            new KeywordCost(new KeywordInstance(g.Action.ToString(), Cost: g.Payment is null ? null : new PayMana(g.Payment.Items.Select(SymbolText).ToList()))));
        Rule<G.Behold>((g, ctx) => new KeywordAction([new KeywordInstance(nameof(KeywordWord.Behold))], Objects: LowerAs<Selector>(g.What, ctx)));
        Rule<G.ExertAsItAttacks>((g, ctx) =>
            new AsEventHappens(new ObjectEvent(new Self(), [nameof(G.PermanentEvent.Attacks)]), new KeywordAction([new KeywordInstance(nameof(G.Keyword.Exert))], Objects: new Self())));
        Rule<G.GiftKeyword>((g, ctx) => new KeywordInstance(nameof(G.Keyword.Gift), Parameter: g.Gift.ToString()));
        Rule<G.ChoiceOfBuffs>((g, ctx) =>
        {
            List<IrNode> other = [];
            var options = KeywordsOf(g.Choices, ctx, other);
            return other.Count == 0 ? new KeywordChoice(options) : new OpaqueNode(ctx.TextOf(g), nameof(G.ChoiceOfBuffs));
        });
        Rule<G.BuffsAndQuotedAbility>((g, ctx) =>
        {
            var subject = ctx.Subject ?? new Self();
            List<Effect> steps = [];
            foreach (var node in LowerAll(g.Buffs, ctx))
                steps.Add(node is BuffGroup group ? PumpOf(group, null, null, ctx) : EffectOf(node));
            List<IrNode> rest = [];
            var alongside = KeywordsOf(g.Alongside, ctx, rest);
            steps.Add(new GrantAbility(subject, AsAbility(Lower(g.Ability, ctx), ctx, g.Ability), OrNull(alongside)));
            return Seq([.. steps]);
        });
        Rule<G.WithStatsEqualTo>((g, ctx) =>
        {
            var amount = LowerAs<Quantity>(g.Amount, ctx);
            return new StatsMod(amount, amount);
        });

        // Selectors.
        Rule<G.ActivatedAbilitiesOf>((g, ctx) => AbilitiesOf(LowerAs<Selector>(g.Owner, ctx), activated: true));
        Rule<G.ActivatedAbilitiesOfGroup>((g, ctx) => AbilitiesOf(LowerAs<Selector>(g.Group, ctx), activated: true));
        Rule<G.ManaOfAnyColor>((g, ctx) =>
            g.Choice.GetResolvedValue() switch
            {
                G.ManaChoice choice => new ManaChoice(QuantityOf(g.Quantity), choice.ToString()),
                G.ManaProducibleBy producible => new ManaChoice(QuantityOf(g.Quantity), $"Any{producible.Aspect}{producible.Tense}", Producer: LowerAs<Selector>(producible.Producer, ctx)),
                G.ColorAmongPermanents among => new ManaChoice(QuantityOf(g.Quantity), nameof(ManaChoiceKind.AnyColorAmong), Producer: LowerAs<Selector>(among.Permanents, ctx)),
                _ => new OpaqueManaOption(ctx.TextOf(g), nameof(G.ManaOfAnyColor)),
            });
        Rule<G.ThatIndefinitePermanent>((g, ctx) =>
            LowerAs<Selector>(g.Permanent, ctx) is ObjectSelector objects ? objects with { Quantifier = Quantifier.Those } : new Reference(null, ctx.TextOf(g)));
        Rule<G.ThatPermanentWord>((g, ctx) => new Reference(null, "that permanent"));
        Rule<G.TargetBeyondFirst>((g, ctx) =>
            new ObjectSelector(Quantifier.Each, new ObjectFilter(Kinds: [Kind(ObjectKind.Target)], Qualifiers: g.BeyondFirst == true ? [Q(Distinction.BeyondTheFirst)] : null)));
        Rule<G.ChosenNumberOfTargets>((g, ctx) =>
        {
            var counts = (g.Counts.GetResolvedValue() switch
            {
                ManyOf<G.Quantity> many => many.Items,
                G.Quantity one => [one],
                _ => [],
            }).Select(QuantityOf).ToList();
            var most = counts.OfType<Fixed>().Select(x => x.Value).DefaultIfEmpty(0).Max();
            var targets = LowerAs<Selector>(g.Targets, ctx);
            return targets is ObjectSelector objects && most > 0 ? objects with { Count = new Fixed(most), UpTo = counts.Count > 1 } : targets;
        });
        Rule<G.AttachedPermanents>((g, ctx) =>
            new ObjectSelector(Quantifier.All, FilterOf(ctx, kinds: g.Kind, narrowing: [new In(Relation.AttachedTo, g.Host)])));
        Rule<G.AttachedToReference>((g, ctx) => new Related(Relation.AttachedTo, LowerAs<Selector>(g.Host, ctx)));
        Rule<G.AnyObjectInZone>((g, ctx) =>
        {
            var filter = FilterOf(ctx, kinds: g.Kind, qualifiers: g.Qualifier);
            return new ObjectSelector(Quantifier.Any, g.Where == G.CopySourceZone.CardInAGraveyard
                ? filter with { Kinds = [Kind("card"), .. filter.Kinds ?? []], Zone = new ZoneRef(nameof(Zone.Graveyard)) }
                : filter with { Zone = new ZoneRef(nameof(Zone.Battlefield)) });
        });
        Rule<G.TheCopy>((g, ctx) => new ObjectSelector(Quantifier.Those, new ObjectFilter(Kinds: [Kind(ObjectKind.Copy)])));
        Rule<G.TargetPossessive>((g, ctx) => LowerAs<Selector>(g.Target, ctx));
        Rule<G.ThatPermanentOrPlayer>((g, ctx) => new Reference(null, "that permanent or player"));
        Rule<G.OfThoseObjects>((g, ctx) => LowerAs<Selector>(g.Objects, ctx));
        Rule<G.SpellsOfKind>((g, ctx) =>
        {
            var among = g.From switch
            {
                G.FromAmongCards.AmongThem => new Related(Relation.Among, new Reference(null, "them")),
                G.FromAmongCards.AmongExiledWithThis => new Related(Relation.Among, new ObjectSelector(Quantifier.All, new ObjectFilter(Kinds: [Kind("card")], With: [new Related(Relation.ExiledWith, new Self())]))),
                _ => null,
            };
            var filter = FilterOf(ctx, qualifiers: (g.Kind.GetResolvedValue() as G.QualifiedSpellsNoun)?.Qualifiers, narrowing: [g.With, among]);
            return new ObjectSelector(Quantifier.All, filter with { Kinds = [Kind("spell")] });
        });
        Rule<G.CardsHandledThisTurn>((g, ctx) =>
        {
            var player = g.Player?.GetResolvedValue() switch
            {
                G.PerfectSubject.They => new PlayerSelector(PlayerKind.ThatPlayer),
                G.WhichPlayer which => Player(which),
                G.PlayerHas has => LowerAs<Selector>(has.Player, ctx),
                _ => new PlayerSelector(PlayerKind.You),
            };
            var other = g.Kinds?.GetResolvedValue() as G.OtherKinds;
            var filter = FilterOf(ctx, qualifiers: other is null ? g.Kinds : other.Qualifiers, narrowing: [new ActedOn(player, g.Verb.ToString())]);
            return new ObjectSelector(Quantifier.All, filter with
            {
                Kinds = [Kind(g.Noun.ToString().ToLowerInvariant())],
                Qualifiers = other is null ? filter.Qualifiers : [Q(Distinction.Other), .. filter.Qualifiers ?? []],
            });
        });
        Rule<G.ThisAndOthers>((g, ctx) =>
        {
            var others = LowerAs<Selector>(g.Others, ctx);
            if (others is ObjectSelector objects)
                others = objects with { Quantifier = Quantifier.Count, Count = new Bound(QuantityOf(g.Count), Comparison.GreaterOrEqual), Other = true };
            return new Union([LowerAs<Selector>(g.Self, ctx), others], Connective.And);
        });

        // Quantities.
        Rule<G.HalfLifeTotal>((g, ctx) => new Scaled(new LifeTotal(PlayerOf(g.Whose)), g.Factor.ToString(), g.Rounded.ToString()));
        Rule<G.PointOfDamageOrLife>((g, ctx) =>
            g.Event.GetResolvedValue() switch
            {
                G.PlayerChangedLife byPlayer => new DamageOrLifeTotal(
                    g.Unit.ToString(),
                    byPlayer.Change.ToString(),
                    byPlayer.Player.GetResolvedValue() is G.WhichPlayer which ? Player(which) : new PlayerSelector(PlayerKind.ThatPlayer),
                    byPlayer.When?.ToString()),
                G.DamageOrLifeHappened happened => new DamageOrLifeTotal(
                    g.Unit.ToString(),
                    happened.Change.ToString(),
                    happened.To is null ? null : Player(happened.To.Player),
                    happened.When.ToString()),
                _ => Opaque(typeof(Quantity), ctx.TextOf(g), nameof(G.PointOfDamageOrLife)),
            });

        // Conditions.
        Rule<G.XThreshold>((g, ctx) =>
            new CompareCount(
                new Variable("X"),
                g.Bound?.Bound switch { G.CountBound.AtLeast => Comparison.GreaterOrEqual, G.CountBound.AtMost => Comparison.LessOrEqual, _ => Comparison.Equal },
                QuantityOf(g.Count)));
        Rule<G.PlayerCardCountCondition>((g, ctx) =>
            new CountCondition(LowerAs<Selector>(g.Cards, ctx), Comparison.GreaterOrEqual, QuantityOf(g.Count.Count)));
        Rule<G.KeywordCostWasPaid>((g, ctx) =>
            g.Keyword is { } keyword
                ? new WasCastWith(LowerAs<Selector>(g.Whose, ctx), keyword.ToString())
                : Opaque(typeof(Condition), ctx.TextOf(g), nameof(G.KeywordCostWasPaid)));
        Rule<G.WasCastFrom>((g, ctx) =>
            new WasCast(LowerAs<Selector>(g.Spell, ctx), null, ZoneOf(g.Zone, ctx), (g.Polarity == G.PastPolarity.Wasnt) ^ g.OtherThan));
        Rule<G.IsPaired>((g, ctx) => new IsPaired(LowerAs<Selector>(g.Paired, ctx), LowerAs<Selector>(g.Partner, ctx)));
        Rule<G.ObjectsEventCount>((g, ctx) =>
        {
            var what = LowerAs<Selector>(g.What, ctx);
            if (what is ObjectSelector objects)
                what = objects with { Quantifier = Quantifier.Count, Count = new Bound(QuantityOf(g.Amount), Comparison.GreaterOrEqual) };
            var under = g.Event == G.ObjectTurnEvent.EnteredUnderYourControl ? new PlayerSelector(PlayerKind.You) : null;
            return new Happened(new ObjectEvent(what, [g.Event.ToString()], under), g.Turn.ToString());
        });

        // Triggers.
        Rule<G.ControlsNoneTrigger>((g, ctx) =>
            new TriggeredAbility(
                new ConditionEvent(new Controls(Player(g.Controller), new ObjectSelector(Quantifier.Any, FilterOf(ctx, kinds: g.Kind)), Negated: true)),
                LowerAs<Effect>(g.Effect, ctx)));
        Rule<G.AbilityActivationTrigger>((g, ctx) =>
        {
            var descriptor = g.Ability;
            List<Qualifier> qualifiers = [];
            switch (descriptor.Kind?.GetResolvedValue())
            {
                case G.AbilityKindWord word:
                    qualifiers.Add(Q(word));
                    break;
                case ManyOf<G.Keyword?> keywords:
                    qualifiers.AddRange(keywords.Items.OfType<G.Keyword>().Select(x => Q(x)));
                    break;
            }
            if (descriptor.NonMana)
                qualifiers.Add(Q(G.AbilityKindWord.Mana, negated: true));
            var abilities = new ObjectSelector(Quantifier.Any, new ObjectFilter(
                Kinds: [Kind(ObjectKind.Ability)],
                Qualifiers: OrNull(qualifiers),
                With: descriptor.Source is null ? null : [new Related(Relation.AbilityOf, LowerAs<Selector>(descriptor.Source.Permanent, ctx))]));
            return new TriggeredAbility(new PlayerAction(Player(g.Activator), nameof(UseVerb.Activate), abilities), LowerAs<Effect>(g.Effect, ctx));
        });
        Rule<G.PlayerTriggerEvent>((g, ctx) => LowerAs<Event>(g.Event, ctx));
    }

    /// <summary>The abilities of <paramref name="of"/>: an ability selector related to its source.</summary>
    static ObjectSelector AbilitiesOf(Selector of, bool activated) =>
        new(Quantifier.All, new ObjectFilter(
            Kinds: [Kind(ObjectKind.Ability)],
            Qualifiers: activated ? [Q(AbilityCategory.Activated)] : null,
            With: [new Related(Relation.AbilityOf, of)]));
}
