namespace MTGRulesIR.Lowering;

/// <summary>Effects: the predicates of a subject, and the imperatives a spell or ability states.</summary>
public sealed partial class Lowerer
{
    void RegisterEffects()
    {
        Rule<G.DestroyTarget>((g, ctx) =>
        {
            var objects = LowerAs<Selector>(g.Destroyed, ctx);
            var manner = g.Manner?.ToString();

            Effect effect = g.Verb == G.RemovalVerb.Exile
                ? new Exile(objects, g.UntilThisLeaves is null ? null : new UntilThisLeaves(), g.WithCounters is null ? null : CountersOf(Lower(g.WithCounters, ctx)), manner)
                : new Destroy(objects, manner);

            return g.AtEndOfCombat ? new Delayed(new PhaseEvent(new PhaseRef("EndOfCombatStep", "End")), effect) : effect;
        });

        Rule<G.DrawOrDiscardCards>((g, ctx) =>
        {
            var count = LowerAs<Quantity>(g.Quantity, ctx);

            if (g.CardVerb == G.CardVerb.Draw)
                return new Draw(Actor(ctx), count, g.UpTo == true, g.Additional == true);

            var filter = g.Qualifier is null && g.Chosen is null ? null : FilterOf(ctx, qualifiers: g.Qualifier, narrowing: [g.Chosen]);
            return new Discard(Actor(ctx), count, filter, g.AtRandom);
        });

        Rule<G.TapOrUntap>((g, ctx) =>
        {
            var objects = LowerAs<Selector>(g.Permanent, ctx);
            var during = g.During is null ? null : Lower(g.During, ctx);

            return g.Action switch
            {
                G.TapAction.Tap => new Tap(objects, during),
                G.TapAction.Untap => new Untap(objects, during),
                _ => new TapOrUntap(objects, during),
            };
        });

        Rule<G.GainsBuffs>((g, ctx) =>
        {
            var duration = DurationOf(Lower(g.Duration, ctx));
            var perEach = g.PerEach is null ? null : LowerAs<Quantity>(g.PerEach, ctx);
            List<Effect> pumps = [];

            foreach (var node in LowerAll(g.Buffs, ctx))
                pumps.Add(node is BuffGroup group ? PumpOf(group, duration, perEach, ctx) : EffectOf(node));

            return Seq([.. pumps]);
        });

        Rule<G.GainedOrLostBuff>((g, ctx) =>
            new BuffGroup(g.PermanentVerb, LowerAll(g.Buff, ctx), g.Additional == true));

        Rule<G.PowerToughnessMod>((g, ctx) =>
            new StatsMod(Signed(g.PowerSign, QuantityOf(g.PowerValue)), Signed(g.ToughnessSign, QuantityOf(g.ToughnessValue))));

        Rule<G.PowerToughness>((g, ctx) => new StatsMod(QuantityOf(g.Power), QuantityOf(g.Toughness)));

        Rule<G.DealDamage>((g, ctx) =>
            new DealDamage(ctx.Subject ?? new Self(), LowerAll(g.Portions, ctx).Select(x => x as DamagePortion ?? new DamagePortion((Quantity)Opaque(typeof(Quantity), (x as IOpaque)?.Text ?? x.ToString(), x.GetType().Name), null)).ToList()));

        Rule<G.DamageTo>((g, ctx) =>
            new DamagePortion(g.EqualTo is null ? LowerAs<Quantity>(g.Amount, ctx) : LowerAs<Quantity>(g.EqualTo, ctx), LowerAs<Selector>(g.Recipient, ctx)));

        Rule<G.DamageAmount>((g, ctx) => QuantityOf(g.Quantity));

        Rule<G.GainOrLoseLife>((g, ctx) =>
        {
            var amount = Multiply(LowerAs<Quantity>(g.Amount, ctx), g.Multiplier is null ? null : LowerAs<Quantity>(g.Multiplier, ctx));
            return g.GainOrLose == G.GainOrLose.Gain ? new GainLife(Actor(ctx), amount) : new LoseLife(Actor(ctx), amount);
        });

        Rule<G.LifeQuantity>((g, ctx) => QuantityOf(g.Quantity));

        Rule<G.PlaceCounters>((g, ctx) =>
        {
            var objects = g.Place is null ? ctx.Subject ?? new Self() : LowerAs<Selector>(g.Place.Permanent, ctx);
            if (Lower(g.Counters, ctx) is not CounterSpec counters)
                return Opaque(typeof(Effect), ctx.TextOf(g), nameof(G.PlaceCounters));
            return g.Action == G.CounterAction.Remove ? new RemoveCounters(objects, counters) : new PutCounters(objects, counters);
        });

        Rule<G.Counters>((g, ctx) =>
        {
            var kind = g.Kind?.GetResolvedValue() switch
            {
                G.CounterType type => PatternOf(type),
                G.PowerToughnessMod mod => ctx.TextOf(mod),
                _ => null,
            };

            return new CounterSpec(kind, LowerAs<Quantity>(g.Quantity, ctx), g.Additional, g.Bound?.ToString(), g.Prefix?.ToString());
        });

        Rule<G.CreateToken>((g, ctx) =>
        {
            var token = Lower(g.Token, ctx);
            var count = g.EqualTo is not null ? LowerAs<Quantity>(g.EqualTo, ctx) : token is TokenSpecWithCount withCount ? withCount.Count : null;
            return new CreateTokens(TokenOf(token, ctx, g.Token), count, g.PerEach is null ? null : LowerAs<Quantity>(g.PerEach, ctx));
        });

        Rule<G.TokenDescription>((g, ctx) =>
        {
            List<IrNode> other = [];
            var keywords = KeywordsOf(g.With, ctx, other);
            var stats = Lower(g.Stats, ctx) as StatsMod;
            var name = g.Name ?? g.NameAfterKeywords;
            var types = Terms(g.Types.Items, ctx, other);
            var subtypes = g.Subtypes is null ? null : Terms(g.Subtypes.Items, ctx, other);
            var colors = Terms(g.Colors, ctx, other);
            var abilities = TokenAbilitiesOf(other, keywords);

            return new TokenSpecWithCount(
                new TokenSpec(
                    OrNull(types),
                    subtypes is null ? null : OrNull(subtypes),
                    OrNull(colors),
                    stats?.Power,
                    stats?.Toughness,
                    name is null ? null : ctx.TextOf(name)?.Replace("named ", ""),
                    OrNull(keywords),
                    OrNull(abilities),
                    g.Tapped is G.EntryState.Tapped or G.EntryState.TappedAndAttacking,
                    g.Tapped is G.EntryState.TappedAndAttacking || g.Attacking is not null),
                QuantityOf(g.Quantity));
        });

        Rule<G.PredefinedTokens>((g, ctx) =>
            new TokenSpecWithCount(new TokenSpec(Predefined: g.Kind.ToString(), Tapped: g.Tapped), QuantityOf(g.Quantity)));

        Rule<G.Sacrifice>((g, ctx) =>
            new Sacrifice(Actor(ctx), g.ChosenBy is null ? LowerAs<Selector>(g.Sacrificed, ctx) : Narrow(LowerAs<Selector>(g.Sacrificed, ctx), Lower(g.ChosenBy, ctx), ctx, g.ChosenBy)));

        Rule<G.ReturnCard>((g, ctx) =>
            new ChangeZone(LowerAs<Selector>(g.Returned, ctx), ZoneOf(g.Destination, ctx)));

        Rule<G.PutIntoZone>((g, ctx) =>
        {
            var destination = g.Destination.GetResolvedValue();
            var zone = ZoneOf(destination, ctx) ?? new ZoneRef("?");
            var tapped = g.Tapped == true;

            if (destination is G.ToTheBattlefieldUnderControl battlefield)
                return ToBattlefield(LowerAs<Selector>(g.Moved, ctx), battlefield, ctx) with { Tapped = tapped || (battlefield.State is G.EntryState.Tapped or G.EntryState.TappedAndAttacking || battlefield.StateAfterControl is G.EntryState.Tapped or G.EntryState.TappedAndAttacking) };

            List<Effect> with = [];
            if (destination is not G.IntoZone and not null)
                with.AddRange(EffectsOf(LowerAll(destination, ctx).Where(x => x is not ZoneRef)));

            return new ChangeZone(LowerAs<Selector>(g.Moved, ctx), zone, tapped, With: OrNull(with));
        });

        Rule<G.AddMana>((g, ctx) =>
            new AddMana(LowerAll(g.Mana, ctx).Select(ManaOptionOf).ToList(), g.Amount?.ToString()));

        Rule<G.ManaCost>((g, ctx) => new ManaAmount(g.Items.Select(SymbolText).ToList()));
        Rule<G.ManaSymbol>((g, ctx) => new Term(SymbolText(g), nameof(G.ManaSymbol)));

        Rule<G.CantBeTargeted>((g, ctx) =>
        {
            List<IrNode> rest = [];
            var by = Terms(g.By, ctx, rest);
            if (by.Count == 0 || rest.Count > 0)
                return Opaque(typeof(Effect), ctx.TextOf(g), nameof(G.CantBeTargeted));
            return new CantBeTargeted(ctx.Subject ?? new Self(), by, g.Whose is null ? null : Lower(g.Whose, ctx) as Controlled, g.Period is null ? null : new ThisTurn(g.Period.Period.ToString()));
        });

        Rule<G.CombatRestriction>((g, ctx) =>
        {
            List<IrNode> rest = [];
            var actions = Terms(g.Action, ctx, rest);
            Selector by = null;
            string recipient = null;
            List<string> modifiers = [];
            List<Condition> conditions = [];

            foreach (var part in new object[] { g.Attacked, g.OrAction, g.By, g.AndOr, g.Blocked, g.Blockee, g.Condition })
                foreach (var node in LowerAll(part, ctx))
                    switch (node)
                    {
                        case Selector selector:
                            by = by is null ? selector : new Union([by, selector], Connective.Or);
                            break;
                        case Term { Vocabulary: nameof(G.CombatAction) } action:
                            actions.Add(action.Value);
                            break;
                        case Condition condition:
                            conditions.Add(condition);
                            break;
                        case Term { Vocabulary: nameof(G.AttackRecipient) } term:
                            recipient = term.Value;
                            break;
                        case Term { Vocabulary: nameof(G.CombatRestriction) } term:
                            modifiers.Add(term.Value);
                            break;
                        default:
                            rest.Add(node);
                            break;
                    }

            if (rest.Count > 0)
                return Opaque(typeof(Effect), ctx.TextOf(g), nameof(G.CombatRestriction));
            if (g.Alone)
                modifiers.Add("Alone");
            if (g.Except)
                modifiers.Add("Except");

            var duration = DurationOf(Lower(g.Duration ?? (object)g.LaterDuration ?? g.Blockee?.Duration, ctx));
            return new CombatRestriction(ctx.Subject ?? new Self(), actions, by, recipient, OrNull(modifiers), conditions.Count == 0 ? null : conditions.Count == 1 ? conditions[0] : new And(conditions), duration);
        });

        Rule<G.ForEach>((g, ctx) => Lower(g.Counted, ctx) is Quantity counted ? counted : new CountOf(LowerAs<Selector>(g.Counted, ctx)));
        Rule<G.EqualTo>((g, ctx) => LowerAs<Quantity>(g.Measure, ctx));
        Rule<G.NumberOf>((g, ctx) =>
            Lower(g.Counted, ctx) is Quantity counted ? counted
            : new CountOf(g.Other == true ? Narrow(LowerAs<Selector>(g.Counted, ctx), Q(Distinction.Other), ctx) : LowerAs<Selector>(g.Counted, ctx)));
        Rule<G.UpToCount>((g, ctx) => LowerAs<Quantity>(g.Count, ctx));
        Rule<G.AtLeastCount>((g, ctx) => LowerAs<Quantity>(g.Count, ctx));
        Rule<G.EntersWithCounters>((g, ctx) =>
            Lower(g.Counters, ctx) is CounterSpec spec
                ? new PutCounters(null, spec with { Count = Multiply(spec.Count, g.PerEach is null ? null : LowerAs<Quantity>(g.PerEach, ctx)) })
                : new OpaqueEffect(ctx.TextOf(g), nameof(G.EntersWithCounters)));
    }

