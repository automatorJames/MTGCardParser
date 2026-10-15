namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Returning a card or permanent to a player's zone: "return target creature card from your graveyard to your hand", "return target creature to its owner's hand". Also a predicate, so "you may return target creature to its owner's hand" resolves.</summary>
/// <exampledoc>Raise Dead</exampledoc>
/// <examplecapture>return target creature card from your graveyard to your hand</examplecapture>
public class ReturnCard : Glyph, IPredicate
{
    public override Nib[] Nibs => ["return", Prop(Returned), "to", Prop(Destination)];

    public OneOf<TargetCardInZone, PermanentPhrase, IndefinitePermanent> Returned { get; set; }
    public PlayersZone Destination { get; set; }
}
