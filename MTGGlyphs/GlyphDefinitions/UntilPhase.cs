namespace MTGGlyphs.GlyphDefinitions;

/// <summary>How long an effect lasts: "until end of turn", "until your next upkeep", "until your next end step", "until its controller's next untap step".</summary>
/// <exampledoc>Coral Helm</exampledoc>
/// <examplecapture>until end of turn</examplecapture>
[Dependent]
public class UntilPhase : Glyph
{
    public override Nib[] Nibs => ["until", Prop(Next), Prop(Phase)];

    [Optional]
    public NextPhaseOf? Next { get; set; }
    public Phase Phase { get; set; }
}
