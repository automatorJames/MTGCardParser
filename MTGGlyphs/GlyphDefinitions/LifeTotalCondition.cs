namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player's life total, as a condition: "your life total is 5 or less", "your life total is less than 7", "your life total is greater than or equal to your starting life total".</summary>
/// <exampledoc>Path of Bravery</exampledoc>
/// <examplecapture>your life total is greater than or equal to your starting life total</examplecapture>
[Dependent]
public class LifeTotalCondition : Glyph
{
    public override Nib[] Nibs => [Prop(Total), "is", Prop(Bound)];

    public LifeTotalOf Total { get; set; }
    public OneOf<NumericBound, LifeComparison> Bound { get; set; }
}
