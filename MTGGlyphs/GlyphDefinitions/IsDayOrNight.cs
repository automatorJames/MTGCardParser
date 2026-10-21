namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Whether it's day or night, as a condition: "it's night", "it's neither day nor night".</summary>
/// <exampledoc>The Celestus</exampledoc>
/// <examplecapture>it's night</examplecapture>
[Dependent]
public class IsDayOrNight : Glyph
{
    public override Nib[] Nibs => ["it's", Prop(State)];

    public DayNightState State { get; set; }
}
