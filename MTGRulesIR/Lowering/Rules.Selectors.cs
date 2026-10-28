namespace MTGRulesIR.Lowering;

/// <summary>Selectors: the card itself, targets, groups of permanents, cards in zones, players, and the parts of a filter.</summary>
public sealed partial class Lowerer
{
    void RegisterSelectors()
    {
        Rule<This>((g, ctx) => new Self());
        Rule<G.ThisCreature>((g, ctx) => new Self());
        Rule<G.ThisSpell>((g, ctx) => new Self());

        Rule<G.SpecificTarget>((g, ctx) =>
            g.Kind.GetResolvedValue() is G.TargetKind { PlayerIdentity: { } identity } && g.Qualifiers is null && g.Controller is null && g.With is null && g.InCombat is null && g.Stats is null
                ? new PlayerSelector(identity == G.PlayerIdentity.Opponent ? PlayerKind.TargetOpponent : PlayerKind.TargetPlayer)
                : new ObjectSelector(
                Quantifier.Target,
                FilterOf(ctx, kinds: g.Kind, qualifiers: g.Qualifiers, controller: g.Controller ?? g.ControllerAfterWith,
                    narrowing: [g.Stats, g.With, g.InCombat, g.OtherThan, g.ChosenBy, g.DealtDamageThisTurn ? new Qualifier("DealtDamageThisTurn") : null]),
                UpTo: g.Prefix is G.TargetPrefix.UpToOne or G.TargetPrefix.UpToOneOther,
                Other: g.Prefix is G.TargetPrefix.Another or G.TargetPrefix.UpToOneOther or G.TargetPrefix.ASecond or G.TargetPrefix.AThird));

        Rule<G.CountedTargets>((g, ctx) =>
            new ObjectSelector(
                Quantifier.Target,
                FilterOf(ctx, kinds: g.Kind, qualifiers: g.Qualifiers, controller: g.Controller, narrowing: [g.With, g.Sharing, g.Suffix]),
                Count: LowerAs<Quantity>(g.Count, ctx),
                UpTo: g.UpTo));

        Rule<G.AnyTarget>((g, ctx) =>
            new ObjectSelector(Quantifier.Target, FilterOf(ctx, narrowing: [g.ChosenBy, g.Happened]) with { Kinds = ["any"] }, Other: g.Which != G.AnyTargetChoice.Any));

        Rule<G.IndefinitePermanent>((g, ctx) =>
            new ObjectSelector(
                Quantifier.Any,
                FilterOf(ctx, kinds: g.Kind, qualifiers: g.Qualifier, controller: g.Controller, narrowing: [g.With, g.OtherThan, g.ChosenBy, g.Colored]),
                Other: g.Determiner == G.IndefiniteDeterminer.Another));

        Rule<G.Permanents>((g, ctx) =>
            new ObjectSelector(
                Quantifier.All,
                FilterOf(ctx, kinds: g.Kind, qualifiers: g.Qualifiers, controller: g.Controller, narrowing: [g.PreNarrowed, g.Narrowed, g.InCombat, g.OtherThan])));

        Rule<G.AllPermanents>((g, ctx) => Group(g.Permanents, Quantifier.All, ctx, other: g.Other));
        Rule<G.EachPermanents>((g, ctx) => Group(g.Permanents, Quantifier.Each, ctx, other: g.Other));
        Rule<G.OneOrMorePermanents>((g, ctx) => Group(g.Permanents, Quantifier.Any, ctx, other: g.Other, count: new Special("OneOrMore")));
        Rule<G.CountedPermanents>((g, ctx) => Group(g.Permanents, Quantifier.Count, ctx, count: LowerAs<Quantity>(g.Count, ctx)));

        Rule<G.EnchantedPermanent>((g, ctx) =>
            new ObjectSelector(Quantifier.Attached, FilterOf(ctx, kinds: g.Kind) with { Qualifiers = [new Qualifier(g.Attachment.ToString())] }));

        Rule<G.TargetCardInZone>((g, ctx) =>
        {
            var selector = new ObjectSelector(Quantifier.Target, CardFilter(g.Card, ctx, g.Qualifiers));

            return g.UpTo?.GetResolvedValue() switch
            {
                G.UpToCount upTo => selector with { Count = LowerAs<Quantity>(upTo.Count, ctx), UpTo = true },
                G.TargetPrefix prefix => selector with { UpTo = prefix is G.TargetPrefix.UpToOne or G.TargetPrefix.UpToOneOther, Other = prefix is not G.TargetPrefix.UpToOne },
                G.Quantity count => selector with { Count = QuantityOf(count) },
                _ => selector,
            };
        });

        Rule<G.CardOutsideBattlefield>((g, ctx) => CardFilter(g, ctx));

        Rule<G.ThatCardPhrase>((g, ctx) =>
            new ObjectSelector(Quantifier.Those, new ObjectFilter(Kinds: g.CardType is { } type ? [type.ToString()] : null), Count: g.Count is { } count ? QuantityOf(count) : null));

        Rule<G.TheRest>((g, ctx) => new ObjectSelector(Quantifier.Rest, null));

        Rule<G.OnTheBattlefield>((g, ctx) => null);

        Rule<G.OtherThanThis>((g, ctx) => new Qualifier("OtherThan:" + ctx.TextOf(g.Self)));

        Rule<G.TargetsController>((g, ctx) =>
            new PlayerSelector(g.Role == G.PlayerRole.Owner ? PlayerKind.OwnerOf : PlayerKind.ControllerOf, LowerAs<Selector>(g.Target, ctx)));

        Rule<G.ThatCardsController>((g, ctx) =>
            new PlayerSelector(g.Role == G.PlayerRole.Owner ? PlayerKind.OwnerOf : PlayerKind.ControllerOf, LowerAs<Selector>(g.Card, ctx)));

        Rule<G.ObjectPossessive>((g, ctx) => LowerAs<Selector>(g.Object, ctx));

        Rule<G.TargetedPlayer>((g, ctx) =>
            new PlayerSelector(g.Player == G.PlayerIdentity.Opponent ? PlayerKind.TargetOpponent : PlayerKind.TargetPlayer));

        Rule<G.EachPlayers>((g, ctx) =>
            new PlayerSelector(g.Player == G.PlayerIdentity.Opponent ? PlayerKind.EachOpponent : g.Other == true ? PlayerKind.EachOtherPlayer : PlayerKind.EachPlayer));

        Rule<G.PlayerPossessive>((g, ctx) => LowerAs<Selector>(g.Player, ctx));

        Rule<G.WhichPlayer>((player, ctx) => Player(player));
        Rule<G.Whose>((whose, ctx) => PlayerOf(whose));

        Rule<G.ControlledBy>((g, ctx) =>
            new Controlled(LowerAs<Selector>(g.Controller, ctx), g.Relation.ToString(), g.Negated));

        // "blue or red" is a choice of qualifiers; "tapped, blue" (and any other run) narrows by all of them.
        Rule<ManyOf<G.TargetQualifier>>((g, ctx) =>
            new QualifierChoice(LowerAll(g.Items, ctx).OfType<Qualifier>().ToList(), g.Conjunction == Conjunction.Or ? "or" : "and"));

        Rule<G.TargetQualifier>((g, ctx) =>
            new Qualifier(Lower(g.Quality, ctx) is Term term ? term.Value : ctx.TextOf(g.Quality), g.IsNegated));

        Rule<G.PluralKind>((g, ctx) => Lower(g.Kind, ctx));

        Rule<G.WithKeywords>((g, ctx) =>
        {
            List<IrNode> rest = [];
            var keywords = Terms(g.Keywords, ctx, rest);
            return rest.Count == 0 ? new HasKeywords(keywords, g.Having == G.Having.Without) : new OpaqueNode(ctx.TextOf(g), nameof(G.WithKeywords));
        });

        Rule<G.WithCharacteristicBound>((g, ctx) =>
            new CharacteristicBound(Terms(g.Characteristic, ctx), LowerAs<Quantity>(g.Quantity, ctx), g.Comparison.ToString(), g.Total));

        Rule<G.CardsInZone>((g, ctx) =>
            new ObjectSelector(Quantifier.All, FilterOf(ctx, qualifiers: g.Qualifiers, zone: g.Zone, narrowing: [g.Named, g.With, g.Narrowed]) with { Kinds = ["card"] }));

        Rule<G.PlayersZone>((g, ctx) => new ZoneRef(g.Zone.ToString(), LowerAs<Selector>(g.Whose, ctx)));
        Rule<G.NonBattlefieldZone>((zone, ctx) => new ZoneRef(zone.ToString()));
        Rule<G.IntoZone>((g, ctx) => new ZoneRef(g.Zone.Zone.ToString(), LowerAs<Selector>(g.Zone.Whose, ctx), g.Depth?.ToString()));
        Rule<G.ToTheBattlefieldUnderControl>((g, ctx) => new ZoneRef("Battlefield", g.Control is null ? null : LowerAs<Selector>(g.Control.Whose, ctx)));
        Rule<G.UnderControlOf>((g, ctx) => LowerAs<Selector>(g.Whose, ctx));
    }

    /// <summary>"all …", "each …", "one or more …", "two …": the permanents' selector with the group's quantifier.</summary>
    Selector Group(G.Permanents permanents, Quantifier quantifier, LoweringContext ctx, bool other = false, Quantity count = null, bool upTo = false)
    {
        var inner = LowerAs<Selector>(permanents, ctx);
        return inner is ObjectSelector selector ? selector with { Quantifier = quantifier, Other = other || selector.Other, Count = count ?? selector.Count, UpTo = upTo || selector.UpTo } : inner;
    }

    /// <summary>A filter for cards outside the battlefield: their zone and owner, and the kinds and bounds that narrow them.</summary>
    ObjectFilter CardFilter(G.CardOutsideBattlefield card, LoweringContext ctx, object qualifiers = null)
    {
        var filter = FilterOf(ctx, qualifiers: qualifiers, controller: card.Owner, zone: card.Zone, narrowing: [card.Bound, card.Narrowed]);
        return filter with { Kinds = ["card"], Qualifiers = card.AtRandom == true ? [.. filter.Qualifiers ?? [], new Qualifier("AtRandom")] : filter.Qualifiers };
    }
}
