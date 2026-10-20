namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"with greater power", "with lesser power": a characteristic compared with the creature that has the ability (skulk, mentor, training).</summary>
/// <exampledoc>Farbog Revenant</exampledoc>
/// <examplecapture>with greater power</examplecapture>
[Dependent]
public class WithRelativeCharacteristic : Glyph
{
    public override Nib[] Nibs => ["with", Prop(Comparison), Prop(Characteristic)];

    public Comparison Comparison { get; set; }
    public Characteristic Characteristic { get; set; }
}
