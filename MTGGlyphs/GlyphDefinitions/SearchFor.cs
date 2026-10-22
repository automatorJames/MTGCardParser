namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Searching a library (or a player's graveyard, hand, and library, for the extirpate effects) for cards, where what happens to them follows as a separate sentence or action: "search your library for up to two creature cards and reveal them", "search target player's library for three cards and exile them", "search that player's library for that many cards", "search your library for an island card, a swamp card, and a mountain card", "search its controller's graveyard, hand, and library for all cards with the same name as that spell and exile them". The whole tutor in one sentence is SearchLibrary.</summary>
/// <exampledoc>Jarad's Orders</exampledoc>
/// <examplecapture>search your library for up to two creature cards and reveal them</examplecapture>
public class SearchFor : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Alt("search", "searches"), Prop(Owner), Prop(Zones), "for", Prop(Wanted), Prop(FollowUp)];

    public OneOf<Whose?, PlayerPossessive> Owner { get; set; }
    public SearchedZones Zones { get; set; }
    public OneOf<ManyOf<WantedCards>, WantedCards> Wanted { get; set; }
    [Optional]
    public SearchFollowUp? FollowUp { get; set; }
}
