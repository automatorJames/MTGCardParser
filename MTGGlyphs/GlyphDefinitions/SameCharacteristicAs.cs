namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"with the same mana value as this card", "with the same name as that card": a card matched to another on one characteristic.</summary>
/// <exampledoc>Brainspoil</exampledoc>
/// <examplecapture>with the same mana value as this card</examplecapture>
[Dependent]
public class SameCharacteristicAs : Glyph
{
    public override Nib[] Nibs => ["with the same", Prop(Characteristic), "as", Prop(Other)];

    public Characteristic Characteristic { get; set; }
    public OneOf<ThisObject, PermanentPhrase> Other { get; set; }
}
