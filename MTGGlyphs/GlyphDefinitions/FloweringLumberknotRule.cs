namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Flowering Lumberknot, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Flowering Lumberknot</exampledoc>
[TokenizationOrder(-1)]
public class FloweringLumberknotRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature can't attack or block unless it's paired with a creature with soulbond"];
}
