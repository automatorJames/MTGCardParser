namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Alluring Suitor // Deadly Dancer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Alluring Suitor // Deadly Dancer</exampledoc>
[Dependent]
public class AlluringSuitorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["transform this creature"];
}
