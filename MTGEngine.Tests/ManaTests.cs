namespace MTGEngine.Tests;

public sealed class ManaTests
{
    [Fact]
    public void ParsesGenericColoredHybridAndX()
    {
        var cost = ManaCost.Parse("{X}{2}{W}{W}{G/U}");

        Assert.Equal(2, cost.Generic);
        Assert.Equal(2, cost.Colored[Color.White]);
        Assert.Single(cost.Hybrid);
        Assert.Equal(1, cost.X);
        Assert.Equal(5, cost.ManaValue);
        Assert.Equal("{X}{2}{W}{W}{G/U}", cost.ToString());
    }

    [Fact]
    public void PoolPaysColoredBeforeGeneric()
    {
        var pool = ManaPool.Empty.Add(Color.Red).Add(Color.Green, 2);

        var remaining = pool.Pay(ManaCost.Parse("{1}{G}"));

        Assert.NotNull(remaining);
        Assert.Equal(1, remaining.Total);
        Assert.Null(pool.Pay(ManaCost.Parse("{R}{R}")));
    }

    [Fact]
    public void BasicLandsPayForASpell()
    {
        var game = GameExtensions.Started(
            Cards.DeckOf("A", (Cards.Basic("Forest"), 2), (Cards.Basic("Mountain"), 1), (Cards.Named("Giant Growth"), 1), (Cards.Basic("Forest"), 10)),
            Cards.DeckOf("B", (Cards.Basic("Plains"), 14)),
            new PassiveAgent());
        var you = new PlayerId(0);
        foreach (var land in game.P0().Hand.Where(id => game.State.Object(id).Card.IsLand).Take(3).ToList())
            game.MoveToZone(land, Zone.Battlefield, you);

        Assert.True(ManaPayment.CanPay(game.State, you, ManaCost.Parse("{1}{G}")));
        Assert.True(ManaPayment.CanPay(game.State, you, ManaCost.Parse("{R}{G}{G}")));
        Assert.False(ManaPayment.CanPay(game.State, you, ManaCost.Parse("{R}{R}")));
        Assert.Equal(3, ManaPayment.MaxX(game.State, you, ManaCost.Parse("{X}")));
    }
}
