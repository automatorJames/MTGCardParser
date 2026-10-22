namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What gets revealed: a hand, the top of a library (or a counted run from it), a card from a hand, a card from outside the game, or one particular card named already ("reveal it", "reveal {this}").</summary>
/// <exampledoc>Distress</exampledoc>
/// <examplecapture>their hand</examplecapture>
[Dependent]
public class RevealedThing : GlyphOneOf
{
    public RevealedHand Hand { get; set; }
    public RevealedTop Top { get; set; }
    public RevealedTopCard TopCard { get; set; }
    public CardsFromTopOfLibrary FromTop { get; set; }
    public RevealedFromHand FromHand { get; set; }
    public CardsFromOutsideGame FromOutside { get; set; }
    public PermanentPhrase Card { get; set; }
}
