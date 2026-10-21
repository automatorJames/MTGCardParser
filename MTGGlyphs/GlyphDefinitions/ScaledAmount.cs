namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A halved amount with its rounding: "half the number of zombies you control, rounded down". The comma is a literal here, since a nested glyph can't open with one.</summary>
/// <exampledoc>Endless Ranks of the Dead</exampledoc>
/// <examplecapture>half the number of zombies you control, rounded down</examplecapture>
[Dependent]
public class ScaledAmount : Glyph
{
    public override Nib[] Nibs => [Prop(Factor), Prop(Base), ",", Prop(Rounded)];

    public ScaleFactor Factor { get; set; }
    public Amount Base { get; set; }
    public Rounding Rounded { get; set; }
}
