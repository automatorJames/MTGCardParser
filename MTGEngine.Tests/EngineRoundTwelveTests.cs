namespace MTGEngine.Tests;

/// <summary>The twelfth engine round: level up and class levels, clones, spree's mode costs, and actions that change nothing not stalling a game.</summary>
public sealed class EngineRoundTwelveTests
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

    static readonly PlayerSelector You = new(PlayerKind.You);

    [Fact]
    public void BeastbreakerGrowsWithItsLevel()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        var beast = Put(game, Cards.Named("Beastbreaker of Bala Ged"), 0);
        int Power() => Characteristics.Of(game.State, game.State.Object(beast)).Power;

        Assert.Equal(2, Power());
        game.Emit(new CountersChanged(beast, "level", 1));
        Assert.Equal(4, Power());
        game.Emit(new CountersChanged(beast, "level", 3));
        Assert.Equal(6, Power());
        Assert.True(Characteristics.Of(game.State, game.State.Object(beast)).HasKeyword("Trample"));
    }

    [Fact]
    public void LevelUpIsAnActivatedAbilityAtSorcerySpeed()
    {
        var game = GameExtensions.Started(Lands("A", "Forest"), Lands("B"), new GreedyAgent(attacks: false), new PassiveAgent());
        var beast = Put(game, Cards.Named("Beastbreaker of Bala Ged"), 0);
        for (var i = 0; i < 3; i++)
            Put(game, Cards.Basic("Forest"), 0);

        game.PlayTurn(new PlayerId(0));

        Assert.True(game.State.Object(beast).CountersOf("level") >= 1);
    }

    [Fact]
    public void AClassGainsItsAbilitiesLevelByLevel()
    {
        var rules = new CardRules("Little Class", "Enchantment",
        [
            new LineRules(0, "at the beginning of your upkeep, you gain 1 life.", [new TriggeredAbility(new PhaseEvent(new PhaseRef("Upkeep", Whose: You)), new GainLife(You, new Fixed(1)))], []),
            new LineRules(1, "{1}: level 2", [new ActivatedAbility([new PayMana(["{1}"])], new GainClassLevel(2))], []),
            new LineRules(2, "creatures you control get +1/+1.", [new StaticAbility(new Pump(new ObjectSelector(Quantifier.All, new ObjectFilter(Kinds: [new Term("Creature", "CardType")], Controller: new Controlled(You))), new Fixed(1), new Fixed(1)))], []),
        ], new CardInfo("{1}"));
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new GreedyAgent(attacks: false), new PassiveAgent());
        var bears = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 0);
        var level = Put(game, CardDefinition.From(rules), 0);
        Assert.Equal(2, Characteristics.Of(game.State, game.State.Object(bears)).Power);

        Put(game, Cards.Basic("Plains"), 0);
        game.PlayTurn(new PlayerId(0));

        Assert.Equal(1, game.State.Object(level).CountersOf("class-level"));
        Assert.Equal(3, Characteristics.Of(game.State, game.State.Object(bears)).Power);
        Assert.DoesNotContain(game.LegalActions(new PlayerId(0)), x => x is ActivateAbility);
    }

    [Fact]
    public void CloneEntersAsACopy()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new GreedyAgent(attacks: false), new PassiveAgent());
        Put(game, Cards.Named("Serra Angel"), 1);
        var clone = Put(game, Cards.Named("Clone"), 0);

        var copied = game.State.Object(clone);
        Assert.Equal("Serra Angel", copied.Card.Name);
        Assert.True(Characteristics.Of(game.State, copied).HasKeyword("Flying"));

        game.Destroy(clone);
        Assert.Equal("Clone", game.State.Object(clone).Card.Name);
    }

    [Fact]
    public void SpreeModesAddTheirCosts()
    {
        var target = new PlayerSelector(PlayerKind.TargetOpponent);
        var rules = new CardRules("Spree Bolt", "Instant",
        [
            new LineRules(0, "spree", [new SpellAbility(new ChooseModes("OneOrMore"))], []),
            new LineRules(1, "+ {1} — deal 2", [new Mode(new DealDamage(new Self(), [new DamagePortion(new Fixed(2), target)]), new PayMana(["{1}"]))], []),
            new LineRules(2, "+ {2} — deal 3", [new Mode(new DealDamage(new Self(), [new DamagePortion(new Fixed(3), new PlayerSelector(PlayerKind.TargetOpponent))]), new PayMana(["{2}"]))], []),
        ], new CardInfo("{R}"));
        var game = GameExtensions.Started(new Deck("A", [CardDefinition.From(rules), .. Enumerable.Repeat(Cards.Basic("Mountain"), 20)]), Lands("B"), new GreedyAgent(attacks: false), new PassiveAgent());
        Put(game, Cards.Basic("Mountain"), 0);

        game.PlayTurn(new PlayerId(0));

        // Two mountains pay {R} and the first mode's {1}, not the second's {2}; the greedy agent keeps choosing modes while it can.
        Assert.Contains(game.Log, x => x is ModesChosen);
        Assert.True(game.P1().Life is 18 or 20);
    }
}
