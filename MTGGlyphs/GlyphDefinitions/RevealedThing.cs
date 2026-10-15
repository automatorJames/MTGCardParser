namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What gets revealed: a hand, the top of a library, or a card from a hand.</summary>
/// <exampledoc>Distress</exampledoc>
/// <examplecapture>their hand</examplecapture>
[Dependent]
public class RevealedThing : GlyphOneOf
{
    public RevealedHand Hand { get; set; }
    public RevealedTop Top { get; set; }
    public RevealedTopCard TopCard { get; set; }
    public RevealedFromHand FromHand { get; set; }
}
