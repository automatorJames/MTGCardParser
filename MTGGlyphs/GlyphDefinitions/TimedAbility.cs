namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An effect introduced by a point in the turn: "at the beginning of your upkeep, ...", "until end of turn, ...".</summary>
public class TimedAbility : Glyph
{
    public override Nib[] Nibs => [Prop(Timing), ",", Prop(Effect)];

    public PhaseTiming Timing { get; set; }

    [AllowUnmatched]
    public DynamicGlyph Effect { get; set; }
}
