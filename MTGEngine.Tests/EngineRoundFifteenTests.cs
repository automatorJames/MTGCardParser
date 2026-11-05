namespace MTGEngine.Tests;

/// <summary>The fifteenth engine round: evoke, dash, storm, delve, improvise, entwine, connive, soulshift, plot.</summary>
public sealed class EngineRoundFifteenTests
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

    static Game Started(string land, IDecisionMaker first, int handSize = 0) =>
        GameExtensions.Started(Cards.DeckOf("A", (Cards.Basic(land), 20)), Cards.DeckOf("B", (Cards.Basic("Plains"), 20)), first, new PassiveAgent(), handSize);

    static void Lands(Game game, string land, int count)
    {
        for (var i = 0; i < count; i++)
            Put(game, Cards.Basic(land), 0);
    }

    [Fact]
    public void AethersnipeEvokedBouncesAndIsSacrificed()
    {
        var game = Started("Island", new GreedyAgent(attacks: false));
        Lands(game, "Island", 3);
        var snipe = Put(game, Cards.Named("Aethersnipe"), 0, Zone.Hand);
        var bears = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 1);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Graveyard, game.State.Object(snipe).Zone);
        Assert.Equal(Zone.Hand, game.State.Object(bears).Zone);
    }

    [Fact]
    public void AleshasVanguardDashedAttacksAndReturns()
    {
        var game = Started("Swamp", new GreedyAgent());
        Lands(game, "Swamp", 3);
        var vanguard = Put(game, Cards.Named("Alesha's Vanguard"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(17, game.P1().Life);
        Assert.Equal(Zone.Hand, game.State.Object(vanguard).Zone);
    }

    [Fact]
    public void GrapeshotCopiesItselfForEachSpellBefore()
    {
        var game = Started("Mountain", new GreedyAgent(attacks: false));
        Lands(game, "Mountain", 3);
        Put(game, Cards.Named("Shock"), 0, Zone.Hand);
        Put(game, Cards.Named("Grapeshot"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(16, game.P1().Life);
    }

    [Fact]
    public void TreasureCruiseDelvesAwayTheGraveyard()
    {
        var game = Started("Island", new GreedyAgent(attacks: false));
        for (var i = 0; i < 7; i++)
            Put(game, Cards.Basic("Island"), 0, Zone.Graveyard);
        Lands(game, "Island", 1);
        var cruise = Put(game, Cards.Named("Treasure Cruise"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Graveyard, game.State.Object(cruise).Zone);
        Assert.Equal(7, game.State.Objects.Values.Count(x => x.Zone == Zone.Exile));
        Assert.Equal(17, game.P0().Library.Count);
    }

    [Fact]
    public void BarricadeBreakerIsImprovisedWithArtifacts()
    {
        var game = Started("Plains", new GreedyAgent(attacks: false));
        Lands(game, "Plains", 3);
        for (var i = 0; i < 4; i++)
            Put(game, Cards.Named("Ornithopter"), 0);
        var breaker = Put(game, Cards.Named("Barricade Breaker"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Battlefield, game.State.Object(breaker).Zone);
    }

    [Fact]
    public void AbunasChantEntwinedDoesBoth()
    {
        var game = Started("Plains", new GreedyAgent(attacks: false));
        Lands(game, "Plains", 6);
        Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 0);
        Put(game, Cards.Named("Abuna's Chant"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(25, game.P0().Life);
        Assert.Contains(game.Log.OfType<ModesChosen>(), x => x.Modes.Count == 2);
    }

    [Fact]
    public void ScientistsConniveAndKeepACounterForANonlandDiscard()
    {
        var game = Started("Island", new GreedyAgent(attacks: false));
        Put(game, Cards.Vanilla("Colossus", "{9}", 9, 9), 0, Zone.Hand);
        var scientists = Put(game, Cards.Named("A.I.M. Scientists"), 0);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(1, game.State.Object(scientists).CountersOf("+1/+1"));
        Assert.Equal(Zone.Graveyard, game.Find("Colossus").Zone);
    }

    [Fact]
    public void BodyOfJukaiSoulshiftsASpiritBack()
    {
        var game = Started("Forest", new GreedyAgent(attacks: false));
        var kami = Put(game, Cards.Named("Kami of Ancient Law"), 0, Zone.Graveyard);
        var body = Put(game, Cards.Named("Body of Jukai"), 0);

        game.MoveToZone(body, Zone.Graveyard);
        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Hand, game.State.Object(kami).Zone);
    }

    [Fact]
    public void DjinnIsPlottedThenCastFreeOnALaterTurn()
    {
        var game = Started("Island", new GreedyAgent(attacks: false));
        Lands(game, "Island", 4);
        var djinn = Put(game, Cards.Named("Djinn of Fool's Fall"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));
        Assert.Equal(Zone.Exile, game.State.Object(djinn).Zone);

        game.PlayTurn(new PlayerId(1));
        game.PlayTurn(new PlayerId(0));
        Assert.Equal(Zone.Battlefield, game.State.Object(djinn).Zone);
    }
}
