namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Moving a card or permanent into a zone or a library position: "put {this} into its owner's graveyard", "put that card into your hand", "put it onto the battlefield", "put that card on the bottom"; also what a subject may do ("you may put it into your graveyard").</summary>
/// <exampledoc>Bronze Tablet</exampledoc>
/// <examplecapture>put {this} into its owner's graveyard</examplecapture>
public class PutIntoZone : Glyph, IPredicate
{
    public override Nib[] Nibs => ["put", Prop(Moved), Prop(Destination)];

    public OneOf<PermanentPhrase, ThatCard> Moved { get; set; }
    public OneOf<IntoZone, ToTheBattlefieldUnderControl, LibraryPlacement> Destination { get; set; }
}
