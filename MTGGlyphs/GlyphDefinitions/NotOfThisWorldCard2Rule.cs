namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Not of This World, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Not of This World</exampledoc>
[TokenizationOrder(-1)]
public class NotOfThisWorldCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter target spell or ability that targets a permanent you control"];
}
