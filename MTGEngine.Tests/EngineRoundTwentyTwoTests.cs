namespace MTGEngine.Tests;

/// <summary>The twenty-second engine round: saddle, station, sneak, riot, afflict, annihilator, outlast, decayed, reinforce, ingest, surge, spectacle, jump-start, blitz.</summary>
public sealed class EngineRoundTwentyTwoTests
{
    /// <summary>Greedy, but takes an action of <paramref name="kind"/> first: activating an ability, or casting a spell another way than for its mana cost.</summary>
    sealed class PreferringAgent(Func<LegalAction, bool> kind, bool attacks = false) : IDecisionMaker
    {
        readonly GreedyAgent _greedy = new(attacks);

        public DecisionResponse Decide(GameState state, Decision decision)
        {
            if (decision is ChooseAction action)
                for (var i = 0; i < action.Options.Count; i++)
                    if (kind(action.Options[i]))
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

    static Game Started(string land, IDecisionMaker first, IDecisionMaker second = null) =>
        GameExtensions.Started(Cards.DeckOf("A", (Cards.Basic(land), 20)), Cards.DeckOf("B", (Cards.Basic("Plains"), 20)), first, second ?? new PassiveAgent(), 0);

    static void Lands(Game game, string land, int count)
    {
        for (var i = 0; i < count; i++)
            Put(game, Cards.Basic(land), 0);
    }

    [Fact]
    public void BrightfieldMustangAttacksSaddled()
    {
        var game = Started("Plains", new GreedyAgent());
        var mustang = Put(game, Cards.Named("Brightfield Mustang"), 0);
        var squire = Put(game, Cards.Vanilla("Squire", "{W}", 1, 1), 0);

        game.PlayTurn(new PlayerId(0));

        Assert.True(game.State.Object(squire).Tapped);
        Assert.Equal(1, game.State.Object(mustang).CountersOf("+1/+1"));
        Assert.False(game.State.Object(mustang).Tapped);
        Assert.Equal(16, game.P1().Life);
    }

    [Fact]
    public void GalvanizingSawshipFliesOnceStationed()
    {
        var game = Started("Mountain", new GreedyAgent(attacks: false));
        var sawship = Put(game, Cards.Named("Galvanizing Sawship"), 0);
        Assert.False(Characteristics.Of(game.State, game.State.Object(sawship)).IsCreature);
        Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 0);
        Put(game, Cards.Vanilla("Elf", "{G}", 1, 1), 0);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(3, game.State.Object(sawship).CountersOf("charge"));
        var traits = Characteristics.Of(game.State, game.State.Object(sawship));
        Assert.True(traits.IsCreature);
        Assert.True(traits.HasKeyword("Flying"));
    }

    [Fact]
    public void FootNinjasSneakInForAnUnblockedAttacker()
    {
        var game = Started("Plains", new GreedyAgent());
        Lands(game, "Plains", 4);
        var squire = Put(game, Cards.Vanilla("Squire", "{W}", 1, 1), 0);
        var ninjas = Put(game, Cards.Named("Foot Ninjas"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Battlefield, game.State.Object(ninjas).Zone);
        Assert.Equal(Zone.Hand, game.State.Object(squire).Zone);
        Assert.Equal(15, game.P1().Life);
        Assert.Equal(23, game.P0().Life);
    }

    [Fact]
    public void ZhurTaaGoblinRiots()
    {
        var game = Started("Mountain", new GreedyAgent(attacks: false));
        Put(game, Cards.Basic("Mountain"), 0);
        Put(game, Cards.Basic("Forest"), 0);
        var goblin = Put(game, Cards.Named("Zhur-Taa Goblin"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        var traits = Characteristics.Of(game.State, game.State.Object(goblin));
        Assert.True(game.State.Object(goblin).CountersOf("+1/+1") == 1 || traits.HasKeyword("Haste"));
    }

    [Fact]
    public void KhenraEternalAfflictsWhenBlocked()
    {
        var game = Started("Swamp", new GreedyAgent(), new GreedyAgent(attacks: false, blocks: true));
        Put(game, Cards.Named("Khenra Eternal"), 0);
        Put(game, Cards.Vanilla("Wall", "{1}{W}", 0, 4), 1);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(19, game.P1().Life);
    }

    [Fact]
    public void ArtisanOfKozilekAnnihilates()
    {
        var game = Started("Plains", new GreedyAgent());
        Put(game, Cards.Named("Artisan of Kozilek"), 0);
        var first = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 1);
        var second = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 1);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Graveyard, game.State.Object(first).Zone);
        Assert.Equal(Zone.Graveyard, game.State.Object(second).Zone);
        Assert.Equal(10, game.P1().Life);
    }

    [Fact]
    public void DisownedAncestorOutlasts()
    {
        var game = Started("Swamp", new GreedyAgent(attacks: false));
        Lands(game, "Swamp", 2);
        var ancestor = Put(game, Cards.Named("Disowned Ancestor"), 0);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(1, game.State.Object(ancestor).CountersOf("+1/+1"));
        Assert.True(game.State.Object(ancestor).Tapped);
    }

    [Fact]
    public void DecayedTokenIsSacrificedAfterAttacking()
    {
        var game = Started("Swamp", new GreedyAgent());
        var zombie = game.CreateToken(new TokenSpec(Types: ["Creature"], Subtypes: ["Zombie"], Power: new Fixed(2), Toughness: new Fixed(2), Keywords: [new KeywordInstance("Decayed")]), new PlayerId(0));
        game.Emit(new SummoningSicknessCleared(new PlayerId(0)));
        Assert.Contains(game.State.Object(zombie).Card.Abilities.OfType<StaticAbility>(), x => x.Effect is CombatRestriction { Cant: ["Block"] });

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(18, game.P1().Life);
        Assert.DoesNotContain(game.BattlefieldOf(0), x => x.Id == zombie);
    }

