namespace MTGRulesIR.Lowering;

/// <summary>
/// The thirteenth round: a spell, cards in all graveyards, counted targets of any kind, one of those cards, you and
/// another player, a player and their permanents, damage sources, damage from a source, attached to an object,
/// entered this turn, keyworded permanents a player controls, a token's keyword ability, colors of mana spent, mana
/// of a kind spent, the number of times kicked, counted energy, casting this spell free, "and" costs, copies that
/// aren't legendary, colors set, outcomes, life compared, spell-cast events, either-or and and-create predicates,
/// both players controlling, paying any amount, two quoted abilities, counters equal to an amount, adding a
/// characteristic, expend, day and night changing, tribute, assigning damage as though unblocked, what players
/// can't do, haunt, drawing in a circumstance, counter and token doublers, "unless a player has this deal damage
/// to them", triggers happening again, and transforming by face name. Indestructible's "it still dies", ante and
/// read-ahead rules text, "a deck can have any number" and cipher's reminder lower to nothing.
/// </summary>
public sealed partial class Lowerer
{
    void RegisterRoundThirteen()
    {
        // Rules text the engine carries out itself, or that says nothing to it.
        Rule<G.StillDiesAtZeroToughness>((g, ctx) => null);
        Rule<G.AnyNumberInDeck>((g, ctx) => null);
        Rule<G.AnteRemovalRule>((g, ctx) => null);
        Rule<G.SagaChapterRule>((g, ctx) => null);
        Rule<G.CipherCast>((g, ctx) => null);

        // Selectors.
        Rule<G.ASpell>((g, ctx) => new ObjectSelector(Quantifier.Any, new ObjectFilter(Kinds: [Kind("spell")])));
        Rule<G.CardsInZoneGroup>((g, ctx) =>
        {
            var filter = FilterOf(ctx, qualifiers: g.Cards.GetResolvedValue() is G.QualifiedCards qualified ? qualified.Qualifiers : null) with { Kinds = [Kind("card")] };
            var zone = g.Zone.GetResolvedValue() switch
            {
                G.AllPlayersZone all => new ZoneRef(ZoneNameOf(all), new PlayerSelector(PlayerKind.EachPlayer)),
                G.PlayersZone players => ZoneOf(players, ctx),
                _ => null,
            };
            return zone is null ? Opaque(typeof(Selector), ctx.TextOf(g), nameof(G.CardsInZoneGroup)) : new ObjectSelector(Quantifier.All, filter with { Zone = zone });
        });
        Rule<G.CountedAnyTargets>((g, ctx) => new ObjectSelector(Quantifier.Target, new ObjectFilter(Kinds: [Kind("any")]), QuantityOf(g.Count), g.UpTo));
        Rule<G.OneOfThoseCards>((g, ctx) =>
            new ObjectSelector(Quantifier.Count, new ObjectFilter(Kinds: [Kind("card")], With: [new Related(Relation.Among, LowerAs<Selector>(g.Cards, ctx))]), QuantityOf(g.Count), g.UpTo == true));
        Rule<G.YouAndPlayerEach>((g, ctx) =>
            new Union([new PlayerSelector(PlayerKind.You), g.Other.GetResolvedValue() is G.WhichPlayer which ? Player(which) : LowerAs<Selector>(g.Other, ctx)], Connective.And));
        Rule<G.PlayerAndPermanents>((g, ctx) => new Union([Player(g.Player), LowerAs<Selector>(g.Group, ctx)], Connective.And));
        Rule<G.DamageSources>((g, ctx) =>
            new ObjectSelector(Quantifier.All, FilterOf(ctx, qualifiers: g.Qualifiers, controller: g.Controller, narrowing: [g.Aspect]) with { Kinds = [Kind(ObjectKind.Source)] }));
        // "Damage from a red source" (as in "if damage from a red source is prevented this way"): damage, by what dealt it.
        Rule<G.DamageFromSource>((g, ctx) =>
            new ObjectSelector(Quantifier.All, new ObjectFilter(
                Kinds: [Kind(ObjectKind.Damage)],
                With: g.Source is null ? null : [new Related(Relation.DealtDamageBy, new ObjectSelector(Quantifier.Any, new ObjectFilter(Kinds: [Kind(ObjectKind.Source)], Qualifiers: [Q(g.Source.Color)])))])));

        // Predicates.
        Rule<G.AttachedToObject>((g, ctx) => new Related(Relation.AttachedTo, LowerAs<Selector>(g.Host, ctx)));
        Rule<G.EnteredThisTurn>((g, ctx) => new Satisfying(new Happened(new ObjectEvent(null, ["Entered"]), nameof(G.TurnSpan.ThisTurn))));
        Rule<G.KeywordedControlledBy>((g, ctx) =>
            Lower(g.Keywords, ctx) is HasKeywords keywords
                ? new ObjectFilter(Controller: Lower(g.Controller, ctx) as Controlled, With: [keywords])
                : Opaque(typeof(Predicate), ctx.TextOf(g), nameof(G.KeywordedControlledBy)));
        // "With protection from black", "with toxic 1": the keyword itself; a token's description collects it, a filter reads it as WithKeyword (see PredicateOf).
        Rule<G.WithKeywordAbility>((g, ctx) =>
            Lower(g.Ability, ctx) switch
            {
                KeywordInstance keyword => keyword,
                Term { Vocabulary: nameof(G.Keyword) } term => new KeywordInstance(term.Value),
                _ => Opaque(typeof(Predicate), ctx.TextOf(g), nameof(G.WithKeywordAbility)),
            });

        // Quantities.
        Rule<G.ColorOfManaSpent>((g, ctx) => new DistinctCount("Color", new ObjectSelector(Quantifier.All, new ObjectFilter(Kinds: [Kind(ObjectKind.ManaSpent)]))));
        // "For each mana from a cave spent to cast it", "for each {s} spent to cast this spell": mana spent, counted.
        Rule<G.CountedThing>((g, ctx) =>
            g.GetResolvedValue() is G.ManaSpent spent ? ManaSpentAmountOf(spent, ctx) : Lower(g.GetResolvedValue(), ctx));
        Rule<G.NumberOfTimes>((g, ctx) =>
            g.Event == G.ObjectHistoryEvent.WasKicked ? new Special(nameof(SpecialAmount.TimesKicked)) : Opaque(typeof(Quantity), ctx.TextOf(g), nameof(G.NumberOfTimes)));
        Rule<G.CountedEnergy>((g, ctx) =>
            g.Count.GetResolvedValue() is G.Quantity quantity ? QuantityOf(quantity)
            : (ctx.TextOf(g) ?? "").Contains("any amount") ? new Special("AnyNumber")
            : new Special("ThatMany"));

        // Effects.
        Rule<G.CastThisSpellFree>((g, ctx) => new PlayCard(Actor(ctx), new Self(), Free: true));
        Rule<G.AndCost>((g, ctx) => LowerAs<Cost>(g.Cost, ctx));
        Rule<G.CopyLegendary>((g, ctx) =>
            new BecomesAspect(new Reference(null, "it"), [new Term(nameof(G.Supertype.Legendary), nameof(G.Supertype))], Negated: g.Change == G.LegendaryChange.NotLegendary));
        Rule<G.ColorIsSet>((g, ctx) =>
        {
            List<Term> colors = g.Colors.GetResolvedValue() switch
            {
                G.SetColor.Colorless => [new Term(nameof(G.ManaColor.Colorless), nameof(G.ManaColor))],
                G.SetColor set => [new Term(set.ToString(), nameof(G.SetColor))],
                _ => TermsOf(g.Colors, ctx),
            };
            return new BecomesAspect(LowerAs<Selector>(g.Objects, ctx), colors, Verb: g.Verb.ToString());
        });
        Rule<G.PredicateEither>((g, ctx) => new ChoiceOf([LowerAs<Effect>(g.First, ctx), LowerAs<Effect>(g.Second, ctx)]));
        Rule<G.PredicateAndCreate>((g, ctx) => Seq(LowerAs<Effect>(g.First, ctx), LowerAs<Effect>(g.Second, ctx)));
        Rule<G.PayAnyAmount>((g, ctx) => new PayCost(new PayAnyAmount(g.Resource.ToString()), ctx.Subject));
        Rule<G.GrantsTwoQuotedAbilities>((g, ctx) =>
        {
            var subject = ctx.Subject ?? new Self();
            return Seq(
                new GrantAbility(subject, AsAbility(Lower(g.First, ctx), ctx, g.First)),
                new GrantAbility(subject, AsAbility(Lower(g.Second, ctx), ctx, g.Second)));
        });
        Rule<G.PlaceCountersEqualTo>((g, ctx) =>
        {
            var kind = g.Kind.GetResolvedValue() switch
            {
                G.CounterType type => PatternOf(type),
                G.PowerToughnessMod mod => ctx.TextOf(mod),
                _ => null,
            };
            var counters = new CounterSpec(kind, LowerAs<Quantity>(g.CountFirst ?? g.Count, ctx));
            var objects = LowerAs<Selector>(g.Place, ctx);
            return g.Action == G.CounterAction.Remove ? new RemoveCounters(objects, counters) : new PutCounters(objects, counters);
        });
        Rule<G.AddCharacteristicTo>((g, ctx) =>
        {
            var amount = new CharacteristicOf(LowerAs<Selector>(g.From, ctx), g.Characteristic.ToString());
            var to = LowerAs<Selector>(g.To, ctx);
            var duration = DurationOf(Lower(g.Duration, ctx));
            return g.Characteristic switch
            {
                G.Characteristic.Power => new Pump(to, Power: amount, Duration: duration),
                G.Characteristic.Toughness => new Pump(to, Toughness: amount, Duration: duration),
                _ => Opaque(typeof(Effect), ctx.TextOf(g), nameof(G.AddCharacteristicTo)),
            };
        });
        Rule<G.AssignsDamageAsThoughUnblocked>((g, ctx) =>
            new AssignsDamageAsThoughUnblocked(ctx.Subject ?? new Self(), g.Duration is null ? null : new ThisTurn(g.Duration.Period.ToString())));
        Rule<G.PlayerCantAct>((g, ctx) => new PlayerCant(ctx.Subject ?? Actor(ctx), g.Action.ToString(), g.Duration is null ? null : DurationOf(Lower(g.Duration, ctx))));
        Rule<G.TransformByFaceName>((g, ctx) => new Transform(new Self()));
        Rule<G.AdditionalTrigger>((g, ctx) => new AdditionalTriggers(LowerAs<Selector>(g.Cause, ctx), g.Event.ToString(), LowerAs<Selector>(g.Source, ctx)));
        Rule<G.CounterIncreaseReplacement>((g, ctx) =>
        {
            if (Lower(g.Counters, ctx) is not CounterSpec counters)
                return Opaque(typeof(Effect), ctx.TextOf(g), nameof(G.CounterIncreaseReplacement));
            var kind = g.Kinds.GetResolvedValue() is G.CountersOfKind ofKind
                ? ofKind.Kind?.GetResolvedValue() switch { G.CounterType type => PatternOf(type), G.PowerToughnessMod mod => ctx.TextOf(mod), _ => null }
                : counters.Kind;
            return new Replacement(
                new WouldHappen(null, new PutCounters(LowerAs<Selector>(g.Recipient, ctx), counters)),
                new Instead(new PutCounters(LowerAs<Selector>(g.Again, ctx), new CounterSpec(kind, IncreasedOf(g.Increase)))));
        });
        Rule<G.TokenIncreaseReplacement>((g, ctx) =>
            new Replacement(
                new WouldHappen(LowerAs<Selector>(g.Control, ctx), new CreateTokens(new TokenSpec(Types: g.Kind is { } kind ? [kind.ToString()] : null))),
                new Instead(new CreateTokens(new ThoseTokens(), IncreasedOf(g.Increase)))));

        // Costs: "unless a player has this creature deal 4 damage to them".
        Rule<G.UnlessHasDo>((g, ctx) =>
        {
            var payer = g.Player.Payer.GetResolvedValue() is G.WhichPlayer which ? Player(which) : LowerAs<Selector>(g.Player.Payer, ctx);
            var saved = ctx.Subject;
            ctx.Subject = LowerAs<Selector>(g.Doer, ctx);
            Effect action;
            try { action = LowerAs<Effect>(g.Action, ctx); }
            finally { ctx.Subject = saved; }
            return new PayCost(new ActionCost(action), payer);
        });

        // Conditions.
        Rule<G.Outcome>((g, ctx) =>
        {
            var player = g.Player.GetResolvedValue() is G.WhichPlayer which ? Player(which) : LowerAs<Selector>(g.Player, ctx);
            object action = g.Result.GetResolvedValue() switch
            {
                G.DidNotDo didNot => didNot.Action,
                G.DidNotDoThisWay didNotThisWay => didNotThisWay.Action,
                _ => null,
            };
            if (action is null)
                return g.Result.GetResolvedValue() is G.OutcomeResult result ? new OutcomeCondition(player, result.ToString()) : Opaque(typeof(Condition), ctx.TextOf(g), nameof(G.Outcome));
            var saved = ctx.Subject;
            ctx.Subject = player;
            try { return new Not(new Did(LowerAs<Effect>(action, ctx))); }
            finally { ctx.Subject = saved; }
        });
        Rule<G.LifeComparedTo>((g, ctx) =>
        {
            Selector Of(object player) => player is G.WhichPlayer which ? Player(which) : LowerAs<Selector>(player, ctx);
            return new CompareCount(
                new LifeTotal(Of(g.Player.GetResolvedValue())),
                g.Comparison == G.Comparison.Greater ? Comparison.Greater : Comparison.Less,
                new LifeTotal(Of(g.Compared.GetResolvedValue())));
        });
        Rule<G.BothPlayersControl>((g, ctx) => new And([LowerAs<Condition>(g.First, ctx), LowerAs<Condition>(g.Second, ctx)]));
        Rule<G.TributeWasntPaid>((g, ctx) => new ChoiceMade(nameof(ChoiceWord.TributeNotPaid)));

        // Events.
        Rule<G.SpellTriggerEvent>((g, ctx) =>
            new SpellCastEvent(
                Player(g.Caster),
                g.Narrowed is null ? LowerAs<Selector>(g.Spell, ctx) : Narrow(LowerAs<Selector>(g.Spell, ctx), Lower(g.Narrowed, ctx), ctx, g.Narrowed),
                g.Verb == G.CastVerb.CastOrCopy));
        Rule<G.ExpendEvent>((g, ctx) => new NamedEvent(nameof(EventKind.Expend), new Fixed(g.Amount)));
        Rule<G.DayNightChanges>((g, ctx) => new NamedEvent(nameof(EventKind.DayNightChanged)));
        Rule<G.HauntTrigger>((g, ctx) =>
        {
            var haunted = new ObjectSelector(Quantifier.Any, new ObjectFilter(Kinds: [Kind(G.CardType.Creature)], With: [new Related(Relation.HauntedBy, new Self())]));
            Event dies = new ObjectEvent(haunted, ["Dies"]);
            return new TriggeredAbility(
                g.Enters is null ? dies : new EitherEvent([new ObjectEvent(new Self(), ["EntersTheBattlefield"]), dies]),
                LowerAs<Effect>(g.Effect, ctx));
        });
        Rule<G.WouldDrawWhen>((g, ctx) =>
        {
            var player = LowerAs<Selector>(g.Player, ctx);
            Condition when = g.When.GetResolvedValue() switch
            {
                G.ExceptFirstDraw => new DrawCircumstance(nameof(DrawCircumstanceKind.ExceptFirstDraw)),
                G.DuringStep during => new ItsPhase(PhaseRefOf(during, ctx) ?? new PhaseRef(ctx.TextOf(during))),
                var other => LowerAs<Condition>(other, ctx),
            };
            return new WouldHappen(player, new Draw(player, new Fixed(1)), when);
        });
    }

