namespace MTGEngine.Tests;

/// <summary>The twenty-third engine round: "that player" and "it" across a spell's sentences; "attacks and isn't blocked", detain, skipping a turn, "one or two targets".</summary>
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

    [Fact]
    public void RuptureDealsTheSacrificedCreaturesPower()
    {
        var game = Started("Mountain", new GreedyAgent(attacks: false));
        Lands(game, "Mountain", 3);
        var brute = Put(game, Cards.Vanilla("Brute", "{3}", 3, 3), 0);
        Put(game, Cards.Named("Rupture"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Graveyard, game.State.Object(brute).Zone);
        Assert.Equal(17, game.P1().Life);
        Assert.Equal(17, game.P0().Life);
    }

    [Fact]
    public void ScribNibblersGainsLifeForAnExiledLand()
    {
        var game = Started("Plains", new GreedyAgent(attacks: false));
        Put(game, Cards.Named("Scrib Nibblers"), 0);

        game.PlayTurn(new PlayerId(0));

        Assert.Single(game.P1().Exile);
        Assert.Equal(21, game.P0().Life);
    }

    [Fact]
    public void SwampMosquitoPoisonsWhenUnblocked()
    {
        var game = Started("Swamp", new GreedyAgent());
        Put(game, Cards.Named("Swamp Mosquito"), 0);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(1, game.P1().Poison);
    }

    [Fact]
    public void LyevDecreeDetainsUntilYourNextTurn()
    {
        var game = Started("Plains", new GreedyAgent(attacks: false));
        Lands(game, "Plains", 2);
        var bears = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 1);
        Put(game, Cards.Named("Lyev Decree"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));
        Assert.True(Characteristics.Of(game.State, game.State.Object(bears)).Cant("Attack"));

        game.PlayTurn(new PlayerId(1));
        game.PlayTurn(new PlayerId(0));
        Assert.False(Characteristics.Of(game.State, game.State.Object(bears)).Cant("Attack"));
    }

    [Fact]
    public void ChronatogSkipsYourNextTurn()
    {
        var game = GameExtensions.Started(Cards.DeckOf("A", (Cards.Basic("Island"), 20)), Cards.DeckOf("B", (Cards.Basic("Plains"), 20)), new GreedyAgent(attacks: false), new PassiveAgent(), 0);
        Put(game, Cards.Named("Chronatog"), 0);

        game.Run(maxTurns: 3);

        // P0's turn 1 activates it; P1 takes turn 2, and P0's next turn is skipped, so P1 takes turn 3 too.
        Assert.Equal(new PlayerId(1), game.State.ActivePlayer);
        Assert.Equal(3, game.State.Turn);
    }

    [Fact]
    public void PredictivePreparationsTakesOneOrTwoTargets()
    {
        var game = Started("Plains", new GreedyAgent(attacks: false));
        Lands(game, "Plains", 2);
        var first = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 0);
        var second = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 0);
        Put(game, Cards.Named("Predictive Preparations"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(1, game.State.Object(first).CountersOf("+1/+1"));
        Assert.Equal(1, game.State.Object(second).CountersOf("+1/+1"));
    }

    [Fact]
    public void MinionsReturnBringsTheCreatureBackOnce()
    {
        var game = Started("Swamp", new GreedyAgent(attacks: false));
        var bears = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 1);
        var aura = Put(game, Cards.Named("Minion's Return"), 0, Zone.Hand);
        game.MoveToZone(aura, Zone.Battlefield, new PlayerId(0), attachTo: bears);

        game.Destroy(bears);
        game.PlayTurn(new PlayerId(0));
        Assert.Equal(Zone.Battlefield, game.State.Object(bears).Zone);
        Assert.Equal(new PlayerId(0), game.State.Object(bears).Controller);
        Assert.Equal(Zone.Graveyard, game.State.Object(aura).Zone);

        game.Destroy(bears);
        game.PlayTurn(new PlayerId(1));
        Assert.Equal(Zone.Graveyard, game.State.Object(bears).Zone);
    }

    [Fact]
    public void MinionsReturnDoesntLoopOnACreatureThatDiesAsItReturns()
    {
        var game = Started("Swamp", new GreedyAgent(attacks: false));
        var husk = Put(game, Cards.Named("Guiding Hydra"), 1);
        game.Emit(new CountersChanged(husk, "+1/+1", 1));
        var aura = Put(game, Cards.Named("Minion's Return"), 0, Zone.Hand);
        game.MoveToZone(aura, Zone.Battlefield, new PlayerId(0), attachTo: husk);

        game.Destroy(husk);
        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Graveyard, game.State.Object(husk).Zone);
        Assert.Equal(Zone.Graveyard, game.State.Object(aura).Zone);
    }

    [Fact]
    public void MinionsReturnIgnoresCreaturesItDoesntEnchant()
    {
        var game = Started("Swamp", new GreedyAgent(attacks: false));
        var bears = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 1);
        var other = Put(game, Cards.Vanilla("Elf", "{G}", 1, 1), 1);
        var aura = Put(game, Cards.Named("Minion's Return"), 0, Zone.Hand);
        game.MoveToZone(aura, Zone.Battlefield, new PlayerId(0), attachTo: bears);

        game.Destroy(other);
        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Graveyard, game.State.Object(other).Zone);
        Assert.Equal(new PlayerId(1), game.State.Object(bears).Controller);
    }

    [Fact]
    public void AquastrandSpiderGraftsOntoACreatureThatEnters()
    {
        // No lands, so the spider's own {g} ability doesn't spend the turn.
        var game = Started("Forest", new GreedyAgent(attacks: false));
        var spider = Put(game, Cards.Named("Aquastrand Spider"), 0);
        Assert.Equal(2, game.State.Object(spider).CountersOf("+1/+1"));
        var bears = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 0);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(1, game.State.Object(spider).CountersOf("+1/+1"));
        Assert.Equal(1, game.State.Object(bears).CountersOf("+1/+1"));
    }

    [Fact]
    public void WizardClassDrawsTwoWhenItBecomesLevelTwo()
    {
        var game = Started("Island", new GreedyAgent(attacks: false));
        Lands(game, "Island", 3);
        Put(game, Cards.Named("Wizard Class"), 0);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(18, game.P0().Library.Count);
    }

    [Fact]
    public void AcidicSoilHitsEachPlayerForTheirOwnLands()
    {
        var game = Started("Mountain", new GreedyAgent(attacks: false));
        Lands(game, "Mountain", 3);
        Put(game, Cards.Basic("Plains"), 1);
        Put(game, Cards.Named("Acidic Soil"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(17, game.P0().Life);
        Assert.Equal(19, game.P1().Life);
    }

    [Fact]
    public void NaturesResurgenceDrawsForEachPlayersOwnCreatureCards()
    {
        var game = Started("Forest", new GreedyAgent(attacks: false));
        Lands(game, "Forest", 4);
        Put(game, Cards.Vanilla("Fallen", "{1}", 1, 1), 0, Zone.Graveyard);
        Put(game, Cards.Vanilla("Fallen", "{1}", 1, 1), 1, Zone.Graveyard);
        Put(game, Cards.Vanilla("Fallen", "{1}", 1, 1), 1, Zone.Graveyard);
        Put(game, Cards.Named("Nature's Resurgence"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(19, game.P0().Library.Count);
        Assert.Equal(18, game.P1().Library.Count);
    }

    [Fact]
    public void EmptyTheCatacombsReturnsEachPlayersCreatureCards()
    {
        var game = Started("Swamp", new GreedyAgent(attacks: false));
        Lands(game, "Swamp", 4);
        var mine = Put(game, Cards.Vanilla("Fallen", "{1}", 1, 1), 0, Zone.Graveyard);
        var theirs = Put(game, Cards.Vanilla("Fallen", "{1}", 1, 1), 1, Zone.Graveyard);
        Put(game, Cards.Named("Empty the Catacombs"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Hand, game.State.Object(theirs).Zone);
        Assert.True(game.State.Object(mine).Zone is Zone.Hand or Zone.Stack or Zone.Battlefield);
    }

    [Fact]
    public void FrostGiantRampagesForEachBlockerBeyondTheFirst()
    {
        var game = GameExtensions.Started(Cards.DeckOf("A", (Cards.Basic("Mountain"), 20)), Cards.DeckOf("B", (Cards.Basic("Plains"), 20)), new GreedyAgent(), new GreedyAgent(attacks: false, blocks: true), 0);
        var giant = Put(game, Cards.Named("Frost Giant"), 0);
        for (var i = 0; i < 3; i++)
            Put(game, Cards.Vanilla("Wall", "{1}{W}", 0, 1), 1);

        game.PlayTurn(new PlayerId(0));

        // Three blockers: two beyond the first, rampage 2 gives +4/+4.
        Assert.Contains(game.Log.OfType<TemporaryEffectAdded>(), x => x.Effect.Target == giant && x.Effect.Power == 4 && x.Effect.Toughness == 4);
    }

    [Fact]
    public void SearingBarbIncubatesATokenThatHatches()
    {
        var game = Started("Mountain", new GreedyAgent(attacks: false));
        Lands(game, "Mountain", 5);
        Put(game, Cards.Named("Searing Barb"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        // Cast for {2}{r}, then {2} more transforms the incubator into a 1/1 Phyrexian artifact creature.
        var incubator = Assert.Single(game.BattlefieldOf(0), x => x.Card.IsToken);
        Assert.Equal(1, incubator.CountersOf("+1/+1"));
        var traits = Characteristics.Of(game.State, incubator);
        Assert.True(traits.IsCreature);
        Assert.Equal(1, traits.Power);
    }

    [Fact]
    public void StrongholdDisciplineCostsEachPlayerForTheirOwnCreatures()
    {
        var game = Started("Swamp", new GreedyAgent(attacks: false));
        Lands(game, "Swamp", 4);
        Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 0);
        for (var i = 0; i < 3; i++)
            Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 1);
        Put(game, Cards.Named("Stronghold Discipline"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(19, game.P0().Life);
        Assert.Equal(17, game.P1().Life);
    }

    [Fact]
    public void TyphoonCountsTheOpponentsIslands()
    {
        var game = Started("Forest", new GreedyAgent(attacks: false));
        Lands(game, "Forest", 3);
        Put(game, Cards.Basic("Island"), 1);
        Put(game, Cards.Basic("Island"), 1);
        Put(game, Cards.Named("Typhoon"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(18, game.P1().Life);
    }

    [Fact]
    public void PainfulTruthsDrawsForEachColorSpent()
    {
        var game = Started("Swamp", new GreedyAgent(attacks: false));
        Put(game, Cards.Basic("Swamp"), 0);
        Put(game, Cards.Basic("Island"), 0);
        Put(game, Cards.Basic("Mountain"), 0);
        Put(game, Cards.Named("Painful Truths"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(17, game.P0().Life);
        Assert.Equal(17, game.P0().Library.Count);
    }

    [Fact]
    public void CurfewReturnsACreatureOfEachPlayers()
    {
        var game = Started("Island", new GreedyAgent(attacks: false));
        Lands(game, "Island", 1);
        var mine = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 0);
        var theirs = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 1);
        Put(game, Cards.Named("Curfew"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Hand, game.State.Object(theirs).Zone);
        Assert.True(game.State.Object(mine).Zone is Zone.Hand or Zone.Stack or Zone.Battlefield);
    }

    [Fact]
    public void SengirVampireGrowsWhenACreatureItDamagedDies()
    {
        var game = Started("Swamp", new GreedyAgent(attacks: false));
        var vampire = Put(game, Cards.Named("Sengir Vampire"), 0);
        var bears = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 1);
        var other = Put(game, Cards.Vanilla("Elf", "{G}", 1, 1), 1);
        game.Emit(new TurnBegan(1, new PlayerId(0)));

        game.DealDamage(vampire, TargetRef.Of(bears), 2, combat: false);
        game.Destroy(other);
        game.CheckStateAndTriggers();
        game.ResolveStack();

        Assert.Equal(Zone.Graveyard, game.State.Object(bears).Zone);
        Assert.Equal(1, game.State.Object(vampire).CountersOf("+1/+1"));
    }

    [Fact]
    public void StirTheGraveTargetsWithTheXItsCastFor()
    {
        var game = Started("Swamp", new GreedyAgent(attacks: false));
        Lands(game, "Swamp", 4);
        var fallen = Put(game, Cards.Vanilla("Fallen", "{2}{B}", 2, 1), 0, Zone.Graveyard);
        Put(game, Cards.Named("Stir the Grave"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Battlefield, game.State.Object(fallen).Zone);
    }

    [Fact]
    public void TheFallOfKroogTargetsALandOfTheChosenOpponent()
    {
        var game = Started("Mountain", new GreedyAgent(attacks: false));
        Lands(game, "Mountain", 6);
        var land = Put(game, Cards.Basic("Plains"), 1);
        Put(game, Cards.Named("The Fall of Kroog"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Graveyard, game.State.Object(land).Zone);
        Assert.Equal(17, game.P1().Life);
    }
}
