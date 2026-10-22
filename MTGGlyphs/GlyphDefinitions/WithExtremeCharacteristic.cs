namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A superlative qualifier: "with the greatest power", "with the least toughness among creatures you control", "with the lowest mana value" - the object(s) whose characteristic is most extreme, optionally among a group.</summary>
/// <exampledoc>Topple</exampledoc>
/// <examplecapture>with the greatest power among creatures on the battlefield</examplecapture>
[Dependent]
public class WithExtremeCharacteristic : Glyph
{
    public override Nib[] Nibs => ["with the", Prop(Extreme), Prop(Characteristic), Prop(Among)];

    public Superlative Extreme { get; set; }
    public Characteristic Characteristic { get; set; }
    [Optional]
    public AmongGroup Among { get; set; }
}
