namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A choice of two costs, either of which pays: "sacrifice a nonland permanent of their choice or discard a card".</summary>
/// <exampledoc>Nicol Bolas, the Deceiver</exampledoc>
/// <examplecapture>sacrifices a nonland permanent of their choice or discards a card</examplecapture>
[Dependent]
public class CostEither : Glyph
{
    public override Nib[] Nibs => [Prop(First), "or", Prop(Second)];

    public Cost First { get; set; }
    public Cost Second { get; set; }
}
