namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ajani, Strength of the Pride, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ajani, Strength of the Pride</exampledoc>
[Dependent]
public class AjaniStrengthOfThePride2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you gain life equal to the number of creatures you control plus the number of planeswalkers you control"];
}
