namespace MTGEngine.Tests;

/// <summary>The seventh engine round: how a spell was cast and the mana spent, cards exiled with a permanent, spell ordinals, state triggers, can't-play effects, attacking as though, energy, alternative costs, convoke.</summary>
public sealed class EngineRoundSevenTests
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

    static EffectContext Context(Game game, ObjectId? source = null) =>
        new(game, new PlayerId(0), source, new Dictionary<IrNode, ImmutableList<TargetRef>>(ReferenceEqualityComparer.Instance));

    static readonly PlayerSelector You = new(PlayerKind.You);
    static readonly CardDefinition Bears = Cards.Vanilla("Grizzly Bears", "{1}{G}", 2, 2, "Bear");

    [Fact]
    public void AzoriusHeraldStaysOnlyIfBlueWasSpent()
    {
        var withBlue = GameExtensions.Started(Hand("A", Cards.Named("Azorius Herald")), Lands("B"), new GreedyAgent(attacks: false), new PassiveAgent());
        foreach (var land in new[] { "Island", "Plains", "Plains" })
            Put(withBlue, Cards.Basic(land), 0);
        withBlue.PlayTurn(new PlayerId(0));
        Assert.Equal(Zone.Battlefield, withBlue.Find("Azorius Herald").Zone);

        var without = GameExtensions.Started(Hand("A", Cards.Named("Azorius Herald")), Lands("B"), new GreedyAgent(attacks: false), new PassiveAgent());
        foreach (var land in new[] { "Plains", "Plains", "Plains" })
            Put(without, Cards.Basic(land), 0);
        without.PlayTurn(new PlayerId(0));
        Assert.Equal(Zone.Graveyard, without.Find("Azorius Herald").Zone);
    }

    [Fact]
    public void AnEnterTriggerKnowsWhetherItWasCast()
    {
        var card = Cards.Vanilla("Proud", "{1}", 1, 1) with
        {
            Abilities = [new TriggeredAbility(new ObjectEvent(new Self(), ["EntersTheBattlefield"]), new GainLife(You, new Fixed(3)), new WasCast(new Self(), null, new ZoneRef("Hand")))],
        };
        var game = GameExtensions.Started(Hand("A", card), Lands("B"), new GreedyAgent(attacks: false), new PassiveAgent());
        Put(game, Cards.Basic("Plains"), 0);

        game.PlayTurn(new PlayerId(0));
        Assert.Equal(23, game.P0().Life);

        Put(game, card, 0);
        game.CheckStateAndTriggers();
        Assert.Equal(23, game.P0().Life);
    }

    [Fact]
    public void CardsExiledWithAPermanentAreKnown()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        var keeper = Put(game, Cards.Vanilla("Keeper", "{1}", 1, 1), 0);
        var bears = Put(game, Bears, 1);
        var ctx = Context(game, keeper) with { Last = [TargetRef.Of(bears)] };
        Interpreter.Execute(new Exile(new Reference(null, "it")), ctx);

        var exiled = new ObjectSelector(Quantifier.All, new ObjectFilter(Kinds: [new Term("Card", nameof(ObjectKind))], With: [new Related(Relation.ExiledWith, new Self())], Zone: new ZoneRef("Exile")));

        Assert.Equal([bears], Resolver.Objects(exiled, Context(game, keeper)));
    }

    [Fact]
    public void TheSecondSpellEachTurnIsCounted()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        game.Emit(new TurnBegan(1, new PlayerId(0)));
        var second = new ObjectFilter(Kinds: [new Term("Spell", nameof(ObjectKind))], Qualifiers: [new Qualifier("Second", Vocabulary: "SpellOrdinal")]);
        var first = Put(game, Cards.Named("Lightning Bolt"), 0, Zone.Hand);
        game.MoveToZone(first, Zone.Stack, new PlayerId(0));
        game.Emit(new SpellCast(new StackItem(game.State.NextStackId, new PlayerId(0), first, StackKind.Spell, [], [], Spell: first)));
        var next = Put(game, Cards.Named("Shock"), 0, Zone.Hand);
        game.MoveToZone(next, Zone.Stack, new PlayerId(0));
        game.Emit(new SpellCast(new StackItem(game.State.NextStackId, new PlayerId(0), next, StackKind.Spell, [], [], Spell: next)));

        Assert.False(Filters.Matches(game.State, second, game.State.Object(first), new PlayerId(0), null));
        Assert.True(Filters.Matches(game.State, second, game.State.Object(next), new PlayerId(0), null));
    }

    [Fact]
    public void AStateTriggerFiresWhenItsConditionHolds()
    {
        var card = Cards.Vanilla("Fragile", "{1}", 1, 1) with
        {
            Abilities = [new TriggeredAbility(new ConditionEvent(new HasCountersCondition(new Self(), new CounterSpec("+1/+1", null), Negated: true)), new Sacrifice(You, new Self()))],
        };
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        var fragile = Put(game, card, 0);
        game.Emit(new CountersChanged(fragile, "+1/+1", 1));
        game.CheckStateAndTriggers();
        Assert.Empty(game.State.Stack);

        game.Emit(new CountersChanged(fragile, "+1/+1", -1));
        game.CheckStateAndTriggers();
        Assert.Single(game.State.Stack);
        game.CheckStateAndTriggers();
        Assert.Single(game.State.Stack);
    }

    [Fact]
    public void PardicMinerStopsLandDrops()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        game.Emit(new TurnBegan(1, new PlayerId(1)));
        game.Emit(new StepBegan(Step.Main1, new PlayerId(1)));
        Assert.Contains(game.LegalActions(new PlayerId(1)), x => x is PlayLand);

        var miner = Cards.Named("Pardic Miner").Abilities.OfType<ActivatedAbility>().Single().Effect;
        var ctx = Context(game);
        ctx.TargetsBySelector[TargetSlots.Of([miner]).Single().Selector] = [TargetRef.Of(new PlayerId(1))];
        Interpreter.Execute(miner, ctx);

        Assert.DoesNotContain(game.LegalActions(new PlayerId(1)), x => x is PlayLand);
    }

    [Fact]
    public void AWallCanAttackAsThoughItDidntHaveDefender()
    {
        var wall = Cards.Vanilla("Walking Wall", "{2}", 3, 3, "Wall") with
        {
            Keywords = ["Defender"],
            Abilities = [new StaticAbility(new CanAsThough(new Self(), "Attack", "Defender", false))],
        };
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new GreedyAgent(attacks: true), new PassiveAgent());
        Put(game, wall, 0);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(17, game.P1().Life);
    }

    [Fact]
    public void EnergyIsGainedAndPaid()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        Interpreter.Execute(new GetEnergy(You, new Fixed(3)), Context(game));
        Assert.Equal(3, game.P0().Energy);

        Interpreter.Execute(new PayCost(new PayMana(["{e}", "{e}"])), Context(game));

        Assert.True(game.LastOptionalTaken);
        Assert.Equal(1, game.P0().Energy);
    }

    [Fact]
    public void AbolishCanBeCastByDiscardingAPlains()
    {
        var game = GameExtensions.Started(Hand("A", Cards.Named("Abolish")), Lands("B"), new GreedyAgent(attacks: false), new PassiveAgent());
        var ring = Put(game, Cards.Named("Sol Ring"), 1);
        game.Emit(new TurnBegan(1, new PlayerId(0)));
        game.Emit(new StepBegan(Step.Main1, new PlayerId(0)));

        var cast = Assert.Single(game.LegalActions(new PlayerId(0)), x => x is CastSpell);
        Assert.True(((CastSpell)cast).Alternative >= 0);

        game.PlayTurn(new PlayerId(0));
        Assert.Equal(Zone.Graveyard, game.State.Object(ring).Zone);
        Assert.Contains(game.Log, x => x is Discarded);
    }

    [Fact]
    public void ConvokeTapsCreaturesToPay()
    {
        var spell = Cards.Vanilla("Convoked Giant", "{2}{G}", 4, 4, "Giant") with { Keywords = ["Convoke"] };
        var game = GameExtensions.Started(Hand("A", spell), Lands("B"), new GreedyAgent(attacks: false), new PassiveAgent());
        Put(game, Bears, 0);
        Put(game, Bears, 0);
        Put(game, Cards.Vanilla("Elf", "{G}", 1, 1, "Elf"), 0);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Battlefield, game.Find("Convoked Giant").Zone);
        Assert.Equal(3, game.BattlefieldOf(0).Count(x => x.Tapped));
    }
}
