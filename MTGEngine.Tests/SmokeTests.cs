using MTGEngine.Agents;

namespace MTGEngine.Tests;

/// <summary>Whole games between random decks of the fixture's real cards, played by random agents: no exceptions, a game ends or runs out its turns, and it replays the same from its seed.</summary>
public sealed class SmokeTests
{
    static Deck RandomDeck(string player, Random rng)
    {
        var spells = Cards.All.Where(x => !x.IsLand).ToList();
        var lands = new[] { "Plains", "Island", "Swamp", "Mountain", "Forest" }.Select(Cards.Basic).ToList();
        List<CardDefinition> deck = [];
        for (var i = 0; i < 24; i++)
            deck.Add(spells[rng.Next(spells.Count)]);
        for (var i = 0; i < 16; i++)
            deck.Add(lands[rng.Next(lands.Count)]);
        return new Deck(player, deck.ToImmutableList());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    public void RandomGamesOfRealCardsPlayThrough(int seed)
    {
        var rng = new Random(seed);
        var decks = new[] { RandomDeck("A", rng), RandomDeck("B", rng) };

        var game = Game.Start(decks, [new RandomAgent(seed), new RandomAgent(seed + 100)], seed);
        game.Run(maxTurns: 30);

        Assert.True(game.State.Over || game.State.Turn >= 30);
    }

    [Fact]
    public void ARandomGameOfRealCardsReplaysFromItsSeed()
    {
        string Play()
        {
            var rng = new Random(42);
            var game = Game.Start([RandomDeck("A", rng), RandomDeck("B", rng)], [new RandomAgent(1), new RandomAgent(2)], 42);
            game.Run(maxTurns: 20);
            return string.Join("\n", game.Log.Select(x => x.GetType().Name));
        }

        Assert.Equal(Play(), Play());
    }
}
