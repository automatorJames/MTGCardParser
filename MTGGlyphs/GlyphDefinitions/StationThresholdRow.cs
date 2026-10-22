namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A station threshold row on a spacecraft: "7+ | flying" - what the permanent has once it has at least that many charge counters. The ability after the bar is any glyph that reads it.</summary>
/// <exampledoc>Sledge-Class Seedship</exampledoc>
/// <examplecapture>7+ | flying</examplecapture>
public class StationThresholdRow : Glyph
{
    public override Nib[] Nibs => [Prop(Threshold), "|", Prop(Ability)];

    public CounterThreshold Threshold { get; set; }
    public DynamicGlyph Ability { get; set; }
}
