namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"with a basic land type", "without a basic land type": a qualifier on lands or land cards having (or lacking) any basic land type, as domain-style searches ask for.</summary>
/// <exampledoc>Perilous Forays</exampledoc>
/// <examplecapture>with a basic land type</examplecapture>
[Dependent]
public class WithBasicLandType : Glyph
{
    public override Nib[] Nibs => [Prop(Having), "a basic land type"];

    public Having Having { get; set; }
}
