namespace MTGRulesIR.Lowering;

/// <summary>
/// The fourteenth round: triggers of a source happening again, paying a cost repeatedly and what happens when you
/// do, being a color, a characteristic bound as a condition, a copy's quoted ability, controlling or having a card
/// in a zone, conditional cost changes, dying and being put into a graveyard as events a replacement waits for,
/// attachment events, alternative kinds wanted, airbend's cast rule, activation limits, activating abilities at
/// instant speed, the chosen player, "x is …" definitions, this or the enchanted creature, "that are enchanted",
/// a spell's colors, shuffling the rest, an amount plus a characteristic, the player with the most, playing from
/// among cards, permanents on the battlefield or attacking, named sources, life total floors and conditions, a
/// keyword cost paid, excess damage, ending the turn, having attacked or blocked, the player a creature attacks,
/// one of what was affected, abilities with alternative costs, others also attacking, power and toughness plus one,
/// being dealt damage, you and your permanents, having an ability, "where x is … as you cast", until cast from
/// exile, "unless you pay …, do", target cards of several kinds, removing all of them, qualified landwalk, playing
/// with hands revealed, paying and doing, the next draw replaced, movements not triggering, looking while exiled,
/// what it's blocking, exert, discarding a card named this, creatures that died, controlling a player, becoming
/// day or night, cards from outside the game, alternative costs for spells, and all creature types. Rules and
/// reminder text (meld, lifelink, indestructible, "damage wears off", cost floors, "choose the same mode more than
/// once", "reduces only colored mana") lowers to nothing.
/// </summary>
public sealed partial class Lowerer
{
    void RegisterRoundFourteen()
    {
        // Rules text the engine carries out itself, or that says nothing to it.
        Rule<G.SameModeMoreThanOnce>((g, ctx) => null);
        Rule<G.ReducesOnlyColoredMana>((g, ctx) => null);
        Rule<G.MeldsWith>((g, ctx) => null);
        Rule<G.LifelinkRule>((g, ctx) => null);
        Rule<G.DamageWearsOff>((g, ctx) => null);
        Rule<G.CostReductionFloor>((g, ctx) => null);
        Rule<G.StockReminder>((g, ctx) => null);
        Rule<G.IndestructibleRule>((g, ctx) => null);
        Rule<G.DiscardToHandSize>((g, ctx) => null);

        // Selectors.
        Rule<G.ChosenPlayer>((g, ctx) => new PlayerSelector(PlayerKind.Chosen));
        Rule<G.ThisOrAttached>((g, ctx) => new Union([new Self(), LowerAs<Selector>(g.Attached, ctx)], Connective.Or));
        Rule<G.YouAndPermanents>((g, ctx) => new Union([new PlayerSelector(PlayerKind.You), LowerAs<Selector>(g.Permanents, ctx)], Connective.And));
        Rule<G.NamedSources>((g, ctx) => new ObjectSelector(Quantifier.All, new ObjectFilter(Kinds: [Kind(ObjectKind.Source)], With: [PredicateOf(Lower(g.Name, ctx), ctx, g.Name)])));
        // "The player or planeswalker it's attacking": the defending player, as the engine attacks players.
        Rule<G.AttackedByIt>((g, ctx) => new PlayerSelector(PlayerKind.DefendingPlayer));
        Rule<G.AnAffectedThisWay>((g, ctx) =>
            Lower(g.Affected, ctx) is ObjectSelector affected ? affected with { Quantifier = Quantifier.Any } : Opaque(typeof(Selector), ctx.TextOf(g), nameof(G.AnAffectedThisWay)));
        Rule<G.PlayerWithMost>((g, ctx) =>
        {
            var others = new PlayerSelector(PlayerKind.EachOtherPlayer);
            Predicate most = g.Measure.GetResolvedValue() switch
            {
                G.PlayerSuperlative.LowestLifeTotal => new Compared(nameof(G.MoreOrFewer.Fewer), others),
                G.PlayerSuperlative.MostCardsInHand => new Compared(nameof(G.MoreOrFewer.More), others, new ObjectSelector(Quantifier.All, new ObjectFilter(Kinds: [Kind("card")], Zone: new ZoneRef(nameof(Zone.Hand), new Reference(null, "who"))))),
                G.ControlsTheMost controls => new Compared(nameof(G.MoreOrFewer.More), others, LowerAs<Selector>(controls.Kind, ctx)),
                _ => new Compared(nameof(G.MoreOrFewer.More), others),
            };
            return new ObjectSelector(Quantifier.Any, new ObjectFilter(Kinds: [Kind("player")], With: [most]));
        });
        Rule<G.TargetCardsOfKindsInZone>((g, ctx) =>
        {
            var zone = ZoneOf(g.Zone, ctx);
            List<Selector> targets = [];
            foreach (var card in g.Cards.Items)
            {
                var filter = FilterOf(ctx, qualifiers: card.Qualifiers) with { Kinds = [Kind("card")], Zone = zone };
                targets.Add(new ObjectSelector(Quantifier.Target, filter, card.UpTo is null ? null : LowerAs<Quantity>(card.UpTo, ctx), card.UpTo is not null, Binding: ctx.Bind(card)));
            }
            return new Union(targets, Connective.And);
        });
        Rule<G.CreaturesThatDied>((g, ctx) =>
        {
            var filter = FilterOf(ctx, kinds: g.Kind, qualifiers: g.Qualifiers) with
            {
                Zone = new ZoneRef(nameof(Zone.Graveyard), g.Died == G.DiedWording.PutIntoYourGraveyard ? new PlayerSelector(PlayerKind.You) : new PlayerSelector(PlayerKind.EachPlayer)),
            };
            var died = new ObjectEvent(null, ["Died"], UnderControlOf: g.Under is null ? null : LowerAs<Selector>(g.Under, ctx));
            return new ObjectSelector(Quantifier.All, filter with { With = [.. filter.With ?? [], new Satisfying(new Happened(died, g.When == G.DeathScope.ThisWay ? nameof(EventSpan.ThisWay) : nameof(G.TurnSpan.ThisTurn)))] });
        });
        Rule<G.CardsFromOutsideGame>((g, ctx) =>
            LowerAs<Selector>(g.Wanted, ctx) is ObjectSelector wanted
                ? wanted with { Filter = (wanted.Filter ?? new ObjectFilter()) with { Zone = new ZoneRef(nameof(Zone.OutsideTheGame), new PlayerSelector(PlayerKind.You)), Controller = new Controlled(new PlayerSelector(PlayerKind.You), "Own") } }
                : Opaque(typeof(Selector), ctx.TextOf(g), nameof(G.CardsFromOutsideGame)));

        // Predicates.
        Rule<G.ThatAreAttached>((g, ctx) => Q(g.Attachment));
        Rule<G.SpellColors>((g, ctx) => new QualifierChoice(g.Colors.Items.Select(x => Q(x)).ToList(), Connective.And));
        Rule<G.AlternativeWantedKind>((g, ctx) => Lower(g.Kind, ctx) is Term kind ? new QualifiedKind(kind, []) : Opaque(typeof(Predicate), ctx.TextOf(g), nameof(G.AlternativeWantedKind)));
        Rule<G.WithAbilityNamed>((g, ctx) => new HasKeywords([g.Keyword.ToString()], g.Having == G.Having.Without));
        Rule<G.InCombatWithIt>((g, ctx) =>
            g.Status == G.InCombatStatus.Blocking
                ? new InCombatWith(new Reference(null, "it"), nameof(CombatRelation.BlockedBy))
                : Opaque(typeof(Predicate), ctx.TextOf(g), nameof(G.InCombatWithIt)));

        // Quantities.
        Rule<G.SacrificedCharacteristicPlus>((g, ctx) =>
            new Adjusted(new CharacteristicOf(LowerAs<Selector>(g.Object, ctx), g.Characteristic.ToString()), Adjustment: QuantityOf(g.Addend)));
        Rule<G.ExcessDamageAmount>((g, ctx) => new AmountThisWay(nameof(ThisWayAmountKind.ExcessDamage)));

        // Conditions.
        Rule<G.HasCharacteristicBound>((g, ctx) =>
        {
            var (comparison, amount) = BoundOf(g.Bound.GetResolvedValue(), ctx);
            return new CompareCount(new CharacteristicOf(LowerAs<Selector>(g.Object, ctx), g.Characteristic.ToString()), comparison, amount);
        });
        Rule<G.ControlsOrCardInZone>((g, ctx) => new Or([LowerAs<Condition>(g.Controls, ctx), LowerAs<Condition>(g.CardInZone, ctx)]));
        Rule<G.LifeTotalCondition>((g, ctx) =>
        {
            var life = LowerAs<Quantity>(g.Total, ctx);
            switch (g.Bound.GetResolvedValue())
            {
                case G.NumericBound numeric:
                    var (comparison, amount) = BoundOf(numeric, ctx);
                    return new CompareCount(life, comparison, amount);
                case G.LifeComparison compared:
                {
                    var than = compared.Than.GetResolvedValue() is G.Quantity quantity ? QuantityOf(quantity) : LowerAs<Quantity>(compared.Than, ctx);
                    var how = compared.Comparison == G.Comparison.Greater
                        ? compared.OrEqual ? Comparison.GreaterOrEqual : Comparison.Greater
                        : compared.OrEqual ? Comparison.LessOrEqual : Comparison.Less;
                    return new CompareCount(life, how, than);
                }
                default:
                    return Opaque(typeof(Condition), ctx.TextOf(g), nameof(G.LifeTotalCondition));
            }
        });
        Rule<G.KeywordCostWasPaid>((g, ctx) => new WasCastWith(LowerAs<Selector>(g.Whose, ctx), g.Keyword?.ToString() ?? "AdditionalCost"));
        Rule<G.AttackedOrBlocked>((g, ctx) =>
            new Happened(new ObjectEvent(LowerAs<Selector>(g.Combatant, ctx), [nameof(ObjectEventKind.AttackedOrBlocked)]), "This" + g.Period.Period));
        Rule<G.OthersAlsoInCombat>((g, ctx) =>
            new Happened(new ObjectEvent(LowerAs<Selector>(g.Who, ctx), [g.Action == G.CombatAction.Block ? "Blocks" : "Attacks"]), nameof(EventSpan.ThisCombat)));
        Rule<G.PermanentsAreOnBattlefield>((g, ctx) =>
            LowerAs<Selector>(g.What, ctx) is ObjectSelector permanents ? new ThereAre(permanents) : Opaque(typeof(Condition), ctx.TextOf(g), nameof(G.PermanentsAreOnBattlefield)));
        Rule<G.PermanentsAreAttacking>((g, ctx) =>
            LowerAs<Selector>(g.What, ctx) is ObjectSelector permanents
                ? new ThereAre(Narrow(permanents, Q(G.CombatState.Attacking), ctx))
                : Opaque(typeof(Condition), ctx.TextOf(g), nameof(G.PermanentsAreAttacking)));

        // Events.
        Rule<G.AttachmentEvent>((g, ctx) =>
            new ObjectEvent(LowerAs<Selector>(g.Attachment, ctx), [g.Change == G.AttachmentChange.Attached ? nameof(ObjectEventKind.BecomesAttached) : nameof(ObjectEventKind.BecomesUnattached)], Other: LowerAs<Selector>(g.Host, ctx)));
        Rule<G.YouAreDealtDamage>((g, ctx) => new PlayerEvent(new PlayerSelector(PlayerKind.You), nameof(PlayerEventWord.IsDealtDamage)));
        // "If this creature would die", "if {this} would be put into a graveyard from anywhere": the event a replacement waits for (see WouldHappen).
        Rule<G.Die>((g, ctx) => new ObjectEvent(null, ["Dies"]));
        Rule<G.BePutIntoGraveyard>((g, ctx) =>
            new ObjectEvent(null, [g.From switch
            {
                G.GraveyardOrigin.FromTheBattlefield => nameof(G.PermanentEvent.IsPutIntoAGraveyardFromTheBattlefield),
                G.GraveyardOrigin.FromAnywhere => nameof(G.PermanentEvent.IsPutIntoAGraveyardFromAnywhere),
                _ => nameof(G.PermanentEvent.IsPutIntoAGraveyard),
            }], ZoneOwner: g.Whose == G.Whose.Any ? null : PlayerOf(g.Whose)));

        // Durations.
        Rule<G.UntilCastFromExile>((g, ctx) => new UntilEvent(new ObjectEvent(new Self(), [nameof(ObjectEventKind.Cast)])));

        // Costs.
        Rule<G.Exert>((g, ctx) => new Exert(LowerAs<Selector>(g.Exerted, ctx)));
        Rule<G.DiscardNamedCard>((g, ctx) =>
            new Discard(Actor(ctx), new Fixed(1), new ObjectFilter(Kinds: [Kind("card")], Qualifiers: [Q(Distinction.NamedThis), .. (ctx.TextOf(g) ?? "").Contains("another") ? new[] { Q(Distinction.Other) } : []])));
        Rule<G.PayAndDo>((g, ctx) => new PayCost(new CompoundCost([LowerAs<Cost>(g.Cost, ctx), new ActionCost(LowerAs<Effect>(g.Action, ctx))]), ctx.Subject));
        Rule<G.PayCostRepeatedly>((g, ctx) =>
            new PayCost(LowerAs<Cost>(g.Cost, ctx), ctx.Subject, Times: g.Times switch
            {
                G.PaymentRepetition.UpToTwoTimes => new Bound(new Fixed(2), Comparison.LessOrEqual),
                G.PaymentRepetition.UpToThreeTimes => new Bound(new Fixed(3), Comparison.LessOrEqual),
                _ => new Special("AnyNumber"),
            }));
        Rule<G.AlternativeCostsAbility>((g, ctx) =>
            new ActivatedAbility(
                [new CostChoice([new CompoundCost(LowerAllAs<Cost>(g.Costs, ctx)), new CompoundCost(LowerAllAs<Cost>(g.OtherCosts, ctx))])],
                LowerAs<Effect>(g.Effect, ctx)));

        // Effects.
        Rule<G.SourceTriggersAgain>((g, ctx) => new AdditionalTriggers(null, null, LowerAs<Selector>(g.Source, ctx)));
        Rule<G.IsColor>((g, ctx) => new BecomesAspect(ctx.Subject ?? new Self(), TermsOf(g.Colors, ctx), Verb: g.Verb.ToString()));
        Rule<G.CopyGrantsQuoted>((g, ctx) =>
        {
            var saved = ctx.Subject;
            ctx.Subject = new Reference(null, "it");
            try { return LowerAs<Effect>(g.Grant, ctx); }
            finally { ctx.Subject = saved; }
        });
        Rule<G.ConditionalCostModification>((g, ctx) =>
            new CostModification(
                LowerAs<Selector>(g.Spells, ctx),
                new PayMana(g.Amount.Items.Select(SymbolText).ToList()),
                g.Change.ToString(),
                (ctx.TextOf(g) ?? "").Contains("to activate") ? nameof(G.CostedAction.Activate) : nameof(G.CostedAction.Cast),
                Condition: LowerAs<Condition>(g.Condition, ctx)));
        Rule<G.AirbendCastRule>((g, ctx) =>
        {
            var it = new Reference(null, "it");
            return new MayPlayFrom(new PlayerSelector(PlayerKind.OwnerOf, it), [new Playable(nameof(G.PlayVerb.Cast), it)], new ZoneRef(nameof(Zone.Exile)), Using: new PayMana(g.Cost.Items.Select(SymbolText).ToList()));
        });
        Rule<G.ActivationLimit>((g, ctx) =>
            new ActivationLimits([new Timing(nameof(TimingKind.TimesEachTurn), Times: g.Times switch
            {
                G.CopyRepetition.Twice => new Fixed(2),
                G.CopyRepetition.ThreeTimes => new Fixed(3),
                G.CopyRepetition.XTimes => new Variable("X"),
                _ => new Fixed(1),
            })]));
        Rule<G.ActivateAbilities>((g, ctx) =>
        {
            if (g.Timing.GetResolvedValue() is not G.InstantSpeed)
                return Opaque(typeof(Effect), ctx.TextOf(g), nameof(G.ActivateAbilities));
            var kind = g.Abilities.Kind.GetResolvedValue();
            List<Qualifier> qualifiers = kind switch
            {
                G.Keyword keyword => [Q(keyword)],
                G.AbilityClass.Loyalty => [Q(AbilityCategory.Loyalty)],
                G.AbilityClass.Mana => [Q(AbilityCategory.Mana)],
                G.AbilityClass.Activated => [Q(AbilityCategory.Activated)],
                G.AbilityClass other => [Q(other)],
                _ => [],
            };
            var abilities = new ObjectSelector(Quantifier.All, new ObjectFilter(
                Kinds: [Kind(ObjectKind.Ability)],
                Qualifiers: OrNull(qualifiers),
                With: g.Abilities.Holder is null ? null : [new Related(Relation.AbilityOf, LowerAs<Selector>(g.Abilities.Holder.Holder, ctx))]));
            return new ActivateAnyTime(abilities, g.AnyPlayersTurn == true);
        });
        Rule<G.VariableDefinition>((g, ctx) => new LetVariable(null, "X", LowerAs<Quantity>(g.Definition, ctx)));
        Rule<G.WhereXIsAsCast>((g, ctx) => new LetVariable(LowerAs<Effect>(g.Effect, ctx), "X", LowerAs<Quantity>(g.Definition, ctx)));
        Rule<G.ShuffleRestIntoLibrary>((g, ctx) =>
            new ShuffleInto(new ObjectSelector(Quantifier.Rest, new ObjectFilter(Kinds: [Kind("card")])), ZoneOf(g.Library, ctx) ?? new ZoneRef(nameof(Zone.Library), Actor(ctx))));
        Rule<G.LifeTotalFloor>((g, ctx) => new LifeTotalFloor(PlayerOf(g.Whose), QuantityOf(g.Floor), QuantityOf(g.ReducedTo)));
        Rule<G.EndTheTurn>((g, ctx) => new EndTheTurn());
        Rule<G.PowerEqualToughnessPlus>((g, ctx) =>
        {
            var objects = LowerAs<Selector>(g.Object, ctx);
            var amount = LowerAs<Quantity>(g.Amount, ctx);
            return Seq(
                new SetStats(objects, nameof(G.PowerAndOrToughness.Power), amount),
                new SetStats(objects, nameof(G.PowerAndOrToughness.Toughness), new Adjusted(amount, Adjustment: QuantityOf(g.ToughnessBonus))));
        });
        Rule<G.UnlessPaidFirst>((g, ctx) => new UnlessPaid(LowerAs<Effect>(g.Effect, ctx), PaymentOf(Lower(g.Payment, ctx), ctx, g.Payment)));
        Rule<G.RemoveNamedCounters>((g, ctx) =>
            new RemoveCounters(g.Place is null ? new Reference(null, "it") : LowerAs<Selector>(g.Place, ctx), new CounterSpec(null, new Special("All"))));
        Rule<G.QualifiedLandwalk>((g, ctx) => new KeywordInstance(g.Keyword.ToString(), Parameter: ctx.TextOf(g.Qualifier)));
        Rule<G.PlayWithHandRevealed>((g, ctx) => new PlayWithHandRevealed(g.Players is { } players ? Player(players) : PlayerOf(g.Whose)));
        Rule<G.PaidThisCostTrigger>((g, ctx) => new WhenDone(new PlayerSelector(PlayerKind.You), LowerAs<Effect>(g.Effect, ctx)));
        Rule<G.NextTimeWouldDraw>((g, ctx) =>
        {
            var player = Player(g.Player);
            var replacement = g.Replacement.GetResolvedValue() switch
            {
                G.InsteadThenEffect instead => LowerAs<Effect>(instead.Effect, ctx),
                G.EffectThenInstead then => LowerAs<Effect>(then.Effect, ctx),
                _ => (Effect)Opaque(typeof(Effect), ctx.TextOf(g.Replacement), nameof(G.NextTimeWouldDraw)),
            };
            return new Replacement(new WouldHappen(player, new Draw(player, new Fixed(1))), new Instead(replacement), new ThisTurn(), NextTime: true);
        });
        Rule<G.MovementsDontTrigger>((g, ctx) =>
            new EventsDontTrigger(LowerAs<Selector>(g.Kinds, ctx), Terms(g.Movements, ctx)));
        Rule<G.LookAtWhileExiled>((g, ctx) => new LookAt(Actor(ctx), LowerAs<Selector>(g.Card, ctx), AnyTime: true));
        Rule<G.ControlPlayerDuringTurn>((g, ctx) => new ControlPlayer(LowerAs<Selector>(g.Player, ctx), g.Period.ToString()));
        Rule<G.BecomesDayOrNight>((g, ctx) => new SetDayNight(g.State.ToString()));
        // "Gains all creature types": an animation adding every creature type; "loses all creature types" takes them away.
        Rule<G.AllCreatureTypes>((g, ctx) => new Animate(ctx.Subject ?? new Self(), AllCreatureTypes: true));

        // Alternative costs for spells other than this one: "you may pay {w}{u}{b}{r}{g} rather than pay the mana cost for spells you cast".
        Rule<G.CostForSpells>((g, ctx) => new AlternativeCost(null, g.Kind.ToString(), Spells: LowerAs<Selector>(g.Spells, ctx)));
        Rule<G.ReplacedKeywordCost>((g, ctx) => new AlternativeCost(null, g.Keyword.ToString()));

        // How a card may be played from elsewhere: "by paying {2} rather than paying its mana cost", "using its flashback ability".
        Rule<G.ByPayingInstead>((g, ctx) => new PayMana(g.Mana.Items.Select(SymbolText).ToList()));
        Rule<G.UsingKeywordAbility>((g, ctx) => new KeywordCost(new KeywordInstance(g.Keyword.ToString())));
    }

    /// <summary>"5 or greater", "less than or equal to its power": a comparison and the amount compared against.</summary>
    (Comparison Comparison, Quantity Amount) BoundOf(object bound, LoweringContext ctx) =>
        bound switch
        {
            G.NumericBound numeric => (numeric.Comparison == G.Comparison.Less ? Comparison.LessOrEqual : Comparison.GreaterOrEqual, QuantityOf(numeric.Quantity)),
            G.BoundByAmount byAmount => (byAmount.Comparison == G.Comparison.Less ? Comparison.LessOrEqual : Comparison.GreaterOrEqual, LowerAs<Quantity>(byAmount.Amount, ctx)),
            _ => (Comparison.Equal, (Quantity)Opaque(typeof(Quantity), ctx.TextOf(bound), bound?.GetType().Name)),
        };
}
