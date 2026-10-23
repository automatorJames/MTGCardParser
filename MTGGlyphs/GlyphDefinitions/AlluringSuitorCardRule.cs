namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Alluring Suitor // Deadly Dancer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Alluring Suitor // Deadly Dancer</exampledoc>
[TokenizationOrder(-1)]
public class AlluringSuitorCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when you attack with exactly two creatures, transform this creature"];
}
