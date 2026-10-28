namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Valki, God of Lies // Tibalt, Cosmic Impostor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Valki, God of Lies // Tibalt, Cosmic Impostor</exampledoc>
[Dependent]
public class ValkiGodOfLiesEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose a creature card exiled with valki with mana value x"];
}
