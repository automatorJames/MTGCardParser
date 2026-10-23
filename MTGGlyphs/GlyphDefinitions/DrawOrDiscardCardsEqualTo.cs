namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Drawing, discarding or milling a measured number of cards: "draw cards equal to the sacrificed creature's power", "draws cards equal to the number of cards in their hand", "discard cards equal to that creature's toughness", "discards a number of cards equal to …", "mills cards equal to its power". Milling is its own verb here so "mill two cards" stays a Mill.</summary>
/// <exampledoc>Life's Legacy</exampledoc>
/// <examplecapture>draw cards equal to the sacrificed creature's power</examplecapture>
public class DrawOrDiscardCardsEqualTo : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Prop(CardVerb), Opt("a number of"), "cards", Prop(Count)];

    public OneOf<CardVerb?, MillVerb?> CardVerb { get; set; }
    public EqualTo Count { get; set; }
}
