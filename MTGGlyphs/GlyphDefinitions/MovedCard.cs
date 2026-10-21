namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a "put" effect moves: a particular permanent, a card named earlier, a targeted card in a zone ("target creature card from a graveyard"), any card of a kind in a zone ("a land card from your hand"), a number of cards in a zone ("two cards from your hand"), cards picked out of a group ("a creature card from among them"), or the top of a library ("the top two cards of your library").</summary>
/// <exampledoc>Endless Obedience</exampledoc>
/// <examplecapture>target creature card from a graveyard</examplecapture>
[Dependent]
public class MovedCard : GlyphOneOf
{
    public TargetCardInZone TargetCardInZone { get; set; }
    public IndefiniteCardInZone IndefiniteCardInZone { get; set; }
    public CountedCardsInZone CountedCardsInZone { get; set; }
    public CardsFromAmong CardsFromAmong { get; set; }
    public LookedAtTop TopCards { get; set; }
    public LookedAtTopCard TopCard { get; set; }
    public PermanentPhrase PermanentPhrase { get; set; }
    public ThatCard ThatCard { get; set; }
}
