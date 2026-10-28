namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Phyrexian Portal, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Phyrexian Portal</exampledoc>
[TokenizationOrder(-1)]
public class PhyrexianPortalRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile one of those piles"];
}
