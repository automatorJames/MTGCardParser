namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Wild Shape, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wild Shape</exampledoc>
[TokenizationOrder(-1)]
public class WildShapeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until end of turn, target creature you control has that base power and toughness, becomes that creature type, and gains that ability"];
}
