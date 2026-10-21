namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"plus 1", "minus 1": adjusting an amount just mentioned.</summary>
/// <exampledoc>Angel of Vitality</exampledoc>
/// <examplecapture>plus 1</examplecapture>
[Dependent]
public class AmountAdjustment : Glyph
{
    public override Nib[] Nibs => [Alt("plus", "minus"), Prop(Amount)];

    public Quantity Amount { get; set; }
}
