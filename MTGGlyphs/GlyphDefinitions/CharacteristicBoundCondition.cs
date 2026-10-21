namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An object's power or toughness reaching a bound, as a condition: "this creature's power is 4 or greater", "that creature's power is 0 or less".</summary>
/// <exampledoc>Bloodshot Trainee</exampledoc>
/// <examplecapture>this creature's power is 4 or greater</examplecapture>
[Dependent]
public class CharacteristicBoundCondition : Glyph
{
    public override Nib[] Nibs => [Prop(Object), Prop(Characteristic), "is", Prop(Quantity), "or", Prop(Comparison)];

    public ObjectPossessive Object { get; set; }
    public Characteristic Characteristic { get; set; }
    public Quantity Quantity { get; set; }
    public Comparison Comparison { get; set; }
}
