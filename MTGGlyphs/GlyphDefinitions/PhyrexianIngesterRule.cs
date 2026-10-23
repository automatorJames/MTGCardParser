namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Phyrexian Ingester, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Phyrexian Ingester</exampledoc>
[TokenizationOrder(-1)]
public class PhyrexianIngesterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature gets +x/+y, where x is the exiled creature card's power and y is its toughness"];
}
