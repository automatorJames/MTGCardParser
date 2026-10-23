namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Time Vault, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Time Vault</exampledoc>
[TokenizationOrder(-1)]
public class TimeVaultRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you would begin your turn while this artifact is tapped, you may skip that turn instead"];
}
