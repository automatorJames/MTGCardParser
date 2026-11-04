namespace MTGRulesIR.Lowering;

/// <summary>
/// The seventh round: casting without paying, searching to the top, "with different", what a spell targets, cards
/// revealed this way, mana spent of a color, paired subjects, exiling graveyards, trigger events on their own,
/// transformed types, extra turns, looking any time, "also a" types, pairs and groups of objects, combat relations,
/// mana in any combination, controlling none, "unless" conditions, extra land plays, all abilities, creating a token
/// if none, no spells last turn, shared and compared characteristics, any number of targets, abilities of a source,
/// one or more cards, the Jace token, station rows, role tokens, and damage dealt by a source.
/// </summary>
public sealed partial class Lowerer
{
    void RegisterRoundSeven()
    {
        // Text restating a keyword's rules.
        Rule<G.ForetellCast>((g, ctx) => null);
        Rule<G.PlotCast>((g, ctx) => null);
        Rule<G.DisguiseTurnFaceUp>((g, ctx) => null);
        Rule<G.SpeedIncreases>((g, ctx) => null);
        Rule<G.MaxSpeed>((g, ctx) => null);
        Rule<G.SiegeAttackable>((g, ctx) => null);
        Rule<G.SiegeDefeated>((g, ctx) => null);
        Rule<G.CreatureAgainIfUnattached>((g, ctx) => null);
        Rule<G.WarpExile>((g, ctx) => null);

        // Effects.
        Rule<G.CascadeTail>((g, ctx) => new Optional(new PlayCard(Actor(ctx), new Reference(null, "it"), Free: true), Actor(ctx)));
        Rule<G.SearchLibraryToTop>((g, ctx) =>
        {
            var you = new PlayerSelector(PlayerKind.You);
            var found = new ObjectSelector(Quantifier.Those, new ObjectFilter(Kinds: [Kind("card")], Qualifiers: [Q(Provenance.Found)]));
            Effect search = new SearchLibrary(you, "Library", LowerAllAs<Selector>(g.Wanted, ctx), g.Revealed,
                new ChangeZone(found, new ZoneRef("Library", you, nameof(Position.Top))), new Shuffle(you));
            return g.May ? new Optional(search, you) : search;
        });
        Rule<G.BothSubjectsDoComma>((g, ctx) => Seq(LowerAs<Effect>(g.First, ctx), LowerAs<Effect>(g.Second, ctx)));
        Rule<G.ExileGraveyard>((g, ctx) =>
        {
            var whose = g.Whose.GetResolvedValue();
            var owner = whose switch
            {
                G.Whose.All => new PlayerSelector(PlayerKind.EachPlayer),
                G.Whose named => PlayerOf(named),
                _ => LowerAs<Selector>(g.Whose, ctx),
            };
            return new Exile(new ObjectSelector(Quantifier.All, new ObjectFilter(Kinds: [Kind("card")], Zone: new ZoneRef("Graveyard", owner))));
        });
        Rule<G.TransformedType>((g, ctx) =>
            Seq(
                new BecomesAspect(new Reference(null, "it"), TermsOf(g.CardType.Items, ctx), Verb: nameof(G.Assertion.Is)),
                g.PowerToughness is null ? null : LowerAs<Effect>(g.PowerToughness, ctx)));
        Rule<G.WithPowerToughnessEqual>((g, ctx) => new SetStats(new Reference(null, "it"), g.PowerAndOrToughness.ToString(), LowerAs<Quantity>(g.Measure, ctx)));
        Rule<G.ExtraTurn>((g, ctx) => new ExtraTurn(ctx.Subject ?? Actor(ctx), g.AfterThisOne));
        Rule<G.LookAtAnyTime>((g, ctx) => new LookAt(Actor(ctx), LowerAs<Selector>(g.What, ctx), AnyTime: true));
        Rule<G.ItsAlsoType>((g, ctx) => new BecomesAspect(new Reference(null, "it"), [new Term(g.Kind.ToString(), nameof(G.PermanentKind))], InAddition: true, Verb: nameof(G.Assertion.Is)));
        Rule<G.EffectUnlessCondition>((g, ctx) => new Conditional(new Not(LowerAs<Condition>(g.Condition, ctx)), LowerAs<Effect>(g.Effect, ctx)));
        Rule<G.PlayAdditionalLands>((g, ctx) =>
        {
            var count = g.Count?.GetResolvedValue();
            return new ExtraLandPlays(
                ctx.Subject ?? Actor(ctx),
                count switch { G.UpToCount upTo => LowerAs<Quantity>(upTo.Count, ctx), G.Quantity quantity => QuantityOf(quantity), _ => new Fixed(1) },
                g.Period?.GetResolvedValue() is G.OnEachTurn,
                g.Period?.GetResolvedValue() is G.ThisPeriod period ? new ThisTurn(period.Period.ToString()) : null);
        });
        Rule<G.CreateFirstIfNone>((g, ctx) =>
        {
            var token = Lower(g.Token, ctx);
            return new Conditional(
                new Not(new Controls(new PlayerSelector(PlayerKind.You), LowerAs<Selector>(g.Missing, ctx))),
                new CreateTokens(TokenOf(token, ctx, g.Token), token is TokenSpecWithCount counted ? counted.Count : null));
        });
        Rule<G.EmpowerJaceToken>((g, ctx) =>
        {
            var you = new PlayerSelector(PlayerKind.You);
            var jace = new ObjectFilter(Kinds: [Kind(G.CardType.Planeswalker)], Qualifiers: [Q("Jace", nameof(G.PlaneswalkerType))]);
            return new Conditional(
                new Not(new Controls(you, new ObjectSelector(Quantifier.Any, jace))),
                new CreateTokens(new TokenSpec([nameof(G.CardType.Planeswalker)], ["Jace"], [nameof(G.ManaColor.Blue)], Abilities:
                [
                    new LoyaltyAbility(new Fixed(-1), new KeywordAction([new KeywordInstance("Surveil", new Fixed(1))])),
                    new LoyaltyAbility(new Fixed(-3), new Draw(you, new Fixed(1))),
                ])));
        });
        Rule<G.CreateRoleToken>((g, ctx) =>
            new CreateTokens(new TokenSpec([nameof(G.CardType.Enchantment)], ["Aura", "Role"], Predefined: g.Role.ToString(), AttachedTo: LowerAs<Selector>(g.AttachedTo, ctx))));
        Rule<G.YouControlEnchanted>((g, ctx) => new GainControl(new PlayerSelector(PlayerKind.You), LowerAs<Selector>(g.Enchanted, ctx)));

        // Events.
        Rule<G.PermanentTriggerEvent>((g, ctx) =>
            ObjectEventOf(g.Event, LowerAs<Selector>(g.Subject, ctx), g.Under is null ? null : LowerAs<Selector>(g.Under.Whose, ctx), null, null, ctx));

        // Frames.
        Rule<G.StationThresholdRow>((g, ctx) => new StationThresholdRow(g.Threshold.Low, AsAbility(Lower(g.Ability, ctx), ctx, g.Ability)));

        // Selectors and predicates.
        Rule<G.CardOfKind>((g, ctx) => new ObjectSelector(Quantifier.Any, FilterOf(ctx, qualifiers: g.Qualifiers) with { Kinds = [Kind("card")] }));
        Rule<G.WithDifferent>((g, ctx) => new Distinct(g.Characteristic.ToString()));
        Rule<G.CardsRevealedThisWay>((g, ctx) =>
        {
            var count = g.Count?.GetResolvedValue();
            var filter = FilterOf(ctx, qualifiers: g.Qualifiers, narrowing: [g.Bound]);
            return new ObjectSelector(
                count switch { G.UpToCount or G.Quantity => Quantifier.Count, _ => Quantifier.All },
                filter with { Kinds = [Kind("card")], Qualifiers = [.. filter.Qualifiers ?? [], Q(g.How + "ThisWay", nameof(AffectedThisWay))] },
                Count: count switch { G.UpToCount upTo => LowerAs<Quantity>(upTo.Count, ctx), G.Quantity quantity => QuantityOf(quantity), _ => null },
                UpTo: count is G.UpToCount);
        });
        Rule<G.ObjectAndOthers>((g, ctx) => new Union([LowerAs<Selector>(g.Object, ctx), LowerAs<Selector>(g.Others, ctx)], Connective.And));
        Rule<G.PermanentPair>((g, ctx) => new Union([LowerAs<Selector>(g.First, ctx), LowerAs<Selector>(g.Second, ctx)], Connective.And));
        Rule<G.CombatRelation>((g, ctx) =>
        {
            var text = ctx.TextOf(g) ?? "";
            var relation = text.StartsWith("blocking or") ? CombatRelation.BlockingOrBlockedBy
                : text.StartsWith("blocked by") ? CombatRelation.BlockedBy
                : text.StartsWith("attacking") ? CombatRelation.Attacking
                : CombatRelation.Blocking;
            return new InCombatWith(LowerAs<Selector>(g.Other, ctx), relation.ToString());
        });
        Rule<G.AllAbilities>((g, ctx) => new ObjectSelector(Quantifier.All, new ObjectFilter(Kinds: [Kind(ObjectKind.Ability)])));
        Rule<G.AbilitiesOf>((g, ctx) =>
            g.Chosen is not null || g.Source.GetResolvedValue() is G.IndefiniteDeterminer
                ? new OpaquePredicate(ctx.TextOf(g), nameof(G.AbilitiesOf))
                : new Related(Relation.AbilityOf, LowerAs<Selector>(g.Source, ctx)));
        Rule<G.SameCharacteristicAs>((g, ctx) => new SharesCharacteristic(g.Characteristic.ToString(), LowerAs<Selector>(g.Other, ctx)));
        Rule<G.WithCharacteristicComparedTo>((g, ctx) =>
            new CharacteristicCompared(
                g.Characteristic.ToString(),
                g.Comparison == G.Comparison.Less ? (g.OrEqual ? Comparison.LessOrEqual : Comparison.Less) : g.OrEqual ? Comparison.GreaterOrEqual : Comparison.Greater,
                LowerAs<Selector>(g.Object, ctx),
                g.OtherCharacteristic.ToString()));
        Rule<G.AnyNumberOfRecipients>((g, ctx) =>
            LowerAs<Selector>(g.Recipients, ctx) is ObjectSelector objects
                ? objects with { Quantifier = Quantifier.Target, Count = new Special("AnyNumber") }
                : LowerAs<Selector>(g.Recipients, ctx));
        Rule<G.TargetsOfKind>((g, ctx) =>
            new ObjectSelector(Quantifier.Target, FilterOf(ctx, kinds: g.Kind, qualifiers: g.Qualifiers, controller: g.Controller, narrowing: [g.With])));
        Rule<G.OneOrMoreCards>((g, ctx) =>
            new ObjectSelector(Quantifier.Count, FilterOf(ctx, qualifiers: g.Qualifiers) with { Kinds = [Kind("card")] }, Count: new Special("OneOrMore")));
        Rule<G.DamageDealtBy>((g, ctx) => new Related(Relation.DealtDamageBy, LowerAs<Selector>(g.Source, ctx)));

        // Mana.
        Rule<G.ManaInAnyCombination>((g, ctx) =>
            new ManaChoice(QuantityOf(g.Quantity), nameof(ManaChoiceKind.AnyCombination),
                OrNull(LowerAll(g.Colors, ctx).OfType<ManaAmount>().SelectMany(x => x.Symbols).ToList())));

        // Conditions.
        Rule<G.IfItTargets>((g, ctx) => new Targets(new Reference(null, "it"), LowerAs<Selector>(g.Target, ctx)));
        Rule<G.ManaWasSpent>((g, ctx) =>
            g.Mana.GetResolvedValue() switch
            {
                G.ManaCost { Items.Count: 1 } cost => new ManaSpent(new Fixed(1), LowerAs<Selector>(g.Use, ctx), ColorOf(SymbolText(cost.Items[0]))),
                G.ManaFromSource source => new ManaSpent(null, LowerAs<Selector>(g.Use, ctx), From: LowerAs<Selector>(source.Source, ctx)),
                _ => Opaque(typeof(Condition), ctx.TextOf(g), nameof(G.ManaWasSpent)),
            });
        Rule<G.PlayerControlsNone>((g, ctx) => new Controls(LowerAs<Selector>(g.Controller, ctx), LowerAs<Selector>(g.Permanents, ctx), Negated: true));
        Rule<G.NoSpellsCastLastTurn>((g, ctx) =>
            new Not(new Happened(
                new PlayerAction(new PlayerSelector(PlayerKind.AnyPlayer), nameof(G.PastPlayerAction.Cast), new ObjectSelector(Quantifier.Any, new ObjectFilter(Kinds: [Kind("spell")]))),
                nameof(EventSpan.LastTurn))));
    }

