namespace MTGEngine.Core;

/// <summary>
/// Keywords read as a spell is cast or a card drawn: casualty (sacrifice a creature with power N or greater as you
/// cast it, and it's copied), cascade (a free spell from the top of the library when it's cast), and dredge (mill N
/// instead of drawing, and return the card from the graveyard to the hand).
/// </summary>
public sealed partial class Game
{
    static readonly PlayerSelector _you = new(PlayerKind.You);

    /// <summary>A keyword's number on <paramref name="card"/> ("casualty 2", "dredge 5"); null when it doesn't have the keyword.</summary>
    static int? KeywordAmount(CardDefinition card, string keyword) =>
        card.Abilities.OfType<KeywordLine>().SelectMany(x => x.Keywords).FirstOrDefault(x => CardDefinition.Normalize(x.Keyword) == keyword) is { } found
            ? found.Amount is Fixed { Value: var n } ? n : 1
            : null;

    /// <summary>Casualty N's cost: sacrificing a creature you control with power N or greater.</summary>
    static Cost CasualtyCost(int power) =>
        new ActionCost(new MTGRulesIR.Model.Sacrifice(_you, new ObjectSelector(Quantifier.Any, new ObjectFilter(Kinds: [new Term("Creature", "CardType")], Controller: new Controlled(_you),
            With: [new CharacteristicBound(["Power"], new Fixed(power), Comparison.GreaterOrEqual)]))));

    /// <summary>Casualty, decided as the spell is cast: the sacrifice is planned with its other costs.</summary>
    bool PlanCasualty(CardDefinition card, PlayerId player, ObjectId id, ref ManaCost mana, List<Effect> actions, ImmutableHashSet<string>.Builder options)
    {
        if (KeywordAmount(card, "Casualty") is not { } power)
            return true;
        var cost = CasualtyCost(power);
        if (!AdditionalPayable(cost, player, id, 0) || !Decide(new YesNo(player, $"Casualty {power}: sacrifice a creature with power {power} or greater to copy {card.Name}?")).Yes)
            return true;
        options.Add("Casualty");
        return PlanAdditional(cost, player, id, 0, ref mana, actions, options);
    }

    /// <summary>The triggers a spell's casting keywords add as it's cast: a copy for casualty, cascade.</summary>
    internal static IEnumerable<PendingTrigger> CastKeywordTriggers(GameObject spell, PlayerId controller)
    {
        if (spell.WasCastWith("Casualty"))
            yield return new PendingTrigger(spell, new TriggeredAbility(new NamedEvent("Casualty"), new CopySpell(new Self(), new Fixed(1), NewTargets: true)), null, controller);
        if (spell.Card.Keywords.Contains("Cascade"))
            yield return new PendingTrigger(spell, new TriggeredAbility(new NamedEvent("Cascade"),
                new KeywordAction([new KeywordInstance("Cascade", Amount: new Fixed(spell.Card.ManaCost.ManaValue - 1))])), null, controller);
    }

    /// <summary>
    /// Dredge: instead of drawing, <paramref name="player"/> may mill N cards and return a card with dredge N from
    /// their graveyard to their hand, if their library has N cards. True when a draw was replaced this way.
    /// </summary>
    bool Dredged(PlayerId player)
    {
        var library = State.Player(player).Library;
        foreach (var card in State.Player(player).Graveyard.Select(State.Object).Where(x => x is not null).ToList())
        {
            if (KeywordAmount(card.Card, "Dredge") is not { } count || library.Count < count)
                continue;
            if (!Decide(new YesNo(player, $"Dredge {card.Card.Name}: mill {count} instead of drawing?")).Yes)
                continue;
            foreach (var milled in State.Player(player).Library.Take(count).ToList())
            {
                Emit(new Milled(player, milled));
                MoveToZone(milled, Zone.Graveyard);
            }
            MoveToZone(card.Id, Zone.Hand);
            return true;
        }
        return false;
    }
}
