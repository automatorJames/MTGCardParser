namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A number of cards in hand, as a threshold: "no cards in hand", "a card in hand", "seven or more cards in hand", "exactly seven cards in hand", "one or fewer cards in hand", "exactly thirteen cards in your hand", "no land cards in hand" (of one card type).</summary>
/// <exampledoc>Library of Alexandria</exampledoc>
/// <examplecapture>exactly seven cards in hand</examplecapture>
[Dependent]
public class CardsInHandCount : Glyph
{
    public override Nib[] Nibs => [Prop(Bound), Prop(Count), Prop(Comparison), Prop(Kind), Plural("card"), "in", Opt(Pattern("(your|their)")), "hand"];

    [Optional]
    public HandCountBound? Bound { get; set; }
    public Quantity Count { get; set; }
    [Optional]
    public CountComparison? Comparison { get; set; }
    [Optional]
    public CardType? Kind { get; set; }
}
