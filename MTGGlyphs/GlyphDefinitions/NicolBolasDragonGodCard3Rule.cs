namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Nicol Bolas, Dragon-God, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nicol Bolas, Dragon-God</exampledoc>
[TokenizationOrder(-1)]
public class NicolBolasDragonGodCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each opponent exiles a card from their hand or a permanent they control"];
}
