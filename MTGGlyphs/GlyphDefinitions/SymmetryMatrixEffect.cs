namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Symmetry Matrix, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Symmetry Matrix</exampledoc>
[Dependent]
public class SymmetryMatrixEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may pay {1}"];
}
