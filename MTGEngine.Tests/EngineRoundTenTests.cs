namespace MTGEngine.Tests;

/// <summary>The tenth engine round: coin flips and die tables, names, casting as though it had flash, skipping steps, staying tapped, player-action triggers.</summary>
public sealed class EngineRoundTenTests
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
    public void ACoinFlipDecidesWhatHappens()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        var effect = new Sequence([
            new FlipCoin(You),
            new Conditional(new OutcomeCondition(You, "WonFlip"), new GainLife(You, new Fixed(1))),
            new Conditional(new OutcomeCondition(You, "LostFlip"), new GainLife(You, new Fixed(10))),
        ]);

        Interpreter.Execute(effect, Context(game));

        Assert.Equal(game.WonLastFlip ? 21 : 30, game.P0().Life);
    }

    [Fact]
    public void ADieTableRunsTheRowThatWasRolled()
    {
        var rules = new CardRules("Dicey", "Sorcery",
        [
            new LineRules(0, "roll a d20.", [new SpellAbility(new RollDice(You, new Fixed(1), 20))], []),
            new LineRules(1, "1—9 | you gain 1 life.", [new DieResultRow(1, 9, new GainLife(You, new Fixed(1)))], []),
            new LineRules(2, "10—20 | you gain 5 life.", [new DieResultRow(10, 20, new GainLife(You, new Fixed(5)))], []),
        ], new CardInfo("{1}"));
        var card = CardDefinition.From(rules);
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());

        Interpreter.Execute(Assert.Single(card.SpellEffects), Context(game));

        Assert.Equal(game.LastDieRoll <= 9 ? 21 : 25, game.P0().Life);
    }

    [Fact]
    public void ACardNamedIsFoundByName()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        var bolt = Put(game, Cards.Named("Lightning Bolt"), 0, Zone.Graveyard);
        var named = new ObjectSelector(Quantifier.All, new ObjectFilter(Kinds: [new Term("Card", nameof(ObjectKind))], With: [new Named("lightning bolt")], Zone: new ZoneRef("Graveyard")));

        Assert.Equal([bolt], Resolver.Objects(named, Context(game)));
    }

    [Fact]
    public void CreaturesCanBeCastAsThoughTheyHadFlash()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        Put(game, Cards.Basic("Forest"), 0);
        Put(game, Cards.Basic("Forest"), 0);
        Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 0, Zone.Hand);
        game.Emit(new TurnBegan(1, new PlayerId(1)));
        game.Emit(new StepBegan(Step.Upkeep, new PlayerId(1)));
        Assert.DoesNotContain(game.LegalActions(new PlayerId(0)), x => x is CastSpell);

        var creatures = new ObjectSelector(Quantifier.All, new ObjectFilter(Kinds: [new Term("Spell", nameof(ObjectKind))], Qualifiers: [new Qualifier("Creature", Vocabulary: "CardType")]));
        Interpreter.Execute(new CastAsThoughFlash(creatures, new ThisTurn()), Context(game));

        Assert.Contains(game.LegalActions(new PlayerId(0)), x => x is CastSpell);
    }

    [Fact]
    public void SkippingTheNextUntapStepLeavesThingsTapped()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        var land = Put(game, Cards.Basic("Plains"), 0);
        game.Emit(new Tapped(land));

        Interpreter.Execute(new Skip(You, new PhaseRef("UntapStep")), Context(game));
        game.PlayTurn(new PlayerId(0));
        Assert.True(game.State.Object(land).Tapped);

        game.PlayTurn(new PlayerId(1));
        game.PlayTurn(new PlayerId(0));
        Assert.False(game.State.Object(land).Tapped);
    }

    [Fact]
    public void YouMayChooseNotToUntap()
    {
        var card = Cards.Vanilla("Stubborn", "{1}", 1, 1) with { Abilities = [new StaticAbility(new MayNotUntap(new Self(), new PhaseRef("UntapStep")))] };
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        var stubborn = Put(game, card, 0);
        game.Emit(new Tapped(stubborn));

        game.PlayTurn(new PlayerId(0));

        // The passive agent answers no.
        Assert.True(game.State.Object(stubborn).Tapped);
    }

    [Fact]
    public void WheneverYouAttackTriggersOnce()
    {
        var card = Cards.Vanilla("Battle Horn", "{1}", 0, 1) with
        {
            Abilities = [new TriggeredAbility(new PlayerEvent(You, "Attacks"), new GainLife(You, new Fixed(1)))],
        };
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new GreedyAgent(attacks: true), new PassiveAgent());
        Put(game, card, 0);
        Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 0);
        Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 0);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(21, game.P0().Life);
    }

    [Fact]
    public void TappingALandForManaTriggers()
    {
        var card = Cards.Vanilla("Mana Watcher", "{1}", 0, 1) with
        {
            Abilities = [new TriggeredAbility(new ObjectEvent(new ObjectSelector(Quantifier.Any, new ObjectFilter(Kinds: [new Term("Land", "CardType")])), ["IsTappedForMana"]), new GainLife(You, new Fixed(1)))],
        };
        var game = GameExtensions.Started(new Deck("A", [Cards.Named("Lightning Bolt"), .. Enumerable.Repeat(Cards.Basic("Mountain"), 20)]), Lands("B"), new GreedyAgent(attacks: false), new PassiveAgent());
        Put(game, card, 0);
        Put(game, Cards.Basic("Mountain"), 0);

        game.PlayTurn(new PlayerId(0));

        Assert.True(game.P0().Life >= 21);
    }
}
