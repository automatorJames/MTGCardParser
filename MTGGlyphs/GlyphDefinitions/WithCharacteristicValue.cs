namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"with mana value x", "with power 1", "with mana value 3": a qualifier fixing a characteristic at exactly a number.</summary>
/// <exampledoc>Disembowel</exampledoc>
/// <examplecapture>with mana value x</examplecapture>
[Dependent]
public class WithCharacteristicValue : Glyph
{
    public override Nib[] Nibs => ["with", Prop(Characteristic), Prop(Value)];

    public Characteristic Characteristic { get; set; }
    public Quantity Value { get; set; }
}
