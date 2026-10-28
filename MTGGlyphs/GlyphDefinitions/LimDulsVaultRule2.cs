namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Lim-Dûl's Vault, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lim-Dûl's Vault</exampledoc>
[TokenizationOrder(-1)]
public class LimDulsVaultRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then shuffle and put the last cards you looked at this way on top in any order"];
}
