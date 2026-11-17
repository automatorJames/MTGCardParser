namespace MTGEngine.Tests;

/// <summary>The twenty-fifth engine round: damage modifiers that halve, add a count, set a fixed amount, or apply only above a threshold.</summary>
public sealed class EngineRoundTwentyFiveTests
{
    static ObjectId Put(Game game, CardDefinition card, int player)
    {
        var id = new ObjectId(game.State.NextObjectId);
        var owner = new PlayerId(player);
        game.Emit(new ObjectCreated(new GameObject(id, card, owner, owner, Zone.Hand)));
        game.MoveToZone(id, Zone.Battlefield, owner);
        game.Emit(new SummoningSicknessCleared(owner));
        return id;
    }

    static Game Started() =>
        GameExtensions.Started(Cards.DeckOf("A", (Cards.Basic("Mountain"), 20)), Cards.DeckOf("B", (Cards.Basic("Plains"), 20)), new PassiveAgent(), new PassiveAgent());

    [Fact]
    public void GhostsOfTheInnocentHalvesDamageRoundedDown()
    {
        var game = Started();
        Put(game, Cards.Named("Ghosts of the Innocent"), 0);
        var mine = Put(game, Cards.Vanilla("Bears", "{1}{G}", 3, 3), 0);
        var theirs = Put(game, Cards.Vanilla("Bears", "{1}{G}", 4, 4), 1);

        game.DealDamage(mine, TargetRef.Of(new PlayerId(1)), 3, combat: true);
        game.DealDamage(theirs, TargetRef.Of(new PlayerId(0)), 4, combat: true);

        Assert.Equal(19, game.P1().Life);
        Assert.Equal(18, game.P0().Life);
    }

    [Fact]
    public void DivinePresenceCapsLargeDamageOnly()
    {
        var game = Started();
        Put(game, Cards.Named("Divine Presence"), 0);
        var small = Put(game, Cards.Vanilla("Bears", "{1}{G}", 3, 3), 1);
        var large = Put(game, Cards.Vanilla("Wurm", "{4}{G}", 6, 6), 1);

        game.DealDamage(small, TargetRef.Of(new PlayerId(0)), 3, combat: true);
        game.DealDamage(large, TargetRef.Of(new PlayerId(0)), 6, combat: true);

        // Three stays three; six becomes three.
        Assert.Equal(20 - 3 - 3, game.P0().Life);
    }

    [Fact]
    public void FatedFirepowerAddsItsFireCounters()
    {
        var game = Started();
        var firepower = Put(game, Cards.Named("Fated Firepower"), 0);
        game.Emit(new CountersChanged(firepower, "fire", 2));
        var mine = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 0);
        var theirs = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 1);

        game.DealDamage(mine, TargetRef.Of(new PlayerId(1)), 2, combat: true);
        game.DealDamage(theirs, TargetRef.Of(new PlayerId(0)), 2, combat: true);

        // Your source deals 2 + 2; the opponent's is unchanged.
        Assert.Equal(16, game.P1().Life);
        Assert.Equal(18, game.P0().Life);
    }

    [Fact]
    public void CombustsDamageCantBePrevented()
    {
        var game = Started();
        var elemental = Put(game, Cards.Named("Dawn Elemental"), 1);
        var bears = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 0);
        var combust = new ObjectId(game.State.NextObjectId);
        game.Emit(new ObjectCreated(new GameObject(combust, Cards.Named("Combust"), new PlayerId(0), new PlayerId(0), Zone.Stack)));

        // "Prevent all damage that would be dealt to this creature" stops the bears, not the spell whose damage can't be prevented.
        game.DealDamage(bears, TargetRef.Of(elemental), 2, combat: true);
        Assert.Equal(0, game.State.Object(elemental).Damage);

        game.DealDamage(combust, TargetRef.Of(elemental), 5, combat: false);
        Assert.Equal(5, game.State.Object(elemental).Damage);
    }

    [Fact]
    public void ErebosStopsOpponentsGainingLife()
    {
        var game = Started();
        Put(game, Cards.Named("Erebos, God of the Dead"), 0);

        game.ChangeLife(new PlayerId(1), 3, "test");
        game.ChangeLife(new PlayerId(0), 3, "test");

        Assert.Equal(20, game.P1().Life);
        Assert.Equal(23, game.P0().Life);
    }

    [Fact]
    public void SkullcrackBansLifeGainAndPreventionForTheTurn()
    {
        var hand = new Deck("A", [Cards.Basic("Mountain"), Cards.Named("Skullcrack"), .. Enumerable.Repeat(Cards.Basic("Mountain"), 20)]);
        var game = GameExtensions.Started(hand, Cards.DeckOf("B", (Cards.Basic("Plains"), 20)), new GreedyAgent(attacks: false), new PassiveAgent());
        Put(game, Cards.Basic("Mountain"), 0);

        game.PlayTurn(new PlayerId(0));

        // The turn it was cast in is over, so both effects have run out; the log shows they were set up for that turn only.
        Assert.Equal(17, game.P1().Life);
        var ban = Assert.Single(game.Log.OfType<LifeGainBanned>());
        Assert.Equal([new PlayerId(0), new PlayerId(1)], ban.Ban.Players);
        Assert.Equal(1, ban.Ban.ExpiresAfterTurn);
        var unpreventable = Assert.Single(game.Log.OfType<UnpreventableDamageCreated>());
        Assert.Null(unpreventable.Entry.Source);
        Assert.Equal(1, unpreventable.Entry.ExpiresAfterTurn);
        Assert.Empty(game.State.LifeGainBanList);
        Assert.Empty(game.State.UnpreventableList);
    }

    [Fact]
    public void LifeGainBanAndUnpreventableDamageHoldWhileTheyLast()
    {
        var game = Started();
        var elemental = Put(game, Cards.Named("Dawn Elemental"), 1);
        var bears = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 0);
        game.Emit(new LifeGainBanned(new LifeGainBan([new PlayerId(1)], game.State.Turn)));
        game.Emit(new UnpreventableDamageCreated(new UnpreventableDamage(null, null, new PlayerId(0), CombatOnly: true, game.State.Turn)));

        game.ChangeLife(new PlayerId(1), 3, "test");
        game.ChangeLife(new PlayerId(0), 3, "test");
        game.DealDamage(bears, TargetRef.Of(elemental), 2, combat: true);

        Assert.Equal(20, game.P1().Life);
        Assert.Equal(23, game.P0().Life);
        Assert.Equal(2, game.State.Object(elemental).Damage);

        // Noncombat damage is still prevented when only combat damage can't be.
        game.DealDamage(bears, TargetRef.Of(elemental), 1, combat: false);
        Assert.Equal(2, game.State.Object(elemental).Damage);
    }
}
