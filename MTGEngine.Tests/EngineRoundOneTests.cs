namespace MTGEngine.Tests;

/// <summary>The first engine round: entering tapped, modes, "unless pays", combat restrictions, keyword actions, loyalty, conditional statics, granted abilities, mana creatures.</summary>
public sealed class EngineRoundOneTests
{
    static Deck Hand(string player, params CardDefinition[] cards) =>
        new(player, [.. cards, .. Enumerable.Repeat(Cards.Basic("Plains"), 20)]);

    static Deck Lands(string player, string land = "Plains") => Cards.DeckOf(player, (Cards.Basic(land), 20));

    static ObjectId Put(Game game, CardDefinition card, int player)
    {
        var id = new ObjectId(game.State.NextObjectId);
        var owner = new PlayerId(player);
        game.Emit(new ObjectCreated(new GameObject(id, card, owner, owner, Zone.Hand)));
        game.MoveToZone(id, Zone.Battlefield, owner);
        game.Emit(new SummoningSicknessCleared(owner));
        return id;
    }

    [Fact]
    public void ALandThatEntersTappedEntersTapped()
    {
        var game = GameExtensions.Started(Hand("A", Cards.Named("Coastal Tower")), Lands("B"), new GreedyAgent(attacks: false));

        game.PlayTurn(new PlayerId(0));

        var tower = game.Find("Coastal Tower");
        Assert.Equal(Zone.Battlefield, tower.Zone);
        Assert.True(tower.Tapped);
    }

    [Fact]
    public void ModalSpellRunsTheChosenMode()
    {
        var game = GameExtensions.Started(Hand("A", Cards.Basic("Mountain"), Cards.Named("Boros Charm")), Lands("B"), new GreedyAgent(attacks: false));
        Put(game, Cards.Basic("Plains"), 0);

        game.PlayTurn(new PlayerId(0));

        Assert.Contains(game.Log, x => x is ModesChosen);
        Assert.Equal(16, game.P1().Life);
        Assert.Empty(game.Skips);
    }

    [Fact]
    public void ManaLeakCountersWhenTheCasterCantPay()
    {
        var game = GameExtensions.Started(
            Hand("A", Cards.Basic("Mountain"), Cards.Named("Lightning Bolt")),
            Hand("B", Cards.Named("Mana Leak")),
            new GreedyAgent(attacks: false),
            new GreedyAgent(attacks: false));
        Put(game, Cards.Basic("Island"), 1);
        Put(game, Cards.Basic("Island"), 1);

        game.PlayTurn(new PlayerId(0));

        Assert.Contains(game.Log, x => x is Countered);
        Assert.Equal(20, game.P1().Life);
    }

    [Fact]
    public void PacifiedCreatureCantAttack()
    {
        var game = GameExtensions.Started(Hand("A", Cards.Basic("Plains"), Cards.Named("Pacifism")), Lands("B"), new GreedyAgent(attacks: false), new GreedyAgent(attacks: true));
        Put(game, Cards.Basic("Plains"), 0);
        var bears = Put(game, Cards.Vanilla("Grizzly Bears", "{1}{G}", 2, 2, "Bear"), 1);

        game.PlayTurn(new PlayerId(0));
        Assert.Equal(bears, game.Find("Pacifism").AttachedTo);
        Assert.True(Characteristics.Of(game.State, game.State.Object(bears)).Cant("Attack"));

        game.PlayTurn(new PlayerId(1));
        Assert.Equal(20, game.P0().Life);
        Assert.DoesNotContain(game.Log, x => x is AttackersDeclared);
    }

    [Fact]
    public void PreordainScriesThenDraws()
    {
        var game = GameExtensions.Started(Hand("A", Cards.Basic("Island"), Cards.Named("Preordain")), Lands("B"), new GreedyAgent(attacks: false));
        var before = game.P0().Hand.Count;

        game.PlayTurn(new PlayerId(0));

        Assert.Contains(game.Log, x => x is Scryed);
        Assert.Equal(before - 1, game.P0().Hand.Count); // land -1, spell -1, draw +1
        Assert.Empty(game.Skips);
    }

