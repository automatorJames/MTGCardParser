namespace MTGGlyphs;

public class PowerToughnessModCounters : Glyph
{
    public override Nib[] Nibs => [Prop(Quantity), Prop(PowerToughnessMod), "counter(s)?"];

    public Quantity Quantity { get; set; }
    public PowerToughnessMod PowerToughnessMod { get; set; }
}