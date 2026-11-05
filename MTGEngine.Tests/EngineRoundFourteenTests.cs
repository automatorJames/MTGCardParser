namespace MTGEngine.Tests;

/// <summary>The fourteenth engine round: land-play triggers, vanishing and fading, foretell.</summary>
public sealed class EngineRoundFourteenTests
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
        GameExtensions.Started(Cards.DeckOf("A", (Cards.Basic(land), 20)), Cards.DeckOf("B", (Cards.Basic("Plains"), 20)), first, new PassiveAgent());

    [Fact]
    public void CityOfTraitorsIsSacrificedWhenYouPlayAnotherLand()
    {
        var game = Started("Swamp", new GreedyAgent(attacks: false));
        var city = Put(game, Cards.Named("City of Traitors"), 0);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Graveyard, game.State.Object(city).Zone);
    }

    [Fact]
    public void BlastodermFadesAway()
    {
        var game = Started("Forest", new PassiveAgent());
        var blastoderm = Put(game, Cards.Named("Blastoderm"), 0);
        Assert.Equal(3, game.State.Object(blastoderm).CountersOf("fade"));

        for (var turn = 0; turn < 3; turn++)
        {
            game.PlayTurn(new PlayerId(0));
            game.PlayTurn(new PlayerId(1));
        }
        Assert.Equal(Zone.Battlefield, game.State.Object(blastoderm).Zone);
        Assert.Equal(0, game.State.Object(blastoderm).CountersOf("fade"));

        game.PlayTurn(new PlayerId(0));
        Assert.Equal(Zone.Graveyard, game.State.Object(blastoderm).Zone);
    }

    [Fact]
    public void KeldonMaraudersVanishes()
    {
        var game = Started("Mountain", new PassiveAgent());
        var marauders = Put(game, Cards.Named("Keldon Marauders"), 0);
        Assert.Equal(2, game.State.Object(marauders).CountersOf("time"));

        game.PlayTurn(new PlayerId(0));
        Assert.Equal(Zone.Battlefield, game.State.Object(marauders).Zone);
        game.PlayTurn(new PlayerId(1));
        game.PlayTurn(new PlayerId(0));
        Assert.Equal(Zone.Graveyard, game.State.Object(marauders).Zone);
    }

    [Fact]
    public void DemonBoltIsForetoldThenCastForItsForetellCost()
    {
        var game = Started("Mountain", new PassiveAgent());
        for (var i = 0; i < 2; i++)
            Put(game, Cards.Basic("Mountain"), 0);
        var bolt = Put(game, Cards.Named("Demon Bolt"), 0, Zone.Hand);
        var bears = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 1);
        game.Emit(new TurnBegan(1, new PlayerId(0)));
        game.Emit(new StepBegan(Step.Main1, new PlayerId(0)));

        var foretell = Assert.Single(game.LegalActions(new PlayerId(0)), x => x is CastSpell { Alternative: Game.ForetellExile });
        game.Take(foretell, new PlayerId(0));
        Assert.True(game.State.Object(bolt) is { Zone: Zone.Exile, FaceDown: true });
        Assert.DoesNotContain(game.LegalActions(new PlayerId(0)), x => x is CastSpell cast && cast.Card == bolt);

        game.Emit(new Untapped(game.State.Battlefield.First(x => x.Card.Name == "Mountain" && x.Controller == new PlayerId(0)).Id));
        game.Emit(new TurnBegan(2, new PlayerId(0)));
        game.Emit(new StepBegan(Step.Main1, new PlayerId(0)));
        var cast = Assert.Single(game.LegalActions(new PlayerId(0)), x => x is CastSpell c && c.Card == bolt);
        game.Take(cast, new PlayerId(0));
        Assert.Equal(Zone.Stack, game.State.Object(bolt).Zone);
        Assert.False(game.State.Object(bolt).FaceDown);
    }
}
