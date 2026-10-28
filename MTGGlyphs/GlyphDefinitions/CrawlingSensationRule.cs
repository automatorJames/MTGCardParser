namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Crawling Sensation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Crawling Sensation</exampledoc>
[TokenizationOrder(-1)]
public class CrawlingSensationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever one or more land cards are put into your graveyard from anywhere for the first time each turn, create a 1/1 green insect creature token"];
}
