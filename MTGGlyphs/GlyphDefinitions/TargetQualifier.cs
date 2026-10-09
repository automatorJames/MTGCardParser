namespace MTGGlyphs.GlyphDefinitions;

/// <summary>One word narrowing what a card can be: "black", "artifact", "tapped", "attacking", or negated, "nonblack", "nonartifact".</summary>
/// <exampledoc>Ashnod's Transmogrant</exampledoc>
/// <examplecapture>nonartifact</examplecapture>
[Dependent]
public class TargetQualifier : Glyph
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => [Prop(IsNegated), Prop(Quality)];

    [RegexPattern("non")]
    public bool IsNegated { get; set; }
    public Quality Quality { get; set; }
}
