namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Who controls something, or doesn't: "you control", "they control", "an opponent controls", "you don't control", "that player controls", "that creature's controller controls".</summary>
/// <exampledoc>Magnetic Mountain</exampledoc>
/// <examplecapture>they control</examplecapture>
[Dependent]
public class ControlledBy : Glyph
{
    public override Nib[] Nibs => [Prop(Controller), Prop(Negated), Pattern("controls?")];

    public OneOf<TheyPlayer, WhichPlayer?, PlayerReference> Controller { get; set; }
    [RegexPattern("don't|doesn't")]
    public bool Negated { get; set; }
}
