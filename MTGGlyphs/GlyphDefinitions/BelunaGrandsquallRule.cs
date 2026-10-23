namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Beluna Grandsquall // Seek Thrills, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Beluna Grandsquall // Seek Thrills</exampledoc>
[TokenizationOrder(-1)]
public class BelunaGrandsquallRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["permanent spells you cast that have an adventure cost {1} less to cast"];
}
