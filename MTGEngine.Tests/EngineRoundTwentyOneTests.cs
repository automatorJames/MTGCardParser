namespace MTGEngine.Tests;

/// <summary>The twenty-first engine round: scavenge, embalm, eternalize, transmute, modular, unleash, mentor, evolve, extort, split second, totem armor, clash in a spell, escape, retrace, cascade, dredge, casualty.</summary>
public sealed class EngineRoundTwentyOneTests
{
    /// <summary>Greedy, but activates an ability before casting a spell.</summary>
    sealed class ActivatingAgent : IDecisionMaker
    {
        readonly GreedyAgent _greedy = new(attacks: false);

        public DecisionResponse Decide(GameState state, Decision decision)
        {
            if (decision is ChooseAction action)
                for (var i = 0; i < action.Options.Count; i++)
                    if (action.Options[i] is ActivateAbility)
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
    public void DeadbridgeGoliathScavengesOntoACreature()
    {
        var game = Started("Forest", new GreedyAgent(attacks: false));
        Lands(game, "Forest", 6);
        var bears = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 0);
        var goliath = Put(game, Cards.Named("Deadbridge Goliath"), 0, Zone.Graveyard);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(5, game.State.Object(bears).CountersOf("+1/+1"));
        Assert.Equal(Zone.Exile, game.State.Object(goliath).Zone);
    }

    [Fact]
    public void SacredCatEmbalmsIntoAWhiteZombie()
    {
        var game = Started("Plains", new GreedyAgent(attacks: false));
        Lands(game, "Plains", 1);
        var cat = Put(game, Cards.Named("Sacred Cat"), 0, Zone.Graveyard);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Exile, game.State.Object(cat).Zone);
        var token = Assert.Single(game.BattlefieldOf(0), x => x.Card.IsToken);
        Assert.Contains("Zombie", token.Card.Subtypes);
        Assert.Equal([Color.White], token.Card.Colors);
        Assert.Equal(0, token.Card.ManaCost.ManaValue);
    }

    [Fact]
    public void ProvenCombatantEternalizesAsABlackFourFour()
    {
        var game = Started("Island", new GreedyAgent(attacks: false));
        Lands(game, "Island", 6);
        Put(game, Cards.Named("Proven Combatant"), 0, Zone.Graveyard);

        game.PlayTurn(new PlayerId(0));

        var token = Assert.Single(game.BattlefieldOf(0), x => x.Card.IsToken);
        Assert.Equal(4, Characteristics.Of(game.State, token).Power);
        Assert.Equal([Color.Black], token.Card.Colors);
    }

