namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Blood of the Martyr, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Blood of the Martyr</exampledoc>
[TokenizationOrder(-1)]
public class BloodOfTheMartyrRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until end of turn, if damage would be dealt to any creature, you may have that damage dealt to you instead"];
}
