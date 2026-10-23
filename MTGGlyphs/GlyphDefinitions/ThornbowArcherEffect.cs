namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Thornbow Archer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Thornbow Archer</exampledoc>
[Dependent]
public class ThornbowArcherEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each opponent who doesn't control an elf loses 1 life"];
}
