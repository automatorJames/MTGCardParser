namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Mathemagics, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mathemagics</exampledoc>
[Dependent]
public class MathemagicsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["2⁰ = 1, 2¹ = 2, 2² = 4, 2³ = 8, 2⁴ = 16, 2⁵ = 32, and so on"];
}
