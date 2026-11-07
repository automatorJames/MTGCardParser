namespace MTGEngine.Tests;

/// <summary>The twenty-fourth engine round: the Doran rule, maximum hand sizes, "those tokens" again, being a creature with base stats, untapping during other players' untap steps, triggers that last a turn, and the Ward cycle's exemption.</summary>
public sealed class EngineRoundTwentyFourTests
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

    static void Lands(Game game, string land, int count, int player = 0)
    {
        for (var i = 0; i < count; i++)
            Put(game, Cards.Basic(land), player);
    }

    static EffectContext Context(Game game, ObjectId? source = null) =>
        new(game, new PlayerId(0), source, new Dictionary<IrNode, ImmutableList<TargetRef>>(ReferenceEqualityComparer.Instance));

    static readonly CardDefinition Rock = new("Rock", ManaCost.Free, [], ["Artifact"], [], null, null, null, [], [], []);

    [Fact]
    public void DoranAssignsCombatDamageByToughness()
    {
        var game = Started("Forest", new GreedyAgent());
        Put(game, Cards.Named("Doran, the Siege Tower"), 0);
        Put(game, Cards.Vanilla("Wall", "{1}{W}", 0, 4), 0);

        game.PlayTurn(new PlayerId(0));

        // Doran is a 0/5 and the wall a 0/4: nine damage by toughness, none by power.
        Assert.Equal(11, game.P1().Life);
    }

    [Fact]
    public void WalkingBulwarkGivesTheRuleUntilEndOfTurn()
    {
        var game = Started("Plains", new PassiveAgent());
        var wall = Put(game, Cards.Vanilla("Wall", "{1}{W}", 0, 4), 0);

        Interpreter.Execute(new AssignsDamageByToughness(new ObjectSelector(Quantifier.All, new ObjectFilter(Kinds: [new Term("Creature", "CardType")])), new ThisTurn()), Context(game));

        Assert.True(Characteristics.Of(game.State, game.State.Object(wall)).Has("AssignsDamageByToughness"));
        game.PlayTurn(new PlayerId(1));
        Assert.False(Characteristics.Of(game.State, game.State.Object(wall)).Has("AssignsDamageByToughness"));
    }

    [Fact]
    public void GnatMiserShrinksOpponentsHands()
    {
        var game = Started("Plains", new GreedyAgent(attacks: false));
        Put(game, Cards.Named("Gnat Miser"), 1);
        for (var i = 0; i < 8; i++)
            Put(game, Cards.Vanilla("Brute", "{5}{G}", 3, 3), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        // Eight cards (plus the draw, on a later turn) against a maximum of six: discards down to six.
        Assert.Equal(6, game.P0().Hand.Count);
    }

    [Fact]
    public void SpellbookRemovesTheMaximum()
    {
        var game = Started("Plains", new GreedyAgent(attacks: false));
        Put(game, Cards.Named("Spellbook"), 0);
        for (var i = 0; i < 9; i++)
            Put(game, Cards.Vanilla("Brute", "{5}{G}", 3, 3), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(9, game.P0().Hand.Count);
    }

    [Fact]
    public void ConquerorsPledgeKickedMakesTwelveOfThoseTokens()
    {
        var game = Started("Plains", new GreedyAgent(attacks: false));
        Lands(game, "Plains", 11);
        Put(game, Cards.Named("Conqueror's Pledge"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        var tokens = game.BattlefieldOf(0).Where(x => x.IsToken).ToList();
        Assert.Equal(12, tokens.Count);
        Assert.All(tokens, x => Assert.Contains("Kor", x.Card.Subtypes));
    }

    [Fact]
    public void ConquerorsPledgeUnkickedMakesSix()
    {
        var game = Started("Plains", new GreedyAgent(attacks: false));
        Lands(game, "Plains", 5);
        Put(game, Cards.Named("Conqueror's Pledge"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(6, game.BattlefieldOf(0).Count(x => x.IsToken));
    }

    [Fact]
    public void EnsoulArtifactMakesTheArtifactAFiveFive()
    {
        var game = Started("Island", new GreedyAgent(attacks: false));
        Lands(game, "Island", 2);
        var rock = Put(game, Rock, 0);
        Put(game, Cards.Named("Ensoul Artifact"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        var traits = Characteristics.Of(game.State, game.State.Object(rock));
        Assert.True(traits.IsCreature);
        Assert.Contains("Artifact", traits.Types);
        Assert.Equal(5, traits.Power);
        Assert.Equal(5, traits.Toughness);
    }

    [Fact]
    public void SeedbornMuseUntapsDuringTheOpponentsUntapStep()
    {
        var game = Started("Forest", new PassiveAgent(), new PassiveAgent());
        Put(game, Cards.Named("Seedborn Muse"), 0);
        var forest = Put(game, Cards.Basic("Forest"), 0);
        game.Tap(forest);

        game.PlayTurn(new PlayerId(1));

        Assert.False(game.State.Object(forest).Tapped);
    }

    [Fact]
    public void ATriggerUntilEndOfTurnFiresMoreThanOnceThenExpires()
    {
        var game = Started("Plains", new PassiveAgent(), new PassiveAgent());
        var first = Put(game, Cards.Vanilla("Bear", "{1}{G}", 2, 2), 1);
        var second = Put(game, Cards.Vanilla("Bear", "{1}{G}", 2, 2), 1);
        var creatures = new ObjectSelector(Quantifier.Any, new ObjectFilter(Kinds: [new Term("Creature", "CardType")]));

        Interpreter.Execute(new Lasting(new Delayed(new ObjectEvent(creatures, ["Dies"]), new GainLife(new PlayerSelector(PlayerKind.You), new Fixed(1))), new ThisTurn()), Context(game));
        Assert.Single(game.State.DelayedTriggers, x => x.Repeats);
        game.MoveToZone(first, Zone.Graveyard, new PlayerId(1));
        game.CheckStateAndTriggers();
        game.MoveToZone(second, Zone.Graveyard, new PlayerId(1));
        game.CheckStateAndTriggers();

        // Both deaths triggered; the triggers resolve as the next turn gives priority, and the trigger itself expires with the turn.
        Assert.Single(game.State.DelayedTriggers);
        Assert.False(game.State.StackIsEmpty);
        game.PlayTurn(new PlayerId(1));
        Assert.Equal(22, game.P0().Life);
        Assert.Empty(game.State.DelayedTriggers);
    }

    [Fact]
    public void ACreatureAssigningDamageAsThoughUnblockedHitsTheDefender()
    {
        var game = Started("Forest", new GreedyAgent(), new GreedyAgent(attacks: false, blocks: true));
        var wolf = new CardDefinition("Lone Wolf", ManaCost.Free, [], ["Creature"], ["Wolf"], 3, 3, null, [new StaticAbility(new AssignsDamageAsThoughUnblocked(new Self()))], [], [Color.Green]);
        Put(game, wolf, 0);
        var wall = Put(game, Cards.Vanilla("Wall", "{1}{W}", 0, 4), 1);

        game.PlayTurn(new PlayerId(0));

        Assert.Contains(game.Log.OfType<BlockersDeclared>(), x => x.Blocks.Count > 0);
        Assert.Equal(17, game.P1().Life);
        Assert.Equal(0, game.State.Object(wall).Damage);
    }

    /// <summary>A black aura granting protection from black: it falls off unless it says the effect doesn't remove it.</summary>
    static CardDefinition DarkWard(bool exempt)
    {
        var enchanted = new ObjectSelector(Quantifier.Attached, new ObjectFilter(Kinds: [new Term("Creature", "CardType")]));
        ImmutableList<Ability> abilities = [new StaticAbility(new Pump(enchanted, Keywords: [new KeywordInstance("Protection", Parameter: "black")]))];
        if (exempt)
            abilities = abilities.Add(new StaticAbility(new ProtectionDoesntRemoveThis()));
        return new CardDefinition("Dark Ward", ManaCost.Free, [], ["Enchantment"], ["Aura"], null, null, null, abilities, [], [Color.Black]);
    }

    [Theory]
    [InlineData(true, Zone.Battlefield)]
    [InlineData(false, Zone.Graveyard)]
    public void AWardStaysOnlyWhenItSaysTheEffectDoesntRemoveIt(bool exempt, Zone expected)
    {
        var game = Started("Plains", new PassiveAgent());
        var knight = Put(game, Cards.Vanilla("Knight", "{B}{B}", 2, 2), 0);
        var ward = Put(game, DarkWard(exempt), 0);
        game.Emit(new Attached(ward, knight));

        game.CheckStateAndTriggers();

        Assert.Equal(expected, game.State.Object(ward).Zone);
    }
}
