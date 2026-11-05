namespace MTGRulesIR.Tests;

/// <summary>The ninth round's shapes: blocking if able, counted player actions as conditions, abilities of a group, exchanging control, state triggers, attached permanents, keyword choices, and mana a land could produce.</summary>
[Collection(nameof(GrammarCollection))]
public sealed class RoundNineTests(GrammarFixture fixture)
{
    [Fact]
    public void BlocksIfAbleIsMustBlock()
    {
        var ability = fixture.LowerAbility("{1}: target artifact creature blocks this creature this turn if able");

        var must = Assert.IsType<MustBlock>(Assert.IsType<ActivatedAbility>(ability).Effect);
        Assert.IsType<Self>(must.Attacker);
        Assert.Equal(Quantifier.Target, Assert.IsType<ObjectSelector>(must.Blockers).Quantifier);
        Assert.IsType<ThisTurn>(must.Duration);
    }

    [Fact]
    public void CountedPlayerActionThisTurnIsOneHappening()
    {
        var ability = fixture.LowerAbility("at the beginning of each end step, if you drew two or more cards this turn, you may put a quest counter on this enchantment", "Enchantment");

        var conditional = Assert.IsType<Conditional>(Assert.IsType<TriggeredAbility>(ability).Effect);
        var happened = Assert.IsType<Happened>(conditional.If);
        var action = Assert.IsType<PlayerAction>(happened.Event);
        Assert.Equal("ThisTurn", happened.Span);
        Assert.Equal(new Bound(new Fixed(2), Comparison.GreaterOrEqual), action.Count);
    }

    [Fact]
    public void ActivatedAbilitiesOfAGroupAreAbilitiesRelatedToIt()
    {
        var ability = fixture.LowerAbility("activated abilities of creatures can't be activated", "Artifact");

        var cant = Assert.IsType<CantBeActivated>(Assert.IsType<StaticAbility>(ability).Effect);
        var abilities = Assert.IsType<ObjectSelector>(cant.Abilities);
        Assert.Equal(nameof(ObjectKind.Ability), Assert.Single(abilities.Filter.Kinds).Value);
        var related = Assert.IsType<Related>(Assert.Single(abilities.Filter.With));
        Assert.Equal(Relation.AbilityOf, related.Relation);
        Assert.True(fixture.Lower("activated abilities of creatures can't be activated", "Artifact").Lines[0].EngineClosed);
    }

    [Fact]
    public void ExchangingControlNamesBothSides()
    {
        var ability = fixture.LowerAbility("{2}{u}{u}: exchange control of this creature and target creature");

        var exchange = Assert.IsType<ExchangeControl>(Assert.IsType<ActivatedAbility>(ability).Effect);
        Assert.Equal(2, exchange.Objects.Count);
        Assert.IsType<Self>(exchange.Objects[0]);
    }

    [Fact]
    public void ControllingNoneIsAStateTrigger()
    {
        var ability = fixture.LowerAbility("when you control no swamps, sacrifice this creature");

        var trigger = Assert.IsType<TriggeredAbility>(ability);
        var controls = Assert.IsType<Controls>(Assert.IsType<ConditionEvent>(trigger.Trigger).Condition);
        Assert.True(controls.Negated);
        Assert.IsType<Sacrifice>(trigger.Effect);
    }

    [Fact]
    public void AttachedPermanentsAreRelatedToTheirHost()
    {
        var ability = fixture.LowerAbility("this creature gets +2/+2 for each aura attached to it");

        var pump = Assert.IsType<Pump>(Assert.IsType<StaticAbility>(ability).Effect);
        var counted = Assert.IsType<ObjectSelector>(Assert.IsType<CountOf>(pump.PerEach).Of);
        var related = Assert.IsType<Related>(Assert.Single(counted.Filter.With));
        Assert.Equal(Relation.AttachedTo, related.Relation);
    }

    [Fact]
    public void ChoiceOfKeywordsIsAPumpChoice()
    {
        var ability = fixture.LowerAbility("{2}: target artifact creature gains your choice of flying, trample, or haste until end of turn", "Artifact");

        var pump = Assert.IsType<Pump>(Assert.IsType<ActivatedAbility>(ability).Effect);
        Assert.Null(pump.Keywords);
        Assert.Equal(["Flying", "Trample", "Haste"], pump.ChoiceOf.Select(x => x.Keyword));
    }

    [Fact]
    public void ManaALandCouldProduceNamesTheLand()
    {
        var ability = fixture.LowerAbility("{t}, untap a tapped land an opponent controls: add one mana of any type that land could produce", "Land");

        var add = Assert.IsType<AddMana>(Assert.IsType<ActivatedAbility>(ability).Effect);
        var choice = Assert.IsType<ManaChoice>(Assert.Single(add.Options));
        Assert.Equal(nameof(ManaChoiceKind.AnyTypeCouldProduce), choice.Choice);
        Assert.IsType<Reference>(choice.Producer);
    }

    [Fact]
    public void HalfLifeAsACostIsPayingLife()
    {
        var rules = fixture.Lower("you lose half your life, rounded up.", "Sorcery");

        var lose = Assert.IsType<LoseLife>(Assert.IsType<SpellAbility>(rules.Lines[0].Abilities[0]).Effect);
        var scaled = Assert.IsType<Scaled>(lose.Amount);
        Assert.IsType<LifeTotal>(scaled.Base);
        Assert.Equal("Half", scaled.Factor);
        Assert.True(rules.Lines[0].EngineClosed);
    }
}
