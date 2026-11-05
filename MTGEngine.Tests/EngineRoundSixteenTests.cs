namespace MTGEngine.Tests;

/// <summary>The sixteenth engine round: combat damage to a creature and cards leaving a graveyard as triggers, mana in any combination of colors, static "doesn't untap" on others, power compared with another's, "if it targets", damage replaced by counters, "any player may activate", party size.</summary>
public sealed class EngineRoundSixteenTests
{
    static ObjectId Put(Game game, CardDefinition card, int player, Zone zone = Zone.Battlefield)
    {
        var id = new ObjectId(game.State.NextObjectId);
        var owner = new PlayerId(player);
        game.Emit(new ObjectCreated(new GameObject(id, card, owner, owner, Zone.Hand)));
        if (zone != Zone.Hand)
            game.MoveToZone(id, zone, owner);
        game.Emit(new SummoningSicknessCleared(owner));
        return id;
    }

    static Game Started(string land, IDecisionMaker first, IDecisionMaker second = null) =>
        GameExtensions.Started(Cards.DeckOf("A", (Cards.Basic(land), 20)), Cards.DeckOf("B", (Cards.Basic("Plains"), 20)), first, second ?? new PassiveAgent(), 0);

    [Fact]
    public void DeathCharmerPunishesTheCreatureItDamages()
    {
        var game = Started("Swamp", new GreedyAgent(), new GreedyAgent(attacks: false, blocks: true));
        Put(game, Cards.Named("Death Charmer"), 0);
        Put(game, Cards.Vanilla("Wall", "{2}", 0, 4), 1);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(18, game.P1().Life);
    }

    [Fact]
    public void ArkOfHungerTriggersWhenACardLeavesYourGraveyard()
    {
        var game = Started("Mountain", new PassiveAgent());
        Put(game, Cards.Named("Ark of Hunger"), 0);
        var card = Put(game, Cards.Basic("Mountain"), 0, Zone.Graveyard);

        game.MoveToZone(card, Zone.Exile);
        game.PlayTurn(new PlayerId(0));

        Assert.Equal(19, game.P1().Life);
        Assert.Equal(21, game.P0().Life);
    }

    [Fact]
    public void AxebaneGuardianMakesAnyColor()
    {
        var game = Started("Forest", new PassiveAgent());
        Put(game, Cards.Named("Axebane Guardian"), 0);

        Assert.True(ManaPayment.CanPay(game.State, new PlayerId(0), ManaCost.Parse("{U}")));
    }

    [Fact]
    public void BackToBasicsKeepsNonbasicLandsTapped()
    {
        var game = Started("Island", new PassiveAgent());
        Put(game, Cards.Named("Back to Basics"), 0);
        var city = Put(game, Cards.Named("City of Traitors"), 1);
        var plains = Put(game, Cards.Basic("Plains"), 1);
        game.Tap(city);
        game.Tap(plains);

        game.PlayTurn(new PlayerId(1));

        Assert.True(game.State.Object(city).Tapped);
        Assert.False(game.State.Object(plains).Tapped);
    }

    [Fact]
    public void DenProtectorCantBeBlockedBySmallerCreatures()
    {
        var game = Started("Forest", new GreedyAgent(), new GreedyAgent(attacks: false, blocks: true));
        Put(game, Cards.Named("Den Protector"), 0);
        Put(game, Cards.Vanilla("Squire", "{1}", 1, 3), 1);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(18, game.P1().Life);
    }

    [Fact]
    public void LuminousRebukeCostsLessTargetingATappedCreature()
    {
        var game = Started("Plains", new GreedyAgent(attacks: false));
        for (var i = 0; i < 2; i++)
            Put(game, Cards.Basic("Plains"), 0);
        var bears = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 1);
        game.Tap(bears);
        Put(game, Cards.Named("Luminous Rebuke"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Graveyard, game.State.Object(bears).Zone);
    }

    [Fact]
    public void LichenthropeTakesCountersInsteadOfDamage()
    {
        var game = Started("Forest", new PassiveAgent());
        var lichenthrope = Put(game, Cards.Named("Lichenthrope"), 0);

        game.DealDamage(null, TargetRef.Of(lichenthrope), 2, combat: false);

        Assert.Equal(2, game.State.Object(lichenthrope).CountersOf("-1/-1"));
        Assert.Equal(0, game.State.Object(lichenthrope).Damage);
    }

    [Fact]
    public void ExcavationCanBeActivatedByAnyPlayer()
    {
        var game = Started("Island", new PassiveAgent());
        var excavation = Put(game, Cards.Named("Excavation"), 0);
        Put(game, Cards.Basic("Plains"), 1);
        Put(game, Cards.Basic("Plains"), 1);

        Assert.Contains(game.LegalActions(new PlayerId(1)), x => x is ActivateAbility activate && activate.Source == excavation);
    }

    [Fact]
    public void CascadeSeerScriesForEachRoleInTheParty()
    {
        var game = Started("Island", new PassiveAgent());
        Put(game, Cards.Vanilla("Acolyte", "{1}", 1, 1, "Human", "Cleric"), 0);
        Put(game, Cards.Vanilla("Apprentice", "{1}", 1, 1, "Human", "Wizard"), 0);
        Put(game, Cards.Vanilla("Other Apprentice", "{1}", 1, 1, "Wizard"), 0);
        Put(game, Cards.Named("Cascade Seer"), 0);

        game.PlayTurn(new PlayerId(0));

        var scry = Assert.Single(game.Log.OfType<Scryed>());
        Assert.Equal(2, scry.Top.Count + scry.Bottom.Count);
    }
}