    [Fact]
    public void BurrentonBombardierReinforces()
    {
        var game = Started("Plains", new PreferringAgent(x => x is ActivateAbility));
        Lands(game, "Plains", 3);
        var bears = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 0);
        var bombardier = Put(game, Cards.Named("Burrenton Bombardier"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(2, game.State.Object(bears).CountersOf("+1/+1"));
        Assert.Equal(Zone.Graveyard, game.State.Object(bombardier).Zone);
    }

    [Fact]
    public void CullingDroneIngests()
    {
        var game = Started("Swamp", new GreedyAgent());
        Put(game, Cards.Named("Culling Drone"), 0);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(18, game.P1().Life);
        Assert.Equal(19, game.P1().Library.Count);
        Assert.Single(game.P1().Exile);
    }

    [Fact]
    public void GoblinFreerunnerSurgesAfterAnotherSpell()
    {
        var game = Started("Mountain", new GreedyAgent(attacks: false));
        Lands(game, "Mountain", 3);
        Put(game, Cards.Vanilla("Squire", "{R}", 1, 1), 0, Zone.Hand);
        var freerunner = Put(game, Cards.Named("Goblin Freerunner"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Battlefield, game.State.Object(freerunner).Zone);
    }

    [Fact]
    public void SpikewheelAcrobatIsASpectacle()
    {
        var game = Started("Mountain", new GreedyAgent(attacks: false));
        Lands(game, "Mountain", 4);
        Put(game, Cards.Named("Shock"), 0, Zone.Hand);
        var acrobat = Put(game, Cards.Named("Spikewheel Acrobat"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(18, game.P1().Life);
        Assert.Equal(Zone.Battlefield, game.State.Object(acrobat).Zone);
    }

    [Fact]
    public void MaximizeAltitudeJumpStartsAndIsExiled()
    {
        var game = Started("Island", new GreedyAgent(attacks: false));
        Lands(game, "Island", 1);
        var bears = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 0);
        var fodder = Put(game, Cards.Vanilla("Colossus", "{9}", 9, 9), 0, Zone.Hand);
        var altitude = Put(game, Cards.Named("Maximize Altitude"), 0, Zone.Graveyard);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Graveyard, game.State.Object(fodder).Zone);
        Assert.Equal(Zone.Exile, game.State.Object(altitude).Zone);
        Assert.Contains(game.Log.OfType<TemporaryEffectAdded>(), x => x.Effect.Target == bears);
    }

    [Fact]
    public void NightClubberBlitzesInAndDrawsWhenSacrificed()
    {
        var game = Started("Swamp", new PreferringAgent(x => x is CastSpell { Alternative: not -1 }, attacks: true));
        Lands(game, "Swamp", 3);
        var clubber = Put(game, Cards.Named("Night Clubber"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(18, game.P1().Life);
        Assert.Equal(Zone.Graveyard, game.State.Object(clubber).Zone);
        Assert.Single(game.P0().Hand);
    }

    [Fact]
    public void AngelicBlessingPumpsAndGrantsFlyingToItsOneTarget()
    {
        var game = Started("Plains", new GreedyAgent(attacks: false));
        Lands(game, "Plains", 3);
        var bears = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 0);
        Put(game, Cards.Named("Angelic Blessing"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        // Both steps applied to the one target (they last until end of turn, so read the log).
        var effects = game.Log.OfType<TemporaryEffectAdded>().Where(x => x.Effect.Target == bears).Select(x => x.Effect).ToList();
        Assert.Contains(effects, x => x.Power == 3);
        Assert.Contains(effects, x => x.Keywords.Contains("Flying"));
    }

    [Fact]
    public void AerialFormationStrivesForEveryTarget()
    {
        var game = Started("Island", new GreedyAgent(attacks: false));
        Lands(game, "Island", 4);
        var first = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 0);
        var second = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 0);
        var formation = Put(game, Cards.Named("Aerial Formation"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        // Two targets: {U} and {2}{U} for the one beyond the first, and both get +1/+1 and flying.
        Assert.Equal(Zone.Graveyard, game.State.Object(formation).Zone);
        Assert.All(game.BattlefieldOf(0).Where(x => x.Card.IsLand), x => Assert.True(x.Tapped));
        var effects = game.Log.OfType<TemporaryEffectAdded>().Select(x => x.Effect).ToList();
        foreach (var bears in new[] { first, second })
        {
            Assert.Contains(effects, x => x.Target == bears && x.Power == 1);
            Assert.Contains(effects, x => x.Target == bears && x.Keywords.Contains("Flying"));
        }
    }

    [Fact]
    public void BlessingsOfNatureDistributesCountersAmongItsTargets()
    {
        var game = Started("Forest", new GreedyAgent(attacks: false));
        Lands(game, "Forest", 5);
        var first = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 0);
        var second = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 0);
        Put(game, Cards.Named("Blessings of Nature"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(4, game.State.Object(first).CountersOf("+1/+1") + game.State.Object(second).CountersOf("+1/+1"));
        Assert.True(game.State.Object(first).CountersOf("+1/+1") >= 1);
        Assert.True(game.State.Object(second).CountersOf("+1/+1") >= 1);
    }
}
