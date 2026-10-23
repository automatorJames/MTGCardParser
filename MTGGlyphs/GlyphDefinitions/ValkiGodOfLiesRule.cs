namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Valki, God of Lies // Tibalt, Cosmic Impostor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Valki, God of Lies // Tibalt, Cosmic Impostor</exampledoc>
[TokenizationOrder(-1)]
public class ValkiGodOfLiesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each opponent, exile a creature card they revealed this way until valki leaves the battlefield"];
}
