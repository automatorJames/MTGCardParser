using Glyphotype.Lowering;
using MTGRulesIR.Display;

namespace MTGRulesIR.Tests;

[Collection(nameof(GrammarCollection))]
public sealed class LoweringEngineTests(GrammarFixture fixture)
{
    readonly RulesLoweringEngine _engine = new(fixture.Lowering);

    LoweredDocument Lower(string text, string types = "Creature") =>
        _engine.Lower(new ProcessedDocument(new Card { Name = "Sample", Text = text, Types = types }, fixture.Grammar));

    [Fact]
    public void NodesCarryFamilyFieldsAndSpans()
    {
        var document = Lower("target player draws three cards", "Instant");

        var line = Assert.Single(document.Lines);
        Assert.True(line.IsFullyLowered);
        var spell = Assert.Single(line.Roots);
        Assert.Equal("SpellAbility", spell.Kind);
        Assert.Equal("Ability", spell.Family);
        Assert.Equal(LoweredNodeStatus.Lowered, spell.Status);
        Assert.Equal(new SourceSpan(0, line.Text.Length), spell.Span);

        var draw = Assert.Single(Assert.Single(spell.Fields).Nodes);
        Assert.Equal("Draw", draw.Kind);
        Assert.Equal("Effect", draw.Family);
        var count = Assert.Single(draw.Fields, x => x.Name == "Count").Nodes[0];
        Assert.Equal("Fixed", count.Kind);
        Assert.Equal("Quantity", count.Family);
        Assert.True(count.IsLeaf);
        Assert.Equal("3", Assert.Single(count.Fields).Value);
        // The draw came from the whole clause; a number from a vocabulary member has no capture of its own to place it by.
        Assert.Equal("target player draws three cards", line.Text.Substring(draw.Span.Value.Index, draw.Span.Value.Length));
    }

    [Fact]
    public void BindingsAndReferencesLinkUp()
    {
        var document = Lower("at the beginning of each player's upkeep, {this} deals 1 damage to that player", "Artifact");

        var nodes = document.Lines[0].Roots.SelectMany(x => x.Descendants()).ToList();
        var binding = Assert.Single(nodes, x => x.Binding is not null);
        Assert.Equal("PlayerSelector", binding.Kind);
        var reference = Assert.Single(nodes, x => x.Status == LoweredNodeStatus.Reference);
        Assert.Equal(binding.Binding, reference.RefersTo);
        Assert.Equal("that player", reference.Text);
        Assert.Equal(0, document.Lines[0].UnboundReferences);
    }

    [Fact]
    public void OpaqueNodesNameTheirGlyph()
    {
        var document = Lower("each creature that's attacking one of your opponents has trample");

        var line = Assert.Single(document.Lines);
        Assert.False(line.IsFullyLowered);
        var opaque = Assert.Single(line.Roots);
        Assert.Equal(LoweredNodeStatus.Opaque, opaque.Status);
        Assert.Equal("OviyaAutomechArtisanRule", opaque.Note);
        Assert.Equal(["OviyaAutomechArtisanRule"], line.OpaqueKinds);

        var summary = _engine.Summarize(new ProcessedDocument(new Card { Name = "Sample", Text = "each creature that's attacking one of your opponents has trample", Types = "Creature" }, fixture.Grammar));
        Assert.Equal(["OviyaAutomechArtisanRule"], summary.Lines[0].OpaqueKinds);
        Assert.Equal(["each creature that's attacking one of your opponents has trample"], summary.Lines[0].OpaqueTexts);
    }

    [Fact]
    public void FamiliesCoverEveryNode()
    {
        var document = Lower("{t}: add {g}.\nflying, trample\nwhenever a land enters, {this} deals 2 damage to that land's controller");

        foreach (var node in document.Lines.SelectMany(x => x.Roots).SelectMany(x => x.Descendants()))
            Assert.Contains(node.Family, _engine.Families);

        Assert.NotNull(document.Text);
        Assert.Contains("\"$kind\": \"ActivatedAbility\"", document.Text);
    }
}
