namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A point in a player's turn: "at the beginning of your upkeep", "until the end of your turn".</summary>
/// <exampledoc>Juzám Djinn</exampledoc>
/// <examplecapture>at the beginning of your upkeep</examplecapture>
[Dependent]
public class AtOrUntilPlayerPhase : Glyph
{
    public override Nib[] Nibs => [Prop(TemporalDisposition), "the", Prop(PhasePart), "of", Prop(Whose), Prop(Phase)];

    public TemporalDisposition TemporalDisposition { get; set; }
    public PhasePart PhasePart { get; set; }
    public Whose Whose { get; set; }
    public Phase Phase { get; set; }
}