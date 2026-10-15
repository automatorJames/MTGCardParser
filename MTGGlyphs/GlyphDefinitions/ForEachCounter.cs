namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A multiplier by counters on an object: "for each age counter on it", "for each wind counter on it".</summary>
/// <exampledoc>Cyclone</exampledoc>
/// <examplecapture>for each wind counter on it</examplecapture>
[Dependent]
public class ForEachCounter : Glyph
{
    public override Nib[] Nibs => ["for each", Prop(Counters)];

    public CountersOn Counters { get; set; }
}
