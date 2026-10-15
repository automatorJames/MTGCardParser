namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Whose spells and abilities: "your opponents control" after "spells or abilities".</summary>
/// <exampledoc>Ascended Lawmage</exampledoc>
/// <examplecapture>your opponents control</examplecapture>
[Dependent]
public class ControlledByWho : Glyph
{
    public override Nib[] Nibs => [Prop(Controller), "control"];

    public WhichPlayer Controller { get; set; }
}
