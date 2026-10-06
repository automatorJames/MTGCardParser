namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A counter of a named kind: "doom counter".</summary>
/// <exampledoc>Armageddon Clock</exampledoc>
/// <examplecapture>doom counter</examplecapture>
public class CounterOnCard : Glyph
{
    public override Nib[] Nibs => [Prop(CounterType), "counter"];

    public CounterType CounterType { get; set; }
}