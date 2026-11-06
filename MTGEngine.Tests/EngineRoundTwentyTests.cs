namespace MTGEngine.Tests;

/// <summary>The twentieth engine round: renown, sunburst, learn, crimes, restricted mana from "add x mana …, where x is …".</summary>
public sealed class EngineRoundTwentyTests
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

    [Fact]
    public void KnightBecomesRenownedOnce()
    {
        var game = Started("Plains", new GreedyAgent());
        var knight = Put(game, Cards.Named("Knight of the Pilgrim's Road"), 0);

        game.PlayTurn(new PlayerId(0));
        Assert.Equal(17, game.P1().Life);
        Assert.Equal(1, game.State.Object(knight).CountersOf("+1/+1"));

        game.PlayTurn(new PlayerId(1));
        game.PlayTurn(new PlayerId(0));
        Assert.Equal(13, game.P1().Life);
        Assert.Equal(1, game.State.Object(knight).CountersOf("+1/+1"));
    }

    [Fact]
    public void SuntouchedMyrCountsTheColorsSpent()
    {
        var game = Started("Plains", new GreedyAgent(attacks: false));
        Put(game, Cards.Basic("Plains"), 0);
        Put(game, Cards.Basic("Island"), 0);
        Put(game, Cards.Basic("Swamp"), 0);
        var myr = Put(game, Cards.Named("Suntouched Myr"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(3, game.State.Object(myr).CountersOf("+1/+1"));
    }

    [Fact]
    public void CramSessionLearnsByRummaging()
    {
        var game = Started("Swamp", new GreedyAgent(attacks: false));
        Put(game, Cards.Basic("Swamp"), 0);
        Put(game, Cards.Basic("Swamp"), 0);
        var colossus = Put(game, Cards.Vanilla("Colossus", "{9}", 9, 9), 0, Zone.Hand);
        Put(game, Cards.Named("Cram Session"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(24, game.P0().Life);
        Assert.Equal(Zone.Graveyard, game.State.Object(colossus).Zone);
        Assert.Equal(19, game.P0().Library.Count);
    }

    [Fact]
    public void DeepmuckDesperadoMillsWhenYouCommitACrime()
    {
        var game = Started("Mountain", new GreedyAgent(attacks: false));
        Put(game, Cards.Basic("Mountain"), 0);
        Put(game, Cards.Named("Deepmuck Desperado"), 0);
        Put(game, Cards.Named("Shock"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(18, game.P1().Life);
        Assert.Equal(17, game.P1().Library.Count);
    }

    [Fact]
    public void FoodChainMakesManaOnlyForCreatureSpells()
    {
        var game = Started("Forest", new GreedyAgent(attacks: false));
        var chain = Put(game, Cards.Named("Food Chain"), 0);
        Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 0);
        game.Emit(new TurnBegan(1, new PlayerId(0)));
        game.Emit(new StepBegan(Step.Main1, new PlayerId(0)));

        game.Take(Assert.Single(game.LegalActions(new PlayerId(0)), x => x is ActivateAbility activate && activate.Source == chain), new PlayerId(0));

        Assert.Equal(3, game.P0().RestrictedList.Sum(x => x.Amount));
        Assert.True(game.P0().Pool.IsEmpty);
    }

    [Fact]
    public void GiantAmbushBeetleForcesABlock()
    {
        var game = Started("Swamp", new GreedyAgent());
        var bears = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 1);
        Put(game, Cards.Named("Giant Ambush Beetle"), 0);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(20, game.P1().Life);
        Assert.Equal(Zone.Graveyard, game.State.Object(bears).Zone);
    }

    [Fact]
    public void MarblePriestIsBlockedByWalls()
    {
        var game = Started("Plains", new GreedyAgent());
        Put(game, Cards.Named("Marble Priest"), 0);
        Put(game, Cards.Vanilla("Stone Wall", "{2}", 0, 4, "Wall"), 1);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(20, game.P1().Life);
    }

    [Fact]
    public void EffectsOnACreatureDontFollowItToANewZone()
    {
        var game = Started("Forest", new PassiveAgent());
        var bears = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 0);
        game.Emit(new TemporaryEffectAdded(new TemporaryEffect(bears, -1, -1, ["Flying"], game.State.Turn)));
        Assert.Equal(1, Characteristics.Of(game.State, game.State.Object(bears)).Power);

        game.MoveToZone(bears, Zone.Exile);
        game.MoveToZone(bears, Zone.Battlefield, new PlayerId(0));

        var traits = Characteristics.Of(game.State, game.State.Object(bears));
        Assert.Equal(2, traits.Power);
        Assert.False(traits.HasKeyword("Flying"));
    }

    [Fact]
    public void PrimordialGnawerDiscoversAndCastsFree()
    {
        var deck = Cards.DeckOf("A", (Cards.Basic("Mountain"), 2), (Cards.Named("Shock"), 1), (Cards.Basic("Mountain"), 17));
        var game = GameExtensions.Started(deck, Cards.DeckOf("B", (Cards.Basic("Plains"), 20)), new GreedyAgent(attacks: false), new PassiveAgent(), 0);
        var gnawer = Put(game, Cards.Named("Primordial Gnawer"), 0);

        game.MoveToZone(gnawer, Zone.Graveyard);
        game.PlayTurn(new PlayerId(0));

        Assert.Equal(18, game.P1().Life);
        Assert.Equal(19, game.P0().Library.Count);
    }

    [Fact]
    public void AdderStaffBoggartWinsAClash()
    {
        var deck = Cards.DeckOf("A", (Cards.Vanilla("Colossus", "{5}", 5, 5), 20));
        var game = GameExtensions.Started(deck, Cards.DeckOf("B", (Cards.Basic("Plains"), 20)), new PassiveAgent(), new PassiveAgent(), 0);
        var boggart = Put(game, Cards.Named("Adder-Staff Boggart"), 0);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(1, game.State.Object(boggart).CountersOf("+1/+1"));
    }

    [Fact]
    public void CranialExtractionExilesEveryCopyEverywhere()
    {
        var game = Started("Swamp", new GreedyAgent(attacks: false));
        for (var i = 0; i < 4; i++)
            Put(game, Cards.Basic("Swamp"), 0);
        var inHand = Put(game, Cards.Named("Shock"), 1, Zone.Hand);
        var inGraveyard = Put(game, Cards.Named("Shock"), 1, Zone.Graveyard);
        Put(game, Cards.Named("Cranial Extraction"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        var shocks = game.State.Objects.Values.Where(x => x.Card.Name == "Shock").ToList();
        Assert.All(shocks, x => Assert.Equal(Zone.Exile, x.Zone));
        Assert.Equal(Zone.Exile, game.State.Object(inHand).Zone);
        Assert.Equal(Zone.Exile, game.State.Object(inGraveyard).Zone);
    }
}
