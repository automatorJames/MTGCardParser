namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The damage a source dealt beyond what was lethal, as an amount: "that excess damage", "the excess damage dealt this way", "the amount of excess damage dealt to that creature this way".</summary>
/// <exampledoc>Goblin Negotiation</exampledoc>
/// <examplecapture>the amount of excess damage dealt to that creature this way</examplecapture>
[Dependent]
public class ExcessDamageAmount : Glyph
{
    public override Nib[] Nibs => [Prop(Measure), "excess damage", Prop(Dealt)];

    public ExcessMeasure Measure { get; set; }
    [Optional]
    public ExcessDealt Dealt { get; set; }
}
