namespace MTGRulesIR.Lowering;

/// <summary>
/// The twelfth round: being a creature with base stats, animations with a quoted ability, named legendary tokens,
/// "those tokens" again, triggers that last for a time, the Doran rule, maximum hand sizes, having all activated
/// abilities of other objects, when a spell was cast, "during" a step, either of two game states, day and night,
/// damage of no stated amount, spells by what they target, abilities of a kind, relative characteristics, the
/// other cards revealed this way, kicker payments counted, the amount of mana spent, returns with the zone first,
/// a card returning to the battlefield, cards put in their zone this turn, kinship, choosing more modes, and craft,
/// rampage and champion as keywords.
/// Reconfigure's "while attached, this isn't a creature", "timing rules still apply" and "remove the words in
/// square brackets" lower to nothing.
/// </summary>
public sealed partial class Lowerer
{
    void RegisterRoundTwelve()
    {
        // Rules text the engine carries out itself, or that says nothing to it.
        Rule<G.WhileAttachedNotCreature>((g, ctx) => null);
        Rule<G.TimingRulesStillApply>((g, ctx) => null);
        Rule<G.RemoveSquareBrackets>((g, ctx) => null);

        // Keywords with parameters.
        Rule<G.CraftWith>((g, ctx) =>
            new KeywordInstance(nameof(G.Keyword.Craft), Cost: new PayMana(g.Cost.Items.Select(SymbolText).ToList()), Parameter: ctx.TextOf(g.Materials) ?? ctx.TextOf(g.Materials.GetResolvedValue())));
        Rule<G.Rampage>((g, ctx) => new KeywordInstance(nameof(G.Keyword.Rampage), QuantityOf(g.Bonus)));
        Rule<G.ChampionKeyword>((g, ctx) => new KeywordInstance(nameof(G.Keyword.Champion), Parameter: ctx.TextOf(g.Kind) ?? ctx.TextOf(g.Kind.GetResolvedValue())));

        // Animation.
        Rule<G.IsCreatureWithBaseStats>((g, ctx) =>
        {
            List<IrNode> other = [];
            var types = g.Types is null ? null : Terms(g.Types.Items, ctx, other);
            var subtypes = g.Subtypes is null ? null : Terms(g.Subtypes.Items, ctx, other);
            var colors = Terms(g.Colors, ctx, other);
            var baseStats = BaseStatsOf(Lower(g.Base, ctx));
            if (baseStats is null || other.Count > 0)
                return Opaque(typeof(Effect), ctx.TextOf(g), nameof(G.IsCreatureWithBaseStats));
            return new Animate(ctx.Subject ?? new Self(), Colors: OrNull(colors), Subtypes: subtypes is null ? null : OrNull(subtypes), Types: types is null ? null : OrNull(types), BaseStats: baseStats, InAddition: g.InAddition);
        });
        Rule<G.BecomesCreatureWithQuotedAbility>((g, ctx) =>
        {
            var subject = ctx.Subject ?? new Self();
            var stats = Lower(g.Stats, ctx) as StatsMod;
            List<IrNode> other = [];
            var types = Terms(g.Types.Types.Items, ctx, other);
            var baseNode = Lower(g.Types.Base, ctx);
            var baseStats = BaseStatsOf(baseNode);
            List<KeywordInstance> keywords = [];
            var inAddition = false;
            Ability granted;
            switch (g.Quoted.GetResolvedValue())
            {
                case G.WithQuotedAbility quoted:
                    granted = AsAbility(Lower(quoted.Ability, ctx), ctx, quoted.Ability);
                    break;
                case G.WithKeywordsAndQuotedAbility withKeywords:
                    keywords = KeywordsOf(withKeywords.Keywords, ctx, other);
                    granted = AsAbility(Lower(withKeywords.Ability, ctx), ctx, withKeywords.Ability);
                    break;
                case G.InAdditionAndGainsQuoted gains:
                    inAddition = true;
                    granted = AsAbility(Lower(gains.Ability, ctx), ctx, gains.Ability);
                    break;
                default:
                    return Opaque(typeof(Effect), ctx.TextOf(g), nameof(G.BecomesCreatureWithQuotedAbility));
            }
            var colors = Terms(g.Colors, ctx, other);
            var subtypes = g.Subtypes is null ? null : Terms(g.Subtypes.Items, ctx, other);
            if (baseNode is not null && baseStats is null && baseNode is not Effect || other.Count > 0)
                return Opaque(typeof(Effect), ctx.TextOf(g), nameof(G.BecomesCreatureWithQuotedAbility));
            List<Effect> steps =
            [
                new Animate(subject, stats?.Power, stats?.Toughness, OrNull(colors), subtypes is null ? null : OrNull(subtypes), OrNull(types), baseStats, OrNull(keywords), InAddition: inAddition),
            ];
            // "Artifact creature with power and toughness each equal to its mana value": the stats as their own effect.
            if (baseStats is null && baseNode is Effect sized)
                steps.Add(sized);
            steps.Add(new GrantAbility(subject, granted));
            return new Sequence(steps);
        });

        // Tokens.
        Rule<G.NamedLegendaryToken>((g, ctx) =>
        {
            List<IrNode> other = [];
            var keywords = KeywordsOf(g.With, ctx, other);
            var stats = Lower(g.Stats, ctx) as StatsMod;
            var text = ctx.TextOf(g) ?? "";
            var comma = text.IndexOf(", a legendary", StringComparison.Ordinal);
            var types = Terms(g.Types.Items, ctx, other);
            var subtypes = Terms(g.Subtypes.Items, ctx, other);
            var colors = Terms(g.Colors, ctx, other);
            var abilities = TokenAbilitiesOf(other, keywords);
            return new TokenSpecWithCount(
                new TokenSpec(OrNull(types), OrNull(subtypes), OrNull(colors), stats?.Power, stats?.Toughness, comma > 0 ? text[..comma] : null, OrNull(keywords), OrNull(abilities), Legendary: true),
                new Fixed(1));
        });
        Rule<G.CountOfThoseTokens>((g, ctx) => new TokenSpecWithCount(new ThoseTokens(), LowerAs<Quantity>(g.Count, ctx)));

        // A triggered ability that exists for a time: "until end of turn, whenever a player taps an island for mana, …".
        Rule<G.DurationScopedTrigger>((g, ctx) =>
            Lower(g.Trigger, ctx) is TriggeredAbility { If: null } trigger
                ? new Lasting(new Delayed(trigger.Trigger, trigger.Effect), DurationOf(Lower(g.Duration, ctx)))
                : Opaque(typeof(Effect), ctx.TextOf(g), nameof(G.DurationScopedTrigger)));

        // Continuous effects.
        Rule<G.AssignsDamageByToughness>((g, ctx) => new AssignsDamageByToughness(ctx.Subject ?? new Self()));
        Rule<G.MaximumHandSize>((g, ctx) =>
        {
            var player = LowerAs<Selector>(g.Whose, ctx);
            return g.Size.GetResolvedValue() switch
            {
                G.HandSizeChange change => new MaximumHandSize(player, Change: change.Direction == G.HandSizeChangeDirection.Reduced ? Signed(G.PlusMinus.Minus, QuantityOf(change.Amount)) : QuantityOf(change.Amount), RestOfGame: g.Duration is not null),
                G.Quantity size => new MaximumHandSize(player, QuantityOf(size), RestOfGame: g.Duration is not null),
                _ => Opaque(typeof(Effect), ctx.TextOf(g), nameof(G.MaximumHandSize)),
            };
        });
        Rule<G.HasAllActivatedAbilities>((g, ctx) =>
            new GainAbilitiesOf(ctx.Subject ?? new Self(), LowerAs<Selector>(g.Source, ctx), nameof(AbilityCategory.Activated), DurationOf(Lower(g.Duration, ctx))));
        Rule<G.EffectDoesntRemoveThis>((g, ctx) => new ProtectionDoesntRemoveThis());

        // Effects.
        Rule<G.CardReturnsToBattlefield>((g, ctx) =>
            new ChangeZone(LowerAs<Selector>(g.Card, ctx), new ZoneRef(nameof(Zone.Battlefield)), UnderControlOf: g.Control is null ? null : LowerAs<Selector>(g.Control, ctx)));
        Rule<G.ReturnToZoneFirst>((g, ctx) =>
            new ChangeZone(LowerAs<Selector>(g.Returned, ctx), ZoneOf(g.Destination, ctx) ?? new ZoneRef("?")));
        Rule<G.KinshipLook>((g, ctx) =>
        {
            var you = new PlayerSelector(PlayerKind.You);
            return new TriggeredAbility(new PhaseEvent(new PhaseRef(nameof(G.Phase.Upkeep), Whose: you)), new Optional(new LookAt(you, TopOfLibrary(new Fixed(1), you))));
        });
        Rule<G.ChooseModesInstead>((g, ctx) =>
        {
            Effect choose = new Instead(new ChooseModes(g.Count.ToString()));
            return g.May ? new Optional(choose) : choose;
        });

        // Conditions.
        Rule<G.DuringStep>((g, ctx) => new DuringPhase(PhaseRefOf(g.Step, ctx) ?? new PhaseRef(ctx.TextOf(g.Step))));
        Rule<G.CastThisSpellDuring>((g, ctx) => new WasCast(new Self(), Player(g.Player), During: PhaseRefOf(g.During, ctx)));
        Rule<G.GameStateEither>((g, ctx) => new Or([LowerAs<Condition>(g.First, ctx), LowerAs<Condition>(g.Second, ctx)]));
        Rule<G.IsDayOrNight>((g, ctx) => new DayNight(g.State.ToString()));

        // Quantities.
        Rule<G.TimesKicked>((g, ctx) => new Special(nameof(SpecialAmount.TimesKicked)));
        Rule<G.ManaSpentAmount>((g, ctx) =>
            new AmountOfManaSpent(LowerAs<Selector>(g.Spell, ctx), g.Kind == G.SpentManaKind.Snow ? nameof(G.SpentManaKind.Snow) : null));
        // Damage of no stated amount ("would deal combat damage to a player"): its kind, read by DamageTo.
        Rule<G.AnyDamage>((g, ctx) => g.Kind is { } kind ? new Term(kind.ToString(), nameof(G.DamageKind)) : null);

        // Selectors and predicates.
        Rule<G.ThatTarget>((g, ctx) =>
            new Targeting(g.Target.GetResolvedValue() is G.WhichPlayer player ? Player(player) : LowerAs<Selector>(g.Target, ctx)));
        Rule<G.WithRelativeCharacteristic>((g, ctx) =>
            new RelativeCharacteristic(g.Characteristic.ToString(), g.Comparison == G.Comparison.Less ? Comparison.Less : Comparison.Greater));
        Rule<G.OtherCardsRevealedThisWay>((g, ctx) =>
            new ObjectSelector(Quantifier.All, new ObjectFilter(
                Kinds: [Kind("card")],
                Qualifiers: [Q((ctx.TextOf(g) ?? "").Contains("exiled") ? AffectedThisWay.ExiledThisWay : AffectedThisWay.RevealedThisWay), Q(Distinction.Other)])));
        Rule<G.PutThereRecently>((g, ctx) =>
        {
            Condition arrived = new Happened(
                new ObjectEvent(null, [g.Origin == G.ArrivalOrigin.FromBattlefield ? nameof(ObjectEventKind.PutThereFromTheBattlefield) : nameof(ObjectEventKind.PutThere)]),
                g.Span.ToString());
            return new Satisfying(g.Arrival == G.Arrival.NotPutThere ? new Not(arrived) : arrived);
        });
        Rule<G.AbilitiesOfKind>((g, ctx) =>
        {
            List<Qualifier> qualifiers = [];
            switch (g.Kind?.GetResolvedValue())
            {
                case G.Keyword keyword:
                    qualifiers.Add(Q(keyword));
                    break;
                case G.AbilityClass.Activated:
                    qualifiers.Add(Q(AbilityCategory.Activated));
                    break;
                case G.AbilityClass.Loyalty:
                    qualifiers.Add(Q(AbilityCategory.Loyalty));
                    break;
                case G.AbilityClass.Mana:
                    qualifiers.Add(Q(AbilityCategory.Mana));
                    break;
                case G.AbilityClass kind:
                    qualifiers.Add(Q(kind));
                    break;
            }
            if (g.NonMana == true)
                qualifiers.Add(Q(AbilityCategory.Mana, negated: true));
            Controlled controller = null;
            List<Predicate> with = [];
            switch (g.Scope?.GetResolvedValue())
            {
                case G.YouActivate.YouActivate:
                    controller = new Controlled(new PlayerSelector(PlayerKind.You), nameof(UseVerb.Activate));
                    break;
                case G.YouActivate.OpponentsActivate:
                    controller = new Controlled(new PlayerSelector(PlayerKind.YourOpponents), nameof(UseVerb.Activate));
                    break;
                case G.AbilitiesOwner owner:
                    with.Add(new Related(Relation.AbilityOf, LowerAs<Selector>(owner.Permanents, ctx)));
                    break;
            }
            if (g.Targeting is not null && Lower(g.Targeting, ctx) is Predicate targeting)
                with.Add(targeting);
            return new ObjectSelector(Quantifier.All, new ObjectFilter(Kinds: [Kind(ObjectKind.Ability)], Qualifiers: OrNull(qualifiers), Controller: controller, With: OrNull(with)));
        });
    }
}
