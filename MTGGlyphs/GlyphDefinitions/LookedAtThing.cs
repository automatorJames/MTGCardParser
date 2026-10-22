namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What gets looked at: a hand, the top of a library (or a counted run from it, "that many cards from the top of your library"), or cards this permanent exiled ("cards exiled with this creature").</summary>
/// <exampledoc>Aven Fateshaper</exampledoc>
/// <examplecapture>the top four cards of your library</examplecapture>
[Dependent]
public class LookedAtThing : GlyphOneOf
{
    public LookedAtHand Hand { get; set; }
    public LookedAtTop Top { get; set; }
    public LookedAtTopCard TopCard { get; set; }
    public CardsFromTopOfLibrary FromTop { get; set; }
    public CardsExiledWithThis ExiledCards { get; set; }
}
