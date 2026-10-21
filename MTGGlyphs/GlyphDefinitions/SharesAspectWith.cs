namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"that share a color with it", "that shares a creature type with it": objects narrowed to those sharing an aspect with a referenced object (intimidate, radiance).</summary>
/// <exampledoc>Grisly Transformation</exampledoc>
/// <examplecapture>that share a color with it</examplecapture>
[Dependent]
public class SharesAspectWith : Glyph
{
    public override Nib[] Nibs => ["that", Pattern("shares?"), Prop(Aspect), "with", Prop(Other)];

    public SharedAspect Aspect { get; set; }
    public It Other { get; set; }
}
