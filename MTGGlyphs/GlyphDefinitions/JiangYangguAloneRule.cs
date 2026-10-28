namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Jiang Yanggu, Alone, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jiang Yanggu, Alone</exampledoc>
[TokenizationOrder(-1)]
public class JiangYangguAloneRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a creature you control attacks a player alone, discard a card, then draw a card"];
}
