namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A group of permanents' combined power or toughness reaching a bound, as a condition: "creatures you control have total power 8 or greater" (formidable).</summary>
/// <exampledoc>Atarka Beastbreaker</exampledoc>
/// <examplecapture>creatures you control have total power 8 or greater</examplecapture>
[Dependent]
public class TotalCharacteristicCondition : Glyph
{
    public override Nib[] Nibs => [Prop(Group), "have total", Prop(Characteristic), Prop(Quantity), "or", Prop(Comparison)];

    public Permanents Group { get; set; }
    public Characteristic Characteristic { get; set; }
    public Quantity Quantity { get; set; }
    public Comparison Comparison { get; set; }
}
