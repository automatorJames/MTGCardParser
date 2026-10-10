namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Moving a card or permanent into a zone: "put {this} into its owner's graveyard", "put that card into your hand", "put it onto the battlefield".</summary>
/// <exampledoc>Bronze Tablet</exampledoc>
/// <examplecapture>put {this} into its owner's graveyard</examplecapture>
public class PutIntoZone : Glyph
{
    public override Nib[] Nibs => ["put", Prop(Moved), Prop(Destination)];

    public OneOf<PermanentPhrase, ThatCard> Moved { get; set; }
    public OneOf<IntoZone, ToTheBattlefieldUnderControl> Destination { get; set; }
}
