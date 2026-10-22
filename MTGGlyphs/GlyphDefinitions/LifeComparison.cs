namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"less than 7", "greater than or equal to your starting life total", "less than or equal to half your starting life total": a life total compared with a number or a starting life total.</summary>
/// <exampledoc>Path of Bravery</exampledoc>
/// <examplecapture>greater than or equal to your starting life total</examplecapture>
[Dependent]
public class LifeComparison : Glyph
{
    public override Nib[] Nibs => [Prop(Comparison), "than", Prop(OrEqual), Prop(Than)];

    public Comparison Comparison { get; set; }
    [RegexPattern("or equal to")]
    public bool OrEqual { get; set; }
    public OneOf<StartingLifeTotal, Quantity?> Than { get; set; }
}
