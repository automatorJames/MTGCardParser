namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Absolving Lammasu, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Absolving Lammasu</exampledoc>
[Dependent]
public class AbsolvingLammasuEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["all suspected creatures are no longer suspected"];
}
