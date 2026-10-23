namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"3/3 black": base power and toughness and a color, leading a description of what a permanent becomes.</summary>
/// <exampledoc>Corrupted Zendikon</exampledoc>
/// <examplecapture>3/3 black</examplecapture>
[Dependent]
public class ColoredStats : Glyph
{
    public override Nib[] Nibs => [Prop(Stats), Prop(Color)];

    public PowerToughness Stats { get; set; }
    public ManaColor Color { get; set; }
}
