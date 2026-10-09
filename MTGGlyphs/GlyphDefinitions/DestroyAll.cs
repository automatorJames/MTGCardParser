namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"destroy all lands", "destroy all creatures blocking or blocked by it": destroys every permanent of a kind.</summary>
/// <exampledoc>Armageddon</exampledoc>
/// <examplecapture>destroy all lands</examplecapture>
public class DestroyAll : Glyph
{
    public override Nib[] Nibs => ["destroy all", Prop(Destroyed)];

    public OneOf<CreaturesBlockingOrBlockedBy, Permanents> Destroyed { get; set; }
}
