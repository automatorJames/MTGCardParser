namespace MTGEngine.Tests;

/// <summary>The sixth engine round: costs that are actions, what a cost took, sharing a characteristic, revealing until, statics during a phase, "isn't a creature".</summary>
public sealed class EngineRoundSixTests
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

    static readonly PlayerSelector You = new(PlayerKind.You);

    [Fact]
    public void FlingDealsTheSacrificedCreaturesPower()
    {
        var game = GameExtensions.Started(new Deck("A", [Cards.Named("Fling"), .. Enumerable.Repeat(Cards.Basic("Mountain"), 20)]), Lands("B"), new GreedyAgent(attacks: false), new PassiveAgent());
        Put(game, Cards.Basic("Mountain"), 0);
        Put(game, Cards.Basic("Mountain"), 0);
        var giant = Put(game, Cards.Vanilla("Hill Giant", "{3}{R}", 3, 3, "Giant"), 0);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Graveyard, game.State.Object(giant).Zone);
        Assert.Equal(17, game.P1().Life);
    }

    [Fact]
    public void AvatarOfDiscordStaysIfYouDiscardTwo()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new GreedyAgent(), new PassiveAgent());
        var avatar = Put(game, Cards.Named("Avatar of Discord"), 0);
        var hand = game.P0().Hand.Count;

        game.CheckStateAndTriggers();
        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Battlefield, game.State.Object(avatar).Zone);
        Assert.True(game.P0().Hand.Count <= hand - 2 + 1);
    }

    [Fact]
    public void AvatarOfDiscordIsSacrificedWithoutCardsToDiscard()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new GreedyAgent(), new PassiveAgent(), handSize: 0);
        var avatar = Put(game, Cards.Named("Avatar of Discord"), 0);

        game.CheckStateAndTriggers();
        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Graveyard, game.State.Object(avatar).Zone);
    }

    [Fact]
    public void AnActivatedAbilityCanCostReturningItself()
    {
        var card = Cards.Vanilla("Returner", "{1}", 1, 1) with
        {
            Abilities = [new ActivatedAbility([new ActionCost(new ChangeZone(new Self(), new ZoneRef("Hand")))], new Draw(You, new Fixed(1)))],
        };
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new GreedyAgent(attacks: false), new PassiveAgent());
        var returner = Put(game, card, 0);
        var hand = game.P0().Hand.Count;

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Hand, game.State.Object(returner).Zone);
        Assert.Contains(game.Log, x => x is AbilityActivated);
    }

    [Fact]
    public void BileBlightHitsEveryCreatureWithTheSameName()
    {
        var game = GameExtensions.Started(new Deck("A", [Cards.Named("Bile Blight"), .. Enumerable.Repeat(Cards.Basic("Swamp"), 20)]), Lands("B"), new GreedyAgent(attacks: false), new PassiveAgent());
        Put(game, Cards.Basic("Swamp"), 0);
        var bears = Cards.Vanilla("Grizzly Bears", "{1}{G}", 2, 2, "Bear");
        var first = Put(game, bears, 1);
        var second = Put(game, bears, 1);
        var other = Put(game, Cards.Vanilla("Hill Giant", "{3}{R}", 3, 4, "Giant"), 1);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Graveyard, game.State.Object(first).Zone);
        Assert.Equal(Zone.Graveyard, game.State.Object(second).Zone);
        Assert.Equal(Zone.Battlefield, game.State.Object(other).Zone);
    }

    [Fact]
    public void RevealUntilFindsTheFirstMatchingCard()
    {
        var game = GameExtensions.Started(new Deck("A", [.. Enumerable.Repeat(Cards.Basic("Plains"), 9), Cards.Vanilla("Bear", "{1}", 2, 2), .. Enumerable.Repeat(Cards.Basic("Plains"), 5)]), Lands("B"), new PassiveAgent(), new PassiveAgent());
        var creature = new ObjectSelector(Quantifier.Any, new ObjectFilter(Kinds: [new Term("Card", nameof(ObjectKind))], Qualifiers: [new Qualifier("Creature", Vocabulary: "CardType")]));
        var effect = new Sequence([
            new RevealUntil(You, "Reveal", new ZoneRef("Library"), new PlayerAction(You, "Reveal", creature)),
            new ChangeZone(new Reference(null, "that card"), new ZoneRef("Hand")),
        ]);

        Interpreter.Execute(effect, Context(game));

        Assert.Contains(game.P0().Hand, id => game.State.Object(id).Card.Name == "Bear");
        Assert.Equal(7, game.P0().Library.Count);
    }

    [Fact]
    public void AStaticDuringYourTurnAppliesOnlyThen()
    {
        var card = Cards.Vanilla("Daytime", "{1}", 1, 1) with
        {
            Abilities = [new StaticAbility(new Pump(new Self(), new Fixed(2), new Fixed(0), Duration: new DuringPhase(new PhaseRef("Turn", Whose: You))))],
        };
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        var creature = Put(game, card, 0);

        game.Emit(new TurnBegan(1, new PlayerId(0)));
        Assert.Equal(3, Characteristics.Of(game.State, game.State.Object(creature)).Power);
        game.Emit(new TurnBegan(2, new PlayerId(1)));
        Assert.Equal(1, Characteristics.Of(game.State, game.State.Object(creature)).Power);
    }

    [Fact]
    public void AGodIsntACreatureWhileItsConditionHolds()
    {
        var god = Cards.Vanilla("Little God", "{1}", 4, 4) with
        {
            Types = ["Enchantment", "Creature"],
            Abilities = [new StaticAbility(new BecomesAspect(new Self(), [new Term("Creature", "CardType")], Negated: true), new HandSize(You, new Fixed(0)))],
        };
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        var id = Put(game, god, 0);

        Assert.True(Characteristics.Of(game.State, game.State.Object(id)).IsCreature);
        foreach (var card in game.P0().Hand.ToList())
            game.Discard(new PlayerId(0), card);
        Assert.False(Characteristics.Of(game.State, game.State.Object(id)).IsCreature);
    }
}
