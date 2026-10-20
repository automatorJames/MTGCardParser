namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Condition "{this} was kicked" / "it was kicked" / "this creature was kicked" / "this spell was kicked", for kicker-dependent effects.</summary>
/// <exampledoc>Caustic Tar</exampledoc>
/// <examplecapture>{this} was kicked</examplecapture>
[Dependent]
public class WasKicked : Glyph
{
    public override Nib[] Nibs => [Prop(Subject), "was kicked"];

    public OneOf<This, It, ThisObject> Subject { get; set; }
}
