namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"who controls the most creatures": the player controlling the most permanents of a kind.</summary>
/// <exampledoc>Thoughtbound Primoc</exampledoc>
/// <examplecapture>who controls the most wizards</examplecapture>
[Dependent]
public class ControlsTheMost : Glyph
{
    public override Nib[] Nibs => ["who controls the most", Prop(Kind)];

    public Permanents Kind { get; set; }
}
