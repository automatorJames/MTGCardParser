namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An amount of life, as paid or lost: "2 life".</summary>
/// <exampledoc>Gitaxian Probe</exampledoc>
/// <examplecapture>2 life</examplecapture>
[Dependent]
public class LifeAmount : Glyph
{
    public override Nib[] Nibs => [Prop(Amount), "life"];

    public Quantity Amount { get; set; }
}
