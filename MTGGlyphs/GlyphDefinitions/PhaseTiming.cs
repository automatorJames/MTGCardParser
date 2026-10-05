namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A point or span in the turn: "until end of turn", "at the beginning of your upkeep", "during your upkeep".</summary>
[Dependent]
public class PhaseTiming : Glyph
{
    public override Nib[] Nibs => [Prop(TemporalDisposition), Prop(PhaseBoundary), Prop(Step)];

    public TemporalDisposition TemporalDisposition { get; set; }

    [Optional]
    public PhaseBoundary PhaseBoundary { get; set; }

    public OneOf<PlayersPhase, Phase?> Step { get; set; }
}

/// <summary>"the beginning of", "the end of".</summary>
[Dependent]
public class PhaseBoundary : Glyph
{
    public override Nib[] Nibs => ["the", Prop(PhasePart), "of"];

    public PhasePart PhasePart { get; set; }
}

/// <summary>A phase or step of some player's turn: "your upkeep", "each player's upkeep", "the next end step".</summary>
[Dependent]
public class PlayersPhase : Glyph
{
    public override Nib[] Nibs => [Prop(Whose), Prop(Phase)];

    public Whose Whose { get; set; }
    public Phase Phase { get; set; }
}
