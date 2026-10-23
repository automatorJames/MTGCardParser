namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Lim-Dûl's Vault, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lim-Dûl's Vault</exampledoc>
/// <examplecapture>as many times as you choose, you may pay 1 life, put those cards on the bottom of your library in any order, then look at the top five cards of your library</examplecapture>
[TokenizationOrder(-1)]
public class LimDulsVaultRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as many times as you choose, you may pay 1 life, put those cards on the bottom of your library in any order, then look at the top five cards of your library"];
}
