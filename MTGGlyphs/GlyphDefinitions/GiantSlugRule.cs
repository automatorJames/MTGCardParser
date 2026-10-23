namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Giant Slug, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Giant Slug</exampledoc>
[TokenizationOrder(-1)]
public class GiantSlugRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature gains landwalk of the chosen type until the end of that turn"];
}
