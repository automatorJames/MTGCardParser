namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"destroy all creatures blocking or blocked by it".</summary>
/// <exampledoc>Abu Ja'far</exampledoc>
/// <examplecapture>destroy all creatures blocking or blocked by it</examplecapture>
public class DestroyAll : Glyph
{
    public override Nib[] Nibs => ["destroy all", Prop(Destroyed)];

    public CreaturesBlockingOrBlockedBy Destroyed { get; set; }
}
