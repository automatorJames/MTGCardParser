namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"and from red": a second quality protection is from.</summary>
/// <exampledoc>Auriok Champion</exampledoc>
/// <examplecapture>and from red</examplecapture>
[Dependent]
public class AndFromQuality : Glyph
{
    public override Nib[] Nibs => ["and from", Prop(Quality)];

    public ProtectionQuality Quality { get; set; }
}
