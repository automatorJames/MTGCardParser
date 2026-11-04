namespace MTGRulesIR.Tests;

/// <summary>The engine-closed measure: open slots, placeholders, and the census, on trees built by hand.</summary>
public sealed class ClosureTests
{
    static LineRules Line(params Ability[] abilities)
    {
        var opaque = abilities.SelectMany(OpaqueNodes.In).Select(x => x.Glyph).ToList();
        var open = abilities.SelectMany(OpenSlots.In).ToList();
        return new LineRules(0, "sample", abilities, opaque, open);
    }

    [Fact]
    public void TypedTreeIsEngineClosed()
    {
        var line = Line(new SpellAbility(new Draw(new PlayerSelector(PlayerKind.You), new Fixed(2))));

        Assert.True(line.FullyLowered);
        Assert.True(line.EngineClosed);
        Assert.Empty(line.Open);
    }

    [Fact]
    public void UntypedSlotHoldingANodeIsOpen()
    {
        var pump = new Pump(new Self(), OtherBuffs: [new Term("Hexproof", "Keyword")]);
        var line = Line(new StaticAbility(pump));

        Assert.True(line.FullyLowered);
        Assert.False(line.EngineClosed);
        Assert.Equal(["Pump.OtherBuffs"], line.Open);
    }

    [Fact]
    public void PlaceholderIsOpen()
    {
        var line = Line(new SpellAbility(new ChangeZone(new Self(), new ZoneRef(OpenSlots.Placeholder))));

        Assert.False(line.EngineClosed);
        Assert.Equal(["ZoneRef.Zone=?"], line.Open);
    }

    [Fact]
    public void EmptyUntypedListIsNotOpen()
    {
        var line = Line(new StaticAbility(new Pump(new Self(), OtherBuffs: [])));

        Assert.True(line.EngineClosed);
    }

    [Fact]
    public void OpaqueLineIsNotEngineClosed()
    {
        var line = Line(new OpaqueAbility("some text", "SomeGlyph"));

        Assert.False(line.FullyLowered);
        Assert.False(line.EngineClosed);
    }

    [Fact]
    public void ReportCountsOpenSlotsOnlyInFullyLoweredLines()
    {
        var card = new CardRules("Sample", "Instant",
        [
            Line(new SpellAbility(new Draw(new PlayerSelector(PlayerKind.You), new Fixed(1)))),
            Line(new StaticAbility(new Pump(new Self(), OtherBuffs: [new Term("Hexproof", "Keyword")]))),
            Line(new StaticAbility(new Pump(new Self(), OtherBuffs: [new OpaqueNode("odd text", "OddGlyph")]))),
        ]);

        var report = new LoweringReport();
        report.Add(card);

        Assert.Equal(3, report.Lines);
        Assert.Equal(2, report.LinesFullyLowered);
        Assert.Equal(1, report.LinesEngineClosed);
        Assert.Equal(0, report.CardsEngineClosed);
        var text = report.ToText();
        Assert.Contains("      1 lines       1x  Pump.OtherBuffs", text);
        Assert.Contains("      1 lines       1x  OddGlyph", text);
    }

    [Fact]
    public void CensusCountsStringSlotsByLine()
    {
        var card = new CardRules("Sample", "Creature",
        [
            Line(new KeywordLine([new KeywordInstance("Flying"), new KeywordInstance("Flying")])),
            Line(new KeywordLine([new KeywordInstance("Trample")])),
            Line(new OpaqueAbility("free text stays out", "Glyph")),
        ]);

        var census = new VocabularyCensus();
        census.Add(card);

        var keywords = census.Slots["KeywordInstance.Keyword"];
        Assert.Equal(1, keywords["Flying"]);
        Assert.Equal(1, keywords["Trample"]);
        Assert.DoesNotContain("OpaqueAbility.Text", census.Slots.Keys);
        Assert.DoesNotContain("OpaqueAbility.Glyph", census.Slots.Keys);
    }
}
