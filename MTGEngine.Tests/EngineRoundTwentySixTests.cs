using MTGEngine.Agents;

namespace MTGEngine.Tests;

/// <summary>The twenty-sixth engine round: static sequences under one "as long as" condition, "enters tapped and doesn't untap", playing cards from a graveyard or exile by a permanent's permission, and additional combat phases.</summary>
public sealed class EngineRoundTwentySixTests
{
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

    [Fact]
    public void JetmirPumpsOnceThreeCreaturesAreOut()
    {
        var game = Started();
        Put(game, Cards.Named("Jetmir, Nexus of Revels"), 0);
        var bears = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 0);

        // Jetmir and one creature: two creatures, no bonus.
        Assert.Equal(2, Characteristics.Of(game.State, game.State.Object(bears)).Power);
        Assert.False(Characteristics.Of(game.State, game.State.Object(bears)).HasKeyword("Vigilance"));

        Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 0);

        // Three creatures: +1/+0 and vigilance; not yet six, so no trample.
        var traits = Characteristics.Of(game.State, game.State.Object(bears));
        Assert.Equal(3, traits.Power);
        Assert.Equal(2, traits.Toughness);
        Assert.True(traits.HasKeyword("Vigilance"));
        Assert.False(traits.HasKeyword("Trample"));
    }

    [Fact]
    public void GrimgrinEntersTappedAndStaysTapped()
    {
        var game = Started();
        var grimgrin = Put(game, Cards.Named("Grimgrin, Corpse-Born"), 0);

        Assert.True(game.State.Object(grimgrin).Tapped);
        game.PlayTurn(new PlayerId(0));
        Assert.True(game.State.Object(grimgrin).Tapped);
    }

    [Fact]
    public void CrucibleOfWorldsLetsYouPlayLandsFromYourGraveyard()
    {
        var game = Started(new GreedyAgent(attacks: false));
        Put(game, Cards.Named("Crucible of Worlds"), 0);
        var mountain = Put(game, Cards.Basic("Mountain"), 0, Zone.Graveyard);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Battlefield, game.State.Object(mountain).Zone);
    }

    [Fact]
    public void RelentlessAssaultAddsACombatPhaseAndAMainPhase()
    {
        var hand = new Deck("A", [Cards.Basic("Mountain"), Cards.Named("Relentless Assault"), .. Enumerable.Repeat(Cards.Basic("Mountain"), 20)]);
        var game = GameExtensions.Started(hand, Cards.DeckOf("B", (Cards.Basic("Plains"), 20)), new GreedyAgent(), new PassiveAgent());
        for (var i = 0; i < 3; i++)
            Put(game, Cards.Basic("Mountain"), 0);
        Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 0);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Graveyard, game.Find("Relentless Assault").Zone);
        Assert.Equal(2, game.Log.OfType<StepBegan>().Count(x => x.Step == Step.BeginCombat));
        Assert.Equal(2, game.Log.OfType<StepBegan>().Count(x => x.Step == Step.Main2));
        Assert.Equal(1, game.Log.OfType<StepBegan>().Count(x => x.Step == Step.End));
        // The bears attacked in the first of the two combats.
        Assert.Equal(18, game.P1().Life);
    }
}