    /// <summary>The buffs a verb gives, before they're folded into a <see cref="Pump"/>.</summary>
    sealed record BuffGroup(G.PermanentVerb Verb, List<IrNode> Buffs, bool Additional) : IrNode;

    /// <summary>A token description with the count it was written with ("two 1/1 … tokens"), before the count moves to <see cref="CreateTokens"/>.</summary>
    sealed record TokenSpecWithCount(Token Token, Quantity Count) : IrNode;

    /// <summary>A buff group as a pump - stats, keywords, base stats - followed by any buff that is an effect of its own (becoming a type, say), each given the group's duration where it has none.</summary>
    Effect PumpOf(BuffGroup group, Duration duration, Quantity perEach, LoweringContext ctx)
    {
        Quantity power = null, toughness = null;
        StatsMod baseStats = null;
        List<KeywordInstance> keywords = [];
        IReadOnlyList<KeywordInstance> choiceOf = null;
        List<Effect> other = [];

        foreach (var buff in group.Buffs)
            switch (buff)
            {
                case StatsMod stats:
                    power = stats.Power;
                    toughness = stats.Toughness;
                    break;
                case KeywordInstance keyword:
                    keywords.Add(keyword);
                    break;
                case KeywordChoice choice:
                    choiceOf = choice.Options;
                    break;
                case Term { Vocabulary: nameof(G.Keyword) } term:
                    keywords.Add(new KeywordInstance(term.Value));
                    break;
                case Term { Vocabulary: nameof(G.BasePowerToughness) } term when BaseStatsOf(term) is { } parsed:
                    baseStats = parsed;
                    break;
                case BecomesAspect { Duration: null } becomes:
                    other.Add(becomes with { Duration = duration });
                    break;
                case ObjectSelector { Filter.Kinds: [{ Value: nameof(ObjectKind.Ability) }] }:
                    other.Add(duration is null ? new LoseAllAbilities(ctx.Subject) : new Lasting(new LoseAllAbilities(ctx.Subject), duration));
                    break;
                default:
                    other.Add(EffectOf(buff));
                    break;
            }

        var pump = new Pump(ctx.Subject, power, toughness, OrNull(keywords), baseStats, duration, group.Verb == G.PermanentVerb.Lose, perEach, choiceOf);
        return other.Count == 0 ? pump : Seq([pump, .. other]);
    }
}
