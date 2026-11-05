namespace MTGRulesIR.Lowering;

/// <summary>
/// The sixth round: die-roll tables, "when the last counter is removed", affinity, keyword actions on objects,
/// graveyard-exile replacements, class levels, castable spells, chosen sources, named cards, spree modes, what mana
/// may be spent on, level brackets and their stats, cast restrictions, remaining in a state, skipping steps and
/// turns, mana spent conditions, compound triggers, who cast a spell from where, life thresholds, what a player did
/// this way, counted players, and damage redirection. Lines that restate a keyword's rules (madness, morph, unearth,
/// buyback, banding) lower to nothing, as reminder text does.
/// </summary>
public sealed partial class Lowerer
{
    void RegisterRoundSix()
    {
        // Frames.
        Rule<G.DieResultRow>((g, ctx) =>
        {
            var high = int.TryParse(new string((ctx.TextOf(g.Results.High) ?? "").Where(char.IsDigit).ToArray()), out var parsed) ? parsed : g.Results.Low;
            return new DieResultRow(g.Results.Low, high, LowerAs<Effect>(g.Effect, ctx));
        });
        Rule<G.LastTimeCounterRemoved>((g, ctx) => new TriggeredAbility(new NamedEvent(nameof(EventKind.LastCounterRemoved)), LowerAs<Effect>(g.Effect, ctx)));
        Rule<G.ClassLevelUp>((g, ctx) => new ActivatedAbility([new PayMana(g.Cost.Items.Select(SymbolText).ToList())], new GainClassLevel(g.Level)));
        Rule<G.SpreeMode>((g, ctx) => new Mode(LowerAs<Effect>(g.Effect, ctx), new PayMana(g.Cost.Items.Select(SymbolText).ToList())));
        Rule<G.LevelBracket>((g, ctx) =>
        {
            var text = ctx.TextOf(g.Levels.High) ?? "";
            var high = int.TryParse(new string(text.Where(char.IsDigit).ToArray()), out var parsed) ? parsed : 0;
            return new LevelRange(g.Levels.Low, high, text.Contains('+') || g.Levels.High.GetResolvedValue() is G.OrMore);
        });
        Rule<G.PowerToughnessBox>((g, ctx) =>
            Lower(g.Stats, ctx) is StatsMod stats ? new StatsLine(stats.Power, stats.Toughness) : Opaque(typeof(Ability), ctx.TextOf(g), nameof(G.PowerToughnessBox)));
        Rule<G.CastRestriction>((g, ctx) =>
            Lower(g.Timing, ctx) is Timing timing ? new ActivationLimits([timing], ForCasting: true) : Opaque(typeof(Ability), ctx.TextOf(g), nameof(G.CastRestriction)));
        Rule<G.CompoundTrigger>((g, ctx) =>
            new TriggeredAbility(new EitherEvent([LowerAs<Event>(g.First, ctx), LowerAs<Event>(g.Second, ctx)]), LowerAs<Effect>(g.Effect, ctx)));
        Rule<G.DurationFirstRule>((g, ctx) => new Lasting(LowerAs<Effect>(g.Effect, ctx), DurationOf(Lower(g.Duration, ctx))));

        // Text restating a keyword's rules means nothing to an engine that has the keyword.
        Rule<G.MadnessCast>((g, ctx) => null);
        Rule<G.TurnFaceUpForManaCost>((g, ctx) => null);
        Rule<G.UnearthExile>((g, ctx) => null);
        Rule<G.BuybackReturn>((g, ctx) => null);
        Rule<G.BandsBlockedAsGroup>((g, ctx) => null);
        Rule<G.BandingDamageRule>((g, ctx) => null);

        // Keywords and keyword actions.
        Rule<G.AffinityFor>((g, ctx) => new KeywordInstance(nameof(G.Keyword.Affinity), Parameter: g.Kind.ToString()));
        Rule<G.KeywordActionInstruction>((g, ctx) => Lower(g.Action, ctx));
        Rule<G.KeywordActionOn>((g, ctx) => new KeywordAction([new KeywordInstance(g.Action.ToString())], Objects: LowerAs<Selector>(g.Object, ctx)));

        // Effects.
        Rule<G.GraveyardExileReplacement>((g, ctx) =>
        {
            var spell = LowerAs<Selector>(g.Spell, ctx);
            var happening = g.Event.GetResolvedValue() is G.CounteredThisWay ? nameof(G.ThisWayAction.Countered) : nameof(G.PermanentEvent.IsPutIntoAGraveyard);
            return new Replacement(new ObjectEvent(spell, [happening]), LowerAs<Effect>(g.Replacement, ctx));
        });
        Rule<G.PreparedUnprepares>((g, ctx) =>
            new BecomesAspect(new Reference(null, "it"), [new Term(nameof(G.PermanentDesignation.Prepared), nameof(G.PermanentDesignation))], Negated: true, Verb: nameof(G.Assertion.Is)));
        Rule<G.Remains>((g, ctx) => new Remains(LowerAs<Selector>(g.Subject, ctx), g.State.ToString()));
        Rule<G.SkipNext>((g, ctx) =>
            g.Step.GetResolvedValue() switch
            {
                G.SkippedEvent skipped => new Skip(ctx.Subject, Event: skipped.ToString()),
                G.NextTurns turns => new Skip(PlayerOf(turns.Whose), new PhaseRef("Turn", Next: true), Turns: QuantityOf(turns.Count)),
                _ => PhaseRefOf(g.Step, ctx) is { } phase ? new Skip(ctx.Subject, phase) : Opaque(typeof(Effect), ctx.TextOf(g), nameof(G.SkipNext)),
            });
        Rule<G.DealDamageAmount>((g, ctx) =>
            new DealDamage(ctx.Subject ?? new Self(), [new DamagePortion(LowerAs<Quantity>(g.Amount, ctx), g.Recipient is null ? null : LowerAs<Selector>(g.Recipient.Permanent, ctx))]));
        Rule<G.RedirectDamage>((g, ctx) =>
        {
            var protectedValue = g.Protected?.GetResolvedValue();
            Selector from = protectedValue switch
            {
                G.ProtectedRecipient recipient => LowerAs<Selector>(recipient.Who, ctx),
                G.DealtToAndBy both => LowerAs<Selector>(both.Who, ctx),
                _ => null,
            };
            return new RedirectDamage(
                g.Amount is null ? new Special("All") : g.Amount.All ? new Special("All") : LowerAs<Quantity>(g.Amount.Next, ctx),
                from,
                g.NewRecipient is null ? null : LowerAs<Selector>(g.NewRecipient.Who, ctx),
                g.Source is null ? null : LowerAs<Selector>(g.Source.Sources, ctx),
                g.Combat,
                protectedValue is G.DealtToAndBy,
                g.ThisTurn || g.ThisTurnFirst || g.ThisTurnLeading ? new ThisTurn() : null);
        });

        // Mana uses.
        Rule<G.ActivateAbilitiesUse>((g, ctx) =>
            new Playable(nameof(UseVerb.Activate), new ObjectSelector(Quantifier.All, FilterOf(ctx, narrowing: [g.Which, g.Owner]) with { Kinds = [Kind(ObjectKind.Ability)] })));
        Rule<G.PayKeywordCostUse>((g, ctx) =>
            new Playable(nameof(UseVerb.PayKeywordCost), new ObjectSelector(Quantifier.All, new ObjectFilter(Kinds: [Kind(ObjectKind.Cost)], Qualifiers: [Q(g.Keyword)]))));
        Rule<G.TurnFaceUpUse>((g, ctx) => new Playable(nameof(UseVerb.TurnFaceUp), LowerAs<Selector>(g.What, ctx)));
        Rule<G.UnlockDoorUse>((g, ctx) => new Playable(nameof(UseVerb.UnlockDoor), new ObjectSelector(Quantifier.Any, new ObjectFilter(Kinds: [Kind(ObjectKind.Door)]))));

        // Selectors and predicates.
        Rule<G.CastableSpells>((g, ctx) =>
        {
            var count = g.Quantity?.GetResolvedValue();
            return new ObjectSelector(
                count is null ? Quantifier.Any : Quantifier.Count,
                FilterOf(ctx, qualifiers: g.Qualifier, narrowing: [g.With, g.From]) with { Kinds = [Kind("spell")] },
                Count: count switch { G.UpToCount upTo => LowerAs<Quantity>(upTo.Count, ctx), G.Quantity quantity => QuantityOf(quantity), _ => null },
                UpTo: count is G.UpToCount);
        });
        Rule<G.ChosenSource>((g, ctx) =>
        {
            var filter = FilterOf(ctx, qualifiers: g.Qualifiers, narrowing: [g.Aspect]);
            return new ObjectSelector(Quantifier.Any, filter with { Kinds = [Kind(ObjectKind.Source)], With = [.. filter.With ?? [], new ChosenBy(new PlayerSelector(PlayerKind.You))] });
        });
        Rule<G.NamedCard>((g, ctx) => new Named((ctx.TextOf(g) ?? "").Replace("named ", "")));
        Rule<G.ThisAbility>((g, ctx) => new SelfAbility());
        Rule<G.SpellNarrowing>((g, ctx) =>
            g.From is not null ? new CastFrom(ZoneOf(g.From, ctx) ?? new ZoneRef(OpenSlots.Placeholder))
            : g.During is not null ? new CastDuring(TurnOf(g.During.Turn))
            : g.Chosen is { } chosen ? new OfChosen(chosen.ToString())
            : g.FromOtherThanHand is { } otherThan ? new Not(new WasCast(new Reference(null, "it"), null, new ZoneRef(nameof(Zone.Hand), PlayerOf(otherThan.Whose))))
            : Lower((object)g.Targets ?? g.Colors ?? g.Feature ?? g.Symbol ?? (object)g.Ownership, ctx));
        Rule<G.CountedPlayers>((g, ctx) =>
            g.Who is not null
                ? Opaque(typeof(Selector), ctx.TextOf(g), nameof(G.CountedPlayers))
                : new PlayerSelector(g.Player.ToString() == "Opponent" ? PlayerKind.EachOpponent : g.Other == true ? PlayerKind.EachOtherPlayer : PlayerKind.EachPlayer));
        Rule<G.PlayersPhase>((g, ctx) => PlayersPhaseOf(g, ctx));
        Rule<G.SpellPaidFor>((g, ctx) => LowerAs<Selector>(g.Spell, ctx));
        Rule<G.AbilityPaidFor>((g, ctx) => new SelfAbility());

        // Conditions.
        Rule<G.ManaSpentCondition>((g, ctx) =>
            new ManaSpent(
                g.Amount?.GetResolvedValue() switch
                {
                    G.AtLeastQuantity atLeast => new Bound(QuantityOf(atLeast.Count), Comparison.GreaterOrEqual),
                    G.AtLeastCount atLeast => new Bound(LowerAs<Quantity>(atLeast.Count, ctx), Comparison.GreaterOrEqual),
                    G.Quantity quantity => QuantityOf(quantity),
                    _ => null,
                },
                LowerAs<Selector>(g.Use, ctx),
                g.Color?.ToString(),
                g.Exactly,
                g.Source is null ? null : LowerAs<Selector>(g.Source.Source, ctx)));
        Rule<G.CastByPlayerFrom>((g, ctx) => new WasCast(new Reference(null, "it"), Player(g.Player), g.From is null ? null : ZoneOf(g.From, ctx), g.Negated));
        Rule<G.LifeThreshold>((g, ctx) =>
            new CompareCount(new LifeTotal(LowerAs<Selector>(g.Player, ctx)), g.Bound == G.LifeBound.Less ? Comparison.LessOrEqual : Comparison.GreaterOrEqual, new Fixed(g.Life)));
        Rule<G.YouControlEnchanted>((g, ctx) => new Controls(new PlayerSelector(PlayerKind.You), LowerAs<Selector>(g.Enchanted, ctx)));
        Rule<G.YouDiscardedCard>((g, ctx) =>
            new Happened(new PlayerAction(new PlayerSelector(PlayerKind.You), nameof(G.PastPlayerAction.Discarded), LowerAs<Selector>(g.Card, ctx)), nameof(G.TurnSpan.ThisTurn)));
        Rule<G.PlayerActedThisWay>((g, ctx) =>
            new Happened(new PlayerAction(LowerAs<Selector>(g.Player, ctx), g.Action.ToString(), LowerAs<Selector>(g.Object, ctx)), nameof(EventSpan.ThisWay)));
    }

    /// <summary>A player's phase or step as a phase reference: "your upkeep", "each player's draw step".</summary>
    PhaseRef PlayersPhaseOf(G.PlayersPhase phase, LoweringContext ctx) =>
        new(phase.Phase.ToString(), Whose: phase.Whose?.GetResolvedValue() is G.Whose whose ? PlayerOf(whose) : phase.Whose is null ? null : LowerAs<Selector>(phase.Whose, ctx));

    /// <summary>A turn as a phase reference by whose it is: "during your turn", "during an opponent's turn".</summary>
    static PhaseRef TurnOf(G.TurnOwner owner) =>
        new("Turn", Whose: owner switch
        {
            G.TurnOwner.YourTurn => new PlayerSelector(PlayerKind.You),
            G.TurnOwner.OpponentsTurn => new PlayerSelector(PlayerKind.AnyOpponent),
            _ => new PlayerSelector(PlayerKind.EachOtherPlayer),
        });
}
