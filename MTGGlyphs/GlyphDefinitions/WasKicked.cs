namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Condition "{this} was kicked" / "it was kicked" / "this creature was kicked" / "this spell was kicked", optionally naming which kicker: "it was kicked with its {2}{u} kicker".</summary>
/// <exampledoc>Caustic Tar</exampledoc>
/// <examplecapture>{this} was kicked</examplecapture>
[Dependent]
public class WasKicked : Glyph
{
    public override Nib[] Nibs => [Prop(Subject), "was kicked", Prop(Kicker)];

    public OneOf<This, It, ThisObject> Subject { get; set; }
    [Optional]
    public WithItsKicker Kicker { get; set; }
}
