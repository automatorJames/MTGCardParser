namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Drawing or discarding a measured number of cards: "draw cards equal to the sacrificed creature's power", "draws cards equal to the number of cards in their hand", "discard cards equal to that creature's toughness".</summary>
/// <exampledoc>Life's Legacy</exampledoc>
/// <examplecapture>draw cards equal to the sacrificed creature's power</examplecapture>
public class DrawOrDiscardCardsEqualTo : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Prop(CardVerb), "cards", Prop(Count)];

    public CardVerb CardVerb { get; set; }
    public EqualTo Count { get; set; }
}
