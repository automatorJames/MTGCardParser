namespace MTGEngine.Tests;

/// <summary>The eighteenth engine round: static pumps "for each" and "where x is", historic and outlaw, "at random".</summary>
public sealed class EngineRoundEighteenTests
{
    static ObjectId Put(Game game, CardDefinition card, int player, Zone zone = Zone.Battlefield, ObjectId? attachTo = null)
    {
        var id = new ObjectId(game.State.NextObjectId);
        var owner = new PlayerId(player);
        game.Emit(new ObjectCreated(new GameObject(id, card, owner, owner, Zone.Hand)));
        if (zone != Zone.Hand)
            game.MoveToZone(id, zone, owner, attachTo: attachTo);
        game.Emit(new SummoningSicknessCleared(owner));
        return id;
    }

    static Game Started(string land, IDecisionMaker first) =>
        GameExtensions.Started(Cards.DeckOf("A", (Cards.Basic(land), 20)), Cards.DeckOf("B", (Cards.Basic("Plains"), 20)), first, new PassiveAgent(), 0);

    [Fact]
    public void ScourgeOfGeierReachGrowsForEachOpposingCreature()
    {
        var game = Started("Mountain", new PassiveAgent());
        var scourge = Put(game, Cards.Named("Scourge of Geier Reach"), 0);
        Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 1);
        Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 1);

        var traits = Characteristics.Of(game.State, game.State.Object(scourge));
        Assert.Equal(5, traits.Power);
        Assert.Equal(5, traits.Toughness);
    }

    [Fact]
    public void TimberpackWolvesCountEachOther()
    {
        var game = Started("Forest", new PassiveAgent());
        var first = Put(game, Cards.Named("Timberpack Wolf"), 0);
        Put(game, Cards.Named("Timberpack Wolf"), 0);

        Assert.Equal(3, Characteristics.Of(game.State, game.State.Object(first)).Power);
    }

    [Fact]
    public void DeathsApproachShrinksByTheGraveyard()
    {
        var game = Started("Swamp", new PassiveAgent());
        var bears = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 1);
        Put(game, Cards.Vanilla("Elk", "{1}", 1, 1), 1, Zone.Graveyard);
        Put(game, Cards.Vanilla("Elk", "{1}", 1, 1), 1, Zone.Graveyard);
        Put(game, Cards.Named("Death's Approach"), 0, attachTo: bears);

        game.CheckStateAndTriggers();

        Assert.Equal(Zone.Graveyard, game.State.Object(bears).Zone);
    }

    [Fact]
    public void JhoiraDrawsForAHistoricSpell()
    {
        var game = Started("Island", new GreedyAgent(attacks: false));
        Put(game, Cards.Named("Jhoira, Weatherlight Captain"), 0);
        Put(game, Cards.Named("Ornithopter"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(19, game.P0().Library.Count);
    }

    [Fact]
    public void CharmbreakerDevilsReturnsOneSpellAtRandom()
    {
        var game = Started("Mountain", new PassiveAgent());
        Put(game, Cards.Named("Charmbreaker Devils"), 0);
        var shock = Put(game, Cards.Named("Shock"), 0, Zone.Graveyard);
        var bolt = Put(game, Cards.Named("Lightning Bolt"), 0, Zone.Graveyard);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(1, new[] { shock, bolt }.Count(x => game.State.Object(x).Zone == Zone.Hand));
    }

    [Fact]
    public void KorSkyfisherReturnsOnePermanentNotAll()
    {
        var game = Started("Plains", new PassiveAgent());
        Put(game, Cards.Basic("Plains"), 0);
        Put(game, Cards.Basic("Plains"), 0);
        Put(game, Cards.Named("Kor Skyfisher"), 0);

        game.PlayTurn(new PlayerId(0));

        Assert.Single(game.P0().Hand);
    }

    [Fact]
    public void BogardanPhoenixReturnsOnceThenIsExiled()
    {
        var game = Started("Mountain", new PassiveAgent());
        var phoenix = Put(game, Cards.Named("Bogardan Phoenix"), 0);

        game.MoveToZone(phoenix, Zone.Graveyard);
        game.PlayTurn(new PlayerId(0));
        Assert.Equal(Zone.Battlefield, game.State.Object(phoenix).Zone);
        Assert.Equal(1, game.State.Object(phoenix).CountersOf("death"));

        game.MoveToZone(phoenix, Zone.Graveyard);
        game.PlayTurn(new PlayerId(1));
        Assert.Equal(Zone.Exile, game.State.Object(phoenix).Zone);
    }
}
