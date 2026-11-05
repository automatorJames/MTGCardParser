namespace MTGEngine.Tests;

/// <summary>The third engine round: kicker and additional costs, cost changes, choices made as a permanent enters, type changes, damage prevention, sagas.</summary>
public sealed class EngineRoundThreeTests
{
    static Deck Hand(string player, params CardDefinition[] cards) =>
        new(player, [.. cards, .. Enumerable.Repeat(Cards.Basic("Plains"), 20)]);

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

    static readonly CardDefinition Bears = Cards.Vanilla("Grizzly Bears", "{1}{G}", 2, 2, "Bear");

    [Fact]
    public void KickedBurstLightningDealsFour()
    {
        var game = GameExtensions.Started(Hand("A", Cards.Named("Burst Lightning")), Lands("B"), new GreedyAgent(attacks: false));
        for (var i = 0; i < 5; i++)
            Put(game, Cards.Basic("Mountain"), 0);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(16, game.P1().Life);
        Assert.Contains(game.Log, x => x is CastOptionsChosen { Options: var options } && options.Contains("Kicked"));
    }

    [Fact]
    public void UnkickedBurstLightningDealsTwo()
    {
        var game = GameExtensions.Started(Hand("A", Cards.Basic("Mountain"), Cards.Named("Burst Lightning")), Lands("B"), new GreedyAgent(attacks: false));

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(18, game.P1().Life);
    }

    [Fact]
    public void AKickedCreatureEntersWithCounters()
    {
        var game = GameExtensions.Started(Hand("A", Cards.Basic("Island"), Cards.Named("Faerie Squadron")), Lands("B"), new GreedyAgent(attacks: false));
        for (var i = 0; i < 4; i++)
            Put(game, Cards.Basic("Island"), 0);

        game.PlayTurn(new PlayerId(0));

        var faerie = game.Find("Faerie Squadron", Zone.Battlefield);
        Assert.NotNull(faerie);
        Assert.Equal(2, faerie.CountersOf("+1/+1"));
    }

