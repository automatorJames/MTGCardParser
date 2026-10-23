namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hit the Mother Lode, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hit the Mother Lode</exampledoc>
[TokenizationOrder(-1)]
public class HitTheMotherLodeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the discovered card's mana value is less than 10, create a number of tapped treasure tokens equal to the difference"];
}
