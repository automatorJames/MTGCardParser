namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Which of the cards just looked at or revealed are picked out: "one of them", "two of those cards", "any number of them", "all creature cards revealed this way", "a land card from among them", "the chosen cards", "one pile".</summary>
/// <exampledoc>Impulse</exampledoc>
/// <examplecapture>one of them</examplecapture>
[Dependent]
public class ChosenLookedAtCards : GlyphOneOf
{
    public CountOfThem Count { get; set; }
    public CardsRevealedThisWay Revealed { get; set; }
    public CardsFromAmong FromAmong { get; set; }
    public TheChosenCards Chosen { get; set; }
    public OneOfTwoPiles Pile { get; set; }
}