    [Fact]
    public void VillageRitesSacrificesAsItsCast()
    {
        var game = GameExtensions.Started(Hand("A", Cards.Basic("Swamp"), Cards.Named("Village Rites")), Lands("B"), new GreedyAgent(attacks: false));
        var bears = Put(game, Bears, 0);
        var before = game.P0().Hand.Count;

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Graveyard, game.State.Object(bears).Zone);
        Assert.Contains(game.Log, x => x is Sacrificed { Object: var id } && id == bears);
        Assert.Equal(before, game.P0().Hand.Count); // land -1, spell -1, two draws +2
    }

    [Fact]
    public void AnAdditionalCostThatCantBePaidStopsTheCast()
    {
        var game = GameExtensions.Started(Hand("A", Cards.Basic("Swamp"), Cards.Named("Village Rites")), Lands("B"), new GreedyAgent(attacks: false));

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Hand, game.Find("Village Rites").Zone);
    }

    [Fact]
    public void ThaliaTaxesNoncreatureSpells()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        Put(game, Cards.Named("Thalia, Guardian of Thraben"), 1);
        var bolt = Put(game, Cards.Named("Lightning Bolt"), 0, Zone.Hand);
        var bears = Put(game, Bears, 0, Zone.Hand);

        Assert.Equal(2, game.CostToCast(game.State.Object(bolt), new PlayerId(0), Cards.Named("Lightning Bolt").ManaCost, null).ManaValue);
        Assert.Equal(2, game.CostToCast(game.State.Object(bears), new PlayerId(0), Bears.ManaCost, null).ManaValue);
    }

    [Fact]
    public void BedlamRevelerCostsLessForEachInstantAndSorceryInYourGraveyard()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        Put(game, Cards.Named("Lightning Bolt"), 0, Zone.Graveyard);
        Put(game, Cards.Named("Divination"), 0, Zone.Graveyard);
        var reveler = Put(game, Cards.Named("Bedlam Reveler"), 0, Zone.Hand);

        var cost = game.CostToCast(game.State.Object(reveler), new PlayerId(0), Cards.Named("Bedlam Reveler").ManaCost, null);

        Assert.Equal(4, cost.Generic);
    }

    [Fact]
    public void VanquishersBannerPumpsTheChosenType()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new GreedyAgent(), new PassiveAgent());
        var first = Put(game, Bears, 0);
        var second = Put(game, Bears, 0);
        var goblin = Put(game, Cards.Vanilla("Goblin Piker", "{1}{R}", 2, 1, "Goblin"), 0);

        var banner = Put(game, Cards.Named("Vanquisher's Banner"), 0);

        Assert.Equal("Bear", game.State.Object(banner).Chosen("CreatureType"));
        Assert.Equal(3, Characteristics.Of(game.State, game.State.Object(first)).Power);
        Assert.Equal(3, Characteristics.Of(game.State, game.State.Object(second)).Toughness);
        Assert.Equal(2, Characteristics.Of(game.State, game.State.Object(goblin)).Power);
    }

    [Fact]
    public void BloodMoonTurnsNonbasicLandsIntoMountains()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        var tower = Put(game, Cards.Named("Coastal Tower"), 0);
        game.Emit(new Untapped(tower));
        Put(game, Cards.Named("Blood Moon"), 1);

        var traits = Characteristics.Of(game.State, game.State.Object(tower));
        var source = Assert.Single(ManaPayment.Sources(game.State, new PlayerId(0)));

        Assert.Contains("Mountain", traits.Subtypes);
        Assert.Equal(new Color?[] { Color.Red }, source.Options.ToArray());
    }

    [Fact]
    public void FogPreventsCombatDamage()
    {
        var game = GameExtensions.Started(Hand("A", Cards.Named("Fog")), Lands("B"), new GreedyAgent(attacks: false), new GreedyAgent(attacks: true));
        Put(game, Cards.Basic("Forest"), 0);
        Put(game, Bears, 1);

        game.PlayTurn(new PlayerId(1));

        Assert.Contains(game.Log, x => x is AttackersDeclared);
        Assert.Equal(20, game.P0().Life);
        Assert.Contains(game.Log, x => x is DamagePrevented { Amount: 2 });
    }

    [Fact]
    public void APreventionShieldStopsOnlyItsAmount()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        var bears = Put(game, Cards.Vanilla("Big Bears", "{3}{G}", 4, 4, "Bear"), 0);
        var ctx = new EffectContext(game, new PlayerId(0), bears, new Dictionary<IrNode, ImmutableList<TargetRef>>(ReferenceEqualityComparer.Instance));

        Interpreter.Execute(new PreventDamage(new Fixed(3), To: new Self(), Duration: new ThisTurn()), ctx);
        game.DealDamage(null, TargetRef.Of(bears), 2, combat: false);
        game.DealDamage(null, TargetRef.Of(bears), 2, combat: false);

        Assert.Equal(1, game.State.Object(bears).Damage);
        Assert.Empty(game.State.ShieldList);
    }

    [Fact]
    public void HistoryOfBenaliaMakesKnightsThenIsSacrificed()
    {
        var game = GameExtensions.Started(Hand("A", Cards.Basic("Plains"), Cards.Named("History of Benalia")), Lands("B"), new GreedyAgent(attacks: false), new PassiveAgent());
        Put(game, Cards.Basic("Plains"), 0);
        Put(game, Cards.Basic("Plains"), 0);

        game.PlayTurn(new PlayerId(0));
        Assert.Equal(1, game.Find("History of Benalia", Zone.Battlefield)?.CountersOf("lore"));
        game.PlayTurn(new PlayerId(1));
        game.PlayTurn(new PlayerId(0));
        game.PlayTurn(new PlayerId(1));
        game.PlayTurn(new PlayerId(0));

        Assert.Equal(2, game.State.Objects.Values.Count(x => x.IsToken && x.Card.Subtypes.Contains("Knight") && x.Zone == Zone.Battlefield));
        Assert.Equal(Zone.Graveyard, game.Find("History of Benalia").Zone);
    }
}
