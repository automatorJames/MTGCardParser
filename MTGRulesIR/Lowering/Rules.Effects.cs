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
                ? new Exile(objects, g.UntilThisLeaves is null ? null : new UntilThisLeaves(), g.WithCounters is null ? null : Lower(g.WithCounters, ctx), manner)
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
                pumps.Add(node is BuffGroup group ? PumpOf(group, duration, perEach, ctx) : new Pump(ctx.Subject, OtherBuffs: [node], Duration: duration, PerEach: perEach));

            return pumps.Count == 1 ? pumps[0] : new Sequence(pumps);
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
            var counters = Lower(g.Counters, ctx) is CounterSpec spec ? spec : new CounterSpec(null, null, Other: Lower(g.Counters, ctx));
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

            return new CounterSpec(kind, LowerAs<Quantity>(g.Quantity, ctx), g.Additional, g.Bound?.ToString(), g.Prefix is null ? null : new Term(g.Prefix.ToString(), nameof(G.CountPrefix)));
        });

        Rule<G.CreateToken>((g, ctx) =>
        {
            var token = Lower(g.Token, ctx);
            var count = g.EqualTo is not null ? LowerAs<Quantity>(g.EqualTo, ctx) : token is TokenSpecWithCount withCount ? withCount.Count : null;
            return new CreateTokens(token is TokenSpecWithCount t ? t.Token : token, count, g.PerEach is null ? null : LowerAs<Quantity>(g.PerEach, ctx));
        });

        Rule<G.TokenDescription>((g, ctx) =>
        {
            List<IrNode> other = [];
            var keywords = KeywordsOf(g.With, ctx, other);
            var stats = Lower(g.Stats, ctx) as StatsMod;
            var name = g.Name ?? g.NameAfterKeywords;

            return new TokenSpecWithCount(
                new TokenSpec(
                    OrNull(Terms(g.Types.Items, ctx, other)),
                    g.Subtypes is null ? null : OrNull(Terms(g.Subtypes.Items, ctx, other)),
                    OrNull(Terms(g.Colors, ctx, other)),
                    stats?.Power,
                    stats?.Toughness,
                    name is null ? null : ctx.TextOf(name)?.Replace("named ", ""),
                    OrNull(keywords),
                    OrNull(other),
                    g.Tapped is G.EntryState.Tapped or G.EntryState.TappedAndAttacking,
                    g.Tapped is G.EntryState.TappedAndAttacking || g.Attacking is not null),
                QuantityOf(g.Quantity));
        });

        Rule<G.PredefinedTokens>((g, ctx) =>
            new TokenSpecWithCount(new TokenSpec(Predefined: g.Kind.ToString(), Tapped: g.Tapped), QuantityOf(g.Quantity)));

        Rule<G.Sacrifice>((g, ctx) =>
            new Sacrifice(Actor(ctx), g.ChosenBy is null ? LowerAs<Selector>(g.Sacrificed, ctx) : Narrow(LowerAs<Selector>(g.Sacrificed, ctx), Lower(g.ChosenBy, ctx))));

        Rule<G.ReturnCard>((g, ctx) =>
            new ChangeZone(LowerAs<Selector>(g.Returned, ctx), ZoneOf(g.Destination, ctx)));

        Rule<G.PutIntoZone>((g, ctx) =>
        {
            var destination = g.Destination.GetResolvedValue();
            var zone = ZoneOf(destination, ctx) ?? new ZoneRef("?");
            List<IrNode> details = [];
            var tapped = g.Tapped == true;
            Selector under = null;

            if (destination is G.ToTheBattlefieldUnderControl battlefield)
            {
                tapped |= battlefield.State is G.EntryState.Tapped or G.EntryState.TappedAndAttacking || battlefield.StateAfterControl is G.EntryState.Tapped or G.EntryState.TappedAndAttacking;
                under = zone.Owner;
                zone = zone with { Owner = null };
                details.AddRange(LowerAll(battlefield.WithCounters, ctx));
                details.AddRange(LowerAll(battlefield.Attached, ctx));
                if (battlefield.Transformed == true || battlefield.AndTransformed == true)
                    details.Add(new Term("Transformed", nameof(G.EntryState)));
                if (battlefield.State is G.EntryState.TappedAndAttacking || battlefield.StateAfterControl is G.EntryState.TappedAndAttacking)
                    details.Add(new Term("Attacking", nameof(G.EntryState)));
                if (battlefield.State is G.EntryState.FaceDown || battlefield.StateAfterControl is G.EntryState.FaceDown)
                    details.Add(new Term("FaceDown", nameof(G.EntryState)));
            }
            else if (destination is not G.IntoZone and not null)
                details.AddRange(LowerAll(destination, ctx).Where(x => x is not ZoneRef));

            return new ChangeZone(LowerAs<Selector>(g.Moved, ctx), zone, tapped, under, OrNull(details));
        });

        Rule<G.AddMana>((g, ctx) =>
            new AddMana(LowerAll(g.Mana, ctx), g.Amount?.ToString()));

        Rule<G.ManaCost>((g, ctx) => new ManaAmount(g.Items.Select(SymbolText).ToList()));
        Rule<G.ManaSymbol>((g, ctx) => new Term(SymbolText(g), nameof(G.ManaSymbol)));

        Rule<G.CantBeTargeted>((g, ctx) =>
        {
            List<IrNode> rest = [];
            var by = Terms(g.By, ctx, rest);
            return new CantBeTargeted(ctx.Subject ?? new Self(), OrNull(by) ?? [ctx.TextOf(g.By)], g.Whose is null ? null : Lower(g.Whose, ctx), g.Period is null ? null : new ThisTurn(g.Period.Period.ToString()));
        });

        Rule<G.CombatRestriction>((g, ctx) =>
        {
            List<IrNode> details = [];
            var actions = Terms(g.Action, ctx, details);
            foreach (var part in new object[] { g.Attacked, g.OrAction, g.By, g.AndOr, g.Blocked, g.Blockee, g.Condition })
                details.AddRange(LowerAll(part, ctx));
            if (g.Alone)
                details.Add(new Term("Alone", nameof(G.CombatRestriction)));
            if (g.Except)
                details.Add(new Term("Except", nameof(G.CombatRestriction)));

            var duration = DurationOf(Lower(g.Duration ?? (object)g.LaterDuration, ctx));
            return new CombatRestriction(ctx.Subject ?? new Self(), actions, OrNull(details), duration);
        });

        Rule<G.ForEach>((g, ctx) => new CountOf(LowerAs<Selector>(g.Counted, ctx)));
        Rule<G.EqualTo>((g, ctx) => LowerAs<Quantity>(g.Measure, ctx));
        Rule<G.NumberOf>((g, ctx) => new CountOf(g.Other == true ? Narrow(LowerAs<Selector>(g.Counted, ctx), new Qualifier("Other")) : LowerAs<Selector>(g.Counted, ctx)));
        Rule<G.UpToCount>((g, ctx) => LowerAs<Quantity>(g.Count, ctx));
        Rule<G.AtLeastCount>((g, ctx) => LowerAs<Quantity>(g.Count, ctx));
        Rule<G.EntersWithCounters>((g, ctx) =>
            Lower(g.Counters, ctx) is CounterSpec spec
                ? new PutCounters(null, spec with { Count = Multiply(spec.Count, g.PerEach is null ? null : LowerAs<Quantity>(g.PerEach, ctx)) })
                : new OpaqueNode(ctx.TextOf(g), nameof(G.EntersWithCounters)));
    }

    /// <summary>The buffs a verb gives, before they're folded into a <see cref="Pump"/>.</summary>
    sealed record BuffGroup(G.PermanentVerb Verb, List<IrNode> Buffs, bool Additional) : IrNode;

    /// <summary>A token description with the count it was written with ("two 1/1 … tokens"), before the count moves to <see cref="CreateTokens"/>.</summary>
    sealed record TokenSpecWithCount(TokenSpec Token, Quantity Count) : IrNode;

    Pump PumpOf(BuffGroup group, Duration duration, Quantity perEach, LoweringContext ctx)
    {
        Quantity power = null, toughness = null;
        List<KeywordInstance> keywords = [];
        List<IrNode> other = [];

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
                case Term { Vocabulary: nameof(G.Keyword) } term:
                    keywords.Add(new KeywordInstance(term.Value));
                    break;
                default:
                    other.Add(buff);
                    break;
            }

        return new Pump(ctx.Subject, power, toughness, OrNull(keywords), OrNull(other), duration, group.Verb == G.PermanentVerb.Lose, perEach);
    }
}
