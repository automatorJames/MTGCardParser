namespace MTGEngine.Tests;

/// <summary>The nineteenth engine round: spree, offspring, warp, backup, awaken.</summary>
public sealed class EngineRoundNineteenTests
{
    /// <summary>Casts for an alternative cost when it can, otherwise plays as <see cref="GreedyAgent"/>.</summary>
    sealed class AlternativeAgent : IDecisionMaker
    {
        readonly GreedyAgent _greedy = new(attacks: false);

        public DecisionResponse Decide(GameState state, Decision decision)
        {
            if (decision is ChooseAction action)
                for (var i = 0; i < action.Options.Count; i++)
                    if (action.Options[i] is CastSpell { Alternative: not -1 })
                        return DecisionResponse.Choose(i);
            return _greedy.Decide(state, decision);
        }
    }

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
    public void DanceOfTheTumbleweedsDoesBothSpreeModes()
    {
        var game = Started("Forest", new GreedyAgent(attacks: false));
        Lands(game, "Forest", 6);
        Put(game, Cards.Named("Dance of the Tumbleweeds"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(19, game.P0().Library.Count);
        Assert.Contains(game.BattlefieldOf(0), x => x.IsToken && x.Card.Subtypes.Contains("Elemental"));
    }

    [Fact]
    public void ThundertrapTrainerWithOffspringMakesATokenCopy()
    {
        var game = Started("Island", new GreedyAgent(attacks: false));
        Lands(game, "Island", 6);
        Put(game, Cards.Named("Thundertrap Trainer"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(2, game.BattlefieldOf(0).Count(x => x.Card.Name == "Thundertrap Trainer"));
        Assert.Single(game.BattlefieldOf(0), x => x.Card.Name == "Thundertrap Trainer" && x.IsToken);
    }

    [Fact]
    public void PinnacleEmissaryWarpsInThenIsCastableFromExile()
    {
        var game = Started("Island", new GreedyAgent(attacks: false));
        Lands(game, "Island", 1);
        var emissary = Put(game, Cards.Named("Pinnacle Emissary"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));
        Assert.Equal(Zone.Exile, game.State.Object(emissary).Zone);

        Put(game, Cards.Basic("Mountain"), 0);
        Put(game, Cards.Basic("Island"), 0);
        Put(game, Cards.Basic("Island"), 0);
        game.Emit(new TurnBegan(3, new PlayerId(0)));
        game.Emit(new StepBegan(Step.Main1, new PlayerId(0)));
        Assert.Contains(game.LegalActions(new PlayerId(0)), x => x is CastSpell { Alternative: -1 } cast && cast.Card == emissary);
    }

    [Fact]
    public void SaibaCryptomancerBacksUpAnotherCreature()
    {
        var game = Started("Island", new PassiveAgent());
        var bears = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 1);
        Put(game, Cards.Named("Saiba Cryptomancer"), 0);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(1, game.State.Object(bears).CountersOf("+1/+1"));
    }

    [Fact]
    public void BoilingEarthAwakensALand()
    {
        var game = Started("Mountain", new AlternativeAgent());
        Lands(game, "Mountain", 7);
        Put(game, Cards.Named("Boiling Earth"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        var awakened = Assert.Single(game.BattlefieldOf(0), x => x.CountersOf("+1/+1") == 4);
        var traits = Characteristics.Of(game.State, awakened);
        Assert.True(traits.IsCreature);
        Assert.Equal(4, traits.Power);
        Assert.True(traits.HasKeyword("Haste"));
    }

    [Fact]
    public void StingSlingerBlightsToActivate()
    {
        var game = Started("Mountain", new GreedyAgent(attacks: false));
        Lands(game, "Mountain", 2);
        Put(game, Cards.Named("Sting-Slinger"), 0);
        Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 0);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(18, game.P1().Life);
        Assert.Equal(1, game.BattlefieldOf(0).Sum(x => x.CountersOf("-1/-1")));
    }

    [Fact]
    public void ForensicResearcherCollectsEvidence()
    {
        var game = Started("Island", new GreedyAgent(attacks: false));
        Put(game, Cards.Named("Forensic Researcher"), 0);
        for (var i = 0; i < 3; i++)
            Put(game, Cards.Named("Shock"), 0, Zone.Graveyard);
        var bears = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 1);

        game.PlayTurn(new PlayerId(0));

        Assert.True(game.State.Object(bears).Tapped);
        Assert.Equal(3, game.State.Objects.Values.Count(x => x.Zone == Zone.Exile));
    }

    [Fact]
    public void SpeedStartsRisesOncePerTurnAndUnlocksMaxSpeed()
    {
        var game = Started("Plains", new GreedyAgent());
        var assailant = Put(game, Cards.Named("Swiftwing Assailant"), 0);
        game.CheckStateAndTriggers();
        Assert.Equal(1, game.P0().Speed);

        game.PlayTurn(new PlayerId(0));
        Assert.Equal(17, game.P1().Life);
        Assert.Equal(2, game.P0().Speed);
        Assert.False(Characteristics.Of(game.State, game.State.Object(assailant)).HasKeyword("Vigilance"));

        game.Emit(new SpeedChanged(new PlayerId(0), 4, 1));
        var traits = Characteristics.Of(game.State, game.State.Object(assailant));
        Assert.True(traits.HasKeyword("Vigilance"));
        Assert.Equal(4, traits.Toughness);
    }

    [Fact]
    public void NyxbornRollickerBestowedEnchantsThenBecomesACreature()
    {
        var game = Started("Mountain", new AlternativeAgent());
        Lands(game, "Mountain", 2);
        var bears = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 0);
        var rollicker = Put(game, Cards.Named("Nyxborn Rollicker"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(bears, game.State.Object(rollicker).AttachedTo);
        Assert.False(Characteristics.Of(game.State, game.State.Object(rollicker)).IsCreature);
        Assert.Equal(3, Characteristics.Of(game.State, game.State.Object(bears)).Power);

        game.MoveToZone(bears, Zone.Graveyard);
        game.CheckStateAndTriggers();

        Assert.Equal(Zone.Battlefield, game.State.Object(rollicker).Zone);
        Assert.True(Characteristics.Of(game.State, game.State.Object(rollicker)).IsCreature);
    }

    [Fact]
    public void BlustersquallOverloadedTapsEachCreature()
    {
        var game = Started("Island", new AlternativeAgent());
        Lands(game, "Island", 4);
        var first = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 1);
        var second = Put(game, Cards.Vanilla("Elk", "{1}", 1, 1), 1);
        Put(game, Cards.Named("Blustersquall"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.True(game.State.Object(first).Tapped);
        Assert.True(game.State.Object(second).Tapped);
    }
}
