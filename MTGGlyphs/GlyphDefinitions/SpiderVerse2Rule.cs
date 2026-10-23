namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Spider-Verse, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spider-Verse</exampledoc>
[TokenizationOrder(-1)]
public class SpiderVerse2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the \"legend rule\" doesn't apply to spiders you control"];
}
