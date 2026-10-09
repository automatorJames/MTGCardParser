namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A change to power and toughness: "+1/+1", "-1/-0".</summary>
/// <exampledoc>Blessing</exampledoc>
/// <examplecapture>+1/+1</examplecapture>
[Dependent]
public class PowerToughnessMod : Glyph
{
    public override Nib[] Nibs => [Prop(PowerSign), Prop(PowerValue), "/", Prop(ToughnessSign), Prop(ToughnessValue)];

    public PlusMinus PowerSign { get; set; }
    public Quantity PowerValue { get; set; }
    public PlusMinus ToughnessSign { get; set; }
    public Quantity ToughnessValue { get; set; }
}