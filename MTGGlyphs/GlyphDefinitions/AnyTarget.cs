namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"any target", "any other target": a target that can be a player or a permanent (other than one already chosen, or than the source).</summary>
/// <exampledoc>Aladdin's Ring</exampledoc>
/// <examplecapture>any target</examplecapture>
[Dependent]
public class AnyTarget : Glyph
{
    public override Nib[] Nibs => ["any", Prop(Other), "target"];

    [RegexPattern("other")]
    public bool Other { get; set; }
}
