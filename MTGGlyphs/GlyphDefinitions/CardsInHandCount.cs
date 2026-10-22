namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A number of cards in hand, as a threshold: "no cards in hand", "seven or more cards in hand", "exactly seven cards in hand", "one or fewer cards in hand", "fewer than seven cards in hand".</summary>
/// <exampledoc>Library of Alexandria</exampledoc>
/// <examplecapture>exactly seven cards in hand</examplecapture>
[Dependent]
public class CardsInHandCount : Glyph
{
    public override Nib[] Nibs => [Prop(Bound), Prop(Count), Prop(Comparison), "cards in hand"];

    [Optional]
    public HandCountBound? Bound { get; set; }
    public Quantity Count { get; set; }
    [Optional]
    public CountComparison? Comparison { get; set; }
}
