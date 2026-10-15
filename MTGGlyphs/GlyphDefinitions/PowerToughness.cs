namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A creature's power and toughness written as a pair: "1/1", "0/1", "x/x".</summary>
/// <exampledoc>Elvish Farmer</exampledoc>
/// <examplecapture>1/1</examplecapture>
[Dependent]
public class PowerToughness : Glyph
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => [Prop(Power), "/", Prop(Toughness)];

    public Quantity Power { get; set; }
    public Quantity Toughness { get; set; }
}
