namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a destroy or exile effect removes when it isn't one particular permanent or a targeted card: a group ("all creatures", "up to two target creatures"), each of a kind ("each creature with mana value 3 or less", "each nonland permanent"), any one permanent of a kind ("a creature you control"), a number of permanents of a kind ("two permanents they control"), some of the cards or objects just named ("one of them", "any number of those cards"), any one card in a zone ("a card from your graveyard"), or a number of cards in a zone ("two cards from your graveyard").</summary>
/// <exampledoc>Wormfang Newt</exampledoc>
/// <examplecapture>a land you control</examplecapture>
[Dependent]
public class RemovedObjects : GlyphOneOf
{
    public PermanentGroup Group { get; set; }
    public EachPermanents EachPermanents { get; set; }
    public EachObject EachObject { get; set; }
    public IndefiniteCardInZone IndefiniteCard { get; set; }
    public CountedCardsInZone CountedCards { get; set; }
    public IndefinitePermanent IndefinitePermanent { get; set; }
    public CountedPermanents CountedPermanents { get; set; }
    public CountOfThem CountOfThem { get; set; }
}
