namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A number of power/toughness counters: "three +1/+1 counters".</summary>
/// <exampledoc>Tetravus</exampledoc>
/// <examplecapture>three +1/+1 counters</examplecapture>
public class PowerToughnessModCounters : Glyph
{
    public override Nib[] Nibs => [Prop(Quantity), Prop(PowerToughnessMod), Pattern("counter(s)?")];

    public Quantity Quantity { get; set; }
    public PowerToughnessMod PowerToughnessMod { get; set; }
}