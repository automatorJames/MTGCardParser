namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Botanical Brawler, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Botanical Brawler</exampledoc>
/// <examplecapture>if it's the first time +1/+1 counters have been put on that permanent this turn, put a +1/+1 counter on this creature</examplecapture>
[Dependent]
public class BotanicalBrawlerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's the first time +1/+1 counters have been put on that permanent this turn, put a +1/+1 counter on this creature"];
}
