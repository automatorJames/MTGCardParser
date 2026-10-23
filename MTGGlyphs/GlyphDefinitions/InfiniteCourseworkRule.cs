namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Infinite Coursework, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Infinite Coursework</exampledoc>
[TokenizationOrder(-1)]
public class InfiniteCourseworkRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it becomes unprepared"];
}
