namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Phyrexian Portal, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Phyrexian Portal</exampledoc>
[TokenizationOrder(-1)]
public class PhyrexianPortalRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["search the other pile for a card, put it into your hand, then shuffle the rest of that pile into your library"];
}
