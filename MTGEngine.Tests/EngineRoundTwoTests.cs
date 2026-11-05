namespace MTGEngine.Tests;

/// <summary>The second engine round: paying costs as effects, turn history, ability words, token qualifiers, player-or-planeswalker targets, counting counters.</summary>
public sealed class EngineRoundTwoTests
{
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

    static EffectContext Context(Game game, ObjectId? source = null) =>
        new(game, new PlayerId(0), source, new Dictionary<IrNode, ImmutableList<TargetRef>>(ReferenceEqualityComparer.Instance));

    static readonly PlayerSelector You = new(PlayerKind.You);

    [Fact]
    public void PayingACostAsAnEffectDecidesIfYouDo()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new GreedyAgent(), new PassiveAgent());
        Put(game, Cards.Basic("Plains"), 0);

        var ctx = Interpreter.Execute(new Optional(new PayCost(new PayMana(["{1}"]))), Context(game));
        Interpreter.Execute(new Conditional(new OutcomeCondition(You, "Did"), new GainLife(You, new Fixed(3))), ctx);
        Assert.Equal(23, game.P0().Life);

        Interpreter.Execute(new PayCost(new PayMana(["{5}"])), Context(game));
        Assert.False(game.LastOptionalTaken);
    }

    [Fact]
    public void RaidKnowsWhetherYouAttacked()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new GreedyAgent(), new PassiveAgent());
        var raid = new Happened(new NamedEvent("YouAttacked"), "ThisTurn");
        Assert.False(Conditions.Evaluate(raid, Context(game)));

        var bears = Put(game, Cards.Vanilla("Grizzly Bears", "{1}{G}", 2, 2, "Bear"), 0);
        game.Emit(new AttackersDeclared(new PlayerId(0), [bears]));
        Assert.True(Conditions.Evaluate(raid, Context(game)));
    }

    [Fact]
    public void AbilityWordsAreUnwrapped()
    {
        var trigger = new TriggeredAbility(new ObjectEvent(new Self(), ["EntersTheBattlefield"]), new GainLife(You, new Fixed(2)));
        var rules = new CardRules("Labeled", "Creature", [new LineRules(0, "landfall — …", [new LabeledAbility("Landfall", trigger)], [])], new CardInfo("{1}", Power: "1", Toughness: "1"));

        var card = CardDefinition.From(rules);

        Assert.IsType<TriggeredAbility>(Assert.Single(card.Abilities));
    }

    [Fact]
    public void NontokenFiltersSkipTokens()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new GreedyAgent(), new PassiveAgent());
        var bears = Put(game, Cards.Vanilla("Grizzly Bears", "{1}{G}", 2, 2, "Bear"), 0);
        var token = game.CreateToken(new TokenSpec(Types: ["Creature"], Subtypes: ["Soldier"], Power: new Fixed(1), Toughness: new Fixed(1)), new PlayerId(0));
        var nontoken = new ObjectSelector(Quantifier.All, new ObjectFilter(Kinds: [new Term("Creature", "CardType")], Qualifiers: [new Qualifier("Token", true, "TokenWord")]));

        var matched = Resolver.Objects(nontoken, Context(game));

        Assert.Contains(bears, matched);
        Assert.DoesNotContain(token, matched);
    }

    [Fact]
    public void TargetOpponentOrPlaneswalkerOffersBoth()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new GreedyAgent(), new PassiveAgent());
        var walker = Put(game, Cards.Named("Ajani Goldmane"), 1);
        var target = new ObjectSelector(Quantifier.Target, new ObjectFilter(Kinds: [new Term("Opponent", "PlayerIdentity"), new Term("Planeswalker", "CardType")]));

        var candidates = TargetSlots.Candidates(new TargetSlot(target, null, false, 1), game, new PlayerId(0), null);

        Assert.Contains(TargetRef.Of(new PlayerId(1)), candidates);
        Assert.DoesNotContain(TargetRef.Of(new PlayerId(0)), candidates);
        Assert.Contains(TargetRef.Of(walker), candidates);
    }

    [Fact]
    public void CountsTheCountersOnAnObject()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new GreedyAgent(), new PassiveAgent());
        var ring = Put(game, Cards.Named("Sol Ring"), 0);
        game.Emit(new CountersChanged(ring, "charge", 3));
        var count = new CountOf(new ObjectSelector(Quantifier.All, new ObjectFilter(Kinds: [new Term("Counter", "ObjectKind")], Qualifiers: [new Qualifier("charge", false, "CounterKind")], With: [new Related(Relation.On, new Self())])));

        Assert.Equal(3, Quantities.Evaluate(count, Context(game, ring)));
    }
}
