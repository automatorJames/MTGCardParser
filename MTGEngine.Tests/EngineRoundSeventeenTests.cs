namespace MTGEngine.Tests;

/// <summary>The seventeenth engine round: fabricate, doubled and switched stats, doubled counters, "the greatest / lowest" among, comparing what players control, "the life lost this way", conditional alternative costs, mana of the chosen color.</summary>
public sealed class EngineRoundSeventeenTests
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
    public void AccomplishedAutomatonFabricates()
    {
        var game = Started("Island", new PassiveAgent());
        var automaton = Put(game, Cards.Named("Accomplished Automaton"), 0);

        game.PlayTurn(new PlayerId(0));

        Assert.True(game.State.Object(automaton).CountersOf("+1/+1") == 1 || game.BattlefieldOf(0).Any(x => x.Card.Subtypes.Contains("Servo")));
    }

    [Fact]
    public void GrunnDoublesItsPowerAttackingAlone()
    {
        var game = Started("Forest", new GreedyAgent());
        Put(game, Cards.Named("Grunn, the Lonely King"), 0);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(10, game.P1().Life);
    }

    [Fact]
    public void DwarvenThaumaturgistSwitchesAWallToDeath()
    {
        var game = Started("Mountain", new GreedyAgent(attacks: false));
        Put(game, Cards.Named("Dwarven Thaumaturgist"), 0);
        var wall = Put(game, Cards.Vanilla("Wall", "{2}", 0, 4), 1);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Graveyard, game.State.Object(wall).Zone);
    }

    [Fact]
    public void SolarionDoublesItsCounters()
    {
        var game = Started("Island", new GreedyAgent(attacks: false));
        var solarion = Put(game, Cards.Named("Solarion"), 0);
        game.Emit(new CountersChanged(solarion, "+1/+1", 2));

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(4, game.State.Object(solarion).CountersOf("+1/+1"));
    }

    [Fact]
    public void AbzanBeastmasterDrawsWithTheToughestCreature()
    {
        var game = Started("Forest", new PassiveAgent());
        Put(game, Cards.Named("Abzan Beastmaster"), 0);
        Put(game, Cards.Vanilla("Wall", "{2}", 0, 4), 0);
        Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 1);

        game.PlayTurn(new PlayerId(0));

        Assert.Single(game.P0().Hand);
    }

    [Fact]
    public void CullingScalesDestroysTheLowestManaValue()
    {
        var game = Started("Plains", new GreedyAgent(attacks: false));
        Put(game, Cards.Named("Culling Scales"), 0);
        var bears = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 1);
        var thopter = Put(game, Cards.Named("Ornithopter"), 1);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Graveyard, game.State.Object(thopter).Zone);
        Assert.Equal(Zone.Battlefield, game.State.Object(bears).Zone);
    }

    [Fact]
    public void GiftOfEstatesSearchesWhenBehindOnLands()
    {
        var game = Started("Plains", new GreedyAgent(attacks: false));
        for (var i = 0; i < 2; i++)
            Put(game, Cards.Basic("Plains"), 0);
        for (var i = 0; i < 3; i++)
            Put(game, Cards.Basic("Plains"), 1);
        Put(game, Cards.Named("Gift of Estates"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(17, game.P0().Library.Count);
    }

    [Fact]
    public void AgentOfMasksGainsTheLifeLost()
    {
        var game = Started("Swamp", new PassiveAgent());
        Put(game, Cards.Named("Agent of Masks"), 0);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(19, game.P1().Life);
        Assert.Equal(21, game.P0().Life);
    }

    [Fact]
    public void BalothCageTrapCostsLessAfterAnOpponentsArtifactEnters()
    {
        var game = Started("Forest", new PassiveAgent());
        for (var i = 0; i < 2; i++)
            Put(game, Cards.Basic("Forest"), 0);
        var trap = Put(game, Cards.Named("Baloth Cage Trap"), 0, Zone.Hand);
        game.Emit(new TurnBegan(1, new PlayerId(0)));
        game.Emit(new StepBegan(Step.Main1, new PlayerId(0)));
        Assert.DoesNotContain(game.LegalActions(new PlayerId(0)), x => x is CastSpell cast && cast.Card == trap);

        Put(game, Cards.Named("Ornithopter"), 1);

        Assert.Contains(game.LegalActions(new PlayerId(0)), x => x is CastSpell { Alternative: >= 0 } cast && cast.Card == trap);
    }

    [Fact]
    public void ColdsteelHeartMakesTheChosenColor()
    {
        var game = Started("Island", new PassiveAgent());
        var heart = Put(game, Cards.Named("Coldsteel Heart"), 0);
        game.Emit(new Untapped(heart));
        var chosen = Colors.OfName(game.State.Object(heart).Chosen("Color"));

        Assert.NotNull(chosen);
        var symbol = chosen.Value switch { Color.White => "{W}", Color.Blue => "{U}", Color.Black => "{B}", Color.Red => "{R}", _ => "{G}" };
        Assert.True(ManaPayment.CanPay(game.State, new PlayerId(0), ManaCost.Parse(symbol)));
        Assert.False(ManaPayment.CanPay(game.State, new PlayerId(0), ManaCost.Parse("{2}")));
    }

    [Fact]
    public void WormfangDrakeReturnsTheCreatureItExiled()
    {
        var game = Started("Island", new GreedyAgent(attacks: false));
        var bears = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 0);
        var drake = Put(game, Cards.Named("Wormfang Drake"), 0);

        game.PlayTurn(new PlayerId(0));
        Assert.Equal(Zone.Exile, game.State.Object(bears).Zone);
        Assert.Equal(Zone.Battlefield, game.State.Object(drake).Zone);

        game.MoveToZone(drake, Zone.Graveyard);
        game.PlayTurn(new PlayerId(1));

        Assert.Equal(Zone.Battlefield, game.State.Object(bears).Zone);
        Assert.Equal(Zone.Graveyard, game.State.Object(drake).Zone);
    }
}
