namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ozai, the Phoenix King, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ozai, the Phoenix King</exampledoc>
[TokenizationOrder(-1)]
public class OzaiThePhoenixKingRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["ozai has flying and indestructible as long as you have six or more unspent mana"];
}
