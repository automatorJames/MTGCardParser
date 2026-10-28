namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An object sharing a characteristic with another, as a state: "it has the same name as that card", "it has the same mana value as the revealed card", "it has the same name as a permanent".</summary>
/// <exampledoc>Jester's Scepter</exampledoc>
/// <examplecapture>it has the same name as that card</examplecapture>
[Dependent]
public class SharesCharacteristicWith : Glyph
{
    public override Nib[] Nibs => [Prop(Subject), "has the same", Prop(Characteristic), "as", Prop(Other)];

    public PermanentPhrase Subject { get; set; }
    public SharedCharacteristic Characteristic { get; set; }
    public OneOf<ThatCardPhrase, ThatCard, HandledObject, IndefinitePermanent> Other { get; set; }
}
