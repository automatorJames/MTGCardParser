namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Whippoorwill, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Whippoorwill</exampledoc>
[TokenizationOrder(-1)]
public class WhippoorwillRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["damage that would be dealt to that creature this turn can't be prevented or dealt instead to another permanent or player"];
}
