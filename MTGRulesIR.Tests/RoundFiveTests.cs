namespace MTGRulesIR.Tests;

/// <summary>The fifth round's shapes: fights, token copies, keyword costs, combat restrictions, game outcomes, and effects as events.</summary>
[Collection(nameof(GrammarCollection))]
public sealed class RoundFiveTests(GrammarFixture fixture)
{
    [Fact]
    public void FightsAnotherTargetCreature()
    {
        var ability = fixture.LowerAbility("when this creature enters, it fights another target creature");

        var fight = Assert.IsType<Fight>(Assert.IsType<TriggeredAbility>(ability).Effect);
        Assert.IsType<Self>(fight.Fighter);
        var opponent = Assert.IsType<ObjectSelector>(fight.Opponent);
        Assert.True(opponent.Other);
        Assert.False(fight.EachOther);
    }

    [Fact]
    public void TokenCopyOfTargetCreature()
    {
        var ability = fixture.LowerAbility("create a token that's a copy of target creature you control", "Sorcery");

        var create = Assert.IsType<CreateTokens>(Assert.IsType<SpellAbility>(ability).Effect);
        var copy = Assert.IsType<TokenCopy>(create.Token);
        Assert.Equal(Quantifier.Target, Assert.IsType<ObjectSelector>(copy.Of).Quantifier);
        Assert.Equal(new Fixed(1), create.Count);
    }

    [Fact]
    public void CumulativeUpkeepWithActionCost()
    {
        var ability = fixture.LowerAbility("cumulative upkeep—put a -1/-1 counter on this creature");

        var keyword = Assert.Single(Assert.IsType<KeywordLine>(ability).Keywords);
        Assert.Equal("CumulativeUpkeep", keyword.Keyword);
        var cost = Assert.IsType<ActionCost>(keyword.Cost);
        Assert.IsType<PutCounters>(cost.Action);
    }

    [Fact]
    public void CantBlockItIsACombatRestriction()
    {
        var ability = fixture.LowerAbility("this creature can't block it");

        var restriction = Assert.IsType<CombatRestriction>(Assert.IsType<StaticAbility>(ability).Effect);
        Assert.Equal(["Block"], restriction.Cant);
        Assert.NotNull(restriction.By);
    }

    [Fact]
    public void CantLoseTheGameThisTurn()
    {
        var ability = fixture.LowerAbility("you can't lose the game this turn", "Instant");

        var outcome = Assert.IsType<GameOutcome>(Assert.IsType<SpellAbility>(ability).Effect);
        Assert.Equal("Lose", outcome.Result);
        Assert.True(outcome.Cant);
        Assert.True(outcome.ThisTurn);
    }

    [Fact]
    public void SacrificingAsATriggerIsAnEffectEvent()
    {
        var ability = fixture.LowerAbility("whenever you sacrifice a creature, draw a card");

        var trigger = Assert.IsType<TriggeredAbility>(ability);
        Assert.IsType<Sacrifice>(Assert.IsType<EffectEvent>(trigger.Trigger).Effect);
        Assert.IsType<Draw>(trigger.Effect);
    }

    [Fact]
    public void ThreeStepsBecomeASequence()
    {
        var ability = fixture.LowerAbility("you draw a card, you gain 2 life, then you lose 1 life", "Sorcery");

        var sequence = Assert.IsType<Sequence>(Assert.IsType<SpellAbility>(ability).Effect);
        Assert.Equal(3, sequence.Steps.Count);
    }
}
