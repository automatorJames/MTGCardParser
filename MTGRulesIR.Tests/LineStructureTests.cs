namespace MTGRulesIR.Tests;

/// <summary>How a line's sentences become abilities: later sentences continue the ability before them (outside reminder text), and characteristic bounds include their number.</summary>
[Collection(nameof(GrammarCollection))]
public sealed class LineStructureTests(GrammarFixture fixture)
{
    [Fact]
    public void LaterSentencesContinueATriggeredAbility()
    {
        var rules = fixture.Lower("whenever this creature deals combat damage to a player, you may discard a card. if you do, draw a card.", "Creature");

        var trigger = Assert.IsType<TriggeredAbility>(Assert.Single(rules.Lines[0].Abilities));
        var steps = Assert.IsType<Sequence>(trigger.Effect).Steps;
        Assert.Equal(2, steps.Count);
        Assert.IsType<Conditional>(steps[1]);
    }

    [Fact]
    public void LaterSentencesContinueAnActivatedAbility()
    {
        var rules = fixture.Lower("{2}, {t}: destroy target creature. then put a charge counter on this artifact.", "Artifact");

        var activated = Assert.IsType<ActivatedAbility>(Assert.Single(rules.Lines[0].Abilities));
        Assert.IsType<PutCounters>(Assert.IsType<Sequence>(activated.Effect).Steps[^1]);
    }

    [Fact]
    public void ReminderTextDoesNotContinueTheAbilityBeforeIt()
    {
        var rules = fixture.Lower("{t}: scry 1. (look at the top card of your library. you may put that card on the bottom.)", "Artifact");

        var activated = Assert.IsType<ActivatedAbility>(rules.Lines[0].Abilities[0]);
        Assert.IsNotType<Sequence>(activated.Effect);
    }

    [Fact]
    public void APowerBoundIncludesItsNumber()
    {
        var ability = fixture.LowerAbility("this creature can't be blocked by creatures with power 3 or greater");

        var restriction = Assert.IsType<CombatRestriction>(Assert.IsType<StaticAbility>(ability).Effect);
        var bound = Assert.IsType<CharacteristicBound>(Assert.Single(Assert.IsType<ObjectSelector>(restriction.By).Filter.With));
        Assert.Equal(Comparison.GreaterOrEqual, bound.Comparison);
    }
}
