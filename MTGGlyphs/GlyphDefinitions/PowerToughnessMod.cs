namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A power/toughness change: "+1/+1", "-2/-0", "+x/+0".</summary>
[Dependent]
public class PowerToughnessMod : Glyph
{
    public override Nib[] Nibs => [Prop(PowerSign), Prop(PowerValue), "/", Prop(ToughnessSign), Prop(ToughnessValue)];
    public override Joiner Joiner => Joiner.None;

    public PlusMinus PowerSign { get; set; }
    public Quantity PowerValue { get; set; }
    public PlusMinus ToughnessSign { get; set; }
    public Quantity ToughnessValue { get; set; }
}
