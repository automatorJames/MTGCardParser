namespace MTGEngine.Tests;

/// <summary>The eighth engine round: copying spells, mana that can be spent only on some things, abilities that can't be activated, blocking only fliers, emblems, attack triggers, cheaper activations, divided damage.</summary>
public sealed class EngineRoundEightTests
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
    public void TwincastCopiesABolt()
    {
        var game = GameExtensions.Started(Hand("A", Cards.Named("Lightning Bolt"), Cards.Named("Twincast")), Lands("B"), new GreedyAgent(attacks: false), new PassiveAgent());
        Put(game, Cards.Basic("Mountain"), 0);
        Put(game, Cards.Basic("Island"), 0);
        Put(game, Cards.Basic("Island"), 0);

        game.PlayTurn(new PlayerId(0));

        Assert.Contains(game.Log, x => x is SpellCopied);
        Assert.Equal(14, game.P1().Life);
        Assert.DoesNotContain(game.State.Objects.Values, x => x.IsToken && x.Card.Name == "Lightning Bolt");
    }

    [Fact]
    public void RestrictedManaPaysOnlyForWhatItAllows()
    {
        var creatureSpells = new ObjectSelector(Quantifier.Any, new ObjectFilter(Kinds: [new Term("Spell", nameof(ObjectKind))], Qualifiers: [new Qualifier("Creature", Vocabulary: "CardType")]));
        var land = Cards.Basic("Rocky Outcrop") with
        {
            Subtypes = [],
            Abilities =
            [
                new ActivatedAbility([new TapSelf()], new AddMana([new ManaAmount(["{r}"])])),
                new StaticAbility(new ManaRestriction("OnlyFor", [new Playable("Cast", creatureSpells)])),
            ],
        };
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        Put(game, land, 0);
        var bolt = Put(game, Cards.Named("Lightning Bolt"), 0, Zone.Hand);
        var goblin = Put(game, Cards.Vanilla("Raging Goblin", "{R}", 1, 1, "Goblin"), 0, Zone.Hand);

        Assert.False(ManaPayment.CanPay(game.State, new PlayerId(0), ManaCost.Parse("{R}"), purpose: new ManaPayment.Purpose(Spell: game.State.Object(bolt))));
        Assert.True(ManaPayment.CanPay(game.State, new PlayerId(0), ManaCost.Parse("{R}"), purpose: new ManaPayment.Purpose(Spell: game.State.Object(goblin))));
    }

    [Fact]
    public void ArrestStopsActivatedAbilities()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        var sorcerer = Put(game, Cards.Named("Prodigal Sorcerer"), 1);
        Assert.Contains(game.LegalActions(new PlayerId(1)), x => x is ActivateAbility);
        var arrest = Put(game, Cards.Named("Arrest"), 0, Zone.Hand);
        game.MoveToZone(arrest, Zone.Battlefield, new PlayerId(0), attachTo: sorcerer);

        Assert.DoesNotContain(game.LegalActions(new PlayerId(1)), x => x is ActivateAbility);
        Assert.True(Characteristics.Of(game.State, game.State.Object(sorcerer)).Cant("Attack"));
    }

    [Fact]
    public void AscendingAvenBlocksOnlyFliers()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new GreedyAgent(attacks: true), new GreedyAgent(blocks: true));
        Put(game, Bears, 0);
        Put(game, Cards.Named("Ascending Aven"), 1);

        game.PlayTurn(new PlayerId(0));

        Assert.DoesNotContain(game.Log, x => x is BlockersDeclared);
        Assert.Equal(18, game.P1().Life);
    }

    [Fact]
    public void AnEmblemsStaticAppliesFromTheCommandZone()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        var bears = Put(game, Bears, 0);
        var yours = new ObjectSelector(Quantifier.All, new ObjectFilter(Kinds: [new Term("Creature", "CardType")], Controller: new Controlled(You)));

        Interpreter.Execute(new CreateEmblem(You, new StaticAbility(new Pump(yours, new Fixed(1), new Fixed(1)))), Context(game));

        Assert.Equal(3, Characteristics.Of(game.State, game.State.Object(bears)).Power);
    }

    [Fact]
    public void MilitaryIntelligenceDrawsForTwoAttackers()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new GreedyAgent(attacks: true), new PassiveAgent());
        Put(game, Cards.Named("Military Intelligence"), 0);
        Put(game, Bears, 0);
        Put(game, Bears, 0);
        var hand = game.P0().Hand.Count;

        game.PlayTurn(new PlayerId(0));

        Assert.Contains(game.Log, x => x is Drew { Player.Value: 0 });
    }

    [Fact]
    public void AnAbilityCostsLessForEachCounter()
    {
        var cheaper = new CostModification(new SelfAbility(), new PayMana(["{1}"]), "Less", "Activate",
            new CountOf(new ObjectSelector(Quantifier.All, new ObjectFilter(Kinds: [new Term("Counter", nameof(ObjectKind))], Qualifiers: [new Qualifier("oil", Vocabulary: "CounterKind")], With: [new Related(Relation.On, new Self())]))));
        var card = Cards.Vanilla("Oily", "{1}", 1, 1) with
        {
            Abilities = [new ActivatedAbility([new PayMana(["{3}"])], new GainLife(You, new Fixed(1))), new StaticAbility(cheaper)],
        };
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        var oily = Put(game, card, 0);
        Put(game, Cards.Basic("Plains"), 0);
        Assert.DoesNotContain(game.LegalActions(new PlayerId(0)), x => x is ActivateAbility);

        game.Emit(new CountersChanged(oily, "oil", 2));

        Assert.Contains(game.LegalActions(new PlayerId(0)), x => x is ActivateAbility);
    }

    [Fact]
    public void DividedDamageIsSplitAmongTheTargets()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        var first = Put(game, Bears, 1);
        var second = Put(game, Bears, 1);
        var targets = new ObjectSelector(Quantifier.Target, new ObjectFilter(Kinds: [new Term("Creature", "CardType")]), new Fixed(2));
        var ctx = Context(game);
        ctx.TargetsBySelector[targets] = [TargetRef.Of(first), TargetRef.Of(second)];

        Interpreter.Execute(new DealDamage(null, [new DamagePortion(new Fixed(4), targets)], Divided: "AsYouChoose"), ctx);

        Assert.Equal(2, game.State.Object(first).Damage);
        Assert.Equal(2, game.State.Object(second).Damage);
    }
}
