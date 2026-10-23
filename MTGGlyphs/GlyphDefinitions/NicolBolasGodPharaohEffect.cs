namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Nicol Bolas, God-Pharaoh, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nicol Bolas, God-Pharaoh</exampledoc>
[Dependent]
public class NicolBolasGodPharaohEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["nicol bolas deals 7 damage to target opponent, creature an opponent controls, or planeswalker an opponent controls"];
}