    /// <summary>
    /// An event glyph as the event it names, on <paramref name="subject"/>: a combat encounter, a graveyard event
    /// with its owner, or the named permanent events; anything the lowering can't name is an opaque event.
    /// </summary>
    Event ObjectEventOf(object eventGlyph, Selector subject, Selector under, string only, Condition whileCondition, LoweringContext ctx)
    {
        if ((eventGlyph as OneOfBase)?.GetResolvedValue() is G.CombatEncounter encounter)
            return new CombatEncounterEvent(subject, encounter.Encounter.ToString(), LowerAs<Selector>(encounter.Other, ctx));

        if (LowerAll(eventGlyph, ctx) is [ObjectEvent partial])
            return partial with { Subject = subject, UnderControlOf = under, Only = only, While = whileCondition };

        List<IrNode> details = [];
        var events = Terms(eventGlyph, ctx, details);
        return details.Count > 0 || events.Count == 0
            ? (Event)Opaque(typeof(Event), ctx.TextOf(eventGlyph), (eventGlyph as OneOfBase)?.GetResolvedType()?.Name ?? eventGlyph?.GetType().Name)
            : new ObjectEvent(subject, events, under, only, whileCondition);
    }

    /// <summary>The color a mana symbol stands for, as the vocabulary names it.</summary>
    static string ColorOf(string symbol) =>
        symbol switch
        {
            "{w}" => nameof(G.ManaColor.White),
            "{u}" => nameof(G.ManaColor.Blue),
            "{b}" => nameof(G.ManaColor.Black),
            "{r}" => nameof(G.ManaColor.Red),
            "{g}" => nameof(G.ManaColor.Green),
            "{c}" => nameof(G.ManaColor.Colorless),
            _ => OpenSlots.Placeholder,
        };
}
