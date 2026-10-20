namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"the color of your choice", "the color or colors of your choice", "the creature type of your choice", "the basic land type of your choice": a characteristic the controller chooses as the effect applies.</summary>
/// <exampledoc>Caldera Kavu</exampledoc>
/// <examplecapture>the color of your choice</examplecapture>
[Dependent]
public class CharacteristicOfChoice : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Characteristic), "of your choice"];

    public ChoosableCharacteristic Characteristic { get; set; }
}
