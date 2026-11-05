namespace MTGRulesIR.Tests;

/// <summary>The tenth round's shapes: prevention from a source, players who did something, sources dealing damage, halving actions, life change thresholds, uncounterable spells, and copy exceptions.</summary>
[Collection(nameof(GrammarCollection))]
public sealed class RoundTenTests(GrammarFixture fixture)
{
    [Fact]
    public void PreventionFromASourceNamesTheSource()
    {
        var ability = fixture.LowerAbility("{w}, sacrifice this creature: prevent all damage a source of your choice would deal to you this turn");

        var prevent = Assert.IsType<PreventDamage>(Assert.IsType<ActivatedAbility>(ability).Effect);
        Assert.Equal(new Special("All"), prevent.Amount);
        Assert.IsType<ObjectSelector>(prevent.Source);
        Assert.IsType<PlayerSelector>(prevent.To);
        Assert.IsType<ThisTurn>(prevent.Duration);
    }

    [Fact]
    public void PlayersWhoDidSomethingAreFilteredPlayers()
    {
        var ability = fixture.LowerAbility("each opponent who attacked with a creature this turn can't cast spells", "Enchantment");

        var cant = Assert.IsType<CantPlay>(Assert.IsType<StaticAbility>(ability).Effect);
        var players = Assert.IsType<ObjectSelector>(cant.Player);
        Assert.Equal(nameof(ObjectKind.Player), Assert.Single(players.Filter.Kinds).Value);
        var happened = Assert.IsType<Happened>(Assert.IsType<Satisfying>(Assert.Single(players.Filter.With)).Condition);
        Assert.IsType<PlayerAction>(happened.Event);
    }

    [Fact]
    public void ASourceDealingDamageIsAnObjectEvent()
    {
        var ability = fixture.LowerAbility("whenever a source deals damage to this creature, that source's controller mills that many cards");

        var trigger = Assert.IsType<TriggeredAbility>(ability);
        var happening = Assert.IsType<ObjectEvent>(trigger.Trigger);
        var source = Assert.IsType<ObjectSelector>(happening.Subject);
        Assert.Equal(nameof(ObjectKind.Source), Assert.Single(source.Filter.Kinds).Value);
        var mill = Assert.IsType<Mill>(trigger.Effect);
        Assert.Equal("source", Assert.IsType<Reference>(Assert.IsType<PlayerSelector>(mill.Player).Of).Antecedent);
    }

    [Fact]
    public void MillingHalfALibraryScalesItsCount()
    {
        var rules = fixture.Lower("target player mills half their library, rounded down.", "Sorcery");

        var mill = Assert.IsType<Mill>(Assert.IsType<SpellAbility>(rules.Lines[0].Abilities[0]).Effect);
        var scaled = Assert.IsType<Scaled>(mill.Count);
        Assert.Equal("Half", scaled.Factor);
        Assert.Equal("Down", scaled.Rounding);
        Assert.IsType<CountOf>(scaled.Base);
        Assert.True(rules.Lines[0].EngineClosed);
    }

    [Fact]
    public void LifeGainedThresholdIsACountedPlayerAction()
    {
        var ability = fixture.LowerAbility("at the beginning of each end step, if you gained 4 or more life this turn, create a 4/4 white angel creature token with flying", "Enchantment");

        var conditional = Assert.IsType<Conditional>(Assert.IsType<TriggeredAbility>(ability).Effect);
        var happened = Assert.IsType<Happened>(conditional.If);
        var action = Assert.IsType<PlayerAction>(happened.Event);
        Assert.Equal("GainedLife", action.Action);
        Assert.Equal("ThisTurn", happened.Span);
    }

    [Fact]
    public void UncounterableSpellsCantBeTargetedByCountering()
    {
        var ability = fixture.LowerAbility("creature spells you control can't be countered", "Enchantment");

        var cant = Assert.IsType<CantBeTargeted>(Assert.IsType<StaticAbility>(ability).Effect);
        Assert.Equal([nameof(TargetingBy.Countered)], cant.By);
        Assert.Contains(Assert.IsType<ObjectSelector>(cant.Objects).Filter.With, x => x is CastBy { Relation: "Control" });
    }

    [Fact]
    public void CopyExceptItHasThisAbilityGrantsTheSameAbility()
    {
        var ability = fixture.LowerAbility("at the beginning of your upkeep, you may have this creature become a copy of another target creature, except it has this ability");

        var optional = Assert.IsType<Optional>(Assert.IsType<TriggeredAbility>(ability).Effect);
        var copy = Assert.IsType<BecomeCopy>(optional.Effect);
        var grant = Assert.IsType<GrantAbility>(Assert.Single(copy.Exceptions));
        Assert.IsType<SameAbility>(grant.Ability);
    }
}
