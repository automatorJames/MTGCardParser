namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Descendants' Path, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Descendants' Path</exampledoc>
[TokenizationOrder(-1)]
public class DescendantsPathRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's a creature card that shares a creature type with a creature you control, you may cast it without paying its mana cost"];
}
