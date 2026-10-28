namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Scrap Trawler, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Scrap Trawler</exampledoc>
[TokenizationOrder(-1)]
public class ScrapTrawlerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever this creature dies or another artifact you control is put into a graveyard from the battlefield, return to your hand target artifact card in your graveyard with lesser mana value"];
}
