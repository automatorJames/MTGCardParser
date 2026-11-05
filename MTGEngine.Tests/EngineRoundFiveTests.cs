namespace MTGEngine.Tests;

/// <summary>The fifth engine round: replacement effects, turn history, exile until this leaves, effects for as long as, block encounters, otherwise.</summary>
public sealed class EngineRoundFiveTests
{
    static Deck Lands(string player, string land = "Plains") => Cards.DeckOf(player, (Cards.Basic(land), 20));

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

    static EffectContext Context(Game game, ObjectId? source = null) =>
        new(game, new PlayerId(0), source, new Dictionary<IrNode, ImmutableList<TargetRef>>(ReferenceEqualityComparer.Instance));

    static readonly CardDefinition Bears = Cards.Vanilla("Grizzly Bears", "{1}{G}", 2, 2, "Bear");

    [Fact]
    public void AngerOfTheGodsExilesWhatWouldDie()
    {
        var game = GameExtensions.Started(new Deck("A", [Cards.Named("Anger of the Gods"), .. Enumerable.Repeat(Cards.Basic("Mountain"), 20)]), Lands("B"), new GreedyAgent(attacks: false), new PassiveAgent());
        Put(game, Cards.Basic("Mountain"), 0);
        Put(game, Cards.Basic("Mountain"), 0);
        var bears = Put(game, Bears, 1);
        var giant = Put(game, Cards.Vanilla("Big Giant", "{5}", 5, 5, "Giant"), 1);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Exile, game.State.Object(bears).Zone);
        Assert.Equal(Zone.Battlefield, game.State.Object(giant).Zone);
    }

    [Fact]
    public void PossessedSkaabIsExiledInsteadOfDying()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        var skaab = Put(game, Cards.Named("Possessed Skaab"), 0);

        game.Destroy(skaab);

        Assert.Equal(Zone.Exile, game.State.Object(skaab).Zone);
    }

    [Fact]
    public void PhantomCentaurPreventsDamageAndLosesACounter()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        var centaur = Put(game, Cards.Named("Phantom Centaur"), 0);
        Assert.Equal(3, game.State.Object(centaur).CountersOf("+1/+1"));

        game.DealDamage(null, TargetRef.Of(centaur), 4, combat: false);

        Assert.Equal(0, game.State.Object(centaur).Damage);
        Assert.Equal(2, game.State.Object(centaur).CountersOf("+1/+1"));
    }

    [Fact]
    public void BanisherPriestReturnsWhatItExiledWhenItLeaves()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new GreedyAgent(attacks: false), new PassiveAgent());
        var bears = Put(game, Bears, 1);
        var priest = Put(game, Cards.Named("Banisher Priest"), 0);
        game.CheckStateAndTriggers();
        game.PlayTurn(new PlayerId(0));
        Assert.Equal(Zone.Exile, game.State.Object(bears).Zone);

        game.Destroy(priest);

        var returned = game.State.Objects.Values.Single(x => x.Card.Name == "Grizzly Bears");
        Assert.Equal(Zone.Battlefield, returned.Zone);
        Assert.Equal(new PlayerId(1), returned.Controller);
    }

    [Fact]
    public void AegisAngelProtectsOnlyWhileYouControlIt()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new GreedyAgent(attacks: false), new PassiveAgent());
        var bears = Put(game, Bears, 0);
        var angel = Put(game, Cards.Named("Aegis Angel"), 0);
        game.PlayTurn(new PlayerId(0));
        Assert.True(Characteristics.Of(game.State, game.State.Object(bears)).HasKeyword("Indestructible"));

        game.Emit(new ControlChanged(angel, new PlayerId(1)));

        Assert.False(Characteristics.Of(game.State, game.State.Object(bears)).HasKeyword("Indestructible"));
    }

    [Fact]
    public void BonePickerIsCheapAfterACreatureDied()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        game.Emit(new TurnBegan(1, new PlayerId(0)));
        var picker = Put(game, Cards.Named("Bone Picker"), 0, Zone.Hand);
        var cost = () => game.CostToCast(game.State.Object(picker), new PlayerId(0), Cards.Named("Bone Picker").ManaCost, null).ManaValue;
        Assert.Equal(4, cost());

        game.Destroy(Put(game, Bears, 1));

        Assert.Equal(1, cost());
    }

    [Fact]
    public void AssembledAlphasHitsWhatBlocksIt()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new GreedyAgent(attacks: true), new GreedyAgent(blocks: true));
        Put(game, Cards.Named("Assembled Alphas"), 0);
        var blocker = Put(game, Bears, 1);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Graveyard, game.State.Object(blocker).Zone);
        Assert.Equal(17, game.P1().Life);
    }

    [Fact]
    public void OtherwiseRunsWhenTheConditionDoesnt()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        var you = new PlayerSelector(PlayerKind.You);
        var effect = new Sequence([
            new Conditional(new Controls(you, new ObjectSelector(Quantifier.Any, new ObjectFilter(Kinds: [new Term("Artifact", "CardType")]))), new GainLife(you, new Fixed(5))),
            new Otherwise(new GainLife(you, new Fixed(1))),
        ]);

        Interpreter.Execute(effect, Context(game));

        Assert.Equal(21, game.P0().Life);
    }
}
