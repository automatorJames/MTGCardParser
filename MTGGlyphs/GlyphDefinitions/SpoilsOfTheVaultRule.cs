namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Spoils of the Vault, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spoils of the Vault</exampledoc>
[TokenizationOrder(-1)]
public class SpoilsOfTheVaultRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile all other cards revealed this way, and you lose 1 life for each of the exiled cards"];
}
