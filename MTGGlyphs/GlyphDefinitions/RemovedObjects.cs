namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a destroy or exile effect removes when it isn't one particular permanent or a targeted card: several whole groups ("all lands or all creatures", "all blocking creatures and all blocked creatures"), cards picked from a group just revealed or looked at ("a nonland card from among them"), a group ("all creatures", "up to two target creatures"), each of a kind ("each creature with mana value 3 or less", "each nonland permanent"), any one permanent of a kind ("a creature you control"), a number of permanents of a kind ("two permanents they control"), some of the cards or objects just named ("one of them", "any number of those cards", "one of those creatures"), the two creatures just named ("both creatures"), any one card in a zone ("a card from your graveyard"), a number of cards in a zone ("two cards from your graveyard"), or whatever's left of those just named ("exile the rest"). The groups list goes first, since a single group would otherwise claim its first item.</summary>
/// <exampledoc>Wormfang Newt</exampledoc>
/// <examplecapture>a land you control</examplecapture>
[Dependent]
public class RemovedObjects : GlyphOneOf
{
    public ManyOf<AllPermanents> Groups { get; set; }
    public CardsFromAmong CardsFromAmong { get; set; }
    public PermanentGroup Group { get; set; }
    public EachPermanents EachPermanents { get; set; }
    public EachObject EachObject { get; set; }
    public IndefiniteCardInZone IndefiniteCard { get; set; }
    public CountedCardsInZone CountedCards { get; set; }
    public IndefinitePermanent IndefinitePermanent { get; set; }
    public CountedPermanents CountedPermanents { get; set; }
    public CountOfThem CountOfThem { get; set; }
    public PairedCreatures PairedCreatures { get; set; }
    public TheRest TheRest { get; set; }
}
