namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"with power 2 or less", "with toughness 3 or greater": a qualifier bounding a characteristic.</summary>
/// <exampledoc>Dwarven Warriors</exampledoc>
/// <examplecapture>with power 2 or less</examplecapture>
[Dependent]
public class WithCharacteristicBound : Glyph
{
    public override Nib[] Nibs => ["with", Prop(Characteristic), Prop(Quantity), "or", Prop(Comparison)];

    public Characteristic Characteristic { get; set; }
    public Quantity Quantity { get; set; }
    public Comparison Comparison { get; set; }
}