    [Fact]
    public void DriftOfPhantasmsTransmutesForACardWithTheSameManaValue()
    {
        var game = Started("Island", new ActivatingAgent());
        Lands(game, "Island", 3);
        var trio = Put(game, Cards.Vanilla("Trio", "{2}{U}", 2, 2), 0, Zone.Library);
        var drift = Put(game, Cards.Named("Drift of Phantasms"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Graveyard, game.State.Object(drift).Zone);
        Assert.Equal(Zone.Hand, game.State.Object(trio).Zone);
    }

    [Fact]
    public void ArcboundWorkerMovesItsCountersWhenItDies()
    {
        var game = Started("Plains", new GreedyAgent(attacks: false));
        var first = Put(game, Cards.Named("Arcbound Worker"), 0);
        var second = Put(game, Cards.Named("Arcbound Worker"), 0);
        Assert.Equal(1, game.State.Object(first).CountersOf("+1/+1"));

        game.Destroy(first);
        game.PlayTurn(new PlayerId(0));

        Assert.Equal(2, game.State.Object(second).CountersOf("+1/+1"));
    }

    [Fact]
    public void GoreHouseChainwalkerUnleashedCantBlock()
    {
        var game = Started("Mountain", new GreedyAgent(attacks: false));
        Lands(game, "Mountain", 2);
        var chainwalker = Put(game, Cards.Named("Gore-House Chainwalker"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(1, game.State.Object(chainwalker).CountersOf("+1/+1"));
        Assert.Contains(game.State.Object(chainwalker).Card.Abilities.OfType<StaticAbility>(), x => x.Effect is CombatRestriction { Cant: ["Block"] });
    }

    [Fact]
    public void SunhomeStalwartMentorsALesserAttacker()
    {
        var game = Started("Plains", new GreedyAgent());
        Put(game, Cards.Named("Sunhome Stalwart"), 0);
        var squire = Put(game, Cards.Vanilla("Squire", "{W}", 1, 1), 0);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(1, game.State.Object(squire).CountersOf("+1/+1"));
        Assert.Equal(16, game.P1().Life);
    }

    [Fact]
    public void CloudfinRaptorEvolvesWhenABiggerCreatureEnters()
    {
        var game = Started("Forest", new GreedyAgent(attacks: false));
        Lands(game, "Forest", 3);
        var raptor = Put(game, Cards.Named("Cloudfin Raptor"), 0);
        Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 0, Zone.Hand);
        Put(game, Cards.Vanilla("Elf", "{G}", 1, 1), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        // The bears are bigger (2/2 against 0/1); the elf then isn't (1/1 against 1/2).
        Assert.Equal(1, game.State.Object(raptor).CountersOf("+1/+1"));
    }

    [Fact]
    public void BasilicaGuardsExtortsWhenASpellIsCast()
    {
        var game = Started("Plains", new GreedyAgent(attacks: false));
        Lands(game, "Plains", 3);
        Put(game, Cards.Named("Basilica Guards"), 0);
        Put(game, Cards.Vanilla("Bears", "{1}{W}", 2, 2), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(19, game.P1().Life);
        Assert.Equal(21, game.P0().Life);
    }

    [Fact]
    public void SuddenShockLeavesNoTimeToRespond()
    {
        var game = Started("Mountain", new GreedyAgent(attacks: false));
        Lands(game, "Mountain", 3);
        var sudden = Put(game, Cards.Named("Sudden Shock"), 0, Zone.Hand);
        Put(game, Cards.Named("Shock"), 0, Zone.Hand);
        game.Emit(new TurnBegan(1, new PlayerId(0)));
        game.Emit(new StepBegan(Step.Main1, new PlayerId(0)));
        Assert.Equal(2, game.LegalActions(new PlayerId(0)).Count(x => x is CastSpell));

        game.Take(Assert.Single(game.LegalActions(new PlayerId(0)), x => x is CastSpell cast && cast.Card == sudden), new PlayerId(0));

        Assert.Single(game.State.Stack);
        Assert.DoesNotContain(game.LegalActions(new PlayerId(0)), x => x is CastSpell);
    }

    [Fact]
    public void BearUmbraIsDestroyedInsteadOfTheCreature()
    {
        var game = Started("Forest", new GreedyAgent(attacks: false));
        var bears = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 0);
        var umbra = Put(game, Cards.Named("Bear Umbra"), 0, Zone.Hand);
        game.MoveToZone(umbra, Zone.Battlefield, new PlayerId(0), attachTo: bears);

        Assert.True(game.Destroy(bears));

        Assert.Equal(Zone.Battlefield, game.State.Object(bears).Zone);
        Assert.Equal(Zone.Graveyard, game.State.Object(umbra).Zone);
    }

    [Fact]
    public void LashOutHitsTheControllerWhenItWinsTheClash()
    {
        var game = Started("Mountain", new GreedyAgent(attacks: false));
        Lands(game, "Mountain", 2);
        var colossus = Put(game, Cards.Vanilla("Colossus", "{9}", 9, 9), 0, Zone.Hand);
        game.MoveToZone(colossus, Zone.Library, new PlayerId(0), position: 0);
        var bears = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 1);
        Put(game, Cards.Named("Lash Out"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Graveyard, game.State.Object(bears).Zone);
        Assert.Equal(17, game.P1().Life);
    }

    [Fact]
    public void SleepOfTheDeadEscapesFromTheGraveyard()
    {
        var game = Started("Island", new GreedyAgent(attacks: false));
        Lands(game, "Island", 3);
        var bears = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 1);
        var others = Enumerable.Range(0, 3).Select(i => Put(game, Cards.Vanilla($"Fodder {i}", "{1}", 1, 1), 0, Zone.Graveyard)).ToList();
        var sleep = Put(game, Cards.Named("Sleep of the Dead"), 0, Zone.Graveyard);

        game.PlayTurn(new PlayerId(0));

        Assert.True(game.State.Object(bears).Tapped);
        Assert.All(others, x => Assert.Equal(Zone.Exile, game.State.Object(x).Zone));
        Assert.Equal(Zone.Graveyard, game.State.Object(sleep).Zone);
    }

    [Fact]
    public void RavensCrimeRetracesByDiscardingALand()
    {
        var game = Started("Swamp", new GreedyAgent(attacks: false));
        Lands(game, "Swamp", 1);
        Put(game, Cards.Basic("Swamp"), 0, Zone.Hand);
        Put(game, Cards.Basic("Swamp"), 0, Zone.Hand);
        var victim = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 1, Zone.Hand);
        var crime = Put(game, Cards.Named("Raven's Crime"), 0, Zone.Graveyard);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Graveyard, game.State.Object(victim).Zone);
        Assert.Equal(Zone.Graveyard, game.State.Object(crime).Zone);
        Assert.Empty(game.P0().Hand);
    }

    [Fact]
    public void ArdentPleaCascadesIntoACheaperSpell()
    {
        var game = Started("Plains", new GreedyAgent(attacks: false));
        Lands(game, "Plains", 2);
        Lands(game, "Island", 1);
        var bears = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 0, Zone.Hand);
        game.MoveToZone(bears, Zone.Library, new PlayerId(0), position: 1);
        var plea = Put(game, Cards.Named("Ardent Plea"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Battlefield, game.State.Object(plea).Zone);
        Assert.Equal(Zone.Battlefield, game.State.Object(bears).Zone);
        // The plains exiled above the bears went to the bottom.
        Assert.Equal(20, game.P0().Library.Count);
    }

    [Fact]
    public void DakmorSalvageDredgesInsteadOfADraw()
    {
        var game = Started("Swamp", new GreedyAgent(attacks: false));
        var salvage = Put(game, Cards.Named("Dakmor Salvage"), 0, Zone.Graveyard);

        game.Draw(new PlayerId(0), 1);

        Assert.Equal(Zone.Hand, game.State.Object(salvage).Zone);
        Assert.Equal(18, game.P0().Library.Count);
        Assert.Equal(2, game.P0().Graveyard.Count);
    }

    [Fact]
    public void IllicitShipmentIsCopiedForItsCasualty()
    {
        var game = Started("Swamp", new GreedyAgent(attacks: false));
        Lands(game, "Swamp", 5);
        var brute = Put(game, Cards.Vanilla("Brute", "{2}{B}", 3, 3), 0);
        Put(game, Cards.Named("Illicit Shipment"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Graveyard, game.State.Object(brute).Zone);
        Assert.Equal(18, game.P0().Library.Count);
    }
}
