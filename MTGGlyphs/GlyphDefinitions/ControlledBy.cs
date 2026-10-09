namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Who controls something: "you control", "they control", "an opponent controls".</summary>
/// <exampledoc>Magnetic Mountain</exampledoc>
/// <examplecapture>they control</examplecapture>
[Dependent]
public class ControlledBy : Glyph
{
    public override Nib[] Nibs => [Prop(Controller), Pattern("controls?")];

    public OneOf<TheyPlayer, WhichPlayer?> Controller { get; set; }
}
