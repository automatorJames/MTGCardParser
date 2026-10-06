namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An amount of life, e.g. as a cost: "10 life".</summary>
/// <exampledoc>Bronze Tablet</exampledoc>
/// <examplecapture>10 life</examplecapture>
public class LifeQuantity : Glyph
{
    public override Nib[] Nibs => [Prop(Quantity), "life"];

    public Quantity Quantity { get; set; }
}