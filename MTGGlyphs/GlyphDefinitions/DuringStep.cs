namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"during your draw step": the step an event happens in.</summary>
/// <exampledoc>Island Sanctuary</exampledoc>
/// <examplecapture>during your draw step</examplecapture>
[Dependent]
public class DuringStep : Glyph
{
    public override Nib[] Nibs => ["during", Prop(Step)];

    public PlayersPhase Step { get; set; }
}
