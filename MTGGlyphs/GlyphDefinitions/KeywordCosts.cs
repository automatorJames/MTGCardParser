namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A keyword's costs, as what a cost change applies to: "flashback costs you pay", "flashback costs your opponents pay", "buyback costs", "all morph costs", "equip costs you pay".</summary>
/// <exampledoc>Catalyst Stone</exampledoc>
/// <examplecapture>flashback costs you pay</examplecapture>
[Dependent]
public class KeywordCosts : Glyph
{
    public override Nib[] Nibs => [Opt("all"), Prop(Keyword), "costs", Prop(Payer)];

    public Keyword Keyword { get; set; }
    [Optional]
    public CostPayer? Payer { get; set; }
}