    [Fact]
    public void InvestigateMakesAClueThatDrawsACard()
    {
        var game = GameExtensions.Started(Hand("A", Cards.Basic("Plains"), Cards.Named("Thraben Inspector")), Lands("B"), new GreedyAgent(attacks: false));
        Put(game, Cards.Basic("Plains"), 0);
        Put(game, Cards.Basic("Plains"), 0);

        game.PlayTurn(new PlayerId(0));

        var clue = game.State.Objects.Values.FirstOrDefault(x => x.Card.Subtypes.Contains("Clue"));
        // The greedy agent cracks the clue with its spare mana in the same turn, so it may already be gone.
        Assert.True(clue is not null || game.Log.Any(x => x is Sacrificed));
        Assert.Contains(game.Log, x => x is ObjectCreated { Object.Card.IsToken: true });
    }

    [Fact]
    public void PlaneswalkerEntersWithLoyaltyAndUsesAnAbility()
    {
        var game = GameExtensions.Started(Hand("A", Cards.Basic("Plains"), Cards.Named("Ajani Goldmane")), Lands("B"), new GreedyAgent(attacks: false));
        Put(game, Cards.Basic("Plains"), 0);
        Put(game, Cards.Basic("Plains"), 0);
        Put(game, Cards.Basic("Plains"), 0);

        game.PlayTurn(new PlayerId(0));

        var ajani = game.Find("Ajani Goldmane");
        Assert.Equal(Zone.Battlefield, ajani.Zone);
        Assert.Equal(5, ajani.CountersOf("loyalty"));
        Assert.Equal(22, game.P0().Life);
    }

    [Fact]
    public void ConditionalStaticAppliesOnlyWhileItsConditionHolds()
    {
        var berserker = Cards.Vanilla("Berserker", "{2}{R}", 2, 2, "Human") with
        {
            Abilities =
            [
                new StaticAbility(
                    new Pump(new Self(), new Fixed(2), new Fixed(2)),
                    new Controls(new PlayerSelector(PlayerKind.You), new ObjectSelector(Quantifier.Any, new ObjectFilter(Kinds: [new Term("Artifact", "CardType")])))),
            ],
        };
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        var creature = Put(game, berserker, 0);

        Assert.Equal(2, Characteristics.Of(game.State, game.State.Object(creature)).Power);
        Put(game, Cards.Named("Sol Ring"), 0);
        Assert.Equal(4, Characteristics.Of(game.State, game.State.Object(creature)).Power);
    }

    [Fact]
    public void GrantedAbilitiesCanBeActivated()
    {
        var granter = Cards.Vanilla("Granter", "{1}", 1, 1, "Construct") with
        {
            Abilities =
            [
                new StaticAbility(new GrantAbility(
                    new ObjectSelector(Quantifier.All, new ObjectFilter(Kinds: [new Term("Creature", "CardType")], Controller: new Controlled(new PlayerSelector(PlayerKind.You)))),
                    new ActivatedAbility([new TapSelf()], new DealDamage(new Self(), [new DamagePortion(new Fixed(1), new PlayerSelector(PlayerKind.TargetOpponent))])))),
            ],
        };
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new GreedyAgent(attacks: false), new PassiveAgent());
        Put(game, granter, 0);
        Put(game, Cards.Vanilla("Grizzly Bears", "{1}{G}", 2, 2, "Bear"), 0);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(18, game.P1().Life);
    }

    [Fact]
    public void ManaCreaturesPayForSpells()
    {
        var game = GameExtensions.Started(Hand("A", Cards.Basic("Forest"), Cards.Named("Giant Growth")), Lands("B"), new GreedyAgent(attacks: false));
        var elf = Put(game, Cards.Named("Llanowar Elves"), 0);
        game.MoveToZone(game.P0().Hand[0], Zone.Battlefield, new PlayerId(0));

        Assert.True(ManaPayment.CanPay(game.State, new PlayerId(0), ManaCost.Parse("{G}{G}")));
        game.PlayTurn(new PlayerId(0));

        Assert.True(game.State.Object(elf).Tapped || game.Find("Giant Growth").Zone == Zone.Graveyard);
    }
}
