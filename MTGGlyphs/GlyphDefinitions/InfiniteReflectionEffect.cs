namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Infinite Reflection, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Infinite Reflection</exampledoc>
[Dependent]
public class InfiniteReflectionEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each other nontoken creature you control becomes a copy of that creature"];
}
