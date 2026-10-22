namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"and from red", "or from the color of your choice": a second quality protection is from, either both ("and") or a choice ("or").</summary>
/// <exampledoc>Auriok Champion</exampledoc>
/// <examplecapture>and from red</examplecapture>
[Dependent]
public class AndFromQuality : Glyph
{
    public override Nib[] Nibs => [Prop(Conjunction), "from", Prop(Quality)];

    public ProtectionConjunction Conjunction { get; set; }
    public ProtectionQuality Quality { get; set; }
}
