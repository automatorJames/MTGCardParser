namespace MTGGlyphs.GlyphDefinitions;

[RegexBoundaryOptionAtrribute(BoundaryOption.None)]
public class PowerToughnessMod : Glyph
{
    public override Nib[] Nibs => [Prop(PowerSign), Prop(PowerValue), "/", Prop(ToughnessSign), Prop(ToughnessValue)];

    public PlusMinus PowerSign { get; set; }
    public Quantity PowerValue { get; set; }
    public PlusMinus ToughnessSign { get; set; }
    public Quantity ToughnessValue { get; set; }
}