namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A card a keyword action is done to, other than a permanent: the top card(s) of a library, or just "a card" as a reminder text names it.</summary>
/// <exampledoc>Ethereal Ambush</exampledoc>
/// <examplecapture>a card</examplecapture>
[Dependent]
public class CardActedOn : GlyphOneOf
{
    public LookedAtTopCard Top { get; set; }
    public SomeCards Cards { get; set; }
}
