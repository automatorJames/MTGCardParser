namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Memorial Vault, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Memorial Vault</exampledoc>
[Dependent]
public class MemorialVaultEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile the top x cards of your library, where x is one plus the mana value of the sacrificed artifact"];
}
