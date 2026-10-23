namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Soul Transfer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Soul Transfer</exampledoc>
[TokenizationOrder(-1)]
public class SoulTransferRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you control an artifact and an enchantment as you cast this spell, you may choose both instead"];
}
