namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Scrap Trawler, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Scrap Trawler</exampledoc>
/// <examplecapture>return to your hand target artifact card in your graveyard with lesser mana value</examplecapture>
[Dependent]
public class ScrapTrawlerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return to your hand target artifact card in your graveyard with lesser mana value"];
}
