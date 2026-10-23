namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Critical Hit, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Critical Hit</exampledoc>
[Dependent]
public class CriticalHitCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["a natural 20 is a roll that displays 20 on the die"];
}
