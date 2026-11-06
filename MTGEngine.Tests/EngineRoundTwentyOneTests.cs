namespace MTGEngine.Tests;

/// <summary>The twenty-first engine round: scavenge, embalm, eternalize, transmute, modular, unleash, mentor, evolve, extort.</summary>
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
}
