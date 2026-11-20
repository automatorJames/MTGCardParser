using MTGEngine.Agents;

namespace MTGEngine.Tests;

/// <summary>The twenty-seventh engine round: "cast a spell from among them", unions of spell selectors in cost reductions, activation-cost reductions granted by other permanents, and "put its counters on target creature".</summary>
public sealed class EngineRoundTwentySevenTests
{
    static readonly PlayerSelector You = new(PlayerKind.You);

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

    static Game Started(IDecisionMaker first = null, IDecisionMaker second = null, int handSize = 0) =>
        GameExtensions.Started(Cards.DeckOf("A", (Cards.Basic("Mountain"), 20)), Cards.DeckOf("B", (Cards.Basic("Plains"), 20)), first ?? new PassiveAgent(), second ?? new PassiveAgent(), handSize);

    static EffectContext Context(Game game, ObjectId? source = null) =>
        new(game, new PlayerId(0), source, new Dictionary<IrNode, ImmutableList<TargetRef>>(ReferenceEqualityComparer.Instance));

    [Fact]
    public void AetherworksMarvelCastsASpellFromAmongTheTopSix()
    {
        var deck = new Deck("A", [Cards.Named("Lightning Bolt"), .. Enumerable.Repeat(Cards.Basic("Mountain"), 20)]);
        var game = GameExtensions.Started(deck, Cards.DeckOf("B", (Cards.Basic("Plains"), 20)), new GreedyAgent(attacks: false), new PassiveAgent(), handSize: 0);
        var marvel = Put(game, Cards.Named("Aetherworks Marvel"), 0);
        var bolt = game.P0().Library[0];
        var ability = game.State.Object(marvel).Card.Abilities.OfType<ActivatedAbility>().Single();

        Interpreter.Execute(ability.Effect, Context(game, marvel));
        game.ResolveStack();

        // The bolt was cast for free from the six looked at; the five lands went to the bottom.
        Assert.Equal(Zone.Graveyard, game.State.Object(bolt).Zone);
        Assert.Equal(17, game.P1().Life);
        Assert.Equal(20, game.P0().Library.Count);
    }

    [Fact]
    public void BallyrushBanneretMakesKithkinAndSoldierSpellsCheaper()
    {
        var game = Started();
        Put(game, Cards.Named("Ballyrush Banneret"), 0);
        Put(game, Cards.Basic("Plains"), 0);
        var kithkin = Put(game, Cards.Vanilla("Kithkin Scout", "{1}{W}", 1, 1, "Kithkin"), 0, Zone.Hand);
        var soldier = Put(game, Cards.Vanilla("Soldier Recruit", "{1}{W}", 1, 1, "Soldier"), 0, Zone.Hand);
        var elf = Put(game, Cards.Vanilla("Elf Scout", "{1}{W}", 1, 1, "Elf"), 0, Zone.Hand);
        game.Emit(new StepBegan(Step.Main1, new PlayerId(0)));

        var actions = game.LegalActions(new PlayerId(0));

        Assert.Contains(actions, x => x is CastSpell cast && cast.Card == kithkin);
        Assert.Contains(actions, x => x is CastSpell cast && cast.Card == soldier);
        Assert.DoesNotContain(actions, x => x is CastSpell cast && cast.Card == elf);
    }

    [Fact]
    public void TrainingGroundsMakesYourCreaturesAbilitiesCheaper()
    {
        var healer = Cards.Vanilla("Healer", "{1}", 1, 1) with { Abilities = [new ActivatedAbility([new PayMana(["{3}"])], new GainLife(You, new Fixed(1)))] };
        var game = Started();
        var mine = Put(game, healer, 0);
        Put(game, Cards.Basic("Plains"), 0);
        Assert.DoesNotContain(game.LegalActions(new PlayerId(0)), x => x is ActivateAbility activate && activate.Source == mine);

        Put(game, Cards.Named("Training Grounds"), 0);

        Assert.Contains(game.LegalActions(new PlayerId(0)), x => x is ActivateAbility activate && activate.Source == mine);
    }

    [Fact]
    public void IronApprenticePassesItsCountersOnWhenItDies()
    {
        var game = Started(new GreedyAgent(attacks: false));
        var apprentice = Put(game, Cards.Named("Iron Apprentice"), 0);
        var bears = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 0);
        Assert.Equal(1, game.State.Object(apprentice).CountersOf("+1/+1"));

        game.Destroy(apprentice);
        game.ResolveStack();

        Assert.Equal(1, game.State.Object(bears).CountersOf("+1/+1"));
        Assert.Equal(3, Characteristics.Of(game.State, game.State.Object(bears)).Power);
    }

    [Fact]
    public void TheOzolithKeepsTheCountersOfACreatureThatLeft()
    {
        var game = Started();
        var ozolith = Put(game, Cards.Named("The Ozolith"), 0);
        var bears = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 0);
        game.Emit(new CountersChanged(bears, "+1/+1", 2));
        game.Emit(new CountersChanged(bears, "flying", 1));

        game.Destroy(bears);
        game.ResolveStack();

        Assert.Equal(2, game.State.Object(ozolith).CountersOf("+1/+1"));
        Assert.Equal(1, game.State.Object(ozolith).CountersOf("flying"));
    }
}
