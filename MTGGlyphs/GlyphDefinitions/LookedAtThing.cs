namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What gets looked at: a hand, the top of a library.</summary>
/// <exampledoc>Aven Fateshaper</exampledoc>
/// <examplecapture>the top four cards of your library</examplecapture>
[Dependent]
public class LookedAtThing : GlyphOneOf
{
    public LookedAtHand Hand { get; set; }
    public LookedAtTop Top { get; set; }
    public LookedAtTopCard TopCard { get; set; }
}
