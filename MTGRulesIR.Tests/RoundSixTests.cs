namespace MTGRulesIR.Tests;

/// <summary>The sixth round's shapes: delayed triggers as events, life thresholds, skipping, class levels, and reminder-like text lowering to nothing.</summary>
[Collection(nameof(GrammarCollection))]
public sealed class RoundSixTests(GrammarFixture fixture)
{
    [Fact]
    public void WhenThatCreatureDiesThisTurnIsAnObjectEvent()
    {
        var rules = fixture.Lower("{this} deals 6 damage to target creature. when that creature dies this turn, exile its controller's graveyard.", "Instant");

        var trigger = rules.Lines[0].Abilities.OfType<TriggeredAbility>().Single();
        var happening = Assert.IsType<ObjectEvent>(trigger.Trigger);
        Assert.Equal("ThisTurn", happening.Only);
        Assert.True(rules.Lines[0].FullyLowered);
    }

    [Fact]
    public void LifeThresholdCompares()
    {
        var ability = fixture.LowerAbility("as long as you have 30 or more life, this creature gets +2/+2");

        var statics = Assert.IsType<StaticAbility>(ability);
        var compare = Assert.IsType<CompareCount>(statics.While);
        Assert.IsType<LifeTotal>(compare.Count);
        Assert.Equal(Comparison.GreaterOrEqual, compare.Comparison);
        Assert.Equal(new Fixed(30), compare.Threshold);
    }

    [Fact]
    public void ClassLevelIsAnActivatedAbility()
    {
        var ability = fixture.LowerAbility("{1}{r}: level 2", "Enchantment");

        var activated = Assert.IsType<ActivatedAbility>(ability);
        Assert.Equal(2, Assert.IsType<GainClassLevel>(activated.Effect).Level);
    }

    [Fact]
    public void MadnessReminderLowersToNothing()
    {
        var rules = fixture.Lower("when you do, cast it for its madness cost or put it into your graveyard", "Instant");

        Assert.Empty(rules.Lines[0].Abilities);
    }
}
