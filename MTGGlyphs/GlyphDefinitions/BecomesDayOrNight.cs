namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The game becoming day or night, optionally as a permanent enters: "it becomes night", "it becomes day as this creature enters".</summary>
/// <exampledoc>Sunrise Cavalier</exampledoc>
/// <examplecapture>it becomes day as this creature enters</examplecapture>
public class BecomesDayOrNight : Glyph
{
    public override Nib[] Nibs => ["it becomes", Prop(State), Prop(Moment)];

    public DayNightState State { get; set; }
    [Optional]
    public AsPermanentMoment Moment { get; set; }
}
