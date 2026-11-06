namespace MTGEngine.Tests;

/// <summary>The twenty-third engine round: "that player" and "it" across a spell's sentences.</summary>
public sealed class EngineRoundTwentyThreeTests
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

    static Game Started(string land, IDecisionMaker first) =>
        GameExtensions.Started(Cards.DeckOf("A", (Cards.Basic(land), 20)), Cards.DeckOf("B", (Cards.Basic("Plains"), 20)), first, new PassiveAgent(), 0);

    static void Lands(Game game, string land, int count)
    {
        for (var i = 0; i < count; i++)
            Put(game, Cards.Basic(land), 0);
    }

    [Fact]
    public void ProphecyGainsLifeWhenTheRevealedCardIsALand()
    {
        var game = Started("Plains", new GreedyAgent(attacks: false));
        Lands(game, "Plains", 1);
        Put(game, Cards.Named("Prophecy"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(21, game.P0().Life);
    }

    [Fact]
    public void BloodTributeTakesHalfTheirLife()
    {
        var game = Started("Swamp", new GreedyAgent(attacks: false));
        Lands(game, "Swamp", 6);
        game.ChangeLife(new PlayerId(1), -5, "test");
        Put(game, Cards.Named("Blood Tribute"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        // Half of 15, rounded up.
        Assert.Equal(7, game.P1().Life);
    }

    [Fact]
    public void DuressTakesANoncreatureNonlandCard()
    {
        var game = Started("Swamp", new GreedyAgent(attacks: false));
        Lands(game, "Swamp", 1);
        var bears = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 1, Zone.Hand);
        var shock = Put(game, Cards.Named("Shock"), 1, Zone.Hand);
        Put(game, Cards.Named("Duress"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Graveyard, game.State.Object(shock).Zone);
        Assert.Equal(Zone.Hand, game.State.Object(bears).Zone);
    }
}
