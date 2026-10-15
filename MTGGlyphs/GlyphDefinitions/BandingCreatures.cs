namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The creatures banding's damage rule applies to: "any creatures with banding you control".</summary>
/// <exampledoc>Benalish Hero</exampledoc>
/// <examplecapture>any creatures with banding you control</examplecapture>
[Dependent]
public class BandingCreatures : Glyph
{
    public override Nib[] Nibs => ["any creatures with banding", Prop(Controller)];

    public ControlledBy Controller { get; set; }
}
