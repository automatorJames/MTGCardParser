namespace MTGRulesIR.Tests;

/// <summary>The eleventh round's shapes: riders on what was affected, several counter placements, repeated actions, mana spent of several symbols, totals, cards in a zone.</summary>
[Collection(nameof(GrammarCollection))]
public sealed class RoundElevenTests(GrammarFixture fixture)
{
    [Fact]
    public void ARiderOnWhatWasDestroyedNamesIt()
    {
        var rules = fixture.Lower("destroy all creatures. creatures destroyed this way can't be regenerated.", "Sorcery");

        var effects = rules.Lines[0].Abilities.OfType<SpellAbility>().Select(x => x.Effect).ToList();
        Assert.Contains(effects, x => x is CantBeRegenerated { Objects: ObjectSelector { Quantifier: Quantifier.Those } });
    }

    [Fact]
    public void RepeatingAnActionCountsTheTimes()
    {
        var rules = fixture.Lower("investigate three times.", "Sorcery");

        var repeated = Assert.IsType<ForEachOf>(Assert.IsType<SpellAbility>(Assert.Single(rules.Lines[0].Abilities)).Effect);
        Assert.Equal(new Fixed(3), repeated.Count);
    }

    [Fact]
    public void TwoSymbolsSpentAreACountOfTheirColor()
    {
        var ability = fixture.LowerAbility("when this creature enters, if {r}{r} was spent to cast it, it deals 2 damage to any target");

        Assert.IsType<TriggeredAbility>(ability);
        var spent = Assert.Single(MTGRulesIR.Lowering.NodeWalker.Nodes(ability).OfType<ManaSpent>());
        Assert.Equal(new Fixed(2), spent.Amount);
        Assert.Equal("Red", spent.Color);
    }

    [Fact]
    public void TheTotalPowerIsATotal()
    {
        var rules = fixture.Lower("you gain life equal to the total power of creatures you control.", "Sorcery");

        var gain = Assert.IsType<GainLife>(Assert.IsType<SpellAbility>(Assert.Single(rules.Lines[0].Abilities)).Effect);
        Assert.Equal("Power", Assert.IsType<TotalOf>(gain.Amount).Characteristic);
    }
}
