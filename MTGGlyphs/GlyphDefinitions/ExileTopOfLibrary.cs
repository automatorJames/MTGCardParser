namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Exiling cards from the top of a library, often to play them later (impulse draw): "exile the top card of your library", "exile the top two cards of your library", "exile the top three cards of target opponent's library", "exile that many cards from the top of your library", "… face down"; also the top card of a graveyard ("exile the top creature card of your graveyard"), mostly as a cost. Also a predicate after a subject, "that player exiles the top card of their library" (ingest), and the second of two actions.</summary>
/// <exampledoc>Alania's Pathmaker</exampledoc>
/// <examplecapture>exile the top card of your library</examplecapture>
public class ExileTopOfLibrary : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Pattern("exiles?"), Prop(What), Prop(FaceDown)];

    public OneOf<RevealedTopCard, LookedAtTopCard, OneOf<RevealedTop, CardsFromTopOfLibrary, TopCardOfGraveyard>> What { get; set; }
    [RegexPattern("face down")]
    public bool FaceDown { get; set; }
}