    static string ZoneNameOf(G.AllPlayersZone zone) =>
        zone switch
        {
            G.AllPlayersZone.Graveyards => nameof(Zone.Graveyard),
            G.AllPlayersZone.Libraries => nameof(Zone.Library),
            _ => nameof(Zone.Hand),
        };

    /// <summary>"Twice that many", "that many plus one", "half that many": an increase of an amount named earlier.</summary>
    static Quantity IncreasedOf(G.CounterIncrease increase) =>
        increase switch
        {
            G.CounterIncrease.Double => new Product(new Special("ThatMany"), new Fixed(2)),
            G.CounterIncrease.Triple => new Product(new Special("ThatMany"), new Fixed(3)),
            G.CounterIncrease.PlusOne => new Adjusted(new Special("ThatMany"), Adjustment: new Fixed(1)),
            _ => new Scaled(new Special("ThatMany"), nameof(G.ScaleFactor.Half)),
        };

    /// <summary>Mana spent to cast a spell, as an amount: all of it, mana of a color or of snow ("{s}"), or mana from a kind of source ("mana from a cave").</summary>
    Quantity ManaSpentAmountOf(G.ManaSpent spent, LoweringContext ctx)
    {
        var spell = LowerAs<Selector>(spent.Use, ctx);
        return spent.Mana.GetResolvedValue() switch
        {
            G.ManaCost { Items.Count: 1 } cost => new AmountOfManaSpent(spell, SymbolText(cost.Items[0]) == "{s}" ? nameof(G.SpentManaKind.Snow) : ColorOf(SymbolText(cost.Items[0]))),
            G.ManaFromSource source => new AmountOfManaSpent(spell, From: LowerAs<Selector>(source.Source, ctx)),
            G.AnyMana => new AmountOfManaSpent(spell),
            _ => (Quantity)Opaque(typeof(Quantity), ctx.TextOf(spent), nameof(G.ManaSpent)),
        };
    }

    /// <summary>Energy paid as a cost: "pay eight {e}", "pay {e}{e}{e}".</summary>
    Cost EnergyCostOf(G.EnergyAmount energy, LoweringContext ctx) =>
        Lower(energy, ctx) is Fixed { Value: var count } ? new PayMana(Enumerable.Repeat("{e}", count).ToList()) : (Cost)Opaque(typeof(Cost), ctx.TextOf(energy), nameof(G.EnergyAmount));
}
