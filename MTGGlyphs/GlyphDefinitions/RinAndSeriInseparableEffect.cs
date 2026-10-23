namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Rin and Seri, Inseparable, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rin and Seri, Inseparable</exampledoc>
[Dependent]
public class RinAndSeriInseparableEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["rin and seri deals damage to any target equal to the number of dogs you control"];
}
