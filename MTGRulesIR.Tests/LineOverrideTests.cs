namespace MTGRulesIR.Tests;

/// <summary>Hand-written IR for a line replaces what the grammar gave it, and is checked for closure like any other line.</summary>
[Collection(nameof(GrammarCollection))]
public sealed class LineOverrideTests(GrammarFixture fixture)
{
    const string Text = "whenever an opponent casts a spell, you gain 1 life.";

    CardRules LowerWith(LineOverrides overrides)
    {
        var card = new Card { Name = "Sample", Text = Text, Types = "Creature" };
        return new CorpusLowering(fixture.Lowering.Lowerer, overrides).Lower(new ProcessedDocument(card, fixture.Grammar));
    }

    [Fact]
    public void AnOverrideReplacesTheLinesAbilities()
    {
        var written = new TriggeredAbility(new SpellCastEvent(new PlayerSelector(PlayerKind.AnyOpponent), new ObjectSelector(Quantifier.Any, new ObjectFilter(Kinds: [new Term("Spell", "ObjectKind")]))), new GainLife(new PlayerSelector(PlayerKind.You), new Fixed(1)));
        var overrides = new LineOverrides();
        overrides.Add(new LineOverride("sample", Text.ToUpperInvariant(), [written], "test"));

        var rules = LowerWith(overrides);

        Assert.Equal(written, Assert.Single(rules.Lines[0].Abilities));
        Assert.True(rules.Lines[0].EngineClosed);
    }

    [Fact]
    public void AnOverrideWithAnOpaqueNodeStaysOpaque()
    {
        var overrides = new LineOverrides();
        overrides.Add(new LineOverride("Sample", Text, [new OpaqueAbility("whatever", "ByHand")]));

        var rules = LowerWith(overrides);

        Assert.Equal(["ByHand"], rules.Lines[0].Opaque);
    }

    [Fact]
    public void OverridesRoundTripThroughJson()
    {
        var written = new LineOverride("Sample", Text, [new SpellAbility(new Draw(new PlayerSelector(PlayerKind.You), new Fixed(2)))], "note");

        var json = IrJson.Serialize(written);
        var back = IrJson.Deserialize<LineOverride>(json);

        Assert.Equal(written.Card, back.Card);
        Assert.Equal(written.Note, back.Note);
        Assert.Equal(json, IrJson.Serialize(back));
        Assert.IsType<SpellAbility>(Assert.Single(back.Abilities));
    }

    [Fact]
    public void WithoutAnOverrideTheGrammarsLoweringStands()
    {
        var rules = LowerWith(LineOverrides.Empty);

        Assert.IsType<TriggeredAbility>(Assert.Single(rules.Lines[0].Abilities));
    }
}
