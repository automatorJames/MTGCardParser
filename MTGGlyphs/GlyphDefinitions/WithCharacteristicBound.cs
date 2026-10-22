namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"with power 2 or less", "with toughness 3 or greater", "with total power 4 or more" (teamwork, crew-like costs), "with power or toughness 1 or less" (either characteristic): a qualifier bounding a characteristic, of each permanent or of the group in total.</summary>
/// <exampledoc>Dwarven Warriors</exampledoc>
/// <examplecapture>with power 2 or less</examplecapture>
[Dependent]
public class WithCharacteristicBound : Glyph
{
    public override Nib[] Nibs => ["with", Prop(Total), Prop(Characteristic), Prop(Quantity), "or", Prop(Comparison)];

    [RegexPattern("total")]
    public bool Total { get; set; }
    public OneOf<ManyOf<Characteristic>, Characteristic?> Characteristic { get; set; }
    public Quantity Quantity { get; set; }
    public Comparison Comparison { get; set; }
}
