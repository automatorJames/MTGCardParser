namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Infinite Reflection, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Infinite Reflection</exampledoc>
[TokenizationOrder(-1)]
public class InfiniteReflectionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when this aura enters attached to a creature, each other nontoken creature you control becomes a copy of that creature"];
}
