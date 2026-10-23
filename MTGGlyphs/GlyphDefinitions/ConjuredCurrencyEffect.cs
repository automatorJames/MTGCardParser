namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Conjured Currency, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Conjured Currency</exampledoc>
[Dependent]
public class ConjuredCurrencyEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may exchange control of this enchantment and target permanent you neither own nor control"];
}
