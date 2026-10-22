namespace MTGGlyphs.GlyphDefinitions;

/// <summary>When a draw being replaced happens: except a turn's first draw, while a state holds, or during a step.</summary>
/// <exampledoc>Alhammarret's Archive</exampledoc>
/// <examplecapture>except the first one you draw in each of your draw steps</examplecapture>
[Dependent]
public class DrawCircumstance : GlyphOneOf
{
    public ExceptFirstDraw ExceptFirstDraw { get; set; }
    public WhileStateHolds WhileStateHolds { get; set; }
    public DuringStep DuringStep { get; set; }
}
