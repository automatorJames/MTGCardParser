namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Crawling Sensation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Crawling Sensation</exampledoc>
/// <examplecapture>create a 1/1 green insect creature token</examplecapture>
[Dependent]
public class CrawlingSensationEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a 1/1 green insect creature token"];
}
