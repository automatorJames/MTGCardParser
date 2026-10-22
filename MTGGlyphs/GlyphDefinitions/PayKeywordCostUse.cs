namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Spending mana to pay a keyword ability's cost: "pay cumulative upkeep costs", "pay a disturb cost", "pay a morph cost".</summary>
/// <exampledoc>Adarkar Unicorn</exampledoc>
/// <examplecapture>pay cumulative upkeep costs</examplecapture>
[Dependent]
public class PayKeywordCostUse : Glyph
{
    public override Nib[] Nibs => [Opt("to"), "pay", Opt(Pattern("an?")), Prop(Keyword), Plural("cost")];

    public Keyword Keyword { get; set; }
}
