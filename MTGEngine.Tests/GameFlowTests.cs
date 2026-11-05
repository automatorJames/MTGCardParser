using MTGEngine.Agents;

namespace MTGEngine.Tests;

public sealed class GameFlowTests
{
    static Deck BearsDeck(string name) =>
        Cards.DeckOf(name, (Cards.Basic("Forest"), 20), (Cards.Vanilla("Grizzly Bears", "{1}{G}", 2, 2, "Bear"), 20));

    [Fact]
    public void VanillaCreatureGameEndsWithAWinner()
    {
        var game = Game.Start([BearsDeck("A"), BearsDeck("B")], [new RandomAgent(1), new RandomAgent(2)], seed: 7);

        game.Run(maxTurns: 80);

        Assert.True(game.State.Over);
        Assert.NotNull(game.State.Winner);
        Assert.Contains(game.Log, x => x is DamageDealt { Combat: true });
        Assert.Contains(game.Log, x => x is PlayerLost);
        Assert.Empty(game.Skips);
    }

    [Fact]
    public void SameSeedReplaysTheSameGame()
    {
        var first = Game.Start([BearsDeck("A"), BearsDeck("B")], [new RandomAgent(3), new RandomAgent(4)], seed: 11);
        var second = Game.Start([BearsDeck("A"), BearsDeck("B")], [new RandomAgent(3), new RandomAgent(4)], seed: 11);

        first.Run(80);
        second.Run(80);

        Assert.Equal(first.Log.Count, second.Log.Count);
        Assert.Equal(first.State.Winner, second.State.Winner);
        Assert.Equal(first.State.Turn, second.State.Turn);
    }

    [Fact]
    public void TurnStepsHappenInOrderAndTheFirstPlayerSkipsTheirFirstDraw()
    {
        var game = GameExtensions.Started(BearsDeck("A"), BearsDeck("B"), new PassiveAgent(), new PassiveAgent());

        game.PlayTurn(new PlayerId(0));

        var steps = game.Log.OfType<StepBegan>().Select(x => x.Step).ToList();
        Assert.Equal([Step.Untap, Step.Upkeep, Step.Draw, Step.Main1, Step.BeginCombat, Step.DeclareAttackers, Step.EndCombat, Step.Main2, Step.End, Step.Cleanup], steps);
        Assert.Equal(7, game.P0().Hand.Count);

        game.PlayTurn(new PlayerId(1));
        Assert.Contains(game.Log, x => x is Drew { Player.Value: 1, Card: not null });
        Assert.Contains(game.Log, x => x is Discarded { Player.Value: 1 });
        Assert.Equal(7, game.P1().Hand.Count);
    }

    [Fact]
    public void DrawingFromAnEmptyLibraryLosesTheGame()
    {
        var game = GameExtensions.Started(Cards.DeckOf("A", (Cards.Basic("Forest"), 7)), Cards.DeckOf("B", (Cards.Basic("Plains"), 9)), new PassiveAgent(), new PassiveAgent());

        game.Run(maxTurns: 10);

        Assert.True(game.State.Over);
        Assert.Equal(new PlayerId(1), game.State.Winner);
        Assert.Contains(game.Log, x => x is PlayerLost { Reason: "empty library" });
    }

    [Fact]
    public void CombatDamageKillsBlockersAndHitsPlayers()
    {
        var bears = Cards.Vanilla("Grizzly Bears", "{1}{G}", 2, 2, "Bear");
        var giant = Cards.Vanilla("Hill Giant", "{3}{R}", 3, 3, "Giant");
        var game = GameExtensions.Started(Cards.DeckOf("A", (Cards.Basic("Forest"), 14)), Cards.DeckOf("B", (Cards.Basic("Forest"), 14)), new GreedyAgent(), new GreedyAgent(attacks: false, blocks: true));

        var attacker = Put(game, giant, 0);
        var blocker = Put(game, bears, 1);
        game.Emit(new SummoningSicknessCleared(new PlayerId(0)));

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Graveyard, game.State.Object(blocker).Zone);
        Assert.Equal(Zone.Battlefield, game.State.Object(attacker).Zone);
        Assert.Equal(20, game.P1().Life);

        game.PlayTurn(new PlayerId(1));
        game.PlayTurn(new PlayerId(0));
        Assert.Equal(17, game.P1().Life);
    }

    static ObjectId Put(Game game, CardDefinition card, int player)
    {
        var id = new ObjectId(game.State.NextObjectId);
        var owner = new PlayerId(player);
        game.Emit(new ObjectCreated(new GameObject(id, card, owner, owner, Zone.Hand)));
        game.MoveToZone(id, Zone.Battlefield, owner);
        return id;
    }
}
