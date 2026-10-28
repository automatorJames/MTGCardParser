namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a "put" effect moves: a particular permanent, a card named earlier, a targeted card in a zone ("target creature card from a graveyard"), any card of a kind in a zone ("a land card from your hand"), a number of cards in a zone ("two cards from your hand"), cards from outside the game ("a card you own from outside the game"), cards picked out of a group ("a creature card from among them", "a creature card and/or a land card from among them", "one of them", "all creature cards revealed this way", "the revealed cards", "them"), the top of a library ("the top two cards of your library"), all permanents of a kind ("all creatures"), a number of targets ("up to two target creatures"), or, last, any one permanent of a kind ("a creature you control").</summary>
/// <exampledoc>Endless Obedience</exampledoc>
/// <examplecapture>target creature card from a graveyard</examplecapture>
[Dependent]
public class MovedCard : GlyphOneOf
{
    public TargetCardInZone TargetCardInZone { get; set; }
    public IndefiniteCardInZone IndefiniteCardInZone { get; set; }
    public CountedCardsInZone CountedCardsInZone { get; set; }
    public CardsFromOutsideGame CardsFromOutsideGame { get; set; }
    public CardOfEachKindFromAmong CardOfEachKindFromAmong { get; set; }
    public CardsFromAmong CardsFromAmong { get; set; }
    public CountOfThem CountOfThem { get; set; }
    public CardsRevealedThisWay Revealed { get; set; }
    public TopCardsOfLibrary TopCards { get; set; }
    public TopCardOfLibrary TopCard { get; set; }
    public AllPermanents AllPermanents { get; set; }
    public PermanentPhrase PermanentPhrase { get; set; }
    public SacrificedPermanent Handled { get; set; }
    public ThatCard ThatCard { get; set; }
    public Them Them { get; set; }
    public CountedTargets CountedTargets { get; set; }
    public IndefinitePermanent IndefinitePermanent { get; set; }
}
