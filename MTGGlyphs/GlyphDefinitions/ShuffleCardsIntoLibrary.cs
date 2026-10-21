namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Shuffling particular cards into a library: "shuffle {this} into its owner's library", "shuffle it into its owner's library", "shuffles up to three target cards from their graveyard into their library", "shuffle any number of cards from your hand into your library". Whole zones are ShuffleZoneIntoLibrary.</summary>
/// <exampledoc>Beacon of Destruction</exampledoc>
/// <examplecapture>shuffle {this} into its owner's library</examplecapture>
public class ShuffleCardsIntoLibrary : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Prop(Verb), Prop(Shuffled), "into", Prop(Library)];

    public ShuffleVerb Verb { get; set; }
    public MovedCard Shuffled { get; set; }
    public PlayersZone Library { get; set; }
}
