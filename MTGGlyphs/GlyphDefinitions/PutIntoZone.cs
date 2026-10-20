namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Moving a card or permanent into a zone or a library position: "put {this} into its owner's graveyard", "put that card into your hand", "put it onto the battlefield", "put that card on the bottom", "put target creature card from a graveyard onto the battlefield under your control", "put a land card from your hand onto the battlefield tapped", "put a creature card from among them into your hand"; also what a subject may do ("you may put it into your graveyard"), or the second of two actions ("reveal it and put it into your hand").</summary>
/// <exampledoc>Bronze Tablet</exampledoc>
/// <examplecapture>put {this} into its owner's graveyard</examplecapture>
public class PutIntoZone : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["put", Prop(Moved), Prop(Destination), Prop(Tapped)];

    public MovedCard Moved { get; set; }
    public OneOf<IntoZone, ToTheBattlefieldUnderControl, LibraryPlacement> Destination { get; set; }
    [Optional]
    [RegexPattern("tapped")]
    public bool? Tapped { get; set; }
}
