namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A point in a player's turn: "at the beginning of your upkeep", "at the beginning of each player's upkeep", "at the beginning of the upkeep of enchanted land's controller", "until the end of your turn".</summary>
/// <exampledoc>Juzám Djinn</exampledoc>
/// <examplecapture>at the beginning of your upkeep</examplecapture>
[Dependent]
public class AtOrUntilPlayerPhase : Glyph
{
    public override Nib[] Nibs => [Prop(TemporalDisposition), "the", Prop(PhasePart), "of", Prop(Step)];

    public TemporalDisposition TemporalDisposition { get; set; }
    public PhasePart PhasePart { get; set; }
    public OneOf<PlayersPhase, PhaseOfPlayer> Step { get; set; }
}
