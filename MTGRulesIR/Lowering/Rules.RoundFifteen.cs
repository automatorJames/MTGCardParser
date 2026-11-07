namespace MTGRulesIR.Lowering;

/// <summary>
/// The fifteenth round: an attachment's host, equip for a kind, counted players, players who can't or do, the other
/// of two, permanents with a shared controller, what can't be targeted by qualified spells or abilities, the nth
/// counter put on a permanent, "the difference", cards from a zone named "it", playing this card while exiled,
/// flash-cast permanents sacrificed, revealing or controlling a kind as cast, and limits on how many creatures
/// attack or block. Harmonize's "tap a creature to reduce that cost" reminder lowers to nothing.
/// </summary>
public sealed partial class Lowerer
{
    void RegisterRoundFifteen()
    {
        Rule<G.TapToReduceCost>((g, ctx) => null);

        // Selectors.
        Rule<G.AttachmentHost>((g, ctx) => new ObjectSelector(Quantifier.Any, FilterOf(ctx, kinds: g.Kind, controller: g.Controller)));
        Rule<G.CountedPlayers>((g, ctx) =>
            new ObjectSelector(
                Quantifier.Each,
                new ObjectFilter(
                    Kinds: [Kind("player")],
                    Qualifiers: g.Player == G.PlayerIdentity.Opponent ? [Q(G.PlayerIdentity.Opponent)] : null,
                    With: g.Who is null ? null : [PlayerRelativeOf(g.Who.Which, ctx)]),
                Other: g.Other == true,
                Binding: ctx.Bind(g)));
        // "The other": of two chosen, the one not acted on.
        Rule<G.TheOther>((g, ctx) => new ObjectSelector(Quantifier.Rest, new ObjectFilter(Kinds: [Kind("any")])));

        // Predicates.
        Rule<G.ControlledByShared>((g, ctx) => new SharedController(g.Controllers.ToString()));
        Rule<G.TargetingObjects>((g, ctx) =>
        {
            List<Qualifier> qualifiers = [];
            foreach (var node in LowerAll(g.Qualifier, ctx))
                if (node is Qualifier qualifier)
                    qualifiers.Add(qualifier);
            if (g.From is not null && Lower(g.From.Qualifier, ctx) is Qualifier source)
                qualifiers.Add(source with { Quality = "From" + source.Quality });
            return new ObjectFilter(Kinds: [new Term(g.Kinds.ToString(), nameof(G.TargetingKind))], Qualifiers: OrNull(qualifiers));
        });

        // Quantities and counters.
        Rule<G.Amount>((g, ctx) => g.Difference ? new Special(nameof(SpecialAmount.TheDifference)) : Lower(g.GetResolvedValue(), ctx));
        Rule<G.NthCounter>((g, ctx) => new CounterSpec(PatternOf(g.Kind), new Fixed(OrdinalOf(g.Ordinal)), Prefix: nameof(CounterPrefixWord.Nth)));

        // Effects.
        Rule<G.EquipKind>((g, ctx) =>
        {
            var text = ctx.TextOf(g) ?? "";
            var brace = text.IndexOf('{');
            var kind = (brace > 6 ? text[6..brace] : text).Trim();
            return new KeywordInstance(nameof(G.Keyword.Equip), Cost: new PayMana(g.Cost.Items.Select(SymbolText).ToList()), Parameter: kind.Length > 0 ? kind : null);
        });
        Rule<G.PlayThisCardWhile>((g, ctx) =>
            new MayPlayFrom(Actor(ctx), [new Playable(g.Verb.ToString(), new Self())], new ZoneRef(nameof(Zone.Exile)), Duration: DurationOf(Lower(g.Period, ctx))));
        // The Ambush cycle: cast at instant speed, the permanent is sacrificed at the next cleanup. "Any time a sorcery couldn't have been cast" is read as "not during your main phase".
        Rule<G.FlashedPermanentSacrificed>((g, ctx) =>
            new Conditional(
                new Not(new WasCast(new Self(), new PlayerSelector(PlayerKind.You), During: new PhaseRef(nameof(G.Phase.MainPhase), Whose: new PlayerSelector(PlayerKind.You)))),
                new Delayed(new PhaseEvent(new PhaseRef("Cleanup", Next: true)), new Sacrifice(new PlayerSelector(PlayerKind.ControllerOf, new Self()), new Self()))));
        Rule<G.CombatantLimit>((g, ctx) => new CombatantLimit(QuantityOf(g.Count), g.Action.ToString()));

        // Conditions.
        Rule<G.RevealedOrControlledAsCast>((g, ctx) =>
            new Or(
            [
                new ChoiceMade(nameof(ChoiceWord.RevealedCard)),
                new Controls(new PlayerSelector(PlayerKind.You), new ObjectSelector(Quantifier.Any, FilterOf(ctx, kinds: g.Controlled))),
            ]));
    }

    /// <summary>"Seventh" as 7; "next" as 1.</summary>
    static int OrdinalOf(G.SpellOrdinal ordinal) =>
        ordinal switch
        {
            G.SpellOrdinal.First => 1,
            G.SpellOrdinal.Second => 2,
            G.SpellOrdinal.Third => 3,
            G.SpellOrdinal.Fourth => 4,
            G.SpellOrdinal.Fifth => 5,
            G.SpellOrdinal.Sixth => 6,
            G.SpellOrdinal.Seventh => 7,
            G.SpellOrdinal.Eighth => 8,
            G.SpellOrdinal.Ninth => 9,
            G.SpellOrdinal.Tenth => 10,
            G.SpellOrdinal.Eleventh => 11,
            G.SpellOrdinal.Twelfth => 12,
            _ => 1,
        };

    /// <summary>"Exiles all cards from it", where "it" is a zone named earlier ("reveals their hand"): the zone the antecedent names, that player's.</summary>
    ZoneRef ZoneOfReference(It it, LoweringContext ctx)
    {
        static string ZoneIn(string text) =>
            text is null ? null : new[] { (nameof(Zone.Hand), text.LastIndexOf("hand", StringComparison.Ordinal)), (nameof(Zone.Graveyard), text.LastIndexOf("graveyard", StringComparison.Ordinal)), (nameof(Zone.Library), text.LastIndexOf("library", StringComparison.Ordinal)) }
                .Where(x => x.Item2 >= 0).OrderByDescending(x => x.Item2).Select(x => x.Item1).FirstOrDefault();
        // The antecedent the grammar found, else the last zone the line named before "it".
        var zone = ZoneIn((ctx.Reference(it) as Reference)?.Antecedent?.ToLowerInvariant()) ?? ZoneIn(ctx.TextBefore(it)?.ToLowerInvariant());
        return zone is null ? null : new ZoneRef(zone, new PlayerSelector(PlayerKind.ThatPlayer));
    }
}
