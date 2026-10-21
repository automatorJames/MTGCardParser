namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"with power less than this creature's power", "with power less than or equal to that creature's power": a characteristic compared with the same characteristic of another object.</summary>
/// <exampledoc>Aura Gnarlid</exampledoc>
/// <examplecapture>with power less than this creature's power</examplecapture>
[Dependent]
public class WithCharacteristicComparedTo : Glyph
{
    public override Nib[] Nibs => ["with", Prop(Characteristic), Prop(Comparison), "than", Prop(OrEqual), Prop(Object), Prop(OtherCharacteristic)];

    public Characteristic Characteristic { get; set; }
    public Comparison Comparison { get; set; }
    [RegexPattern("or equal to")]
    public bool OrEqual { get; set; }
    public OneOf<Its, ObjectPossessive> Object { get; set; }
    public Characteristic OtherCharacteristic { get; set; }
}
