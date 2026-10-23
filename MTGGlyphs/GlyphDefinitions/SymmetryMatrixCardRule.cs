namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Symmetry Matrix, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Symmetry Matrix</exampledoc>
[TokenizationOrder(-1)]
public class SymmetryMatrixCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a creature you control with power equal to its toughness enters, you may pay {1}"];
}
