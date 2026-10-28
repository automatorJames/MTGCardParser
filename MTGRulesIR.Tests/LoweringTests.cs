namespace MTGRulesIR.Tests;

[Collection(nameof(GrammarCollection))]
public sealed class LoweringTests(GrammarFixture fixture)
{
    [Fact]
    public void TargetPlayerDrawsThreeCards()
    {
        var ability = fixture.LowerAbility("target player draws three cards", "Instant");

        var spell = Assert.IsType<SpellAbility>(ability);
        var draw = Assert.IsType<Draw>(spell.Effect);
        Assert.Equal(new Fixed(3), draw.Count);
        Assert.Equal(PlayerKind.TargetPlayer, Assert.IsType<PlayerSelector>(draw.Player).Kind);
    }

    [Fact]
    public void DealsDamageToAnyTarget()
    {
        var ability = fixture.LowerAbility("{this} deals 3 damage to any target", "Instant");

        var damage = Assert.IsType<DealDamage>(Assert.IsType<SpellAbility>(ability).Effect);
        Assert.IsType<Self>(damage.Source);
        var portion = Assert.Single(damage.Portions);
        Assert.Equal(new Fixed(3), portion.Amount);
        var target = Assert.IsType<ObjectSelector>(portion.To);
        Assert.Equal(Quantifier.Target, target.Quantifier);
        Assert.Equal(["any"], target.Filter.Kinds);
    }

    [Fact]
    public void TapForMana()
    {
        var ability = fixture.LowerAbility("{t}: add {g}");

        var activated = Assert.IsType<ActivatedAbility>(ability);
        Assert.IsType<TapSelf>(Assert.Single(activated.Costs));
        var mana = Assert.IsType<AddMana>(activated.Effect);
        Assert.Equal(["{g}"], Assert.IsType<ManaAmount>(Assert.Single(mana.Options)).Symbols);
    }

    [Fact]
    public void KeywordLineWithReminder()
    {
        var ability = fixture.LowerAbility("flying (this creature can't be blocked except by creatures with flying or reach.)");

        var keywords = Assert.IsType<KeywordLine>(ability);
        Assert.Equal("Flying", Assert.Single(keywords.Keywords).Keyword);
        Assert.Null(keywords.Other);
    }

    [Fact]
    public void ThresholdStaticWithCondition()
    {
        var ability = fixture.LowerAbility("threshold — enchanted creature has shroud as long as there are seven or more cards in your graveyard.", "Enchantment");

        var labeled = Assert.IsType<LabeledAbility>(ability);
        Assert.Equal("Threshold", labeled.Word);
        var statics = Assert.IsType<StaticAbility>(labeled.Ability);
        var pump = Assert.IsType<Pump>(statics.Effect);
        Assert.Equal("Shroud", Assert.Single(pump.Keywords).Keyword);
        Assert.Equal(Quantifier.Attached, Assert.IsType<ObjectSelector>(pump.Objects).Quantifier);
        var count = Assert.IsType<CountCondition>(statics.While);
        Assert.Equal(">=", count.Comparison);
        Assert.Equal(new Fixed(7), count.Threshold);
        var cards = Assert.IsType<ObjectSelector>(count.Of);
        Assert.Equal("Graveyard", cards.Filter.Zone.Zone);
        Assert.Equal(PlayerKind.You, Assert.IsType<PlayerSelector>(cards.Filter.Zone.Owner).Kind);
    }

    [Fact]
    public void PhaseTriggerBindsThatPlayer()
    {
        var ability = fixture.LowerAbility("at the beginning of each player's upkeep, {this} deals 1 damage to that player", "Artifact");

        var triggered = Assert.IsType<TriggeredAbility>(ability);
        var phase = Assert.IsType<PhaseEvent>(triggered.Trigger).At;
        Assert.Equal("Upkeep", phase.Phase);
        var whose = Assert.IsType<PlayerSelector>(phase.Whose);
        Assert.Equal(PlayerKind.EachPlayer, whose.Kind);
        Assert.NotNull(whose.Binding);

        var damage = Assert.IsType<DealDamage>(triggered.Effect);
        var reference = Assert.IsType<Reference>(Assert.Single(damage.Portions).To);
        Assert.Equal(whose.Binding, reference.Binding);
    }

    [Fact]
    public void EventTriggerBindsThatLandsController()
    {
        var ability = fixture.LowerAbility("whenever a land enters, {this} deals 2 damage to that land's controller", "Artifact");

        var triggered = Assert.IsType<TriggeredAbility>(ability);
        var trigger = Assert.IsType<ObjectEvent>(triggered.Trigger);
        var land = Assert.IsType<ObjectSelector>(trigger.Subject);
        Assert.Equal(["Land"], land.Filter.Kinds);
        Assert.NotNull(land.Binding);

        var controller = Assert.IsType<PlayerSelector>(Assert.Single(Assert.IsType<DealDamage>(triggered.Effect).Portions).To);
        Assert.Equal(PlayerKind.ControllerOf, controller.Kind);
        Assert.Equal(land.Binding, Assert.IsType<Reference>(controller.Of).Binding);
    }

    [Fact]
    public void PumpUntilEndOfTurn()
    {
        var ability = fixture.LowerAbility("target creature gets +2/+2 and gains flying until end of turn", "Instant");

        var effect = Assert.IsType<SpellAbility>(ability).Effect;
        var steps = Assert.IsType<Sequence>(effect).Steps;
        Assert.Equal(2, steps.Count);
        var stats = Assert.IsType<Pump>(steps[0]);
        Assert.Equal(new Fixed(2), stats.Power);
        Assert.Equal(new Fixed(2), stats.Toughness);
        var keyword = Assert.IsType<Pump>(steps[1]);
        Assert.Equal("Flying", Assert.Single(keyword.Keywords).Keyword);
        Assert.Equal("EndOfTurn", Assert.IsType<UntilPhase>(keyword.Duration).Phase.Phase);
    }

    [Fact]
    public void CardSpecificLiteralStaysOpaque()
    {
        var rules = fixture.Lower("each creature that's attacking one of your opponents has trample");

        Assert.False(rules.FullyLowered);
        var opaque = Assert.IsType<OpaqueAbility>(Assert.Single(rules.Lines[0].Abilities));
        Assert.Equal("OviyaAutomechArtisanRule", opaque.Glyph);
    }

    [Fact]
    public void JsonRoundTrips()
    {
        var rules = fixture.Lower("at the beginning of each player's upkeep, {this} deals 1 damage to that player", "Artifact");

        var json = IrJson.Serialize(rules);
        var back = IrJson.Deserialize<CardRules>(json);

        Assert.Contains("\"$kind\":\"TriggeredAbility\"", json);
        Assert.Equal(IrJson.Serialize(rules), IrJson.Serialize(back));
    }
}
