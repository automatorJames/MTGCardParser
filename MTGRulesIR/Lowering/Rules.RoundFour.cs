namespace MTGRulesIR.Lowering;

/// <summary>The fourth round of rules: the parts the third round's effects are made of - split placements, cards from among a group, play permissions, described kinds, arithmetic amounts, counters as things counted, quoted abilities on tokens.</summary>
public sealed partial class Lowerer
{
    void RegisterRoundFour()
    {
        Rule<G.PutSplitCards>((g, ctx) => Lower(g.Split, ctx));
        Rule<G.SplitLookedAtCards>((g, ctx) =>
            Seq(
                new ChangeZone(LowerAs<Selector>(g.Cards, ctx), ZoneOf(g.Chosen, ctx) ?? new ZoneRef("?")),
                new ChangeZone(new ObjectSelector(Quantifier.Rest, new ObjectFilter(Kinds: ["card"])), ZoneOf(g.Rest, ctx) ?? new ZoneRef("?"))));
        Rule<G.CardsFromAmong>((g, ctx) =>
        {
            var count = g.Count.GetResolvedValue();
            var filter = FilterOf(ctx, qualifiers: g.Qualifiers, narrowing: [g.Bound]) with { Kinds = ["card"] };
            return new ObjectSelector(
                Quantifier.Count,
                filter with { With = [.. filter.With ?? [], LowerAs<Selector>(g.Among, ctx)] },
                Count: count switch { G.UpToCount upTo => LowerAs<Quantity>(upTo.Count, ctx), G.Quantity quantity => QuantityOf(quantity), _ => null },
                UpTo: count is G.UpToCount);
        });
        Rule<G.PutThemBack>((g, ctx) =>
            new ChangeZone(new ObjectSelector(Quantifier.Those, new ObjectFilter(Kinds: ["card"])), new ZoneRef("Library", Position: "Top AnyOrder")));
        Rule<G.PlayPermission>((g, ctx) => new Playable(g.Verb?.ToString(), LowerAs<Selector>(g.What, ctx)));
        Rule<G.TopOfLibrary>((g, ctx) => new ZoneRef("Library", LowerAs<Selector>(g.Owner, ctx), "Top"));
        Rule<G.CardsInPlayersZone>((g, ctx) => new ObjectSelector(Quantifier.All, new ObjectFilter(Kinds: ["card"], Zone: ZoneOf(g.Zone, ctx))));
        Rule<G.DamagedPermanent>((g, ctx) =>
            Narrow(LowerAs<Selector>(g.Permanent, ctx), new Qualifier("DealtDamage:" + (Lower(g.Source, ctx) is Term source ? source.Value : ctx.TextOf(g.Source)))));
        Rule<G.ColoredKind>((g, ctx) =>
            new QualifiedKind(Lower(g.Kind, ctx) is Term kind ? kind.Value : ctx.TextOf(g.Kind), [new Qualifier(g.Color.ToString())]));
        Rule<G.DescribedKinds>((g, ctx) =>
        {
            List<IrNode> other = [];
            var stats = Lower(g.Stats, ctx);
            var keywords = g.With is null ? null : Lower(g.With, ctx) is HasKeywords has ? has.Keywords.Select(x => new KeywordInstance(x)).ToList() : null;
            return new TokenSpec(
                g.Creature == true ? ["Creature"] : null,
                OrNull(Terms(g.Kinds.Items, ctx, other)),
                stats is Term color ? [color.Value] : null,
                (stats as StatsMod)?.Power,
                (stats as StatsMod)?.Toughness,
                Keywords: keywords,
                Other: OrNull([.. other, .. stats is null or Term or StatsMod ? [] : new[] { stats }]));
        });
        Rule<G.BecomesLandType>((g, ctx) => new Term(g.LandType.ToString(), nameof(G.LandType)));
        Rule<G.CharacteristicOfChoice>((g, ctx) => new Term($"{g.Characteristic}OfYourChoice", nameof(G.CharacteristicOfChoice)));
        Rule<G.ArithmeticAmount>((g, ctx) => new Arithmetic(LowerAs<Quantity>(g.Left, ctx), g.Operator.ToString(), LowerAs<Quantity>(g.Right, ctx)));
        Rule<G.CardsInHandCount>((g, ctx) =>
        {
            Quantity count = QuantityOf(g.Count);
            if (g.Comparison is { } comparison)
                count = new Bound(count, comparison.ToString());
            else if (g.Bound is { } bound)
                count = new Bound(count, bound.ToString());
            return count;
        });
        Rule<G.ComparedHandSize>((g, ctx) => new Compared(g.Comparison.ToString(), LowerAs<Selector>(g.Other, ctx)));
        Rule<G.DevotionTo>((g, ctx) => new Devotion(LowerAll(g.Colors, ctx)));
        Rule<G.FromZone>((g, ctx) => ZoneOf(g.Zone, ctx));
        Rule<G.PlayerFindsCard>((g, ctx) => new PlayerAction(LowerAs<Selector>(g.Player, ctx), g.Verb.ToString(), LowerAs<Selector>(g.Card, ctx)));
        Rule<G.CardIsFound>((g, ctx) => new PlayerAction(null, "Reveal", LowerAs<Selector>(g.Card, ctx)));
        Rule<G.MoreThanCount>((g, ctx) => new Bound(QuantityOf(g.Count), ">"));
        Rule<G.IndefiniteCard>((g, ctx) =>
            new ObjectSelector(Quantifier.Any, FilterOf(ctx, qualifiers: g.Qualifiers) with { Kinds = g.OrToken ? ["card", "token"] : ["card"] }));
        Rule<G.ASource>((g, ctx) =>
            new ObjectSelector(Quantifier.Any, FilterOf(ctx, qualifiers: g.Qualifiers, controller: g.Controller) with { Kinds = [g.Noun.ToString()] }, Other: ctx.TextOf(g)?.StartsWith("another") == true));
        Rule<G.CountersOfKind>((g, ctx) =>
            new CounterSpec(g.Kind?.GetResolvedValue() switch { G.CounterType type => PatternOf(type), G.PowerToughnessMod mod => ctx.TextOf(mod), _ => null }, null));
        Rule<G.AnyCounters>((g, ctx) => new CounterSpec(null, null));
        Rule<G.CreaturesBlockingOrBlockedBy>((g, ctx) =>
            new ObjectSelector(Quantifier.All, new ObjectFilter(Kinds: ["Creature"], With: [new InCombatWith(LowerAs<Selector>(g.Combatant, ctx))])));
        Rule<G.EqualToSmallCount>((g, ctx) => LowerAs<Quantity>(g.Count, ctx));
        Rule<G.ExcludedKind>((g, ctx) => new TermSet(Terms(g.Kinds, ctx), "Excluded"));
        Rule<G.QualifiedCardName>((g, ctx) => new ObjectSelector(Quantifier.Any, FilterOf(ctx, qualifiers: g.Qualifiers) with { Kinds = ["card name"] }));
        Rule<G.TargetPlayerWhoOutnumbers>((g, ctx) =>
            new ObjectSelector(Quantifier.Target, new ObjectFilter(Kinds: [g.Player.ToString()], With: [Lower(g.Measure, ctx), new Compared("More", LowerAs<Selector>(g.Compared, ctx)), .. g.Proviso is null ? [] : new IrNode[] { new Term(g.Proviso.ToString(), "TargetPlayerProviso") }])));
        Rule<G.WithQuotedAbility>((g, ctx) => new GrantAbility(null, AsAbility(Lower(g.Ability, ctx), ctx, g.Ability)));
        Rule<G.WithKeywordsAndQuotedAbility>((g, ctx) =>
        {
            List<IrNode> other = [];
            var keywords = KeywordsOf(g.Keywords, ctx, other);
            return Seq(new Pump(null, Keywords: OrNull(keywords), OtherBuffs: OrNull(other)), new GrantAbility(null, AsAbility(Lower(g.Ability, ctx), ctx, g.Ability)));
        });
        Rule<G.WithQuotedAbilityAndEquip>((g, ctx) =>
            Seq(new GrantAbility(null, AsAbility(Lower(g.Ability, ctx), ctx, g.Ability)), new Pump(null, Keywords: [new KeywordInstance("Equip", Cost: new PayMana(g.Cost.Items.Select(SymbolText).ToList()))])));
        Rule<G.PlayersCounters>((g, ctx) =>
            new ObjectSelector(Quantifier.All, new ObjectFilter(Kinds: ["counter"], Qualifiers: [new Qualifier(PatternOf(g.Kind))], Controller: new Controlled(LowerAs<Selector>(g.Player, ctx), "Has"))));
        Rule<G.CountersOn>((g, ctx) =>
            new ObjectSelector(Quantifier.All, new ObjectFilter(Kinds: ["counter"], Qualifiers: g.CounterType is { } type ? [new Qualifier(PatternOf(type))] : null, With: [LowerAs<Selector>(g.Object, ctx)])));
        Rule<G.BasePowerToughness>((g, ctx) => new Term(ctx.TextOf(g), nameof(G.BasePowerToughness)));
        Rule<G.EnchantedPermanentsController>((g, ctx) =>
            new PlayerSelector(PlayerKind.ControllerOf, new ObjectSelector(Quantifier.Attached, FilterOf(ctx, kinds: g.Kind) with { Qualifiers = [new Qualifier("Enchanted")] })));
        Rule<G.NthSpellThisTurn>((g, ctx) =>
        {
            var filter = FilterOf(ctx, qualifiers: g.Qualifiers) with { Kinds = ["spell"] };
            return new ObjectSelector(Quantifier.Any, filter with { Qualifiers = [.. filter.Qualifiers ?? [], new Qualifier(g.Ordinal.ToString()), new Qualifier(g.Period.ToString())] });
        });
        Rule<G.CopiedSpell>((copied, ctx) =>
            copied == G.CopiedSpell.ThisSpell ? new Self() : new ObjectSelector(Quantifier.Those, new ObjectFilter(Kinds: [copied.ToString()])));

        // Quick wins from the report after round four.
        Rule<G.PermanentFromZone>((g, ctx) =>
            LowerAs<Selector>(g.Card, ctx) is ObjectSelector objects
                ? objects with { Filter = (objects.Filter ?? new ObjectFilter()) with { Zone = ZoneOf(g.Zone, ctx) } }
                : new ObjectSelector(Quantifier.Any, new ObjectFilter(Zone: ZoneOf(g.Zone, ctx), With: [LowerAs<Selector>(g.Card, ctx)]), Count: new Fixed(1)));
        Rule<G.CountOfThem>((g, ctx) =>
            new ObjectSelector(Quantifier.Count, new ObjectFilter(With: [new Reference(null, "them")]), Count: QuantityOf(g.Quantity), UpTo: ctx.TextOf(g)?.StartsWith("up to") == true));
        Rule<G.EnergySymbols>((g, ctx) => new Fixed((ctx.TextOf(g) ?? "").Split("{e}").Length - 1));
        Rule<G.NamedThis>((g, ctx) => new Qualifier("NamedThis", g.Not));
        Rule<G.AnyCard>((g, ctx) =>
            new ObjectSelector(Quantifier.Any, FilterOf(ctx, qualifiers: g.Qualifiers, narrowing: [g.With]) with { Kinds = ["card"] }, Other: ctx.TextOf(g)?.StartsWith("another") == true));
        Rule<G.CastSpellsUse>((g, ctx) => new Playable("Cast", g.Narrowed is null ? LowerAs<Selector>(g.Spells, ctx) : Narrow(LowerAs<Selector>(g.Spells, ctx), Lower(g.Narrowed, ctx))));
        Rule<G.HaveDo>((g, ctx) =>
        {
            var previous = ctx.Subject;
            ctx.Subject = LowerAs<Selector>(g.Doer, ctx);
            try
            {
                return LowerAs<Effect>(g.Action, ctx);
            }
            finally
            {
                ctx.Subject = previous;
            }
        });
        Rule<G.EndOfCombatEffect>((g, ctx) => WithSubject(ctx, g.Subject, () => new Delayed(new PhaseEvent(new PhaseRef("EndOfCombatStep", "End")), LowerAs<Effect>(g.Effect, ctx))));
        Rule<G.CantBeCountered>((g, ctx) => new CantBeTargeted(LowerAs<Selector>(g.Spell, ctx), [ctx.TextOf(g)?.Contains("copied") == true ? "Copied" : "Countered"]));
        Rule<G.ThatToken>((g, ctx) => new ObjectSelector(Quantifier.Those, new ObjectFilter(Kinds: ["token"])));
        Rule<G.PerfectSubject>((subject, ctx) => new PlayerSelector(subject == G.PerfectSubject.You ? PlayerKind.You : PlayerKind.ThatPlayer));
        Rule<G.TurnOwner>((owner, ctx) =>
            owner.ToString() switch
            {
                var name when name.StartsWith("Your") => new PlayerSelector(PlayerKind.You),
                var name when name.StartsWith("Opponent") => new PlayerSelector(PlayerKind.AnyOpponent),
                var name when name.StartsWith("Other") => new PlayerSelector(PlayerKind.EachOtherPlayer),
                var name when name.StartsWith("Each") => new PlayerSelector(PlayerKind.EachPlayer),
                _ => new PlayerSelector(PlayerKind.ThatPlayer),
            });
    }
}
