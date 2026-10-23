namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Eureka, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Eureka</exampledoc>
[TokenizationOrder(-1)]
public class Eureka2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["starting with you, each player may put a permanent card from their hand onto the battlefield"];
}
