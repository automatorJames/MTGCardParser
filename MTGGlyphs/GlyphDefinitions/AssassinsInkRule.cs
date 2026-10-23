namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Assassin's Ink, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Assassin's Ink</exampledoc>
[TokenizationOrder(-1)]
public class AssassinsInkRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this spell costs {1} less to cast if you control an artifact and {1} less to cast if you control an enchantment"];
}
